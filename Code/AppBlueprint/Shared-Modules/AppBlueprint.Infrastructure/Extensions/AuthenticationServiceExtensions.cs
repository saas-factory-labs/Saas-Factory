using AppBlueprint.Application.Options;
using AppBlueprint.Infrastructure.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AppBlueprint.Infrastructure.Extensions;

/// <summary>
/// Extension methods for setting up authentication services in an <see cref="IServiceCollection" />.
/// </summary>
public static class AuthenticationServiceExtensions
{
    /// <summary>
    /// Adds authentication services with configuration-based provider selection.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration containing Authentication section</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddAppBlueprintAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        // Bind authentication options from configuration
        services.Configure<AuthenticationOptions>(
            configuration.GetSection(AuthenticationOptions.SectionName));

        // Register TokenStorageService for client-side token management
        services.AddScoped<ITokenStorageService, TokenStorageService>();

        // Register HttpClient for authentication providers
        services.AddHttpClient("AuthenticationProvider");

        // Register the authentication provider factory
        services.AddScoped<IAuthenticationProviderFactory, AuthenticationProviderFactory>();

        // Register IAuthenticationProvider using the factory
        services.AddScoped<Authorization.Providers.IAuthenticationProvider>(sp =>
        {
            var factory = sp.GetRequiredService<IAuthenticationProviderFactory>();
            return factory.CreateProvider();
        });

        // Register legacy UserAuthenticationProvider using adapter if needed
        services.AddScoped<IUserAuthenticationProvider, UserAuthenticationProviderAdapter>();

        // Register Kiota IAuthenticationProvider interface
        services.AddScoped<Microsoft.Kiota.Abstractions.Authentication.IAuthenticationProvider>(sp =>
            sp.GetRequiredService<IUserAuthenticationProvider>());

        return services;
    }

    /// <summary>
    /// Adds authentication services to the specified <see cref="IServiceCollection" />.
    /// Legacy method for backward compatibility - use AddAppBlueprintAuthentication instead.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    [Obsolete("Use AddAppBlueprintAuthentication(services, configuration) instead for configuration-based provider selection.")]
    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services)
    {
        // Register TokenStorageService for client-side token management
        services.AddScoped<ITokenStorageService, TokenStorageService>();

        // Register HttpClient for authentication providers
        services.AddHttpClient("AuthenticationProvider");

        // Register UserAuthenticationProvider using the factory pattern with adapter
        services.AddScoped<IUserAuthenticationProvider, UserAuthenticationProviderAdapter>();

        // Register IAuthenticationProvider (Kiota interface) using the same adapter
        services.AddScoped<Microsoft.Kiota.Abstractions.Authentication.IAuthenticationProvider>(sp => sp.GetRequiredService<IUserAuthenticationProvider>());

        return services;
    }
}