using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace AppBlueprint.AppHost;

internal static class X509CertificateLoader
{
    public static X509Certificate2 LoadPkcs12FromFile(string filePath, string password)
    {
        ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));
        ArgumentException.ThrowIfNullOrEmpty(password, nameof(password));

        // Always use UserKeySet which doesn't require admin rights 
        // and will work with the browser security contexts
        try
        {
            const X509KeyStorageFlags flags = X509KeyStorageFlags.Exportable |
                                       X509KeyStorageFlags.PersistKeySet |
                                       X509KeyStorageFlags.UserKeySet;

            // Suppress obsolete warning - we need to use this constructor
            // as the new recommended APIs don't support the flags we need for browser compatibility
#pragma warning disable SYSLIB0057
            return new X509Certificate2(filePath, password, flags);
#pragma warning restore SYSLIB0057
        }
        catch (CryptographicException ex)
        {
            // Log the error and try a different approach
            var message = $"Certificate loading with UserKeySet failed: {ex.Message}";
            Console.WriteLine(message);

            // Try with ephemeral key set which has lowest permission requirements
            // Don't try MachineKeySet since that's causing issues with browsers
#pragma warning disable SYSLIB0057
            return new X509Certificate2(filePath, password,
                X509KeyStorageFlags.Exportable | X509KeyStorageFlags.EphemeralKeySet);
#pragma warning restore SYSLIB0057
        }
    }
}
