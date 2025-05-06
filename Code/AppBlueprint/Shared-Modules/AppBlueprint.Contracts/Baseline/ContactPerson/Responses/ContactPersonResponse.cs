using AppBlueprint.Contracts.Baseline.Address.Responses;
using AppBlueprint.Contracts.Baseline.EmailAddress.Responses;
using AppBlueprint.Contracts.Baseline.PhoneNumber.Responses;

namespace AppBlueprint.Contracts.Baseline.ContactPerson.Responses;

public class ContactPersonResponse(
    string? firstName,
    string? lastName,
    IReadOnlyList<EmailAddressResponse?> emailAddresses,
    List<AddressResponse> addresses,
    List<PhoneNumberResponse> phoneNumbers)
{
    public string? FirstName { get; set; } = firstName;
    public string? LastName { get; set; } = lastName;
    public IReadOnlyList<EmailAddressResponse?> EmailAddresses { get; set; } = emailAddresses;
    public IReadOnlyList<AddressResponse> Addresses { get; set; } = addresses;
    public IReadOnlyList<PhoneNumberResponse> PhoneNumbers { get; set; } = phoneNumbers;
}
