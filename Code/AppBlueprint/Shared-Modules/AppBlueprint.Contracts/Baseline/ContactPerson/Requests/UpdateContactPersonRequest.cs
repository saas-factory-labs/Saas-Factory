using AppBlueprint.Contracts.Baseline.Address.Requests;
using AppBlueprint.Contracts.Baseline.EmailAddress.Requests;
using AppBlueprint.Contracts.Baseline.PhoneNumber.Requests;

namespace AppBlueprint.Contracts.Baseline.ContactPerson.Requests;

public class UpdateContactPersonRequest
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required IReadOnlyList<UpdateEmailAddressRequest> EmailAddresses { get; set; }
    public required IReadOnlyList<UpdateAddressRequest> Addresses { get; set; }
    public required IReadOnlyList<UpdatePhoneNumberRequest> PhoneNumbers { get; set; }
}
