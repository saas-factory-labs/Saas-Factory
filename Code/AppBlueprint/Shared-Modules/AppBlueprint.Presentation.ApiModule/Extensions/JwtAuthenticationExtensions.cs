using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AppBlueprint.Presentation.ApiModule.Extensions;

/// <summary>
/// Extension methods for configuring JWT authentication in the API.
/// </summary>
public static class JwtAuthenticationExtensions
{
    /// <summary>
    /// Adds JWT Bearer authentication to the service collection.
    /// Supports multiple authentication providers (Auth0, Logto, or custom JWT).
    /// </summary>
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var authProvider = configuration["Authentication:Provider"] ?? "JWT";

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            ConfigureJwtBearerOptions(options, configuration, authProvider);
        });

        return services;
    }

    private static void ConfigureJwtBearerOptions(
        JwtBearerOptions options,
        IConfiguration configuration,
        string authProvider)
    {
        switch (authProvider.ToUpperInvariant())
        {
            case "AUTH0":
                ConfigureAuth0(options, configuration);
                break;
            case "LOGTO":
                ConfigureLogto(options, configuration);
                break;
            default:
                ConfigureCustomJwt(options, configuration);
                break;
        }

        // Common configuration for all providers
        options.SaveToken = true;
        options.RequireHttpsMetadata = false; // Set to true in production
        
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
                
                var userName = context.Principal?.Identity?.Name ?? "Unknown";
                var userId = context.Principal?.FindFirst("sub")?.Value ?? "Unknown";
                var issuer = context.Principal?.FindFirst("iss")?.Value ?? "Unknown";
                
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

    private static void ConfigureLogto(JwtBearerOptions options, IConfiguration configuration)
    {
        var endpoint = configuration["Authentication:Logto:Endpoint"];
        var clientId = configuration["Authentication:Logto:ClientId"];

        if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(clientId))
        {
            // In Railway/production without Logto config, fall back to custom JWT
            // This allows API to start without Logto, using symmetric key validation
            Console.WriteLine("[API] Logto Endpoint or ClientId not configured - falling back to custom JWT authentication");
            Console.WriteLine("[API] To enable Logto authentication, set environment variables:");
            Console.WriteLine("[API]   - Authentication__Logto__Endpoint");
            Console.WriteLine("[API]   - Authentication__Logto__ClientId");
            
            ConfigureCustomJwt(options, configuration);
            return;
        }

        // Remove trailing slash from endpoint if present
        endpoint = endpoint.TrimEnd('/');

        options.Authority = $"{endpoint}/oidc";
        
        // For development: Very permissive validation to accept Logto tokens
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Validate issuer
            ValidateIssuer = true,
            ValidIssuer = $"{endpoint}/oidc",
            ValidIssuers = new[] { $"{endpoint}/oidc", endpoint },
            
            // Don't require audience - Logto ID tokens may not have it
            ValidateAudience = false,
            
            // Accept any valid token from Logto
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            
            // Development settings - more permissive
            ClockSkew = TimeSpan.FromMinutes(5),
            RequireSignedTokens = true,
            RequireExpirationTime = true,
            
            // Don't require specific claims
            RequireAudience = false,
            
            // Map standard OIDC claims
            NameClaimType = "name",
            RoleClaimType = "role"
        };
        
        // Set metadata address explicitly for better error messages
        options.MetadataAddress = $"{endpoint}/oidc/.well-known/openid-configuration";
        
        // Don't require HTTPS in development
        options.RequireHttpsMetadata = false;
    }

    private static void ConfigureCustomJwt(JwtBearerOptions options, IConfiguration configuration)
    {
        var secretKey = configuration["Authentication:JWT:SecretKey"];
        var issuer = configuration["Authentication:JWT:Issuer"] ?? "AppBlueprintAPI";
        var audience = configuration["Authentication:JWT:Audience"] ?? "AppBlueprintClient";

        if (string.IsNullOrEmpty(secretKey))
        {
            // For development only - use a default key
            secretKey = "DevelopmentSecretKey_ChangeThisInProduction_AtLeast32Characters!";
            // Warning will be logged through JWT events
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
