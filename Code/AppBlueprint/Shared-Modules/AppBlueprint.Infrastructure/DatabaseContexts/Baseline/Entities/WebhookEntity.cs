using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;

public class WebhookEntity : BaseEntity
{
    public required Uri Url { get; set; }
    public required string Secret { get; set; }
    public string? Description { get; set; }
}
