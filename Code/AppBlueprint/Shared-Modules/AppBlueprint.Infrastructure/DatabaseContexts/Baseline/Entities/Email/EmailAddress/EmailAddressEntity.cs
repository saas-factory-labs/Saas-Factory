using AppBlueprint.Application.Attributes;
using AppBlueprint.Application.Enums;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Tenant.Tenant;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.ContactPerson;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Email.EmailAddress;

public class EmailAddressEntity : BaseEntity, ITenantScoped
{
    public EmailAddressEntity()
    {
        Id = PrefixedUlid.Generate("email_addr");
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

    [DataClassification(GDPRType.DirectlyIdentifiable)]
    public string Address { get; set; }

    public string UserId { get; set; }
    public UserEntity User { get; set; }

    public string? CustomerId { get; set; }
    public CustomerEntity? Customer { get; set; }

    public ContactPersonEntity? ContactPerson { get; set; }

    public string TenantId { get; set; }
    public TenantEntity? Tenant { get; set; }
}
