using AppBlueprint.Infrastructure.Persistence.DatabaseContexts.Baseline.Entities.Addressing.Address;
using AppBlueprint.Infrastructure.Persistence.DatabaseContexts.Baseline.Entities.Email.EmailAddress;

namespace AppBlueprint.Infrastructure.Persistence.DatabaseContexts.Baseline.Entities.User.Person;

public class PersonEntity
{
    public PersonEntity()
    {
        Addresses = [];
        Emails = [];
    }

    // used in user model and customer model

    public int Id { get; set; }

    public required string FirstName { get; set; }

    // Middle name is optional but can be in LastName field as well
    public required string LastName { get; set; }

    public IReadOnlyList<AddressEntity> Addresses { get; set; }

    public IReadOnlyList<EmailAddressEntity> Emails { get; set; }
}
