namespace AppBlueprint.Contracts.Baseline.Integrations.Requests;

public class UpdateIntegrationRequest
{
    public string? Title { get; set; }
    public string? Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
}
