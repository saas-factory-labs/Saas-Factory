namespace DeploymentManager.Web.Services.Impersonation;

/// <summary>
/// Mints a Logto-signed, read-only access token that lets an admin act as a target user. The token is
/// issued by Logto (so it passes every target app's issuer/signing-key/audience validation unchanged);
/// the control plane only orchestrates the exchange and records who impersonated whom.
/// </summary>
public interface ILogtoImpersonationTokenService
{
    /// <summary>
    /// Issues a short-lived read-only impersonation token for <paramref name="targetUserId"/>,
    /// recording <paramref name="impersonatorAdminId"/> as the actor. Throws on any Logto failure
    /// (fail closed - no token, no session).
    /// </summary>
    Task<ImpersonationTokenResult> IssueReadOnlyTokenAsync(
        string targetUserId,
        string impersonatorAdminId,
        CancellationToken cancellationToken = default);
}
