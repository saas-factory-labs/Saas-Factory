namespace AppBlueprint.Contracts.Baseline.EmailAddress.Requests;

public class UpdateEmailAddressRequest
{
    public string Id { get; set; } = string.Empty;
    public string NewEmailAddress { get; set; } = string.Empty;
}
