using AppBlueprint.Application.Attributes;
using AppBlueprint.Application.Enums;
using AppBlueprint.Contracts.B2B.Contracts.Tenant.Responses;
using AppBlueprint.Contracts.Baseline.Customer.Responses;

namespace AppBlueprint.Contracts.Baseline.Address.Responses;

public class AddressResponse
{
    public enum AddressType
    {
        Home,
        Office
    }

    // public AddressResponse()
    // {
    //     City = new CityEntity
    //     {
    //         Name = "Unknown",
    //         Country = new CountryEntity(),
    //         PostalCode = "Unknown",
    //     };
    //     Country = new CountryEntity();
    //     Street = new StreetEntity();
    // }

    public string Id { get; set; } = string.Empty;


    // Geographic details
    public string CityId { get; set; } = string.Empty;
    // public CityEntity City { get; set; }

    public string CountryId { get; set; } = string.Empty;
    // public CountryEntity Country { get; set; }

    public string StreetId { get; set; } = string.Empty;
    // public StreetEntity Street { get; set; }

    public bool IsPrimary { get; set; }
    public string? Longitude { get; set; } // Fixed typo
    public string? Latitude { get; set; }

    public required string Floor { get; set; } // e.g., 0 = ground floor, -1 = basement, etc.

    [DataClassification(GDPRType.DirectlyIdentifiable)]
    public required string StreetNumber { get; set; }

    [DataClassification(GDPRType.DirectlyIdentifiable)]
    public string? UnitNumber { get; set; } // Apartment or unit number

    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;

    // Relationships
    public string? CustomerId { get; set; }
    public CustomerResponse? Customer { get; set; }

    public string? TenantId { get; set; }
    public TenantResponse? Tenant { get; set; }
}
