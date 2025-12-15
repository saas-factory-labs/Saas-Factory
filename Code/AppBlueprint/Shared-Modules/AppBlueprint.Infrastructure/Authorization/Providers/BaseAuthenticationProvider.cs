using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;

namespace AppBlueprint.Infrastructure.Authorization.Providers;

public abstract class BaseAuthenticationProvider : IAuthenticationProvider
{
    protected ITokenStorageService TokenStorage { get; }
    protected string? AccessToken { get; set; }
    protected string? RefreshToken { get; set; }
    protected DateTime TokenExpiration { get; set; } = DateTime.MinValue;

    public event Action? OnAuthenticationStateChanged;

    protected BaseAuthenticationProvider(ITokenStorageService tokenStorage)
    {
        TokenStorage = tokenStorage ?? throw new ArgumentNullException(nameof(tokenStorage));
    }

    public virtual bool IsAuthenticated()
    {
        return !string.IsNullOrEmpty(AccessToken) && DateTime.UtcNow < TokenExpiration;
    }

    public abstract Task<AuthenticationResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    
    public virtual async Task LogoutAsync()
    {
        AccessToken = null;
        RefreshToken = null;
        TokenExpiration = DateTime.MinValue;
        
        await TokenStorage.RemoveTokenAsync();
        OnAuthenticationStateChanged?.Invoke();
    }

    public abstract Task<AuthenticationResult> RefreshTokenAsync(CancellationToken cancellationToken = default);

    public virtual async Task InitializeAsync()
    {
        var storedToken = await TokenStorage.GetTokenAsync();
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
            request.Headers.Add("Authorization", $"Bearer {AccessToken}");
        }

        return Task.CompletedTask;
    }

    protected virtual async Task StoreTokens(AuthenticationResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (result.IsSuccess && !string.IsNullOrEmpty(result.AccessToken))
        {
            AccessToken = result.AccessToken;
            RefreshToken = result.RefreshToken;
            TokenExpiration = result.ExpiresAt ?? DateTime.UtcNow.AddHours(1);

            await TokenStorage.StoreTokenAsync(result.AccessToken);
            OnAuthenticationStateChanged?.Invoke();
        }
    }

    protected abstract Task TryRestoreFromStoredToken(string storedToken);

    protected virtual void NotifyAuthenticationStateChanged()
    {
        OnAuthenticationStateChanged?.Invoke();
    }
}