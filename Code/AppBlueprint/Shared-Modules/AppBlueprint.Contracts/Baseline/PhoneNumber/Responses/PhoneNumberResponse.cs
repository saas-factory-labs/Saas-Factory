namespace AppBlueprint.Contracts.Baseline.PhoneNumber.Responses;

public class PhoneNumberResponse
{
    public string Id { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string? CountryCode { get; set; }
    public string? Extension { get; set; }
    public string? UserId { get; set; }
    public string? CustomerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
