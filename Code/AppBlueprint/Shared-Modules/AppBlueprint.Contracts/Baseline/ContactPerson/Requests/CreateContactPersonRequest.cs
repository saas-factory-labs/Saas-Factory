using AppBlueprint.Contracts.Baseline.Address.Requests;
using AppBlueprint.Contracts.Baseline.EmailAddress.Requests;
using AppBlueprint.Contracts.Baseline.PhoneNumber.Requests;

namespace AppBlueprint.Contracts.Baseline.ContactPerson.Requests;

public class CreateContactPersonRequest
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required IReadOnlyList<CreateEmailAddressRequest> EmailAddresses { get; init; }
    public required IReadOnlyList<CreateAddressRequest> Addresses { get; init; }
    public required IReadOnlyList<CreatePhoneNumberRequest> PhoneNumbers { get; init; }
}
