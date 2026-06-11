using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing.Country;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing.State;

public class StateEntity : BaseEntity
{
    public StateEntity()
    {
        Id = PrefixedUlid.Generate("state");
        Country = new CountryEntity
        {
            Id = PrefixedUlid.Generate("country"),
            Name = "Denmark",
            IsoCode = Addressing.Country.IsoCode.Da,
            CityId = string.Empty,
            GlobalRegionId = string.Empty
        };
    }

    public required string Name { get; set; }
    public string? IsoCode { get; set; }
    public string? CountryId { get; set; }
    public CountryEntity? Country { get; set; }
}
