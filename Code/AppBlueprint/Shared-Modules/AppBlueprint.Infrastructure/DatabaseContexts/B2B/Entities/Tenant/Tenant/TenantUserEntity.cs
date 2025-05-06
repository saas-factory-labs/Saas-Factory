namespace AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Tenant.Tenant;

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

    //public string Type { get; set; } // admin / user
    // List<Role> Roles // maybe should be looked up against auth0 api ?
}
