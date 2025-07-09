namespace AppBlueprint.Presentation.ApiModule.Controllers.Baseline;

public static class AuthorizationPolicies
{
    public const string AdminOnly = "AdminOnly";
    public const string HasUserClaim = "HasUserClaim";
    public const string Over18 = "Over18";
    public const string Require2Fa = "Require2fa";
    public const string DenmarkOnly = "DenmarkOnly";
    public const string CombinedPolicyExample = "CombinedPolicyExample";
    public const string MitIdVerified = "MitIdVerified";
    public const string ApiKey = "ApiKey";
}
