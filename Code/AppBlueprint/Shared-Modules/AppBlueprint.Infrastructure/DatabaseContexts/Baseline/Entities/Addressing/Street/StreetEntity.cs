namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing;

public class StreetEntity
{
    public StreetEntity()
    {
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
    public string Name { get; set; }
    public CityEntity City { get; set; }
    public int CityId { get; set; }

    public CityEntity Country { get; set; }
    public int CountryId { get; set; }
}
