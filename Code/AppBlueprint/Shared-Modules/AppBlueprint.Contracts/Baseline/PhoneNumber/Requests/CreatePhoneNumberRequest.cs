namespace AppBlueprint.Contracts.Baseline.PhoneNumber.Requests;

public class CreatePhoneNumberRequest
{
    public required string Number { get; set; }
    public string? CountryCode { get; set; }
    public string? Extension { get; set; }
    public string? UserId { get; set; }
    public string? CustomerId { get; set; }
}
