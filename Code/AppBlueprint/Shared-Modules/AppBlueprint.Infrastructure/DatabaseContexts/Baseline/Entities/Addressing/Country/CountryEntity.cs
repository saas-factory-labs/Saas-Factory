using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing.Region;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing;

public enum IsoCode
{
    Da,
    Us
}

public class CountryEntity
{
    public CountryEntity()
    {
        Cities = new List<CityEntity>();
        GlobalRegion = new GlobalRegionEntity();
    }

    public int Id { get; set; }

    public string
        Name
    {
        get;
        set;
    } // Denmark, United States of America - populate from dictionary created from database at startup

    public IsoCode IsoCode { get; set; }

    public List<CityEntity> Cities { get; set; }
    public string CityId { get; set; }

    public GlobalRegionEntity GlobalRegion { get; set; }
    public int GlobalRegionId { get; set; }
}
