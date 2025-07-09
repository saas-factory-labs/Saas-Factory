using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.Team;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.ContactPerson;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Tenant.Tenant;

public sealed class TenantEntity : BaseEntity
{
    private readonly List<ContactPersonEntity> _contactPersons = new();
    private readonly List<UserEntity> _users = new();
    private readonly List<TeamEntity> _teams = new();

    public TenantEntity()
    {
        Id = PrefixedUlid.Generate("tenant");
        Customer = new CustomerEntity();
        Name = string.Empty;
        Email = string.Empty;
        Phone = string.Empty;
        Type = string.Empty;
        VatNumber = string.Empty;
        Country = string.Empty;
    }

    public string Name { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public bool IsPrimary { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Type { get; set; } // Personal/Company
    public string VatNumber { get; set; }
    public string Country { get; set; }

    // Relationships
    public IReadOnlyCollection<ContactPersonEntity> ContactPersons => _contactPersons.AsReadOnly();
    public CustomerEntity Customer { get; set; }
    public IReadOnlyCollection<UserEntity> Users => _users.AsReadOnly();
    public IReadOnlyCollection<TeamEntity> Teams => _teams.AsReadOnly();

    // Domain methods for controlled collection management
    public void AddContactPerson(ContactPersonEntity contactPerson)
    {
        ArgumentNullException.ThrowIfNull(contactPerson);

        if (_contactPersons.Any(cp => cp.Id == contactPerson.Id))
            return; // Contact person already exists

        _contactPersons.Add(contactPerson);
    }

    public void RemoveContactPerson(ContactPersonEntity contactPerson)
    {
        ArgumentNullException.ThrowIfNull(contactPerson);
        _contactPersons.Remove(contactPerson);
    }

    public void AddUser(UserEntity user)
    {
        ArgumentNullException.ThrowIfNull(user);

        if (_users.Any(u => u.Id == user.Id))
            return; // User already exists

        _users.Add(user);
        user.TenantId = Id;
    }

    public void RemoveUser(UserEntity user)
    {
        ArgumentNullException.ThrowIfNull(user);
        _users.Remove(user);
    }

    public void AddTeam(TeamEntity team)
    {
        ArgumentNullException.ThrowIfNull(team);

        if (_teams.Any(t => t.Id == team.Id))
            return; // Team already exists

        _teams.Add(team);
        team.TenantId = Id;
    }

    public void RemoveTeam(TeamEntity team)
    {
        ArgumentNullException.ThrowIfNull(team);
        _teams.Remove(team);
    }
}
