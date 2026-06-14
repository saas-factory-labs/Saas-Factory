namespace AppBlueprint.AdminPortalKernel.Services;

/// <summary>
/// Identity of the signed-in admin, safe to use inside Blazor Server circuits.
/// Deliberately NOT IHttpContextAccessor-based - that is unreliable in interactive
/// server rendering; the implementation uses AuthenticationStateProvider.
/// </summary>
public interface IAdminPortalUserContext
{
    /// <summary>True when the current user carries the DeploymentManagerAdmin role.</summary>
    Task<bool> IsDeploymentManagerAdminAsync();

    /// <summary>Stable user id (the "sub" claim) for audit entries.</summary>
    Task<string?> GetUserIdAsync();

    /// <summary>Email address of the signed-in admin for audit entries.</summary>
    Task<string?> GetEmailAsync();

    /// <summary>
    /// True when the signed-in admin completed multi-factor authentication, as asserted by the
    /// auth provider's claim (Logto owns enrolment/verification; the portal only enforces it).
    /// </summary>
    Task<bool> HasCompletedMfaAsync();

    /// <summary>
    /// True when the signed-in admin is a configured primary super-admin (system owner) eligible
    /// for the seamless, fully-audited bypass of the manual reason/ticket prompt.
    /// </summary>
    Task<bool> IsPrimarySuperAdminAsync();
}
