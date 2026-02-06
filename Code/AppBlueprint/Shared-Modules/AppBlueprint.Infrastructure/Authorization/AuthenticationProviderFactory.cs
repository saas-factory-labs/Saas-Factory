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

/// <summary>
/// Factory for creating authentication providers based on configuration.
/// Supports: Auth0, Logto, Azure AD B2C, AWS Cognito, JWT, Mock
/// </summary>
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
            AuthenticationProviderType.AzureAD => CreateAzureADProvider(),
            AuthenticationProviderType.Cognito => CreateCognitoProvider(),
            AuthenticationProviderType.Firebase => CreateFirebaseProvider(),
            AuthenticationProviderType.JWT => CreateJwtProvider(),
            AuthenticationProviderType.Mock => CreateMockProvider(),
            _ => throw new InvalidOperationException($"Unsupported authentication provider: {providerType}")
        };
    }

    public AuthenticationProviderType GetConfiguredProviderType()
    {
        string providerName = _configuration["Authentication:Provider"] ?? "Mock";

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

        // Check if we should use Authorization Code Flow (recommended)
        var useAuthCodeFlow = _configuration.GetValue<bool>("Authentication:Logto:UseAuthorizationCodeFlow", true);

        if (useAuthCodeFlow)
        {
            var logger = _serviceProvider.GetRequiredService<ILogger<LogtoAuthorizationCodeProvider>>();
            return new LogtoAuthorizationCodeProvider(tokenStorage, httpClient, _configuration, logger);
        }
        else
        {
            var logger = _serviceProvider.GetRequiredService<ILogger<LogtoProvider>>();
            return new LogtoProvider(tokenStorage, httpClient, _configuration, logger);
        }
    }

    private IAuthenticationProvider CreateAzureADProvider()
    {
        // TODO: Implement Azure AD B2C provider
        throw new NotImplementedException(
            "Azure AD B2C authentication provider is not yet implemented. " +
            "Supported providers: Auth0, Logto, JWT, Mock. " +
            "Please set Authentication:Provider to one of these in your configuration.");
    }

    private IAuthenticationProvider CreateCognitoProvider()
    {
        // TODO: Implement AWS Cognito provider
        throw new NotImplementedException(
            "AWS Cognito authentication provider is not yet implemented. " +
            "Supported providers: Auth0, Logto, JWT, Mock. " +
            "Please set Authentication:Provider to one of these in your configuration.");
    }

    private IAuthenticationProvider CreateFirebaseProvider()
    {
        // TODO: Implement Firebase Authentication provider
        throw new NotImplementedException(
            "Firebase Authentication provider is not yet implemented. " +
            "Supported providers: Auth0, Logto, Mock. " +
            "Please set Authentication:Provider to one of these in your configuration.");
    }

    private IAuthenticationProvider CreateJwtProvider()
    {
        // TODO: Implement simple JWT provider for development/testing
        throw new NotImplementedException(
            "JWT authentication provider is not yet implemented. " +
            "Supported providers: Auth0, Logto, Mock. " +
            "Please set Authentication:Provider to one of these in your configuration.");
    }

    private IAuthenticationProvider CreateMockProvider()
    {
        var tokenStorage = _serviceProvider.GetRequiredService<ITokenStorageService>();
        var logger = _serviceProvider.GetRequiredService<ILogger<MockProvider>>();

        return new MockProvider(tokenStorage, logger);
    }
}

/// <summary>
/// Supported authentication provider types
/// </summary>
public enum AuthenticationProviderType
{
    /// <summary>
    /// Mock provider for testing (no real authentication)
    /// </summary>
    Mock,

    /// <summary>
    /// Auth0 authentication platform
    /// </summary>
    Auth0,

    /// <summary>
    /// Logto open-source identity solution
    /// </summary>
    Logto,

    /// <summary>
    /// Azure AD B2C (Coming soon)
    /// </summary>
    AzureAD,

    /// <summary>
    /// AWS Cognito (Coming soon)
    /// </summary>
    Cognito,

    /// <summary>
    /// Firebase Authentication (Coming soon)
    /// </summary>
    Firebase,

    /// <summary>
    /// Simple JWT-based authentication (Coming soon - for development/testing only)
    /// </summary>
    JWT
}
