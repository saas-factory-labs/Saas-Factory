using Microsoft.Kiota.Abstractions.Authentication;

namespace AppBlueprint.Infrastructure.Authorization.Providers;

public interface IAuthenticationProvider : Microsoft.Kiota.Abstractions.Authentication.IAuthenticationProvider
{
    bool IsAuthenticated();
    Task<AuthenticationResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task LogoutAsync();
    Task<AuthenticationResult> RefreshTokenAsync(CancellationToken cancellationToken = default);
    Task InitializeAsync();

    event Action? OnAuthenticationStateChanged;
}

public class LoginRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public Dictionary<string, object> AdditionalProperties { get; set; } = new();
}

public class AuthenticationResult
{
    public bool IsSuccess { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? Error { get; set; }
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}
