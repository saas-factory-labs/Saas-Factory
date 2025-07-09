using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Tenant.Tenant;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Email.EmailAddress;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.ContactPerson;

public sealed class ContactPersonEntity : BaseEntity, ITenantScoped
{
    private readonly List<EmailAddressEntity> _emailAddresses = new();
    private readonly List<AddressEntity> _addresses = new();
    private readonly List<PhoneNumberEntity> _phoneNumbers = new();

    public ContactPersonEntity()
    {
        Id = PrefixedUlid.Generate("contact-person");
        FirstName = string.Empty;
        LastName = string.Empty;
        TenantId = string.Empty;
        CustomerId = string.Empty;
    }

    public string FirstName { get; set; }
    public string LastName { get; set; }

    // ITenantScoped implementation
    public string TenantId { get; set; }
    public TenantEntity? Tenant { get; set; }

    public string CustomerId { get; set; }
    public CustomerEntity? Customer { get; set; }

    public IReadOnlyCollection<EmailAddressEntity> EmailAddresses => _emailAddresses.AsReadOnly();
    public IReadOnlyCollection<AddressEntity> Addresses => _addresses.AsReadOnly();
    public IReadOnlyCollection<PhoneNumberEntity> PhoneNumbers => _phoneNumbers.AsReadOnly();

    public bool IsActive { get; set; } = true;
    public bool IsPrimary { get; set; }

    // Domain methods for controlled collection management
    public void AddEmailAddress(EmailAddressEntity emailAddress)
    {
        ArgumentNullException.ThrowIfNull(emailAddress);
        
        if (_emailAddresses.Any(ea => ea.Id == emailAddress.Id))
            return; // Email address already exists
            
        _emailAddresses.Add(emailAddress);
    }

    public void RemoveEmailAddress(EmailAddressEntity emailAddress)
    {
        ArgumentNullException.ThrowIfNull(emailAddress);
        _emailAddresses.Remove(emailAddress);
    }

    public void AddAddress(AddressEntity address)
    {
        ArgumentNullException.ThrowIfNull(address);
        
        if (_addresses.Any(a => a.Id == address.Id))
            return; // Address already exists
            
        _addresses.Add(address);
    }

    public void RemoveAddress(AddressEntity address)
    {
        ArgumentNullException.ThrowIfNull(address);
        _addresses.Remove(address);
    }

    public void AddPhoneNumber(PhoneNumberEntity phoneNumber)
    {
        ArgumentNullException.ThrowIfNull(phoneNumber);
        
        if (_phoneNumbers.Any(pn => pn.Id == phoneNumber.Id))
            return; // Phone number already exists
            
        _phoneNumbers.Add(phoneNumber);
    }

    public void RemovePhoneNumber(PhoneNumberEntity phoneNumber)
    {
        ArgumentNullException.ThrowIfNull(phoneNumber);
        _phoneNumbers.Remove(phoneNumber);
    }
}
