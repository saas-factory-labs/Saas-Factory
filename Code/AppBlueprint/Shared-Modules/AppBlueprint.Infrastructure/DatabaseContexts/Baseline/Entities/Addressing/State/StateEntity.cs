using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing;

public class StateEntity : BaseEntity
{
    public StateEntity()
    {
        Id = PrefixedUlid.Generate("state");
        Country = new CountryEntity();
    }

    public string Name { get; set; }
    public string IsoCode { get; set; }
    public string CountryId { get; set; }
    public CountryEntity Country { get; set; }
}
