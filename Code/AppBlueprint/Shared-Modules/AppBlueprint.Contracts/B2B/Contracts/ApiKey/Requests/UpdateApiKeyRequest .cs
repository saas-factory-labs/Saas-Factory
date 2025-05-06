namespace AppBlueprint.Contracts.B2B.Contracts.ApiKey.Requests;

public class UpdateApiKeyRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}
