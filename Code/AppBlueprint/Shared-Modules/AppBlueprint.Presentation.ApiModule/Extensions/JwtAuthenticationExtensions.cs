using AppBlueprint.Application.Constants;
using AppBlueprint.Infrastructure.Authentication;
using AppBlueprint.Infrastructure.Authentication.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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

            // When a request carries only x-api-key (no Bearer token), forward to the ApiKey scheme.
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            ConfigureJwtBearerOptions(options, configuration, authProvider, environment);
        })
        .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
            ApiKeyAuthenticationHandler.SchemeName,
            _ => { });

        services.AddSingleton<IAuthorizationHandler, MinimumAgeHandler>();

        services.AddAuthorization(options =>
        {
            // Policy that accepts either a valid JWT or a valid API key.
            options.AddPolicy(AuthorizationPolicyNames.ApiKey, policy =>
                policy.AddAuthenticationSchemes(
                        JwtBearerDefaults.AuthenticationScheme,
                        ApiKeyAuthenticationHandler.SchemeName)
                    .RequireAuthenticatedUser());

            options.AddPolicy(AuthorizationPolicyNames.Over18, policy =>
                policy.Requirements.Add(new MinimumAgeRequirement(18)));

            options.AddPolicy(AuthorizationPolicyNames.AdminOnly, policy =>
                policy.RequireRole(Roles.DeploymentManagerAdmin));

            options.AddPolicy(AuthorizationPolicyNames.UserOrAdmin, policy =>
                policy.RequireRole(
                    Roles.User,
                    Roles.TenantAdmin,
                    Roles.DeploymentManagerAdmin));
        });

        return services;
    }

    private static void ConfigureJwtBearerOptions(
        JwtBearerOptions options,
        IConfiguration configuration,
        string authProvider,
        IHostEnvironment environment)
    {
        if (authProvider.Equals("LOGTO", StringComparison.OrdinalIgnoreCase))
        {
            ConfigureLogto(options, configuration, environment);
        }
        else if (authProvider.Equals("FIREBASE", StringComparison.OrdinalIgnoreCase))
        {
            ConfigureFirebase(options, configuration, environment);
        }
        else
        {
            throw new InvalidOperationException(
                $"Unsupported authentication provider '{authProvider}'. Supported providers: 'Logto', 'Firebase'. " +
                "Set AUTHENTICATION_PROVIDER in configuration.");
        }

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

                // SECURITY (OWASP A09): never log token material (not even a prefix) -
                // log only whether a token was present plus the failure cause.
                bool hadAuthHeader = !string.IsNullOrEmpty(context.Request.Headers.Authorization.ToString());

                logger.LogError(context.Exception,
                    "Authentication failed. HasAuthHeader: {HasAuthHeader}, Exception Type: {ExceptionType}, Message: {Message}",
                    hadAuthHeader,
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
                    "Authorization challenge. Error: {Error}, ErrorDescription: {ErrorDescription}, HasAuthHeader: {HasToken}",
                    context.Error,
                    context.ErrorDescription,
                    hasToken);

                return Task.CompletedTask;
            },
            OnMessageReceived = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILogger<JwtBearerEvents>>();

                var authHeader = context.Request.Headers.Authorization.ToString();
                var hasToken = !string.IsNullOrEmpty(authHeader);

                logger.LogDebug(
                    "Message received. HasAuthHeader: {HasToken}",
                    hasToken);

                return Task.CompletedTask;
            }
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

        // SECURITY (OWASP A02/A07): audience validation must not be silently skipped in production.
        // A missing API resource there means tokens for any audience would be accepted, so fail fast.
        if (!environment.IsDevelopment() && string.IsNullOrEmpty(apiResource))
        {
            throw new InvalidOperationException(
                "LOGTO_APIRESOURCE must be configured in non-development environments so JWT audience " +
                "validation is enforced. Set LOGTO_APIRESOURCE (e.g. https://api.appblueprint.local).");
        }

        bool validateAudience = !string.IsNullOrEmpty(apiResource);

        options.Authority = $"{endpoint}/oidc";

        // For development: Validate JWT access tokens for API resource
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Validate issuer
            ValidateIssuer = true,
            ValidIssuer = $"{endpoint}/oidc",
            ValidIssuers = new[] { $"{endpoint}/oidc", endpoint },

            // Validate audience whenever an API resource is configured (always true in production).
            ValidateAudience = validateAudience,
            ValidAudience = apiResource,

            // Accept any valid token from Logto
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            // Development settings - more permissive
            ClockSkew = TimeSpan.FromMinutes(5),
            RequireSignedTokens = true,
            RequireExpirationTime = true,

            // Require audience whenever it is being validated.
            RequireAudience = validateAudience,

            // Map standard OIDC claims
            NameClaimType = "name",
            RoleClaimType = "role"
        };

        // Set metadata address explicitly for better error messages
        options.MetadataAddress = $"{endpoint}/oidc/.well-known/openid-configuration";

        // Don't require HTTPS in development
        options.RequireHttpsMetadata = !environment.IsDevelopment();
    }

    private static void ConfigureFirebase(JwtBearerOptions options, IConfiguration configuration, IHostEnvironment environment)
    {
        // Read project ID from flat UPPERCASE env var first, then hierarchical config
        string? projectId = Environment.GetEnvironmentVariable("FIREBASE_PROJECT_ID")
                         ?? Environment.GetEnvironmentVariable("AUTHENTICATION_FIREBASE_PROJECTID")
                         ?? configuration["Authentication:Firebase:ProjectId"];

        if (string.IsNullOrEmpty(projectId))
        {
            throw new InvalidOperationException(
                "Firebase authentication configuration is missing. Required environment variable:\n" +
                "  - FIREBASE_PROJECT_ID (e.g., my-firebase-project)\n" +
                "Set this in AppHost Program.cs or environment variables.");
        }

        string issuer = $"https://securetoken.google.com/{projectId}";

        options.Authority = issuer;
        options.MetadataAddress = $"{issuer}/.well-known/openid-configuration";

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,

            ValidateAudience = true,
            ValidAudience = projectId,

            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ClockSkew = TimeSpan.FromMinutes(5),
            RequireSignedTokens = true,
            RequireExpirationTime = true,

            NameClaimType = "name",
            RoleClaimType = "role"
        };

        options.RequireHttpsMetadata = !environment.IsDevelopment();
    }
}

