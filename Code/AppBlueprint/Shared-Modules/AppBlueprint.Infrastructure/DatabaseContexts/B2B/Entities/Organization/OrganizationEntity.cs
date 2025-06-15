using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.Team;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Organization;

/// <summary>
/// B2B Organization entity representing business organizations in multi-tenant scenarios.
/// Provides organizational hierarchy, user management, and team coordination for B2B operations.
/// </summary>
public sealed class OrganizationEntity
{
    public OrganizationEntity()
    {
        Teams = new List<TeamEntity>();
        Customers = new List<CustomerEntity>();
    }

    public OrganizationEntity(string name, string description, UserEntity owner) : this()
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        CreatedAt = DateTime.UtcNow;
    }

    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; } = true;

    // Relationships
    public int OwnerId { get; set; }
    public required UserEntity Owner { get; set; }

    public required List<TeamEntity> Teams { get; set; }
    public required List<CustomerEntity> Customers { get; set; }
}
