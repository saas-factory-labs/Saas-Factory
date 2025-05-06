namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing;

public class CityEntity
{
    public CityEntity()
    {
        Country = new CountryEntity();
        State = new StateEntity();
    }

    public int Id { get; set; }
    public required string Name { get; set; }
    public int CountryId { get; set; }
    public required CountryEntity Country { get; set; }
    public required string PostalCode { get; set; }

    public StateEntity? State { get; set; }
    public int StateId { get; set; }
}
