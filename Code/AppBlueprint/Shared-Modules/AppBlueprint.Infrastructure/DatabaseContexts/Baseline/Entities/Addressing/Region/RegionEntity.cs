using System.Collections.ObjectModel;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing.Region;

public class GlobalRegionEntity: BaseEntity
{
    public GlobalRegionEntity()
    {
        Countries = new Collection<CountryEntity>();
    }

    // North America, South America, Europe, Asia, Africa, Australia
    public required string Name { get; set; }

    /// <summary>
    /// Standardized region code for programmatic reference (e.g., "NA", "EU", "AS", "SA", "AF", "OC").
    /// Optional property to enable consistent regional identification across different languages and systems.
    /// </summary>
    public string? Code { get; set; }

    public Collection<CountryEntity> Countries { get; }
}
