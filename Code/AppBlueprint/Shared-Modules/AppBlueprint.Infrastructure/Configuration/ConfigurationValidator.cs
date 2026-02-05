using Microsoft.Extensions.Configuration;

namespace AppBlueprint.Infrastructure.Configuration;

/// <summary>
/// Helper class for validating configuration values with helpful error messages
/// </summary>
public static class ConfigurationValidator
{
    /// <summary>
    /// Validates that a required configuration value exists and is not empty
    /// </summary>
    /// <param name="value">The configuration value to validate</param>
    /// <param name="configKey">The configuration key name</param>
    /// <param name="helpMessage">Optional help message explaining how to set the value</param>
    /// <exception cref="InvalidOperationException">Thrown when the configuration value is missing or empty</exception>
    public static void RequireConfiguration(string? value, string configKey, string? helpMessage = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            var errorMessage = $"Required configuration '{configKey}' is missing or empty.\n";
            
            if (!string.IsNullOrWhiteSpace(helpMessage))
            {
                errorMessage += $"\n{helpMessage}";
            }
            else
            {
                errorMessage += $"\nPlease set the '{configKey}' configuration value.";
            }
            
            throw new InvalidOperationException(errorMessage);
        }
    }

    /// <summary>
    /// Validates a database connection string with helpful error messages
    /// </summary>
    /// <param name="connectionString">The connection string to validate</param>
    /// <param name="configKeys">Configuration keys that were checked</param>
    /// <exception cref="InvalidOperationException">Thrown when connection string is invalid</exception>
    public static void ValidateDatabaseConnectionString(string? connectionString, params string[] configKeys)
    {
        ArgumentNullException.ThrowIfNull(configKeys);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            var keysChecked = configKeys.Length > 0 
                ? string.Join(", ", configKeys) 
                : "DATABASE_CONNECTIONSTRING, appblueprintdb, postgres-server, DefaultConnection";

            throw new InvalidOperationException(
                "Database connection string is required but not configured.\n" +
                "\n" +
                "To fix this, set the connection string using one of these methods:\n" +
                "\n" +
                "1. Environment Variable (recommended for production):\n" +
                "   DATABASE_CONNECTIONSTRING=Host=localhost;Database=appblueprint;Username=postgres;Password=yourpassword\n" +
                "\n" +
                "2. Configuration file (appsettings.json):\n" +
                "   {\n" +
                "     \"ConnectionStrings\": {\n" +
                "       \"appblueprintdb\": \"Host=localhost;Database=appblueprint;Username=postgres;Password=yourpassword\"\n" +
                "     }\n" +
                "   }\n" +
                "\n" +
                $"Fallback keys checked: {keysChecked}");
        }
    }

    /// <summary>
    /// Validates Logto authentication configuration
    /// </summary>
    /// <param name="configuration">The configuration instance</param>
    /// <param name="throwOnMissing">Whether to throw an exception if configuration is missing</param>
    /// <returns>True if Logto is configured, false otherwise</returns>
    /// <exception cref="InvalidOperationException">Thrown when configuration is invalid and throwOnMissing is true</exception>
    public static bool ValidateLogtoConfiguration(IConfiguration configuration, bool throwOnMissing = false)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        // Read from flat UPPERCASE environment variables first, then fall back to IConfiguration
        string? logtoAppId = Environment.GetEnvironmentVariable("LOGTO_APP_ID") 
                          ?? Environment.GetEnvironmentVariable("LOGTO_APPID") 
                          ?? configuration["Logto:AppId"];
        string? logtoEndpoint = Environment.GetEnvironmentVariable("LOGTO_ENDPOINT") 
                             ?? configuration["Logto:Endpoint"];
        string? logtoAppSecret = Environment.GetEnvironmentVariable("LOGTO_APP_SECRET") 
                              ?? Environment.GetEnvironmentVariable("LOGTO_APPSECRET") 
                              ?? configuration["Logto:AppSecret"];

        bool hasEndpoint = !string.IsNullOrWhiteSpace(logtoEndpoint);
        bool hasAppId = !string.IsNullOrWhiteSpace(logtoAppId);
        bool hasAppSecret = !string.IsNullOrWhiteSpace(logtoAppSecret);

        // If nothing is configured, that's okay (authentication is optional)
        if (!hasEndpoint && !hasAppId && !hasAppSecret)
        {
            return false;
        }

        // If partial configuration exists, validate it
        var missingConfig = new List<string>();
        
        if (!hasEndpoint)
            missingConfig.Add("Logto:Endpoint");
        
        if (!hasAppId)
            missingConfig.Add("Logto:AppId");
        
        if (!hasAppSecret)
            missingConfig.Add("Logto:AppSecret");

        if (missingConfig.Count > 0)
        {
            var errorMessage = 
                $"Incomplete Logto authentication configuration. Missing: {string.Join(", ", missingConfig)}\n" +
                "\n" +
                "To enable Logto authentication, set all required values:\n" +
                "\n" +
                "1. Environment Variables (UPPERCASE format):\n" +
                "   LOGTO_APPID=your_app_id\n" +
                "   LOGTO_ENDPOINT=https://your-tenant.logto.app\n" +
                "   LOGTO_APPSECRET=your_app_secret\n" +
                "\n" +
                "2. Configuration file (appsettings.json):\n" +
                "   {\n" +
                "     \"Logto\": {\n" +
                "       \"AppId\": \"your_app_id\",\n" +
                "       \"Endpoint\": \"https://your-tenant.logto.app\",\n" +
                "       \"AppSecret\": \"your_app_secret\"\n" +
                "     }\n" +
                "   }\n" +
                "\n" +
                "To run without authentication, remove all Logto configuration values.";

            if (throwOnMissing)
            {
                throw new InvalidOperationException(errorMessage);
            }
            else
            {
                Console.WriteLine($"[Configuration Validation] WARNING: {errorMessage}");
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Gets a configuration value with a helpful error message if missing
    /// </summary>
    /// <param name="configuration">The configuration instance</param>
    /// <param name="key">The configuration key</param>
    /// <param name="helpMessage">Optional help message</param>
    /// <returns>The configuration value</returns>
    /// <exception cref="InvalidOperationException">Thrown when the value is missing</exception>
    public static string GetRequiredConfiguration(IConfiguration configuration, string key, string? helpMessage = null)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        string? value = configuration[key];
        RequireConfiguration(value, key, helpMessage);
        return value!;
    }

    /// <summary>
    /// Validates a URL configuration value
    /// </summary>
    /// <param name="url">The URL to validate</param>
    /// <param name="configKey">The configuration key name</param>
    /// <exception cref="InvalidOperationException">Thrown when the URL is invalid</exception>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1054:URI parameters should not be strings", Justification = "This method validates string configuration values before converting to Uri")]
    public static void ValidateUrl(string? url, string configKey)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new InvalidOperationException(
                $"Required URL configuration '{configKey}' is missing or empty.\n" +
                $"Please provide a valid URL for '{configKey}'.");
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || 
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new InvalidOperationException(
                $"Configuration '{configKey}' contains an invalid URL: '{url}'\n" +
                "The URL must be a valid absolute HTTP or HTTPS URL.\n" +
                $"Example: https://example.com");
        }
    }
}
