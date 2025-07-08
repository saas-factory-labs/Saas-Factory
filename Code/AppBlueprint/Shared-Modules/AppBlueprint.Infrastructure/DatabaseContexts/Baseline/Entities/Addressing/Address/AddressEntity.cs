using AppBlueprint.Application.Attributes;
using AppBlueprint.Application.Enums;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Tenant.Tenant;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing;

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
            Country = new CountryEntity
            {
                Name = "Country"
            }
        };
        Country = new CountryEntity();
        Street = new StreetEntity
        {
            Name = "Street",
            City = new CityEntity
            {
                Name = "City",
                PostalCode = "0000",
                Country = new CountryEntity
                {
                    Name = "Country"
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
