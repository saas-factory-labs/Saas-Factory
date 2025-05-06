using AppBlueprint.Infrastructure.Authorization;
using Microsoft.Extensions.DependencyInjection;

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
            
            // Register UserAuthenticationProvider for authentication operations using the factory pattern
            services.AddScoped<IUserAuthenticationProvider>(sp => 
            {
                var tokenStorage = sp.GetRequiredService<ITokenStorageService>();
                return new UserAuthenticationProvider(tokenStorage);
            });
            
            return services;
        }
    }
}