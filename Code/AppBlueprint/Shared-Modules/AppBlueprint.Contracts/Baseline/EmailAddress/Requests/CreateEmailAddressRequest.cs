using AppBlueprint.Application.Attributes;
using AppBlueprint.Application.Enums;
using AppBlueprint.Contracts.B2B.Contracts.Tenant.Requests;
using AppBlueprint.Contracts.Baseline.Address.Requests;
using AppBlueprint.Contracts.Baseline.ContactPerson.Requests;
using AppBlueprint.Contracts.Baseline.PhoneNumber.Requests;

namespace AppBlueprint.Contracts.Baseline.EmailAddress.Requests;

/// <summary>
/// Request to create a new email address for a user, customer, or contact person.
/// </summary>
public class CreateEmailAddressRequest
{
    public CreateEmailAddressRequest()
    {
        Address = string.Empty;
        Tenant = new CreateTenantRequest { Name = string.Empty };
        var emailAddresses = new List<CreateEmailAddressRequest>();
        var Addresses = new List<CreateAddressRequest>();
        var PhoneNumbers = new List<CreatePhoneNumberRequest>();
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

    /// <summary>
    /// Email address (GDPR: Directly Identifiable).
    /// </summary>
    [DataClassification(GDPRType.DirectlyIdentifiable)]
    public string Address { get; set; }

    /// <summary>
    /// Indicates if this is the primary email address.
    /// </summary>
    public bool IsPrimary { get; set; }
    
    /// <summary>
    /// Indicates if the email address has been verified.
    /// </summary>
    public bool IsVerified { get; set; }

    /// <summary>
    /// Optional customer ID this email belongs to.
    /// </summary>
    public int? CustomerId { get; set; }
    
    /// <summary>
    /// Optional contact person ID this email belongs to.
    /// </summary>
    public int? ContactPersonId { get; set; }
    
    /// <summary>
    /// Optional tenant ID this email belongs to.
    /// </summary>
    public int? TenantId { get; set; }
    
    /// <summary>
    /// Optional user ID this email belongs to.
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// Associated contact person details.
    /// </summary>
    public CreateContactPersonRequest ContactPerson { get; set; }
    
    /// <summary>
    /// Associated tenant details.
    /// </summary>
    public CreateTenantRequest Tenant { get; set; }
}
