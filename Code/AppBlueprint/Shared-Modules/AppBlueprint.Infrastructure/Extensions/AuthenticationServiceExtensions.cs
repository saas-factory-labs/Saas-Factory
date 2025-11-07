using AppBlueprint.Infrastructure.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Kiota.Abstractions.Authentication;

namespace AppBlueprint.Infrastructure.Extensions
{
    /// <summary>
    /// Extension methods for setting up authentication services in an <see cref="IServiceCollection" />.
    /// </summary>
    public static class AuthenticationServiceExtensions
    {
        /// <summary>
        /// Adds the authentication services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddAuthenticationServices(this IServiceCollection services)
        {
            // Register TokenStorageService for client-side token management
            services.AddScoped<ITokenStorageService, TokenStorageService>();

            // Register HttpClient for authentication providers
            services.AddHttpClient<IAuthenticationProviderFactory>();

            // Register the authentication provider factory
            services.AddScoped<IAuthenticationProviderFactory, AuthenticationProviderFactory>();

            // Register UserAuthenticationProvider using the factory pattern with adapter
            services.AddScoped<IUserAuthenticationProvider, UserAuthenticationProviderAdapter>();
            
            // Register IAuthenticationProvider (Kiota interface) using the same adapter
            services.AddScoped<IAuthenticationProvider>(sp => sp.GetRequiredService<IUserAuthenticationProvider>());

            return services;
        }
    }
}