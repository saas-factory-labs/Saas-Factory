using AppBlueprint.Application.Attributes;
using AppBlueprint.Application.Enums;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Tenant.Tenant;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.ContactPerson;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Email.EmailAddress;

public class EmailAddressEntity
{
    public EmailAddressEntity()
    {
        User = new UserEntity
        {
            FirstName = "FirstName",
            LastName = "LastName",
            UserName = "UserName",
            Email = "Email",
            Profile = new ProfileEntity()
        };
        Address = string.Empty;
    }

    public int Id { get; set; }

    [DataClassification(GDPRType.DirectlyIdentifiable)]
    public string Address { get; set; }

    public int UserId { get; set; }
    public UserEntity User { get; set; }

    public int? CustomerId { get; set; }
    public CustomerEntity? Customer { get; set; }

    public ContactPersonEntity? ContactPerson { get; set; }

    public int? TenantId { get; set; }
    public TenantEntity? Tenant { get; set; }
}
