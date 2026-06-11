using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing.Country;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing.CountryRegion;

public class CountryRegionEntity : BaseEntity
{
    public CountryRegionEntity()
    {
        Id = PrefixedUlid.Generate("country_region");
        Country = new CountryEntity
        {
            Name = "Country",
            CityId = PrefixedUlid.Generate("city"),
            GlobalRegionId = PrefixedUlid.Generate("region")
        };
    }

    // Syddanmark, Midtjylland, Nordjylland, Sjælland, Hovedstaden
    public required string Name { get; set; } //  populate from dictionary created from database at startup

    public CountryEntity Country { get; set; }
    public string CountryId { get; set; } = string.Empty;
}
