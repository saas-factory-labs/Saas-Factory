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
    public required string Email { get; init; }
    public required string Password { get; init; }
    public Dictionary<string, object> AdditionalProperties { get; set; } = new();
}

public class AuthenticationResult
{
    public bool IsSuccess { get; init; }
    public string? AccessToken { get; init; }
    public string? RefreshToken { get; init; }
    public DateTime? ExpiresAt { get; init; }
    public string? Error { get; set; }
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}
