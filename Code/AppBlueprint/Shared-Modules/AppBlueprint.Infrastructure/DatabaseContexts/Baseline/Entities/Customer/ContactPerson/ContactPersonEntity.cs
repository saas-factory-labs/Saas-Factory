using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Tenant.Tenant;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Email.EmailAddress;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.ContactPerson;

public class ContactPersonEntity
{
    public ContactPersonEntity()
    {
        Tenant = new TenantEntity();
        EmailAddresses = new List<EmailAddressEntity>();
        Addresses = new List<AddressEntity>();
        PhoneNumbers = new List<PhoneNumberEntity>();
        Customer = new CustomerEntity();
    }

    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int TenantId { get; set; }
    public TenantEntity Tenant { get; set; }
    public List<EmailAddressEntity> EmailAddresses { get; set; }
    public List<AddressEntity> Addresses { get; set; }
    public List<PhoneNumberEntity> PhoneNumbers { get; set; }

    public CustomerEntity Customer { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsPrimary { get; set; } = false;
}
