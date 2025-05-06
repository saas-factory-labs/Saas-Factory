namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing.Region;

public class GlobalRegionEntity
{
    public GlobalRegionEntity()
    {
        Countries = new List<CountryEntity>();
    }

    public int Id { get; set; }

    // North America, South America, Europe, Asia, Africa, Australia
    public string Name { get; set; }

    public List<CountryEntity> Countries { get; set; }
}
