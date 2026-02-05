using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Hosting;

namespace AppBlueprint.Presentation.ApiModule.Extensions;

/// <summary>
/// Simplified extension methods for quickly setting up authentication in new applications.
/// This provides an easy-to-use wrapper around the more detailed JwtAuthenticationExtensions.
/// </summary>
public static class AuthenticationSetupExtensions
{
    private const string AuthenticationProviderConfigKey = "Authentication:Provider";

    /// <summary>
    /// Quickly adds authentication to your application with sensible defaults.
    /// Perfect for dating apps and other consumer applications that need user login.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">Application configuration</param>
    /// <param name="environment">The hosting environment</param>
    /// <param name="providerType">The authentication provider to use (Logto, Auth0, or JWT)</param>
    /// <returns>The service collection for chaining</returns>
    /// <example>
    /// <code>
    /// // In your Program.cs:
    /// builder.Services.AddQuickAuthentication(builder.Configuration, builder.Environment, AuthProvider.Logto);
    /// </code>
    /// </example>
    public static IServiceCollection AddQuickAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        AuthProvider providerType = AuthProvider.Logto)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(environment);

        // Set the provider in configuration if not already set
        var configProvider = configuration[AuthenticationProviderConfigKey];
        if (string.IsNullOrEmpty(configProvider))
        {
            // Create a mutable configuration source
            var configDict = new Dictionary<string, string?>
            {
                [AuthenticationProviderConfigKey] = providerType.ToString()
            };
            
            var tempConfig = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .AddInMemoryCollection(configDict)
                .Build();

            // Use the JWT authentication extension with the provider set
            return services.AddJwtAuthentication(tempConfig, environment);
        }

        // Use existing configuration
        return services.AddJwtAuthentication(configuration, environment);
    }

    /// <summary>
    /// Adds Logto authentication with explicit configuration values.
    /// Use this when you want to provide configuration in code rather than appsettings.json.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="environment">The hosting environment</param>
    /// <param name="endpoint">Logto endpoint URL (e.g., https://your-tenant.logto.app)</param>
    /// <param name="clientId">Your Logto application client ID</param>
    /// <param name="clientSecret">Your Logto application client secret (optional for API validation)</param>
    /// <returns>The service collection for chaining</returns>
    /// <example>
    /// <code>
    /// builder.Services.AddLogtoAuthentication(
    ///     builder.Environment,
    ///     endpoint: "https://my-app.logto.app",
    ///     clientId: "my-client-id"
    /// );
    /// </code>
    /// </example>
    public static IServiceCollection AddLogtoAuthentication(
        this IServiceCollection services,
        IHostEnvironment environment,
        string endpoint,
        string clientId,
        string? clientSecret = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(environment);
        
        if (string.IsNullOrWhiteSpace(endpoint))
        {
            throw new ArgumentException("Logto endpoint must be provided", nameof(endpoint));
        }
        
        if (string.IsNullOrWhiteSpace(clientId))
        {
            throw new ArgumentException("Logto client ID must be provided", nameof(clientId));
        }

        // Build in-memory configuration
        var configDict = new Dictionary<string, string?>
        {
            [AuthenticationProviderConfigKey] = "Logto",
            ["Authentication:Logto:Endpoint"] = endpoint,
            ["Authentication:Logto:ClientId"] = clientId
        };

        if (!string.IsNullOrWhiteSpace(clientSecret))
        {
            configDict["Authentication:Logto:ClientSecret"] = clientSecret;
        }

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict)
            .Build();

        return services.AddJwtAuthentication(configuration, environment);
    }

    /// <summary>
    /// Adds Auth0 authentication with explicit configuration values.
    /// Use this when you want to provide configuration in code rather than appsettings.json.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="environment">The hosting environment</param>
    /// <param name="domain">Auth0 domain (e.g., https://your-tenant.auth0.com)</param>
    /// <param name="audience">Auth0 API audience</param>
    /// <param name="clientId">Auth0 client ID (optional)</param>
    /// <returns>The service collection for chaining</returns>
    /// <example>
    /// <code>
    /// builder.Services.AddAuth0Authentication(
    ///     builder.Environment,
    ///     domain: "https://my-app.auth0.com",
    ///     audience: "https://my-api"
    /// );
    /// </code>
    /// </example>
    public static IServiceCollection AddAuth0Authentication(
        this IServiceCollection services,
        IHostEnvironment environment,
        string domain,
        string audience,
        string? clientId = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(environment);
        
        if (string.IsNullOrWhiteSpace(domain))
        {
            throw new ArgumentException("Auth0 domain must be provided", nameof(domain));
        }
        
        if (string.IsNullOrWhiteSpace(audience))
        {
            throw new ArgumentException("Auth0 audience must be provided", nameof(audience));
        }

        // Build in-memory configuration
        var configDict = new Dictionary<string, string?>
        {
            [AuthenticationProviderConfigKey] = "Auth0",
            ["Authentication:Auth0:Domain"] = domain,
            ["Authentication:Auth0:Audience"] = audience
        };

        if (!string.IsNullOrWhiteSpace(clientId))
        {
            configDict["Authentication:Auth0:ClientId"] = clientId;
        }

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict)
            .Build();

        return services.AddJwtAuthentication(configuration, environment);
    }

    /// <summary>
    /// Adds simple JWT authentication with a secret key.
    /// Best for development and testing. Not recommended for production.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="environment">The hosting environment</param>
    /// <param name="secretKey">JWT secret key (minimum 32 characters)</param>
    /// <param name="issuer">Token issuer (optional)</param>
    /// <param name="audience">Token audience (optional)</param>
    /// <returns>The service collection for chaining</returns>
    /// <example>
    /// <code>
    /// // For development/testing only
    /// builder.Services.AddSimpleJwtAuthentication(
    ///     builder.Environment,
    ///     secretKey: "your-super-secret-key-at-least-32-characters-long!"
    /// );
    /// </code>
    /// </example>
    public static IServiceCollection AddSimpleJwtAuthentication(
        this IServiceCollection services,
        IHostEnvironment environment,
        string secretKey,
        string issuer = "AppBlueprintAPI",
        string audience = "AppBlueprintClient")
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(environment);
        
        if (string.IsNullOrWhiteSpace(secretKey))
        {
            throw new ArgumentException("Secret key must be provided", nameof(secretKey));
        }

        if (secretKey.Length < 32)
        {
            throw new ArgumentException("Secret key must be at least 32 characters long for security", nameof(secretKey));
        }

        // Build in-memory configuration
        var configDict = new Dictionary<string, string?>
        {
            [AuthenticationProviderConfigKey] = "JWT",
            ["Authentication:JWT:SecretKey"] = secretKey,
            ["Authentication:JWT:Issuer"] = issuer,
            ["Authentication:JWT:Audience"] = audience
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict)
            .Build();

        return services.AddJwtAuthentication(configuration, environment);
    }
}

/// <summary>
/// Supported authentication providers
/// </summary>
public enum AuthProvider
{
    /// <summary>
    /// Logto - Modern open-source identity infrastructure
    /// </summary>
    Logto,
    
    /// <summary>
    /// Auth0 - Popular authentication service
    /// </summary>
    Auth0,
    
    /// <summary>
    /// Simple JWT with secret key (for development/testing)
    /// </summary>
    JWT
}
