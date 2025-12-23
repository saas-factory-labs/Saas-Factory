using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Tenant;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Billing.Subscription;

public class SubscriptionEntity : BaseEntity, ITenantScoped
{
    public SubscriptionEntity()
    {
        Id = PrefixedUlid.Generate("sub");
    }

    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string Code { get; set; }
    public required string Status { get; set; }
    public required string CreatedBy { get; set; }
    public required string UpdatedBy { get; set; }

    public required string TenantId { get; set; }
    public TenantEntity? Tenant { get; set; }
}
