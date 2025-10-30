using AppBlueprint.Infrastructure.Authorization.Providers;
using AppBlueprint.Infrastructure.Authorization.Providers.Auth0;
using AppBlueprint.Infrastructure.Authorization.Providers.Logto;
using AppBlueprint.Infrastructure.Authorization.Providers.Mock;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.Authorization;

public interface IAuthenticationProviderFactory
{
    IAuthenticationProvider CreateProvider();
    AuthenticationProviderType GetConfiguredProviderType();
}

public class AuthenticationProviderFactory : IAuthenticationProviderFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public AuthenticationProviderFactory(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public IAuthenticationProvider CreateProvider()
    {
        var providerType = GetConfiguredProviderType();

        return providerType switch
        {
            AuthenticationProviderType.Auth0 => CreateAuth0Provider(),
            AuthenticationProviderType.Logto => CreateLogtoProvider(),
            AuthenticationProviderType.Mock => CreateMockProvider(),
            _ => throw new InvalidOperationException($"Unsupported authentication provider: {providerType}")
        };
    }

    public AuthenticationProviderType GetConfiguredProviderType()
    {
        var providerName = _configuration["Authentication:Provider"] ?? "Mock";
        
        if (!Enum.TryParse<AuthenticationProviderType>(providerName, true, out var providerType))
        {
            throw new InvalidOperationException($"Invalid authentication provider configured: {providerName}. " +
                $"Valid options are: {string.Join(", ", Enum.GetNames<AuthenticationProviderType>())}");
        }

        return providerType;
    }

    private IAuthenticationProvider CreateAuth0Provider()
    {
        var tokenStorage = _serviceProvider.GetRequiredService<ITokenStorageService>();
        var httpClient = _serviceProvider.GetRequiredService<HttpClient>();
        var logger = _serviceProvider.GetRequiredService<ILogger<Auth0Provider>>();

        return new Auth0Provider(tokenStorage, httpClient, _configuration, logger);
    }

    private IAuthenticationProvider CreateLogtoProvider()
    {
        var tokenStorage = _serviceProvider.GetRequiredService<ITokenStorageService>();
        var httpClient = _serviceProvider.GetRequiredService<HttpClient>();
        var logger = _serviceProvider.GetRequiredService<ILogger<LogtoProvider>>();

        return new LogtoProvider(tokenStorage, httpClient, _configuration, logger);
    }

    private IAuthenticationProvider CreateMockProvider()
    {
        var tokenStorage = _serviceProvider.GetRequiredService<ITokenStorageService>();
        var logger = _serviceProvider.GetRequiredService<ILogger<MockProvider>>();

        return new MockProvider(tokenStorage, logger);
    }
}

public enum AuthenticationProviderType
{
    Mock,
    Auth0,
    Logto
}