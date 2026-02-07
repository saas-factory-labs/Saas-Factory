using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace AppBlueprint.AppHost;

internal static class SecretManagerHelper
{
    private static readonly IConfigurationRoot ConfigurationRoot = InitializeConfiguration();

    private static IConfigurationRoot InitializeConfiguration()
    {
        var builder = new ConfigurationBuilder();
        try
        {
            var userSecretsId = GetUserSecretsId();

            if (!string.IsNullOrEmpty(userSecretsId))
            {
                Console.WriteLine($"SecretManagerHelper: Using User Secrets ID: {userSecretsId}");
                builder.AddUserSecrets(userSecretsId);
            }
            else
            {
                Console.WriteLine("SecretManagerHelper: User Secrets ID not found. Proceeding without user secrets.");
            }
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"SecretManagerHelper: Error initializing configuration: {ex.Message}");
            Console.WriteLine("SecretManagerHelper: Proceeding without user secrets.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"SecretManagerHelper: Error initializing configuration: {ex.Message}");
            Console.WriteLine("SecretManagerHelper: Proceeding without user secrets.");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"SecretManagerHelper: Error initializing configuration: {ex.Message}");
            Console.WriteLine("SecretManagerHelper: Proceeding without user secrets.");
        }
        return builder.Build();
    }

    private static string? GetUserSecretsId()
    {
        try
        {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly is null)
                return null;

            var attribute = assembly.GetCustomAttribute<UserSecretsIdAttribute>();
            return attribute?.UserSecretsId;
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Warning: Invalid operation getting UserSecretsId: {ex.Message}");
            return null;
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine($"Warning: Null argument getting UserSecretsId: {ex.Message}");
            return null;
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Warning: IO exception getting UserSecretsId: {ex.Message}");
            return null;
        }
        catch (System.Security.SecurityException ex)
        {
            Console.WriteLine($"Warning: Security exception getting UserSecretsId: {ex.Message}");
            return null;
        }
    }

    public static IResourceBuilder<ParameterResource> CreateStablePassword(
        this IDistributedApplicationBuilder builder,
        string secretName
    )
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));
        ArgumentException.ThrowIfNullOrEmpty(secretName, nameof(secretName));

        var password = ConfigurationRoot[secretName];

        if (string.IsNullOrEmpty(password))
        {
            var passwordGenerator = new GenerateParameterDefault
            {
                MinLower = 5,
                MinUpper = 5,
                MinNumeric = 3,
                MinSpecial = 3
            };
            password = passwordGenerator.GetDefaultValue();
            SaveSecrectToDotNetUserSecrets(secretName, password);
        }

        return builder.CreateResourceBuilder(new ParameterResource(secretName, _ => password, true));
    }

    public static void GenerateAuthenticationTokenSigningKey(string secretName)
    {
        ArgumentException.ThrowIfNullOrEmpty(secretName, nameof(secretName));

        if (string.IsNullOrEmpty(ConfigurationRoot[secretName]))
        {
            var key = new byte[64]; // 512-bit key
            RandomNumberGenerator.Fill(key);
            var base64Key = Convert.ToBase64String(key);
            SaveSecrectToDotNetUserSecrets(secretName, base64Key);
        }
    }

    private static void SaveSecrectToDotNetUserSecrets(string key, string value)
    {
        ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));
        ArgumentException.ThrowIfNullOrEmpty(value, nameof(value));

        var userSecretsId = GetUserSecretsId();
        if (string.IsNullOrEmpty(userSecretsId))
        {
            Console.WriteLine($"Warning: Cannot save secret '{key}' - UserSecretsId not available");
            return;
        }

        var args = $"user-secrets set {key} {value} --id {userSecretsId}";
        var startInfo = new ProcessStartInfo("dotnet", args)
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo);
        if (process is null)
        {
            Console.WriteLine($"Warning: Failed to start dotnet process to save secret '{key}'");
            return;
        }

        process.WaitForExit();
    }
}
