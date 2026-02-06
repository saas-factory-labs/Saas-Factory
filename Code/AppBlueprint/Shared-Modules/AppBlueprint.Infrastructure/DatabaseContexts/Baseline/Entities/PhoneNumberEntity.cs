using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.ContactPerson;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Tenant;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.SharedKernel;
using AppBlueprint.SharedKernel.Attributes;
using AppBlueprint.SharedKernel.Enums;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;

public sealed class PhoneNumberEntity : BaseEntity, ITenantScoped
{
    public PhoneNumberEntity()
    {
        Id = PrefixedUlid.Generate("phone-number");
        Number = string.Empty;
        CountryCode = string.Empty;
        TenantId = string.Empty;
    }
    [DataClassification(GDPRType.IndirectlyIdentifiable)]
    public string Number { get; set; } // store phone number as 25 varchar which should be sufficient for all phone numbers for all countries
    // dont store phone numbers with spaces but can contain dashes
    // format the phone number in the frontend UI, dont store it directly with a format in the db and dont have the backend or db do formatting. leave it to the UI

    // store phone number prefix code(045/+45/45) in seperate column string field
    [DataClassification(GDPRType.IndirectlyIdentifiable)]
    public string CountryCode { get; set; }

    public bool IsPrimary { get; set; }
    public bool IsVerified { get; set; }

    // Relationships
    public string? UserId { get; set; }
    public UserEntity? User { get; set; }

    public string? CustomerId { get; set; }
    public CustomerEntity? Customer { get; set; }

    public string? ContactPersonId { get; set; }
    public ContactPersonEntity? ContactPerson { get; set; }

    // ITenantScoped implementation
    public string TenantId { get; set; }
    public TenantEntity? Tenant { get; set; }
}
