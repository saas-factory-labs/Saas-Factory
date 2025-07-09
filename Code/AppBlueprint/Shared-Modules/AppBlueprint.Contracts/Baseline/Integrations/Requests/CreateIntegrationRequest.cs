namespace AppBlueprint.Contracts.Baseline.Integrations.Requests;

public class CreateIntegrationRequest
{
    public required string Title { get; set; }
    public required string Message { get; set; }
    public bool IsRead { get; set; }
}
