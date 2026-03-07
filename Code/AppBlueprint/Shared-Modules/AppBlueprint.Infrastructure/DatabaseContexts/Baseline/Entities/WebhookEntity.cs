using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;

public class WebhookEntity : BaseEntity, ITenantScoped
{
    public WebhookEntity()
    {
        Id = PrefixedUlid.Generate("wh");
    }

    public required Uri Url { get; set; }
    public required string Secret { get; set; }
    public string? Description { get; set; }

    /// <summary>Comma-separated list of event types this endpoint subscribes to (e.g. "subscription.created,payment.succeeded"). Empty means all events.</summary>
    public string EventTypes { get; set; } = string.Empty;

    public required string TenantId { get; set; }
}
