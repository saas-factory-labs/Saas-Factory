using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Aspire.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace AppBlueprint.AppHost;

internal static class SslCertificateManager
{
    /// <summary>
    /// Generates a stable password based on the key and machine-specific information.
    /// This creates a deterministic but unique password for development certificates.
    /// </summary>
    private static string GenerateStablePassword(string key)
    {
        // Use machine name and key to generate a stable password
        string seed = $"{Environment.MachineName}_{key}";
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(seed));

        // Convert to base64 and take first 32 characters for a reasonable password length
        return Convert.ToBase64String(hash)[..32];
    }

    private static string? GetUserSecretsId()
    {
        try
        {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly == null)
                return null;

            var attribute = assembly.GetCustomAttribute<UserSecretsIdAttribute>();
            return attribute?.UserSecretsId;
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Warning: Failed to get UserSecretsId: {ex.Message}");
            return null;
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine($"Warning: Failed to get UserSecretsId: {ex.Message}");
            return null;
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Warning: Failed to get UserSecretsId: {ex.Message}");
            return null;
        }
    }

    public static string CreateSslCertificateIfNotExists(this IDistributedApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Try to use user secrets only if we have a secrets ID
        var configBuilder = new ConfigurationBuilder();
        var userSecretsId = GetUserSecretsId();

        if (!string.IsNullOrEmpty(userSecretsId))
        {
            Console.WriteLine($"Using User Secrets ID: {userSecretsId}");
            configBuilder.AddUserSecrets(userSecretsId);
        }
        else
        {
            Console.WriteLine("User Secrets ID not found. Proceeding without user secrets.");
        }

        var config = configBuilder.Build();

        const string certificatePasswordKey = "certificate-password";
        string certificatePassword;

        var configValue = config[certificatePasswordKey];
        if (configValue is not null)
        {
            certificatePassword = configValue;
        }
        else
        {
            // Generate a stable password directly instead of using ParameterResource.Value (which is obsolete)
            // This is acceptable for development certificates that are created once per machine
            certificatePassword = GenerateStablePassword(certificatePasswordKey);
        }

        // Use web-service.pfx for consistency with the Web project
        var certificateLocation = GetCertificateLocation("web-service");
        try
        {
            // Only check if the file exists first - avoid loading if not needed
            if (!File.Exists(certificateLocation))
            {
                Console.WriteLine($"Certificate does not exist at {certificateLocation}. Creating a new certificate...");
                CleanExistingCertificates();
                CreateNewSelfSignedDeveloperCertificate(certificateLocation, certificatePassword);

                // Also set the password as an environment variable for other services to use
                Environment.SetEnvironmentVariable("CERTIFICATE_PASSWORD", certificatePassword);
                Console.WriteLine("Certificate password set as environment variable CERTIFICATE_PASSWORD");

                return certificatePassword;
            }

            // Certificate exists, try to load it
            using var certificate = X509CertificateLoader.LoadPkcs12FromFile(certificateLocation, certificatePassword);
            Console.WriteLine($"Existing certificate found at {certificateLocation}");

            // Only create a new certificate if the current one is clearly invalid
            if (!IsCertificateValid(certificate))
            {
                Console.WriteLine("Certificate exists but is not valid or has expired. Creating a new certificate...");
                CleanExistingCertificates();
                CreateNewSelfSignedDeveloperCertificate(certificateLocation, certificatePassword);
            }
            else
            {
                Console.WriteLine("Certificate is valid. Using existing certificate.");

                // Only check trust if the certificate hasn't been regenerated
                if (!IsCertificateTrusted(certificate))
                {
                    Console.WriteLine("Certificate exists but is not trusted. Re-trusting certificate...");
                    TrustCertificate(certificateLocation, certificatePassword);
                }
                else
                {
                    Console.WriteLine("Certificate is already trusted.");
                }
            }

            // Set the certificate password as an environment variable so it can be accessed by other services
            Environment.SetEnvironmentVariable("CERTIFICATE_PASSWORD", certificatePassword);
            Console.WriteLine("Certificate password set as environment variable CERTIFICATE_PASSWORD");
        }
        catch (CryptographicException ex)
        {
            Console.WriteLine($"Certificate cryptographic error: {ex.Message}. Generating new certificate at {certificateLocation}");
            CleanExistingCertificates();
            CreateNewSelfSignedDeveloperCertificate(certificateLocation, certificatePassword);

            // Set the password as environment variable even after errors
            Environment.SetEnvironmentVariable("CERTIFICATE_PASSWORD", certificatePassword);
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Certificate access error: {ex.Message}. Generating new certificate.");
            CleanExistingCertificates();
            CreateNewSelfSignedDeveloperCertificate(certificateLocation, certificatePassword);

            // Set the password as environment variable even after errors
            Environment.SetEnvironmentVariable("CERTIFICATE_PASSWORD", certificatePassword);
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"Certificate file not found: {ex.Message}. Generating new certificate.");
            CleanExistingCertificates();
            CreateNewSelfSignedDeveloperCertificate(certificateLocation, certificatePassword);

            // Set the password as environment variable even after errors
            Environment.SetEnvironmentVariable("CERTIFICATE_PASSWORD", certificatePassword);
        }

        return certificatePassword;
    }

    private static string GetCertificateLocation(string domain)
    {
        ArgumentException.ThrowIfNullOrEmpty(domain, nameof(domain));

        var userFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(userFolder, "ASP.NET", "Https", $"{domain}.pfx");
    }

    private static void CleanExistingCertificates()
    {
        Console.WriteLine("Cleaning existing development certificates...");
        RunDotNetDevCertsCommand("https --clean --quiet");
    }

    private static bool IsCertificateValid(X509Certificate2 certificate)
    {
        ArgumentNullException.ThrowIfNull(certificate);

        try
        {
            // Check if certificate is valid for the current date
            DateTime now = DateTime.Now;

            // Only invalidate if the certificate is already expired
            if (certificate.NotAfter < now)
            {
                Console.WriteLine($"Certificate {certificate.Thumbprint} is expired.");
                Console.WriteLine($"Valid from {certificate.NotBefore} to {certificate.NotAfter}, but current date is {now}");
                return false;
            }

            return true;
        }
        catch (CryptographicException ex)
        {
            Console.WriteLine($"Error checking certificate validity (cryptographic): {ex.Message}");
            return false;
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Error checking certificate validity (invalid argument): {ex.Message}");
            return false;
        }
    }

    private static void CreateNewSelfSignedDeveloperCertificate(string certificateLocation, string password)
    {
        ArgumentException.ThrowIfNullOrEmpty(certificateLocation, nameof(certificateLocation));
        ArgumentException.ThrowIfNullOrEmpty(password, nameof(password));

        if (File.Exists(certificateLocation))
        {
            Console.WriteLine($"Deleting existing certificate at {certificateLocation}");
            File.Delete(certificateLocation);
        }

        var certificateDirectory = Path.GetDirectoryName(certificateLocation);
        if (certificateDirectory is not null && !Directory.Exists(certificateDirectory))
        {
            Console.WriteLine($"Certificate directory {certificateDirectory} does not exist. Creating it.");
            Directory.CreateDirectory(certificateDirectory);
        }

        // Create a new certificate with a 2-year validity period
        Console.WriteLine("Creating new certificate with dotnet dev-certs...");
        RunDotNetDevCertsCommand($"https -ep \"{certificateLocation}\" -p \"{password}\" --quiet");

        // Extra step: Trust certificate in all system stores
        Console.WriteLine("Trusting certificate in system stores...");
        TrustCertificate(certificateLocation, password);

        // Verify the certificate was created and is trusted
        try
        {
            using var cert = X509CertificateLoader.LoadPkcs12FromFile(certificateLocation, password);
            Console.WriteLine($"Successfully created certificate. Valid from {cert.NotBefore} to {cert.NotAfter}");

            if (!IsCertificateTrusted(cert))
            {
                Console.WriteLine("Warning: New certificate is not trusted yet. Running additional trust steps...");
                RunDotNetDevCertsCommand("https --trust --quiet");

                // Additional trust step: explicitly trust it using system tools
                ExplicitlyTrustCertificateInBrowserStores(cert);
            }
        }
        catch (CryptographicException ex)
        {
            Console.WriteLine($"Error verifying new certificate (cryptographic): {ex.Message}");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"Error verifying new certificate (file not found): {ex.Message}");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Error verifying new certificate (access denied): {ex.Message}");
        }
    }

    private static bool IsCertificateTrusted(X509Certificate2 certificate)
    {
        ArgumentNullException.ThrowIfNull(certificate);

        try
        {
            // Check if certificate is in the trusted root store for CurrentUser
            using var userRootStore = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
            userRootStore.Open(OpenFlags.ReadOnly);

            var userRootResult = userRootStore.Certificates.Find(X509FindType.FindByThumbprint, certificate.Thumbprint, false);
            if (userRootResult.Count > 0)
            {
                Console.WriteLine("Certificate is trusted in CurrentUser Root store.");
                return true;
            }

            // Also check trusted root for LocalMachine which browsers often use
            using var machineRootStore = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            machineRootStore.Open(OpenFlags.ReadOnly);

            var machineRootResult = machineRootStore.Certificates.Find(X509FindType.FindByThumbprint, certificate.Thumbprint, false);
            if (machineRootResult.Count > 0)
            {
                Console.WriteLine("Certificate is trusted in LocalMachine Root store.");
                return true;
            }

            // Also check if it's in the My store for CurrentUser or LocalMachine
            using var userMyStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            userMyStore.Open(OpenFlags.ReadOnly);

            var userMyResult = userMyStore.Certificates.Find(X509FindType.FindByThumbprint, certificate.Thumbprint, false);
            if (userMyResult.Count > 0)
            {
                Console.WriteLine("Certificate is trusted in CurrentUser My store.");
                return true;
            }

            using var machineMyStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            machineMyStore.Open(OpenFlags.ReadOnly);

            var machineMyResult = machineMyStore.Certificates.Find(X509FindType.FindByThumbprint, certificate.Thumbprint, false);
            if (machineMyResult.Count > 0)
            {
                Console.WriteLine("Certificate is trusted in LocalMachine My store.");
                return true;
            }

            // If we get here, the certificate is not trusted in any store
            Console.WriteLine("Certificate is not trusted in any certificate store.");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking certificate trust: {ex.Message}");
            return false;
        }
    }
    private static void TrustCertificate(string certificatePath, string password)
    {
        ArgumentException.ThrowIfNullOrEmpty(certificatePath, nameof(certificatePath));
        ArgumentException.ThrowIfNullOrEmpty(password, nameof(password));

        Console.WriteLine("Explicitly trusting the development certificate...");

        try
        {
            // First, trust the certificate using dotnet dev-certs
            RunDotNetDevCertsCommand("https --trust --quiet");

            try
            {
                // Load certificate with proper flags - use the loader directly
                using var cert = X509CertificateLoader.LoadPkcs12FromFile(certificatePath, password);

                // Only add to user stores which don't require admin privileges
                InstallCertificateToStore(cert, StoreName.Root, StoreLocation.CurrentUser);
                InstallCertificateToStore(cert, StoreName.My, StoreLocation.CurrentUser);

                // Skip LocalMachine stores entirely as they're causing issues
                // and require admin rights
                Console.WriteLine("Skipping LocalMachine stores to focus on user-level certificate stores.");

                // Additional browser-specific trust step using user-level stores
                ExplicitlyTrustCertificateInBrowserStores(cert);
            }
            catch (CryptographicException ex)
            {
                Console.WriteLine($"Warning: Cryptographic error adding certificate to stores: {ex.Message}");
                Console.WriteLine("The certificate may still work correctly in your browser.");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Warning: IO error adding certificate to stores: {ex.Message}");
                Console.WriteLine("The certificate may still work correctly in your browser.");
            }
            catch (System.Security.SecurityException ex)
            {
                Console.WriteLine($"Warning: Security error adding certificate to stores: {ex.Message}");
                Console.WriteLine("The certificate may still work correctly in your browser.");
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Warning: IO error during certificate trust operation: {ex.Message}");
            Console.WriteLine("You may need to manually trust the certificate in your browser.");
        }
        catch (System.Security.SecurityException ex)
        {
            Console.WriteLine($"Warning: Security error during certificate trust operation: {ex.Message}");
            Console.WriteLine("You may need to manually trust the certificate in your browser.");
        }
    }

    private static void InstallCertificateToStore(X509Certificate2 certificate, StoreName storeName, StoreLocation storeLocation)
    {
        ArgumentNullException.ThrowIfNull(certificate);

        try
        {
            using var store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadWrite);

            // Check if certificate already exists in store
            var existingCerts = store.Certificates.Find(X509FindType.FindByThumbprint, certificate.Thumbprint, false);
            if (existingCerts.Count == 0)
            {
                store.Add(certificate);
                Console.WriteLine($"Certificate {certificate.Thumbprint} added to {storeName}/{storeLocation} store.");
            }
            else
            {
                Console.WriteLine($"Certificate {certificate.Thumbprint} already in {storeName}/{storeLocation} store.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to add certificate to {storeName}/{storeLocation} store: {ex.Message}");
        }
    }

    private static void ExplicitlyTrustCertificateInBrowserStores(X509Certificate2 certificate)
    {
        ArgumentNullException.ThrowIfNull(certificate);

        // Windows-specific certificate browser trust enhancement
        if (OperatingSystem.IsWindows())
        {
            try
            {
                // On Windows, try to run certutil to make browsers trust the certificate
                string tempCerPath = Path.Combine(Path.GetTempPath(), $"dev_cert_{Guid.NewGuid()}.cer");
                try
                {
                    // Export the certificate to a .cer file 
                    File.WriteAllBytes(tempCerPath, certificate.Export(X509ContentType.Cert));

                    // First try with -user parameter which doesn't require admin rights
                    using var userProcess = Process.Start(new ProcessStartInfo
                    {
                        FileName = "certutil",
                        Arguments = $"-addstore -user \"Root\" \"{tempCerPath}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    });

                    if (userProcess is not null)
                    {
                        userProcess.WaitForExit();

                        if (userProcess.ExitCode == 0)
                        {
                            Console.WriteLine("Successfully added certificate to user's browser trust store using certutil.");
                            return; // Success, no need to try the admin version
                        }
                        else
                        {
                            var error = userProcess.StandardError.ReadToEnd();
                            Console.WriteLine($"User-level certutil operation failed: {error}");

                            // If user-level failed, try to suggest a manual approach
                            Console.WriteLine("You may need to manually trust the certificate in your browser settings.");
                            Console.WriteLine("Certificate was created successfully, but you might see a warning in your browser.");
                        }
                    }
                }
                finally
                {
                    if (File.Exists(tempCerPath))
                    {
                        File.Delete(tempCerPath);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Couldn't complete browser trust operations: {ex.Message}");
                Console.WriteLine("This is not critical - the certificate will still work for HTTPS, but browsers might show a warning.");
                Console.WriteLine("If you see certificate warnings in your browser, you may need to manually trust the certificate.");
            }
        }
        // macOS-specific certificate browser trust enhancement
        else if (OperatingSystem.IsMacOS())
        {
            try
            {
                // On macOS, use security tool to add certificate to keychain
                string tempCerPath = Path.Combine(Path.GetTempPath(), $"dev_cert_{Guid.NewGuid()}.cer");
                try
                {
                    // Export the certificate
                    File.WriteAllBytes(tempCerPath, certificate.Export(X509ContentType.Cert));

                    // Add to keychain and trust
                    using var process = Process.Start(new ProcessStartInfo
                    {
                        FileName = "security",
                        Arguments = $"add-trusted-cert -d -r trustRoot -k ~/Library/Keychains/login.keychain {tempCerPath}",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    });

                    if (process is not null)
                    {
                        process.WaitForExit();
                        if (process.ExitCode == 0)
                        {
                            Console.WriteLine("Successfully added certificate to macOS keychain.");
                        }
                        else
                        {
                            var error = process.StandardError.ReadToEnd();
                            Console.WriteLine($"Warning: macOS security command failed: {error}");
                            Console.WriteLine("This could be due to permission restrictions. The certificate will still work, but you may need to manually trust it.");
                        }
                    }
                }
                finally
                {
                    if (File.Exists(tempCerPath))
                    {
                        File.Delete(tempCerPath);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Couldn't complete macOS keychain trust operations: {ex.Message}");
                Console.WriteLine("This is not critical - the certificate should still work, but may show security warnings.");
            }
        }
        // Linux certificate trust enhancement
        else if (OperatingSystem.IsLinux())
        {
            Console.WriteLine("Note: For Linux systems, you may need to manually trust the certificate.");
            Console.WriteLine("Please refer to your distribution's documentation for adding certificates to the trust store.");
            Console.WriteLine("Common methods include:");
            Console.WriteLine("- Ubuntu/Debian: sudo cp cert.crt /usr/local/share/ca-certificates/ && sudo update-ca-certificates");
            Console.WriteLine("- Fedora/RHEL: sudo cp cert.crt /etc/pki/ca-trust/source/anchors/ && sudo update-ca-trust extract");
        }
    }

    private static void RunDotNetDevCertsCommand(string arguments)
    {
        ArgumentException.ThrowIfNullOrEmpty(arguments, nameof(arguments));

        Console.WriteLine($"Running dotnet dev-certs {arguments}");

        using var process = Process.Start(new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"dev-certs {arguments}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = false
        }) ?? throw new InvalidOperationException("Failed to start dotnet dev-certs process.");
        process.WaitForExit();

        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();

        if (!string.IsNullOrEmpty(output))
        {
            Console.WriteLine($"Output: {output}");
        }

        if (!string.IsNullOrEmpty(error))
        {
            Console.WriteLine($"Error: {error}");
        }

        if (process.ExitCode != 0)
        {
            Console.WriteLine($"dotnet dev-certs command failed with exit code {process.ExitCode}");
        }
    }
}
