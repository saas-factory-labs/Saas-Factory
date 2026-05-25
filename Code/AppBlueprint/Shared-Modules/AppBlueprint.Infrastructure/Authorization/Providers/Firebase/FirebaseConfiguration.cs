namespace AppBlueprint.Infrastructure.Authorization.Providers.Firebase;

public class FirebaseConfiguration
{
    public string ProjectId { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string AuthDomain { get; set; } = string.Empty;
    public string? DatabaseUrl { get; set; }
    public string? StorageBucket { get; set; }
    public string? MessagingSenderId { get; set; }
    public string? AppId { get; set; }
    public string? MeasurementId { get; set; }

    public bool IsValid() =>
        !string.IsNullOrEmpty(ProjectId) &&
        !string.IsNullOrEmpty(ApiKey) &&
        !string.IsNullOrEmpty(AuthDomain);
}
