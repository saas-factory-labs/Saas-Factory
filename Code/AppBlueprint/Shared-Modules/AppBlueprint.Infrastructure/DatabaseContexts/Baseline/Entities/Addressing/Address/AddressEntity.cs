using AppBlueprint.Application.Attributes;
using AppBlueprint.Application.Enums;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Tenant.Tenant;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing;

public class AddressEntity
{
    public enum AddressType
    {
        Home,
        Office
    }

    public AddressEntity()
    {
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
        Street = new StreetEntity();
        State = string.Empty;
        PostalCode = string.Empty;
        Floor = string.Empty;
        StreetNumber = string.Empty;
    }

    public int Id { get; set; }
    public int CityId { get; set; }
    public CityEntity City { get; set; }
    public int CountryId { get; set; }
    public CountryEntity Country { get; set; }
    public int StreetId { get; set; }
    public StreetEntity Street { get; set; }

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

    public int? CustomerId { get; set; }
    public CustomerEntity? Customer { get; set; }

    public int? TenantId { get; set; }
    public TenantEntity? Tenant { get; set; }
}
