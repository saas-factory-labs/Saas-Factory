using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing.City;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing.Country;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing.Street;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Tenant;
using AppBlueprint.SharedKernel;
using AppBlueprint.SharedKernel.Attributes;
using AppBlueprint.SharedKernel.Enums;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing.Address;

public class AddressEntity : BaseEntity, ITenantScoped
{
    public enum AddressType
    {
        Home,
        Office
    }
    public AddressEntity()
    {
        Id = PrefixedUlid.Generate("address");
        City = new CityEntity
        {
            Name = "City",
            PostalCode = "0000",
            CountryId = PrefixedUlid.Generate("country"),
            StateId = PrefixedUlid.Generate("state"),
            Country = new CountryEntity
            {
                Name = "Country",
                CityId = PrefixedUlid.Generate("city"),
                GlobalRegionId = PrefixedUlid.Generate("region")
            }
        };
        Country = new CountryEntity
        {
            Name = "Country",
            CityId = PrefixedUlid.Generate("city"),
            GlobalRegionId = PrefixedUlid.Generate("region")
        };
        Street = new StreetEntity
        {
            Name = "Street",
            City = new CityEntity
            {
                Name = "City",
                PostalCode = "0000",
                CountryId = PrefixedUlid.Generate("country"),
                StateId = PrefixedUlid.Generate("state"),
                Country = new CountryEntity
                {
                    Name = "Country",
                    CityId = PrefixedUlid.Generate("city"),
                    GlobalRegionId = PrefixedUlid.Generate("region")
                }
            }
        };
        State = string.Empty;
        PostalCode = string.Empty;
        Floor = string.Empty;
        StreetNumber = string.Empty;
        CityId = string.Empty;
        CountryId = string.Empty;
        StreetId = string.Empty;
        TenantId = string.Empty;
    }

    public string CityId { get; set; }
    public CityEntity? City { get; set; }
    public string CountryId { get; set; }
    public CountryEntity? Country { get; set; }
    public string StreetId { get; set; }
    public StreetEntity? Street { get; set; }

    public bool IsPrimary { get; set; }
    public string? Longitude { get; set; }
    public string? Latitude { get; set; }
    public required string Floor { get; set; }

    [DataClassification(GDPRType.DirectlyIdentifiable)]
    public required string StreetNumber { get; set; }

    [DataClassification(GDPRType.DirectlyIdentifiable)]
    public string? UnitNumber { get; set; }

    public string State { get; set; }
    public string PostalCode { get; set; }

    public string? CustomerId { get; set; }
    public CustomerEntity? Customer { get; set; }

    // ITenantScoped implementation
    public string TenantId { get; set; }
    public TenantEntity? Tenant { get; set; }
}
