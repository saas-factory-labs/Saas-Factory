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

    public int Id { get; set; }


    // Geographic details
    public int CityId { get; set; }
    // public CityEntity City { get; set; }

    public int CountryId { get; set; }
    // public CountryEntity Country { get; set; }

    public int StreetId { get; set; }
    // public StreetEntity Street { get; set; }

    public bool IsPrimary { get; set; }
    public string? Longitude { get; set; } // Fixed typo
    public string? Latitude { get; set; }

    public required string Floor { get; set; } // e.g., 0 = ground floor, -1 = basement, etc.

    [DataClassification(GDPRType.DirectlyIdentifiable)]
    public required string StreetNumber { get; set; }

    [DataClassification(GDPRType.DirectlyIdentifiable)]
    public string? UnitNumber { get; set; } // Apartment or unit number

    public string State { get; set; }
    public string PostalCode { get; set; }

    // Relationships
    public int? CustomerId { get; set; }
    public CustomerResponse Customer { get; set; }

    public int? TenantId { get; set; }
    public TenantResponse Tenant { get; set; }
}
