using Microsoft.Extensions.DependencyInjection;

namespace AppBlueprint.Infrastructure.Authorization;

/// <summary>
/// Extension methods for registering authentication services
/// </summary>
public static class AuthServiceExtensions
{
    /// <summary>
    /// Adds client-side authentication services to the service collection
    /// </summary>
    public static IServiceCollection AddClientAuthentication(this IServiceCollection services, string authEndpoint)
    {
        // Register TokenStorageService
        services.AddScoped<ITokenStorageService, TokenStorageService>();
        
        // Register HttpClient for authentication
        services.AddHttpClient();
        
        // Register UserAuthenticationProvider with the auth endpoint
        services.AddScoped<IUserAuthenticationProvider>(sp => 
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var tokenStorage = sp.GetRequiredService<ITokenStorageService>();
            var httpClient = httpClientFactory.CreateClient("Auth");
            
            return new UserAuthenticationProvider(httpClient, authEndpoint, tokenStorage);
        });
        
        return services;
    }
}