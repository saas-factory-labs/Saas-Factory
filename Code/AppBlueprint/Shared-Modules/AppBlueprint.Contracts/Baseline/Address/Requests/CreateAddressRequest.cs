namespace AppBlueprint.Contracts.Baseline.Address.Requests;

/// <summary>
/// Request to create a new physical address.
/// </summary>
public class CreateAddressRequest
{
    /// <summary>
    /// Street address including number and street name.
    /// </summary>
    public required string Street { get; set; }
    
    /// <summary>
    /// City name.
    /// </summary>
    public required string City { get; set; }
    
    /// <summary>
    /// State, province, or region.
    /// </summary>
    public required string State { get; set; }
    
    /// <summary>
    /// Postal code or ZIP code.
    /// </summary>
    public required string PostalCode { get; set; }
    
    /// <summary>
    /// Country name or ISO country code.
    /// </summary>
    public required string Country { get; set; }
}
