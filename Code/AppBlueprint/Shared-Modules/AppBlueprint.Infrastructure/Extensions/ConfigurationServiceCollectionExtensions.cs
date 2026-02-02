using AppBlueprint.Application.Interfaces.Configuration;
using AppBlueprint.Application.Options;
using AppBlueprint.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AppBlueprint.Infrastructure.Extensions;

/// <summary>
/// Extension methods for configuring application configuration services.
/// </summary>
public static class ConfigurationServiceCollectionExtensions
{
    /// <summary>
    /// Registers all AppBlueprint configuration options with validation.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <param name="environment">The hosting environment.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAppBlueprintConfiguration(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(environment);
        
        // Register configuration service
        services.AddSingleton<IConfigurationService, ConfigurationService>();
        
        // Register all options with validation
        services.AddDatabaseContextOptions(configuration);
        services.AddMultiTenancyOptions(configuration);
        services.AddAuthenticationOptions(configuration);
        services.AddExternalServiceOptions(configuration);
        services.AddFeatureFlagsOptions(configuration);
        
        return services;
    }
    
    /// <summary>
    /// Registers DatabaseContext options with validation.
    /// </summary>
    private static IServiceCollection AddDatabaseContextOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<DatabaseContextOptions>()
            .BindConfiguration(DatabaseContextOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart()
            .Validate(options =>
            {
                options.Validate();
                return true;
            });
            
        return services;
    }
    
    /// <summary>
    /// Registers MultiTenancy options with validation.
    /// </summary>
    private static IServiceCollection AddMultiTenancyOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<MultiTenancyOptions>()
            .BindConfiguration(MultiTenancyOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
            
        return services;
    }
    
    /// <summary>
    /// Registers Authentication options with validation.
    /// </summary>
    private static IServiceCollection AddAuthenticationOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<AuthenticationOptions>()
            .BindConfiguration(AuthenticationOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
            
        return services;
    }
    
    /// <summary>
    /// Registers external service options (Stripe, Cloudflare R2, Resend) with validation.
    /// </summary>
    private static IServiceCollection AddExternalServiceOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Stripe - optional, only validate if configured
        services.AddOptions<StripeOptions>()
            .BindConfiguration(StripeOptions.SectionName)
            .ValidateOnStart()
            .Validate(options =>
            {
                // Only validate if ApiKey is provided
                if (!string.IsNullOrWhiteSpace(options.ApiKey))
                {
                    options.Validate();
                }
                return true;
            });
        
        // Cloudflare R2 - optional, only validate if configured
        // Support both appsettings.json format (Cloudflare:R2:*) and environment variable format (CLOUDFLARE_R2_*)
        services.AddOptions<CloudflareR2Options>()
            .BindConfiguration(CloudflareR2Options.SectionName)
            .Configure<IConfiguration>((options, config) =>
            {
                // Try both prefixes - use APPBLUEPRINT_ first (development), then fall back to no prefix (production)
                string[] prefixes = ["APPBLUEPRINT_CLOUDFLARE_R2_", "CLOUDFLARE_R2_"];
                
                foreach (string prefix in prefixes)
                {
                    string? accessKeyId = Environment.GetEnvironmentVariable($"{prefix}ACCESSKEYID");
                    if (!string.IsNullOrWhiteSpace(accessKeyId))
                        options.AccessKeyId = accessKeyId;
                    
                    string? secretAccessKey = Environment.GetEnvironmentVariable($"{prefix}SECRETACCESSKEY");
                    if (!string.IsNullOrWhiteSpace(secretAccessKey))
                        options.SecretAccessKey = secretAccessKey;
                    
                    string? endpointUrl = Environment.GetEnvironmentVariable($"{prefix}ENDPOINTURL");
                    if (!string.IsNullOrWhiteSpace(endpointUrl))
                        options.EndpointUrl = endpointUrl;
                    
                    string? privateBucketName = Environment.GetEnvironmentVariable($"{prefix}PRIVATEBUCKETNAME");
                    if (!string.IsNullOrWhiteSpace(privateBucketName))
                        options.PrivateBucketName = privateBucketName;
                    
                    string? publicBucketName = Environment.GetEnvironmentVariable($"{prefix}PUBLICBUCKETNAME");
                    if (!string.IsNullOrWhiteSpace(publicBucketName))
                        options.PublicBucketName = publicBucketName;
                    
                    string? publicDomain = Environment.GetEnvironmentVariable($"{prefix}PUBLICDOMAIN");
                    if (!string.IsNullOrWhiteSpace(publicDomain))
                        options.PublicDomain = publicDomain;
                    
                    string? maxImageSizeMB = Environment.GetEnvironmentVariable($"{prefix}MAXIMAGESIZEMB");
                    if (!string.IsNullOrWhiteSpace(maxImageSizeMB) && int.TryParse(maxImageSizeMB, out int imageSizeMB))
                        options.MaxImageSizeMB = imageSizeMB;
                    
                    string? maxDocumentSizeMB = Environment.GetEnvironmentVariable($"{prefix}MAXDOCUMENTSIZEMB");
                    if (!string.IsNullOrWhiteSpace(maxDocumentSizeMB) && int.TryParse(maxDocumentSizeMB, out int documentSizeMB))
                        options.MaxDocumentSizeMB = documentSizeMB;
                    
                    string? maxVideoSizeMB = Environment.GetEnvironmentVariable($"{prefix}MAXVIDEOSIZEMB");
                    if (!string.IsNullOrWhiteSpace(maxVideoSizeMB) && int.TryParse(maxVideoSizeMB, out int videoSizeMB))
                        options.MaxVideoSizeMB = videoSizeMB;
                }
                
                // Debug logging
                Console.WriteLine($"[CloudflareR2Options] Loaded - AccessKeyId: {!string.IsNullOrWhiteSpace(options.AccessKeyId)}, SecretAccessKey: {!string.IsNullOrWhiteSpace(options.SecretAccessKey)}, EndpointUrl: {options.EndpointUrl}, PrivateBucket: {options.PrivateBucketName}, PublicBucket: {options.PublicBucketName}, PublicDomain: {options.PublicDomain}");
            })
            .Validate(options =>
            {
                // Only validate if any credential is configured
                bool hasCredentials = !string.IsNullOrWhiteSpace(options.AccessKeyId) || 
                                      !string.IsNullOrWhiteSpace(options.SecretAccessKey) ||
                                      !string.IsNullOrWhiteSpace(options.EndpointUrl);
                
                if (hasCredentials)
                {
                    options.Validate();
                }
                
                return true;
            });
        
        // Resend Email - optional, only validate if configured
        services.AddOptions<ResendEmailOptions>()
            .BindConfiguration(ResendEmailOptions.SectionName)
            .ValidateOnStart()
            .Validate(options =>
            {
                // Only validate if ApiKey is provided
                if (!string.IsNullOrWhiteSpace(options.ApiKey))
                {
                    options.Validate();
                }
                return true;
            });
            
        return services;
    }
    
    /// <summary>
    /// Registers feature flags options.
    /// </summary>
    private static IServiceCollection AddFeatureFlagsOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<FeatureFlagsOptions>()
            .BindConfiguration(FeatureFlagsOptions.SectionName);
            
        return services;
    }
}
