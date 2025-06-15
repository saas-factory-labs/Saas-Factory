namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing;

public class StreetEntity
{
    public StreetEntity()
    {
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

    public int Id { get; set; }
    public required string Name { get; set; }
    public required CityEntity City { get; set; }
    public int CityId { get; set; }
}
