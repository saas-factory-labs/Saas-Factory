using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Tenant.Tenant;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Billing.Subscription;

public class SubscriptionEntity : BaseEntity, ITenantScoped
{
    public SubscriptionEntity()
    {
        Id = PrefixedUlid.Generate("sub");
    }

    public string Name { get; set; }
    public string? Description { get; set; }
    public string Code { get; set; }
    public string Status { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }

    public string TenantId { get; set; }
    public TenantEntity? Tenant { get; set; }
}
