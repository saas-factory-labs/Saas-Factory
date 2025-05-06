namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing;

public class StateEntity
{
    public StateEntity()
    {
        Country = new CountryEntity();
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public string IsoCode { get; set; }
    public int CountryId { get; set; }
    public CountryEntity Country { get; set; }
}
