namespace AppBlueprint.Contracts.Baseline.Auth.Responses;

public class VerifyMfaResponse
{
    public string? UserId { get; set; }
    public string? Code { get; set; }
}
