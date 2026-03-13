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
    [JsonPropertyName("idToken")]
    public string IdToken { get; init; } = string.Empty;
    [JsonPropertyName("refreshToken")]
    public string? RefreshToken { get; init; }
    [JsonPropertyName("expiresIn")]
    public string ExpiresIn { get; init; } = string.Empty;
    [JsonPropertyName("localId")]
    public string LocalId { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public bool Registered { get; init; }
}

internal sealed class FirebaseErrorResponse
{
    [JsonPropertyName("error")]
    public FirebaseError? Error { get; init; }
}

internal sealed class FirebaseError
{
    [JsonPropertyName("code")]
    public int Code { get; init; }

    [JsonPropertyName("message")]
    public string Message { get; init; } = string.Empty;

    // Firebase sender ofte et array af fejl-detaljer med
    [JsonPropertyName("errors")]
    public List<FirebaseErrorDetail>? Errors { get; init; }
}

internal sealed class FirebaseErrorDetail
{
    [JsonPropertyName("message")]
    public string Message { get; init; } = string.Empty;
    
    [JsonPropertyName("domain")]
    public string Domain { get; init; } = string.Empty;
    
    [JsonPropertyName("reason")]
    public string Reason { get; init; } = string.Empty;
}
