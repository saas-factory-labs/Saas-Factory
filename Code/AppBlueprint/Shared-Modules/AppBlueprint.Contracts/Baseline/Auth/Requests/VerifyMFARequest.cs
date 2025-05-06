namespace AppBlueprint.Contracts.Baseline.Auth.Requests;

public class VerifyMfaRequest
{
    public string? UserId { get; set; }
    public string? Code { get; set; }
}
