namespace AppBlueprint.Infrastructure.Authorization.Providers.Auth0;

public class Auth0Configuration
{
    public string Domain { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}

internal sealed class Auth0TokenResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public string? RefreshToken { get; init; }
    public int ExpiresIn { get; init; }
    public string TokenType { get; init; } = string.Empty;
}
