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
    private readonly List<TeamEntity> _teams = new();
    private readonly List<CustomerEntity> _customers = new();

    public OrganizationEntity()
    {
        Id = PrefixedUlid.Generate("organization");
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

    public IReadOnlyCollection<TeamEntity> Teams => _teams.AsReadOnly();
    public IReadOnlyCollection<CustomerEntity> Customers => _customers.AsReadOnly();

    // Domain methods for controlled collection management
    public void AddTeam(TeamEntity team)
    {
        ArgumentNullException.ThrowIfNull(team);
        
        if (_teams.Any(t => t.Id == team.Id))
            return; // Team already exists
            
        _teams.Add(team);
        team.OrganizationId = Id;
    }

    public void RemoveTeam(TeamEntity team)
    {
        ArgumentNullException.ThrowIfNull(team);
        _teams.Remove(team);
    }

    public void AddCustomer(CustomerEntity customer)
    {
        ArgumentNullException.ThrowIfNull(customer);
        
        if (_customers.Any(c => c.Id == customer.Id))
            return; // Customer already exists
            
        _customers.Add(customer);
    }

    public void RemoveCustomer(CustomerEntity customer)
    {
        ArgumentNullException.ThrowIfNull(customer);
        _customers.Remove(customer);
    }
}
