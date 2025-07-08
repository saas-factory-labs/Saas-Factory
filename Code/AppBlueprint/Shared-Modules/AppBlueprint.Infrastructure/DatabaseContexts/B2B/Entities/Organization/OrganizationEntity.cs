using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.Team;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Organization;

/// <summary>
/// B2B Organization entity representing business organizations in multi-tenant scenarios.
/// Provides organizational hierarchy, user management, and team coordination for B2B operations.
/// </summary>
public sealed class OrganizationEntity : BaseEntity, ITenantScoped
{
    public OrganizationEntity()
    {
        Id = PrefixedUlid.Generate("organization");
        Teams = new List<TeamEntity>();
        Customers = new List<CustomerEntity>();
        TenantId = string.Empty;
        OwnerId = string.Empty;
    }

    public OrganizationEntity(string name, string description, UserEntity owner) : this()
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        OwnerId = owner.Id;
    }

    public required string Name { get; set; }

    public required string Description { get; set; }



    public bool IsActive { get; set; } = true;

    // ITenantScoped implementation
    public string TenantId { get; set; }

    // Relationships
    public string OwnerId { get; set; }
    public required UserEntity? Owner { get; set; }

    public required List<TeamEntity> Teams { get; set; }
    public required List<CustomerEntity> Customers { get; set; }
}
