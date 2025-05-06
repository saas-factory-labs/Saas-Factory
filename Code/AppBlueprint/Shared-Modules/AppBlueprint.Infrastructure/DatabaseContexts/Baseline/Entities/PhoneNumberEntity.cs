using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Tenant.Tenant;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.ContactPerson;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;

public class PhoneNumberEntity
{
    public PhoneNumberEntity()
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
        Tenant = new TenantEntity();
        ContactPerson = new ContactPersonEntity();
    }

    // country code (045/+45/45)

    public int Id { get; set; }

    public string
        Number
    {
        get;
        set;
    } // store phone number as 25 varchar which should be sufficient for all phone numbers for all countries
    // dont store phone numbers with spaces but can contain dashes
    // format the phone number in the frontend UI, dont store it directly with a format in the db and dont have the backend or db do formatting. leave it to the UI


    // store phone number prefix code(045/+45/45) in seperate column string field
    public string CountryCode { get; set; }

    public bool IsPrimary { get; set; }
    public bool IsVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }

    // Relationships
    public int UserId { get; set; }
    public UserEntity User { get; set; }

    public int? CustomerId { get; set; }
    public CustomerEntity Customer { get; set; }

    public ContactPersonEntity ContactPerson { get; set; }

    public int? TenantId { get; set; }
    public TenantEntity Tenant { get; set; }
}
