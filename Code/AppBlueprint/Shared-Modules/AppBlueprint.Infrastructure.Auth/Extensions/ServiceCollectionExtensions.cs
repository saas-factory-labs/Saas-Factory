using AppBlueprint.Application.Options;
using AppBlueprint.Infrastructure.Authentication;
using AppBlueprint.Infrastructure.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AppBlueprint.Infrastructure.Auth.Extensions;

/// <summary>
/// Extension methods for registering AppBlueprint authentication and authorization services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds AppBlueprint authentication and authorization services including Logto, Firebase,
    /// JWT bearer, cookie auth, data protection, and token storage.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="environment">The hosting environment.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAppBlueprintAuth(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(environment);

        // Register authentication options
        services.Configure<AuthenticationOptions>(
            configuration.GetSection(AuthenticationOptions.SectionName));

        // Register web authentication (Logto, cookies, data protection)
        services.AddWebAuthentication(configuration, environment);

        // Register token storage service
        services.AddScoped<ITokenStorageService, TokenStorageService>();

        // Register HttpClient for authentication providers
        services.AddHttpClient("AuthenticationProvider");

        // Register the authentication provider factory
        services.AddScoped<IAuthenticationProviderFactory, AuthenticationProviderFactory>();

        // Register IAuthenticationProvider using the factory
        services.AddScoped<AppBlueprint.Infrastructure.Authorization.Providers.IAuthenticationProvider>(sp =>
        {
            var factory = sp.GetRequiredService<IAuthenticationProviderFactory>();
            return factory.CreateProvider();
        });

        // Register legacy UserAuthenticationProvider using adapter
        services.AddScoped<IUserAuthenticationProvider, UserAuthenticationProviderAdapter>();

        // Register Kiota IAuthenticationProvider interface
        services.AddScoped<Microsoft.Kiota.Abstractions.Authentication.IAuthenticationProvider>(sp =>
            sp.GetRequiredService<IUserAuthenticationProvider>());

        return services;
    }
}
