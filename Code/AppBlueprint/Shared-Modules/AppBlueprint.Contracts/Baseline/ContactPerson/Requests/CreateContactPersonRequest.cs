using AppBlueprint.Contracts.Baseline.Address.Requests;
using AppBlueprint.Contracts.Baseline.EmailAddress.Requests;
using AppBlueprint.Contracts.Baseline.PhoneNumber.Requests;

namespace AppBlueprint.Contracts.Baseline.ContactPerson.Requests;

// Primary constructor: parameters become public init-only properties
public class CreateContactPersonRequest(
    IReadOnlyList<CreateEmailAddressRequest> EmailAddresses,
    IReadOnlyList<CreateAddressRequest> Addresses,
    IReadOnlyList<CreatePhoneNumberRequest> PhoneNumbers)
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
}

//
//
// using AppBlueprint.Contracts.Baseline.Address.Requests;
// using AppBlueprint.Contracts.Baseline.EmailAddress.Requests;
// using AppBlueprint.Contracts.Baseline.PhoneNumber.Requests;
//
// namespace AppBlueprint.Contracts.Baseline.ContactPerson.Requests;
//
// public class CreateContactPersonRequest(
//     List<CreateEmailAddressRequest> emailAddresses,
//     List<CreateAddressRequest> addresses,
//     List<CreatePhoneNumberRequest> phoneNumbers)
// {
//     public required string FirstName { get; init; }
//     public required string LastName { get; init; } 
//     public required IReadOnlyList<CreateEmailAddressRequest> EmailAddresses { get; set; } = emailAddresses;
//     public required IReadOnlyList<CreateAddressRequest> Addresses { get; set; } = addresses;
//     public required IReadOnlyList<CreatePhoneNumberRequest> PhoneNumbers { get; set; } = phoneNumbers;
// }
