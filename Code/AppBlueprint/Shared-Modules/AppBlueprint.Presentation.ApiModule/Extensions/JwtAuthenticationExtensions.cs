using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace AppBlueprint.Presentation.ApiModule.Extensions;

/// <summary>
/// Extension methods for configuring JWT authentication in the API.
/// </summary>
public static class JwtAuthenticationExtensions
{
    private const string AuthenticationProviderConfigKey = "Authentication:Provider";

    /// <summary>
    /// Adds JWT Bearer authentication to the service collection.
    /// Supports multiple authentication providers (Auth0, Logto, or custom JWT).
    /// </summary>
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(environment);

        // Read from flat UPPERCASE environment variable first, then fall back to hierarchical config
        string authProvider = Environment.GetEnvironmentVariable("AUTHENTICATION_PROVIDER")
                           ?? configuration[AuthenticationProviderConfigKey]
                           ?? "Logto"; // Default to Logto (only supported provider)

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            ConfigureJwtBearerOptions(options, configuration, authProvider, environment);
        });

        return services;
    }

    private static void ConfigureJwtBearerOptions(
        JwtBearerOptions options,
        IConfiguration configuration,
        string authProvider,
        IHostEnvironment environment)
    {
        // Only Logto authentication is supported
        if (!authProvider.Equals("LOGTO", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Unsupported authentication provider '{authProvider}'. Only 'Logto' is supported. " +
                "Set AUTHENTICATION_PROVIDER=Logto in configuration.");
        }

        ConfigureLogto(options, configuration, environment);

        // Common configuration for all providers
        options.SaveToken = true;

        // Require HTTPS metadata in production
        options.RequireHttpsMetadata = !environment.IsDevelopment();

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILogger<JwtBearerEvents>>();

                var authHeader = context.Request.Headers.Authorization.ToString();
                var tokenPreview = authHeader.Replace("Bearer ", string.Empty, StringComparison.OrdinalIgnoreCase);
                if (tokenPreview.Length > 20)
                {
                    tokenPreview = tokenPreview[..20];
                }

                logger.LogError(context.Exception,
                    "Authentication failed. Token preview: {TokenPreview}, Exception Type: {ExceptionType}, Message: {Message}",
                    tokenPreview,
                    context.Exception?.GetType().Name,
                    context.Exception?.Message);

                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILogger<JwtBearerEvents>>();

                string userName = context.Principal?.Identity?.Name ?? "Unknown";
                string userId = context.Principal?.FindFirst("sub")?.Value ?? "Unknown";
                string issuer = context.Principal?.FindFirst("iss")?.Value ?? "Unknown";

                logger.LogInformation(
                    "Token validated successfully. User: {User}, UserId: {UserId}, Issuer: {Issuer}",
                    userName, userId, issuer);

                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILogger<JwtBearerEvents>>();

                var authHeader = context.Request.Headers.Authorization.ToString();
                var hasToken = !string.IsNullOrEmpty(authHeader);

                logger.LogWarning(
                    "Authorization challenge. Error: {Error}, ErrorDescription: {ErrorDescription}, HasAuthHeader: {HasToken}, Path: {Path}",
                    context.Error,
                    context.ErrorDescription,
                    hasToken,
                    context.Request.Path);

                return Task.CompletedTask;
            },
            OnMessageReceived = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILogger<JwtBearerEvents>>();

                var authHeader = context.Request.Headers.Authorization.ToString();
                var hasToken = !string.IsNullOrEmpty(authHeader);

                logger.LogDebug(
                    "Message received. HasAuthHeader: {HasToken}, Path: {Path}",
                    hasToken,
                    context.Request.Path);

                return Task.CompletedTask;
            }
        };
    }

    private static void ConfigureAuth0(JwtBearerOptions options, IConfiguration configuration)
    {
        var domain = configuration["Authentication:Auth0:Domain"];
        var audience = configuration["Authentication:Auth0:Audience"];

        if (string.IsNullOrEmpty(domain) || string.IsNullOrEmpty(audience))
        {
            throw new InvalidOperationException(
                "Auth0 Domain and Audience must be configured in appsettings.json");
        }

        options.Authority = domain;
        options.Audience = audience;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = domain,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.FromMinutes(5)
        };
    }

    private static void ConfigureLogto(JwtBearerOptions options, IConfiguration configuration, IHostEnvironment environment)
    {
        // Read environment variables first (flat UPPERCASE format), then fall back to hierarchical config
        string? endpoint = Environment.GetEnvironmentVariable("LOGTO_ENDPOINT")
                        ?? Environment.GetEnvironmentVariable("AUTHENTICATION_LOGTO_ENDPOINT")
                        ?? configuration["Authentication:Logto:Endpoint"];
        string? clientId = Environment.GetEnvironmentVariable("LOGTO_APPID")
                        ?? Environment.GetEnvironmentVariable("LOGTO_CLIENTID")
                        ?? Environment.GetEnvironmentVariable("AUTHENTICATION_LOGTO_CLIENTID")
                        ?? Environment.GetEnvironmentVariable("LOGTO_APP_ID")
                        ?? configuration["Authentication:Logto:ClientId"];
        string? apiResource = Environment.GetEnvironmentVariable("LOGTO_APIRESOURCE")
                           ?? Environment.GetEnvironmentVariable("LOGTO_RESOURCE")
                           ?? Environment.GetEnvironmentVariable("AUTHENTICATION_LOGTO_APIRESOURCE")
                           ?? configuration["Authentication:Logto:ApiResource"];

        if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(clientId))
        {
            throw new InvalidOperationException(
                "Logto authentication configuration is missing. Required environment variables:\n" +
                "  - LOGTO_ENDPOINT (e.g., https://32nkyp.logto.app)\n" +
                "  - LOGTO_APPID (e.g., uovd1gg5ef7i1c4w46mt6)\n" +
                "  - LOGTO_APIRESOURCE (e.g., https://api.appblueprint.local)\n" +
                "Set these in AppHost Program.cs or environment variables.");
        }

        // Remove trailing slash from endpoint if present
        endpoint = endpoint.TrimEnd('/');

        options.Authority = $"{endpoint}/oidc";

        // For development: Validate JWT access tokens for API resource
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Validate issuer
            ValidateIssuer = true,
            ValidIssuer = $"{endpoint}/oidc",
            ValidIssuers = new[] { $"{endpoint}/oidc", endpoint },

            // Validate audience if API resource is configured
            ValidateAudience = !string.IsNullOrEmpty(apiResource),
            ValidAudience = apiResource,

            // Accept any valid token from Logto
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            // Development settings - more permissive
            ClockSkew = TimeSpan.FromMinutes(5),
            RequireSignedTokens = true,
            RequireExpirationTime = true,

            // Require audience if API resource is configured
            RequireAudience = !string.IsNullOrEmpty(apiResource),

            // Map standard OIDC claims
            NameClaimType = "name",
            RoleClaimType = "role"
        };

        // Set metadata address explicitly for better error messages
        options.MetadataAddress = $"{endpoint}/oidc/.well-known/openid-configuration";

        // Don't require HTTPS in development
        options.RequireHttpsMetadata = environment.IsDevelopment() ? false : true;
    }

    private static void ConfigureCustomJwt(JwtBearerOptions options, IConfiguration configuration)
    {
        var secretKey = configuration["Authentication:JWT:SecretKey"];
        string issuer = configuration["Authentication:JWT:Issuer"] ?? "AppBlueprintAPI";
        string audience = configuration["Authentication:JWT:Audience"] ?? "AppBlueprintClient";

        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException(
                "JWT SecretKey is missing. Set Authentication:JWT:SecretKey in configuration.");
        }

        var key = Encoding.ASCII.GetBytes(secretKey);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5),
            // Additional security settings
            RequireExpirationTime = true,
            RequireSignedTokens = true
        };
    }
}

