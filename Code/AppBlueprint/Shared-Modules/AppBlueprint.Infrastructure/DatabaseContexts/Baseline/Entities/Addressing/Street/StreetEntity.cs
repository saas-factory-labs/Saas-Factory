using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing.City;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing.Country;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing.Street;

public class StreetEntity : BaseEntity
{
    public StreetEntity()
    {
        Id = PrefixedUlid.Generate("street");
        Name = string.Empty;
        CityId = string.Empty;
        City = new CityEntity
        {
            Name = "City",
            PostalCode = "0000",
            CountryId = string.Empty,
            StateId = string.Empty,
            Country = new CountryEntity
            {
                Name = "Country",
                CityId = string.Empty,
                GlobalRegionId = string.Empty
            }
        };
    }

    public required string Name { get; set; }
    public required CityEntity City { get; set; }
    public string CityId { get; set; }
}
