using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Tenant.Tenant;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User.TenantUser;

public class TenantUserEntity
{
    public TenantUserEntity()
    {
        Tenant = new TenantEntity();
    }

    public int TenantId { get; set; }
    public TenantEntity Tenant { get; set; }

    public int UserId { get; set; }

    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
}
