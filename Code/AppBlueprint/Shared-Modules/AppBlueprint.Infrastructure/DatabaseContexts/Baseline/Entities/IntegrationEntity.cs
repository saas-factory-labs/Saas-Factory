using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Integration;

public class IntegrationEntity : BaseEntity
{
    public IntegrationEntity()
    {
        Id = PrefixedUlid.Generate("integ");
    }

    public string OwnerId { get; set; }

    public string Name { get; set; }

    // Stripe, SendGrid, Twilio, etc
    public string ServiceName { get; set; }
    public string? Description { get; set; }
    public string ApiKeySecretReference { get; set; }
}
