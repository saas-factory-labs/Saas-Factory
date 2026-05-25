namespace AppBlueprint.Contracts.Baseline.PhoneNumber.Requests;

/// <summary>
/// Request to create a new phone number for a user or customer.
/// </summary>
public class CreatePhoneNumberRequest
{
    /// <summary>
    /// Phone number in E.164 format or local format.
    /// </summary>
    public required string Number { get; set; }
    
    /// <summary>
    /// Country code (e.g., "+1" for US, "+45" for Denmark).
    /// </summary>
    public string? CountryCode { get; set; }
    
    /// <summary>
    /// Extension number for office phone systems (optional).
    /// </summary>
    public string? Extension { get; set; }
    
    /// <summary>
    /// User ID this phone number belongs to (optional).
    /// </summary>
    public string? UserId { get; set; }
    
    /// <summary>
    /// Customer ID this phone number belongs to (optional).
    /// </summary>
    public string? CustomerId { get; set; }
}
