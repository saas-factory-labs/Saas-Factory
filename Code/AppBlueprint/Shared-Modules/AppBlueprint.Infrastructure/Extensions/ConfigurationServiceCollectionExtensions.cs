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
        services.AddDatabaseContextOptions();
        services.AddMultiTenancyOptions();
        services.AddAuthenticationOptions();
        services.AddExternalServiceOptions();
        services.AddFeatureFlagsOptions();
        
        return services;
    }
    
    /// <summary>
    /// Registers DatabaseContext options with validation.
    /// Supports flat environment variables with UPPERCASE_UNDERSCORE naming:
    /// - DATABASECONTEXT_TYPE (new standard)
    /// - DATABASECONTEXT_CONTEXTTYPE (legacy)
    /// - DATABASECONTEXT_ENABLEHYBRIDMODE
    /// - DATABASECONTEXT_BASELINEONLY
    /// </summary>
    private static IServiceCollection AddDatabaseContextOptions(
        this IServiceCollection services)
    {
        services.AddOptions<DatabaseContextOptions>()
            .BindConfiguration(DatabaseContextOptions.SectionName)
            .Configure<IConfiguration>((options, config) =>
            {
                // Override with flat environment variables
                string prefix = "DATABASECONTEXT_";
                
                // Check TYPE first (new standard), then CONTEXTTYPE (legacy)
                string? contextType = Environment.GetEnvironmentVariable($"{prefix}TYPE")
                                   ?? Environment.GetEnvironmentVariable($"{prefix}CONTEXTTYPE");
                if (!string.IsNullOrWhiteSpace(contextType) && Enum.TryParse<DatabaseContextType>(contextType, ignoreCase: true, out DatabaseContextType parsedType))
                    options.ContextType = parsedType;
                
                string? enableHybridMode = Environment.GetEnvironmentVariable($"{prefix}ENABLEHYBRIDMODE");
                if (!string.IsNullOrWhiteSpace(enableHybridMode) && bool.TryParse(enableHybridMode, out bool hybridMode))
                    options.EnableHybridMode = hybridMode;
                
                string? baselineOnly = Environment.GetEnvironmentVariable($"{prefix}BASELINEONLY");
                if (!string.IsNullOrWhiteSpace(baselineOnly) && bool.TryParse(baselineOnly, out bool baseline))
                    options.BaselineOnly = baseline;
                
                string? commandTimeout = Environment.GetEnvironmentVariable($"{prefix}COMMANDTIMEOUT");
                if (!string.IsNullOrWhiteSpace(commandTimeout) && int.TryParse(commandTimeout, out int timeout))
                    options.CommandTimeout = timeout;
                
                string? maxRetryCount = Environment.GetEnvironmentVariable($"{prefix}MAXRETRYCOUNT");
                if (!string.IsNullOrWhiteSpace(maxRetryCount) && int.TryParse(maxRetryCount, out int retryCount))
                    options.MaxRetryCount = retryCount;
            })
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
        this IServiceCollection services)
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
        this IServiceCollection services)
    {
        services.AddOptions<AuthenticationOptions>()
            .BindConfiguration(AuthenticationOptions.SectionName)
            .Configure<IConfiguration>((options, config) =>
            {
                // Override Provider with flat environment variable if set
                string? provider = Environment.GetEnvironmentVariable("AUTHENTICATION_PROVIDER");
                if (!string.IsNullOrWhiteSpace(provider))
                    options.Provider = provider;
            })
            .ValidateDataAnnotations()
            .ValidateOnStart();
            
        return services;
    }
    
    /// <summary>
    /// Registers external service options (Stripe, Cloudflare R2, Resend) with validation.
    /// </summary>
    private static IServiceCollection AddExternalServiceOptions(
        this IServiceCollection services)
    {
        AddStripeOptions(services);
        AddCloudflareR2Options(services);
        AddResendEmailOptions(services);
        return services;
    }

    private static void AddStripeOptions(IServiceCollection services)
    {
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
    }

    private static void AddCloudflareR2Options(IServiceCollection services)
    {
        services.AddOptions<CloudflareR2Options>()
            .BindConfiguration(CloudflareR2Options.SectionName)
            .Configure<IConfiguration>((options, config) =>
            {
                ConfigureCloudflareR2FromEnvironment(options);
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
    }

    private static void ConfigureCloudflareR2FromEnvironment(CloudflareR2Options options)
    {
        const string prefix = "CLOUDFLARE_R2_";
        
        SetStringOption(prefix, "ACCESSKEYID", v => options.AccessKeyId = v);
        SetStringOption(prefix, "SECRETACCESSKEY", v => options.SecretAccessKey = v);
        SetStringOption(prefix, "ENDPOINTURL", v => options.EndpointUrl = v);
        SetStringOption(prefix, "PRIVATEBUCKETNAME", v => options.PrivateBucketName = v);
        SetStringOption(prefix, "PUBLICBUCKETNAME", v => options.PublicBucketName = v);
        SetStringOption(prefix, "PUBLICDOMAIN", v => options.PublicDomain = v);
        SetIntOption(prefix, "MAXIMAGESIZEMB", v => options.MaxImageSizeMB = v);
        SetIntOption(prefix, "MAXDOCUMENTSIZEMB", v => options.MaxDocumentSizeMB = v);
        SetIntOption(prefix, "MAXVIDEOSIZEMB", v => options.MaxVideoSizeMB = v);
        
        // Debug logging
        Console.WriteLine($"[CloudflareR2Options] Loaded - AccessKeyId: {!string.IsNullOrWhiteSpace(options.AccessKeyId)}, SecretAccessKey: {!string.IsNullOrWhiteSpace(options.SecretAccessKey)}, EndpointUrl: {options.EndpointUrl}, PrivateBucket: {options.PrivateBucketName}, PublicBucket: {options.PublicBucketName}, PublicDomain: {options.PublicDomain}");
    }

    private static void SetStringOption(string prefix, string key, Action<string> setter)
    {
        string? value = Environment.GetEnvironmentVariable($"{prefix}{key}");
        if (!string.IsNullOrWhiteSpace(value))
            setter(value);
    }

    private static void SetIntOption(string prefix, string key, Action<int> setter)
    {
        string? value = Environment.GetEnvironmentVariable($"{prefix}{key}");
        if (!string.IsNullOrWhiteSpace(value) && int.TryParse(value, out int parsedValue))
            setter(parsedValue);
    }

    private static void AddResendEmailOptions(IServiceCollection services)
    {
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
    }
    
    /// <summary>
    /// Registers feature flags options.
    /// </summary>
    private static IServiceCollection AddFeatureFlagsOptions(
        this IServiceCollection services)
    {
        services.AddOptions<FeatureFlagsOptions>()
            .BindConfiguration(FeatureFlagsOptions.SectionName);
            
        return services;
    }
}
