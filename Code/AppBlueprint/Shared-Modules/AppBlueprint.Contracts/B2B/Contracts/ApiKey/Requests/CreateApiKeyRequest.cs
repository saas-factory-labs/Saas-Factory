namespace AppBlueprint.Contracts.B2B.Contracts.ApiKey.Requests;

public class CreateApiKeyRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}
