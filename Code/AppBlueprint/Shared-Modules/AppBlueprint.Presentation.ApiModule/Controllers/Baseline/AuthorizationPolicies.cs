using AppBlueprint.Application.Constants;

namespace AppBlueprint.Presentation.ApiModule.Controllers.Baseline;

[Obsolete("Use AppBlueprint.Application.Constants.AuthorizationPolicyNames instead. This compatibility type will be removed in a future release.")]
public static class AuthorizationPolicies
{
    public const string AdminOnly = AuthorizationPolicyNames.AdminOnly;

    [Obsolete("No active authorization policy registration exists for this name. Add a policy before using it.")]
    public const string HasUserClaim = "HasUserClaim";

    public const string Over18 = AuthorizationPolicyNames.Over18;

    [Obsolete("No active authorization policy registration exists for this name. Add a policy before using it.")]
    public const string Require2Fa = "Require2fa";

    [Obsolete("No active authorization policy registration exists for this name. Add a policy before using it.")]
    public const string DenmarkOnly = "DenmarkOnly";

    [Obsolete("No active authorization policy registration exists for this name. Add a policy before using it.")]
    public const string CombinedPolicyExample = "CombinedPolicyExample";

    [Obsolete("No active authorization policy registration exists for this name. Add a policy before using it.")]
    public const string MitIdVerified = "MitIdVerified";

    public const string ApiKey = AuthorizationPolicyNames.ApiKey;
}
