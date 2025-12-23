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
            .ValidateDataAnnotations()
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
        services.AddOptions<CloudflareR2Options>()
            .BindConfiguration(CloudflareR2Options.SectionName)
            .ValidateDataAnnotations()
            .Validate(options =>
            {
                // Only validate if any key is provided
                if (!string.IsNullOrWhiteSpace(options.AccessKeyId) ||
                    !string.IsNullOrWhiteSpace(options.EndpointUrl))
                {
                    options.Validate();
                }
                return true;
            });
        
        // Resend Email - optional, only validate if configured
        services.AddOptions<ResendEmailOptions>()
            .BindConfiguration(ResendEmailOptions.SectionName)
            .ValidateDataAnnotations()
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
