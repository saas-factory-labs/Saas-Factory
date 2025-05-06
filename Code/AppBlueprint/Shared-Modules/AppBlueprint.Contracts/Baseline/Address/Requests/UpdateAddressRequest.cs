using AppBlueprint.Application.Attributes;
using AppBlueprint.Application.Enums;
using AppBlueprint.Contracts.B2B.Tenant.Requests;
using AppBlueprint.Contracts.Baseline.Customer.Requests;

namespace AppBlueprint.Contracts.Baseline.Address.Requests;

public class UpdateAddressRequest
{
    public enum AddressType
    {
        Home,
        Office
    }


    // 

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
    public UpdateCustomerRequest Customer { get; set; }

    public int? TenantId { get; set; }
    public UpdateTenantRequest Tenant { get; set; }
    public UpdateTenantRequest TenantRequest { get; set; }
}
