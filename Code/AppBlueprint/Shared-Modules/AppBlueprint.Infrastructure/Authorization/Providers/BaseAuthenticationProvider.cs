using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;

namespace AppBlueprint.Infrastructure.Authorization.Providers;

public abstract class BaseAuthenticationProvider : IAuthenticationProvider
{
    protected readonly ITokenStorageService _tokenStorage;
    protected string? _accessToken;
    protected string? _refreshToken;
    protected DateTime _tokenExpiration = DateTime.MinValue;

    public event Action? OnAuthenticationStateChanged;

    protected BaseAuthenticationProvider(ITokenStorageService tokenStorage)
    {
        _tokenStorage = tokenStorage ?? throw new ArgumentNullException(nameof(tokenStorage));
    }

    public virtual bool IsAuthenticated()
    {
        return !string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiration;
    }

    public abstract Task<AuthenticationResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    
    public virtual async Task LogoutAsync()
    {
        _accessToken = null;
        _refreshToken = null;
        _tokenExpiration = DateTime.MinValue;
        
        await _tokenStorage.RemoveTokenAsync();
        OnAuthenticationStateChanged?.Invoke();
    }

    public abstract Task<AuthenticationResult> RefreshTokenAsync(CancellationToken cancellationToken = default);

    public virtual async Task InitializeAsync()
    {
        var storedToken = await _tokenStorage.GetTokenAsync();
        if (!string.IsNullOrEmpty(storedToken))
        {
            await TryRestoreFromStoredToken(storedToken);
        }
    }

    public virtual Task AuthenticateRequestAsync(
        RequestInformation request,
        Dictionary<string, object>? additionalAuthenticationContext = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (IsAuthenticated())
        {
            request.Headers.Add("Authorization", $"Bearer {_accessToken}");
        }

        return Task.CompletedTask;
    }

    protected virtual async Task StoreTokens(AuthenticationResult result)
    {
        if (result.IsSuccess && !string.IsNullOrEmpty(result.AccessToken))
        {
            _accessToken = result.AccessToken;
            _refreshToken = result.RefreshToken;
            _tokenExpiration = result.ExpiresAt ?? DateTime.UtcNow.AddHours(1);

            await _tokenStorage.StoreTokenAsync(result.AccessToken);
            OnAuthenticationStateChanged?.Invoke();
        }
    }

    protected abstract Task TryRestoreFromStoredToken(string storedToken);

    protected virtual void NotifyAuthenticationStateChanged()
    {
        OnAuthenticationStateChanged?.Invoke();
    }
}