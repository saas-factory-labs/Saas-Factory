namespace AppBlueprint.Contracts.Baseline.PhoneNumber.Requests;

public class UpdatePhoneNumberRequest
{
    public string Id { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string? CountryCode { get; set; }
    public string? Extension { get; set; }
}
