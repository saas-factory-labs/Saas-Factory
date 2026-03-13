using System.Text.Json.Serialization;

namespace AppBlueprint.Infrastructure.Authorization.Providers.Firebase;

public class FirebaseConfiguration
{
    public string ApiKey { get; set; } = string.Empty;
    public string ProjectId { get; set; } = string.Empty;
    public string AuthDomain { get; set; } = string.Empty;
}

internal sealed class FirebaseSignInResponse
{
    [JsonPropertyName("id_token")]
    public string IdToken { get; init; } = string.Empty;
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; init; }
    [JsonPropertyName("expires_in")]
    public string ExpiresIn { get; init; } = string.Empty;
    [JsonPropertyName("local_id")]
    public string LocalId { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public bool Registered { get; init; }
}

internal sealed class FirebaseRefreshResponse
{
    [JsonPropertyName("id_token")]
    public string IdToken { get; init; } = string.Empty;
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; init; }
    [JsonPropertyName("expires_in")]
    public string ExpiresIn { get; init; } = string.Empty;
    [JsonPropertyName("user_id")]
    public string UserId { get; init; } = string.Empty;
    [JsonPropertyName("token_type")]
    public string TokenType { get; init; } = string.Empty;
}

internal sealed class FirebaseErrorResponse
{
    public FirebaseError? Error { get; init; }
}

internal sealed class FirebaseError
{
    public int Code { get; init; }
    public string Message { get; init; } = string.Empty;
}
