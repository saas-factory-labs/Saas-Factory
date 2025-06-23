using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Tenant.Tenant;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Email.EmailAddress;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.ContactPerson;

public class ContactPersonEntity : BaseEntity, ITenantScoped
{    public ContactPersonEntity()
    {
        Id = PrefixedUlid.Generate("contact-person");
        FirstName = string.Empty;
        LastName = string.Empty;
        TenantId = string.Empty;
        CustomerId = string.Empty;
        EmailAddresses = new List<EmailAddressEntity>();
        Addresses = new List<AddressEntity>();
        PhoneNumbers = new List<PhoneNumberEntity>();
    }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    // ITenantScoped implementation
    public string TenantId { get; set; }
    public TenantEntity? Tenant { get; set; }
    
    public string CustomerId { get; set; }
    public CustomerEntity? Customer { get; set; }
    
    public List<EmailAddressEntity> EmailAddresses { get; set; }
    public List<AddressEntity> Addresses { get; set; }
    public List<PhoneNumberEntity> PhoneNumbers { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsPrimary { get; set; } = false;
}
