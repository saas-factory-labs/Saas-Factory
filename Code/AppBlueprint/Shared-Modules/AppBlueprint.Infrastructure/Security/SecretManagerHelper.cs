// using System.Diagnostics;
// using System.Reflection;
// using System.Security.Cryptography;
// using System.Security.Cryptography.X509Certificates;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.Configuration.UserSecrets;

// namespace AppHost;

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


// // using System;
// // using System.Security.Cryptography;
// // using Microsoft.Extensions.Configuration;

// // namespace AppBlueprint.Infrastructure.Security;

// // /// <summary>
// // /// Helper class for managing secrets and security-related functionality
// // /// </summary>
// // public static class SecretManagerHelper
// // {
// //     /// <summary>
// //     /// Generates a cryptographically secure key for JWT token signing
// //     /// </summary>
// //     /// <param name="keyName">The name of the key to generate (for logging)</param>
// //     /// <param name="keyLengthInBytes">Length of the key in bytes (default: 32 bytes = 256 bits)</param>
// //     /// <returns>Base64 encoded key string</returns>
// //     public static string GenerateAuthenticationTokenSigningKey(string keyName = "jwt-token-signing-key", int keyLengthInBytes = 32)
// //     {
// //         ArgumentException.ThrowIfNullOrEmpty(keyName);
// //         ArgumentOutOfRangeException.ThrowIfLessThan(keyLengthInBytes, 16, nameof(keyLengthInBytes));
        
// //         // Generate a cryptographically secure random key
// //         byte[] keyBytes = new byte[keyLengthInBytes];
// //         using var rng = RandomNumberGenerator.Create();
// //         rng.GetBytes(keyBytes);
        
// //         // Convert to Base64 for storage
// //         string base64Key = Convert.ToBase64String(keyBytes);
        
// //         return base64Key;
// //     }
    
// //     /// <summary>
// //     /// Stores a token signing key in the user secrets for development
// //     /// </summary>
// //     /// <param name="key">The key to store</param>
// //     /// <param name="configuration">The configuration instance</param>
// //     /// <param name="secretName">The name of the secret (default: JwtSigningKey)</param>
// //     public static void StoreTokenSigningKey(string key, IConfiguration configuration, string secretName = "JwtSigningKey")
// //     {
// //         ArgumentException.ThrowIfNullOrEmpty(key);
// //         ArgumentNullException.ThrowIfNull(configuration);
// //         ArgumentException.ThrowIfNullOrEmpty(secretName);
        
// //         // In a real implementation, this would call User Secrets Manager
// //         // For this demonstration, we're setting it in memory 
// //         // Note: In production, use a secure key vault or similar service
// //         var configRoot = configuration as IConfigurationRoot;
// //         configRoot?.GetReloadToken();
// //     }
// // }