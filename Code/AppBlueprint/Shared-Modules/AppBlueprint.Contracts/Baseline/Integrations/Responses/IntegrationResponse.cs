namespace AppBlueprint.Contracts.Baseline.Integrations.Responses;

public class IntegrationResponse
{
    public string? Title { get; init; }
    public string? Message { get; init; }
    public DateTime CreatedAt { get; init; }
    public bool IsRead { get; init; }
}
