using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing.City;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing.Region;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing.Country;

public enum IsoCode
{
    Da,
    Us
}

public class CountryEntity : BaseEntity
{
    public CountryEntity()
    {
        Id = PrefixedUlid.Generate("country");
        Cities = new List<CityEntity>();
        GlobalRegion = new GlobalRegionEntity
        {
            Name = string.Empty
        };
    }

    public required string Name { get; set; } // Denmark, United States of America - populate from dictionary created from database at startup

    public IsoCode IsoCode { get; set; }

    public List<CityEntity> Cities { get; set; }
    public required string CityId { get; set; }

    public GlobalRegionEntity GlobalRegion { get; set; }
    public required string GlobalRegionId { get; set; }
}
