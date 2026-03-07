using AppBlueprint.Contracts.B2B.Contracts.Tenant.Requests;
using AppBlueprint.Contracts.Baseline.Address.Requests;
using AppBlueprint.Contracts.Baseline.ContactPerson.Requests;
using AppBlueprint.Contracts.Baseline.PhoneNumber.Requests;
using AppBlueprint.SharedKernel.Attributes;
using AppBlueprint.SharedKernel.Enums;

namespace AppBlueprint.Contracts.Baseline.EmailAddress.Requests;

public class CreateEmailAddressRequest
{
    public CreateEmailAddressRequest()
    {
        Address = string.Empty;
        Tenant = new CreateTenantRequest { Name = string.Empty };
        List<CreateEmailAddressRequest> emailAddresses = [];
        List<CreateAddressRequest> Addresses = [];
        List<CreatePhoneNumberRequest> PhoneNumbers = [];
        ContactPerson = new CreateContactPersonRequest
        {
            FirstName = string.Empty,
            LastName = string.Empty,
            EmailAddresses = emailAddresses,
            Addresses = Addresses,
            PhoneNumbers = PhoneNumbers
        };
    }

    public CreateEmailAddressRequest(string address, bool isPrimary, bool isVerified) : this()
    {
        Address = address;
        IsPrimary = isPrimary;
        IsVerified = isVerified;
    }

    [DataClassification(GDPRType.DirectlyIdentifiable)]
    public string Address { get; set; }

    public bool IsPrimary { get; set; }
    public bool IsVerified { get; set; }

    public int? CustomerId { get; set; }
    public int? ContactPersonId { get; set; }
    public int? TenantId { get; set; }
    public int? UserId { get; set; }

    public CreateContactPersonRequest ContactPerson { get; set; }
    public CreateTenantRequest Tenant { get; set; }
}
