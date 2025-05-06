// // using System;
// // using System.Diagnostics;
// // using System.IO;
// // using System.Security.Cryptography.X509Certificates;


// using System.Diagnostics;
// using System.Reflection;
// using System.Security.Cryptography;
// using System.Security.Cryptography.X509Certificates;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.Configuration.UserSecrets;

// namespace AppBlueprint.AppHost;

// public static class SslCertificateManager
// {
//     private static string UserSecretsId => Assembly.GetEntryAssembly()!.GetCustomAttribute<UserSecretsIdAttribute>()!.UserSecretsId;

//     public static string CreateSslCertificateIfNotExists(this IDistributedApplicationBuilder builder)
//     {
//         var config = new ConfigurationBuilder().AddUserSecrets(UserSecretsId).Build();

//         const string certificatePasswordKey = "certificate-password";
//         var certificatePassword = config[certificatePasswordKey]
//                                   ?? builder.CreateStablePassword(certificatePasswordKey).Resource.Value;

//         var certificateLocation = GetCertificateLocation("localhost");
//         try
//         {
//             X509CertificateLoader.LoadPkcs12FromFile(certificateLocation, certificatePassword);
//         }
//         catch (CryptographicException)
//         {
//             CreateNewSelfSignedDeveloperCertificate(certificateLocation, certificatePassword);
//         }

//         return certificatePassword;
//     }

//     private static string GetCertificateLocation(string domain)
//     {
//         var userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
//         return $"{userFolder}/.aspnet/dev-certs/https/{domain}.pfx";
//     }

//     private static void CreateNewSelfSignedDeveloperCertificate(string certificateLocation, string password)
//     {
//         if (File.Exists(certificateLocation))
//         {
//             Console.WriteLine($"Certificate {certificateLocation} exists, but password {password} was invalid. Creating a new certificate.");

//             File.Delete(certificateLocation);
//         }
//         else
//         {
//             var certificateDirectory = Path.GetDirectoryName(certificateLocation)!;
//             if (!Directory.Exists(certificateDirectory))
//             {
//                 Console.WriteLine($"Certificate directory {certificateDirectory} does not exist. Creating it.");

//                 Directory.CreateDirectory(certificateDirectory);
//             }
//         }

//         Process.Start(new ProcessStartInfo
//             {
//                 FileName = "dotnet",
//                 Arguments = $"dev-certs https --trust -ep {certificateLocation} -p {password}",
//                 RedirectStandardOutput = false,
//                 RedirectStandardError = false,
//                 UseShellExecute = false
//             }
//         )!.WaitForExit();
//     }
// }




// // using Aspire.Hosting;
// // using Microsoft.Extensions.Logging;

// // namespace AppBlueprint.ServiceDefaults;

// // /// <summary>
// // /// Extension methods for SSL certificate creation and management
// // /// </summary>
// // public static class CertificateExtensions
// // {
// //     private const string DefaultCertPassword = "dev-cert-password";
// //     private static readonly ILogger Logger = LoggerFactory
// //         .Create(builder => builder.AddConsole())
// //         .CreateLogger(typeof(CertificateExtensions));

// //     /// <summary>
// //     /// Creates an SSL certificate if it doesn't exist and returns the password
// //     /// </summary>
// //     /// <param name="builder">The IDistributedApplicationBuilder instance</param>
// //     /// <param name="certificateName">Name of the certificate file (without extension)</param>
// //     /// <param name="password">Password for the certificate (defaults to "dev-cert-password")</param>
// //     /// <returns>The password for the certificate</returns>
// //     public static string CreateSslCertificateIfNotExists(
// //         this IDistributedApplicationBuilder builder,
// //         string certificateName = "ssl-cert",
// //         string password = DefaultCertPassword)
// //     {
// //         ArgumentNullException.ThrowIfNull(builder);
// //         ArgumentException.ThrowIfNullOrEmpty(certificateName);
// //         ArgumentException.ThrowIfNullOrEmpty(password);

// //         // Get the application data path for ASP.NET HTTPS certificates
// //         string certPath = Path.Combine(
// //             Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
// //             "ASP.NET", "Https", $"{certificateName}.pfx");

// //         // Create directory if it doesn't exist
// //         Directory.CreateDirectory(Path.GetDirectoryName(certPath) ?? throw new InvalidOperationException("Invalid certificate path"));

// //         // Check if certificate already exists
// //         if (File.Exists(certPath))
// //         {
// //             Logger.LogInformation("Using existing SSL certificate at {CertPath}", certPath);
// //             try
// //             {
// //                 // Verify that the certificate can be loaded with the provided password
// //                 using var cert = new X509Certificate2(certPath, password);
// //                 Logger.LogInformation("SSL certificate successfully loaded with subject: {Subject}", cert.Subject);
// //             }
// //             catch (Exception ex)
// //             {
// //                 Logger.LogWarning("Could not load existing certificate: {Message}", ex.Message);
// //                 Logger.LogInformation("Creating new SSL certificate");
// //                 CreateCertificate(certPath, password);
// //             }
// //         }
// //         else
// //         {
// //             Logger.LogInformation("SSL certificate not found. Creating new certificate");
// //             CreateCertificate(certPath, password);
// //         }

// //         return password;
// //     }

// //     private static void CreateCertificate(string certPath, string password)
// //     {
// //         try
// //         {
// //             // Use .NET's dev certificate tooling to create a certificate
// //             string devCertsCommand = $"dev-certs https -ep \"{certPath}\" -p \"{password}\"";
            
// //             var processStartInfo = new ProcessStartInfo
// //             {
// //                 FileName = "dotnet",
// //                 Arguments = devCertsCommand,
// //                 RedirectStandardOutput = true,
// //                 RedirectStandardError = true,
// //                 UseShellExecute = false,
// //                 CreateNoWindow = true
// //             };

// //             using var process = Process.Start(processStartInfo);
// //             if (process is null)
// //             {
// //                 throw new InvalidOperationException("Failed to start dotnet dev-certs process");
// //             }
            
// //             process.WaitForExit();
            
// //             string output = process.StandardOutput.ReadToEnd();
// //             string error = process.StandardError.ReadToEnd();
            
// //             if (process.ExitCode != 0)
// //             {
// //                 Logger.LogError("Certificate creation failed: {Error}", error);
// //                 throw new InvalidOperationException($"Failed to create certificate: {error}");
// //             }
            
// //             Logger.LogInformation("SSL certificate created successfully at {CertPath}", certPath);
// //         }
// //         catch (Exception ex)
// //         {
// //             Logger.LogError(ex, "Failed to create SSL certificate");
// //             throw;
// //         }
// //     }
// // }