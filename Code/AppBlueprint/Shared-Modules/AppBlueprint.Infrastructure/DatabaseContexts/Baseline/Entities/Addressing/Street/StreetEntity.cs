using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing;

public class StreetEntity : BaseEntity
{
    public StreetEntity()
    {
        Id = PrefixedUlid.Generate("street");
        Name = string.Empty;
        City = new CityEntity
        {
            Name = "City",
            PostalCode = "0000",
            Country = new CountryEntity
            {
                Name = "Country"
            }
        };
    }

    public required string Name { get; set; }
    public required CityEntity City { get; set; }
    public string CityId { get; set; }
}
