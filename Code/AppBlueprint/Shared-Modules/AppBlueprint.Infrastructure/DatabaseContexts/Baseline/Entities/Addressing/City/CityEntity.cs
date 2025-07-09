using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing;

public class CityEntity : BaseEntity
{
    public CityEntity()
    {
        Id = PrefixedUlid.Generate("city");
        Country = new CountryEntity 
        { 
            Name = string.Empty, 
            CityId = string.Empty, 
            GlobalRegionId = string.Empty 
        };
        State = new StateEntity
        {
            Name = "State"
        };
    }

    public required string Name { get; set; }
    public required string CountryId { get; set; }
    public required CountryEntity Country { get; set; }
    public required string PostalCode { get; set; }

    public StateEntity? State { get; set; }
    public required string StateId { get; set; }
}
