using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing;

public class CityEntity : BaseEntity
{
    public CityEntity()
    {
        Id = PrefixedUlid.Generate("city");
        Country = new CountryEntity();
        State = new StateEntity();
    }

    public required string Name { get; set; }
    public string CountryId { get; set; }
    public required CountryEntity Country { get; set; }
    public required string PostalCode { get; set; }

    public StateEntity? State { get; set; }
    public string StateId { get; set; }
}
