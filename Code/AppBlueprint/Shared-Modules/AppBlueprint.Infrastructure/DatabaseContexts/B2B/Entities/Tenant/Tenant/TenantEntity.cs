using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.Team;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.ContactPerson;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Tenant.Tenant;

public class TenantEntity : BaseEntity
{    public TenantEntity()
    {
        Id = PrefixedUlid.Generate("tenant");
        ContactPersons = new List<ContactPersonEntity>();
        Customer = new CustomerEntity();
        Users = new List<UserEntity>();
        Teams = new List<TeamEntity>();
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
    public List<ContactPersonEntity> ContactPersons { get; set; }
    public CustomerEntity Customer { get; set; }
    public List<UserEntity> Users { get; set; }
    public List<TeamEntity> Teams { get; set; }
}
