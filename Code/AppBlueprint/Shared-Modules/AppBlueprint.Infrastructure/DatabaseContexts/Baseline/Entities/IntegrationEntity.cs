namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Integration;

public class IntegrationEntity
{
    public int Id { get; set; }
    public int OwnerId { get; set; }

    public string Name { get; set; }

    // Stripe, SendGrid, Twilio, etc
    public string ServiceName { get; set; }
    public string? Description { get; set; }
    public string ApiKeySecretReference { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
}
