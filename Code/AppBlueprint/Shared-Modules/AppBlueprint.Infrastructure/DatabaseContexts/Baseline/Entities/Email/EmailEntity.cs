using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.ContactPerson;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Tenant;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User.Profile;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Email;

public class EmailEntity
{
    public EmailEntity()
    {
        User = new UserEntity
        {
            FirstName = "FirstName",
            LastName = "LastName",
            UserName = "UserName",
            Email = "Email",
            Profile = new ProfileEntity()
        };
        Customer = new CustomerEntity();
        ContactPerson = new ContactPersonEntity();
        Tenant = new TenantEntity();
    }

    public int Id { get; set; }
    public string Address { get; set; } = string.Empty;

    public int UserId { get; set; }
    public UserEntity User { get; set; }

    public int? CustomerId { get; set; }
    public CustomerEntity Customer { get; set; }

    public ContactPersonEntity ContactPerson { get; set; }

    public int? TenantId { get; set; }
    public TenantEntity Tenant { get; set; }
}
