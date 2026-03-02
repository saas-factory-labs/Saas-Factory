namespace AppBlueprint.Infrastructure.Authorization.Providers.Firebase;

public class FirebaseConfiguration
{
    public string ApiKey { get; set; } = string.Empty;
    public string ProjectId { get; set; } = string.Empty;
    public string AuthDomain { get; set; } = string.Empty;
}

internal sealed class FirebaseSignInResponse
{
    public string IdToken { get; init; } = string.Empty;
    public string? RefreshToken { get; init; }
    public string ExpiresIn { get; init; } = string.Empty;
    public string LocalId { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public bool Registered { get; init; }
}

internal sealed class FirebaseRefreshResponse
{
    public string Id_token { get; init; } = string.Empty;
    public string? Refresh_token { get; init; }
    public string Expires_in { get; init; } = string.Empty;
    public string User_id { get; init; } = string.Empty;
    public string Token_type { get; init; } = string.Empty;
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
