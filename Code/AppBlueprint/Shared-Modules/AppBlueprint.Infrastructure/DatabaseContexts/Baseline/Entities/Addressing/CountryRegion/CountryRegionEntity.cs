﻿using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing;

public class CountryRegionEntity
{
    public CountryRegionEntity()
    {
        Country = new CountryEntity
        {
            Name = "Country",
            CityId = PrefixedUlid.Generate("city"),
            GlobalRegionId = PrefixedUlid.Generate("region")
        };
    }

    public int Id { get; set; }

    // Syddanmark, Midtjylland, Nordjylland, Sjælland, Hovedstaden
    public required string Name { get; set; } //  populate from dictionary created from database at startup

    public CountryEntity Country { get; set; }
    public int CountryId { get; set; }
}
