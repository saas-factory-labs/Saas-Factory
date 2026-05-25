using AppBlueprint.Contracts.Baseline.Address.Requests;
using AppBlueprint.Contracts.Baseline.EmailAddress.Requests;
using AppBlueprint.Contracts.Baseline.PhoneNumber.Requests;

namespace AppBlueprint.Contracts.Baseline.ContactPerson.Requests;

/// <summary>
/// Request to create a new contact person with associated contact information.
/// </summary>
public class CreateContactPersonRequest
{
    /// <summary>
    /// First name of the contact person.
    /// </summary>
    public required string FirstName { get; init; }
    
    /// <summary>
    /// Last name of the contact person.
    /// </summary>
    public required string LastName { get; init; }
    
    /// <summary>
    /// Email addresses associated with this contact person.
    /// </summary>
    public required IReadOnlyList<CreateEmailAddressRequest> EmailAddresses { get; init; }
    
    /// <summary>
    /// Physical addresses associated with this contact person.
    /// </summary>
    public required IReadOnlyList<CreateAddressRequest> Addresses { get; init; }
    
    /// <summary>
    /// Phone numbers associated with this contact person.
    /// </summary>
    public required IReadOnlyList<CreatePhoneNumberRequest> PhoneNumbers { get; init; }
}
