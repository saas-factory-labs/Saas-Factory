using AppBlueprint.Infrastructure.Authorization.Providers;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;

namespace AppBlueprint.Infrastructure.Authorization;

public class UserAuthenticationProviderAdapter : IUserAuthenticationProvider, IDisposable
{
    private readonly Providers.IAuthenticationProvider? _provider;
    private readonly IAuthenticationProviderFactory _factory;

    public UserAuthenticationProviderAdapter(IAuthenticationProviderFactory factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        
        try
        {
            _provider = _factory.CreateProvider();
            if (_provider is null)
            {
                Console.Error.WriteLine("Error: Authentication provider factory returned null.");
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error creating authentication provider: {ex.Message}");
            _provider = null;
        }
    }

    public bool IsAuthenticated()
    {
        if (_provider is null)
        {
            Console.Error.WriteLine("Error: Authentication provider is null in IsAuthenticated()");
            return false;
        }
        
        return _provider.IsAuthenticated();
    }

    public async Task<bool> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        if (_provider is null)
        {
            await Console.Error.WriteLineAsync("Error: Authentication provider is null in LoginAsync()");
            throw new InvalidOperationException("Authentication provider is not initialized. Check your authentication configuration.");
        }
        
        var request = new LoginRequest
        {
            Email = email,
            Password = password
        };

        var result = await _provider.LoginAsync(request, cancellationToken);
        return result.IsSuccess;
    }

    public async Task LogoutAsync()
    {
        if (_provider is null)
        {
            await Console.Error.WriteLineAsync("Error: Authentication provider is null in LogoutAsync()");
            return;
        }
        
        await _provider.LogoutAsync();
    }

    public async Task AuthenticateRequestAsync(
        RequestInformation request,
        Dictionary<string, object>? additionalAuthenticationContext = null,
        CancellationToken cancellationToken = default)
    {
        if (_provider is null)
        {
            await Console.Error.WriteLineAsync("Error: Authentication provider is null in AuthenticateRequestAsync()");
            return;
        }
        
        await _provider.AuthenticateRequestAsync(request, additionalAuthenticationContext, cancellationToken);
    }

    public async Task InitializeFromStorageAsync()
    {
        if (_provider is null)
        {
            await Console.Error.WriteLineAsync("Error: Authentication provider is null in InitializeFromStorageAsync()");
            return;
        }
        
        await _provider.InitializeAsync();
    }

    public string? GetLogoutUrl(string postLogoutRedirectUri)
    {
        if (_provider is null)
        {
            Console.Error.WriteLine("Error: Authentication provider is null in GetLogoutUrl()");
            return null;
        }
        
        // Check if the provider is LogtoAuthorizationCodeProvider
        if (_provider is Providers.Logto.LogtoAuthorizationCodeProvider logtoProvider)
        {
            return logtoProvider.GetLogoutUrl(postLogoutRedirectUri);
        }
        
        // For other providers, return null (they may not need a logout URL)
        return null;
    }

    public void Dispose()
    {
        if (_provider is IDisposable disposableProvider)
        {
            disposableProvider.Dispose();
        }
        GC.SuppressFinalize(this);
    }
}