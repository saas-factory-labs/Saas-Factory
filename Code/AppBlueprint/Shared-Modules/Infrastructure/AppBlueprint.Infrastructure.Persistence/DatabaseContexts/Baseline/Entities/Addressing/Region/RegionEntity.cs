using System.Collections.ObjectModel;
using AppBlueprint.Infrastructure.Persistence.DatabaseContexts.Baseline.Entities.Addressing.Country;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.Persistence.DatabaseContexts.Baseline.Entities.Addressing.Region;

public class GlobalRegionEntity : BaseEntity
{
    public GlobalRegionEntity()
    {
        Countries = new Collection<CountryEntity>();
    }

    // North America, South America, Europe, Asia, Africa, Australia
    public required string Name { get; set; }

    public Collection<CountryEntity> Countries { get; }
}
