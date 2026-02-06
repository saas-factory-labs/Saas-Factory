using AppBlueprint.Contracts.B2B.Contracts.Tenant.Requests;
using AppBlueprint.Contracts.Baseline.Customer.Requests;
using AppBlueprint.SharedKernel.Attributes;
using AppBlueprint.SharedKernel.Enums;

namespace AppBlueprint.Contracts.Baseline.Address.Requests;

public class UpdateAddressRequest
{
    public enum AddressType
    {
        Home,
        Office
    }


    // 

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
    public UpdateCustomerRequest? Customer { get; set; }

    public string? TenantId { get; set; }
    public UpdateTenantRequest? Tenant { get; set; }
    public UpdateTenantRequest? TenantRequest { get; set; }
}
