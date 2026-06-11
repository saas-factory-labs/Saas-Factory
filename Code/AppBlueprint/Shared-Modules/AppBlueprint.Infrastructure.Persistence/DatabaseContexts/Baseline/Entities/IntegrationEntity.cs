using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;

public class IntegrationEntity : BaseEntity
{
    public IntegrationEntity()
    {
        Id = PrefixedUlid.Generate("integ");
    }

    public required string OwnerId { get; set; }

    public required string Name { get; set; }

    // Stripe, SendGrid, Twilio, etc
    public required string ServiceName { get; set; }
    public string? Description { get; set; }
    public required string ApiKeySecretReference { get; set; }
}
