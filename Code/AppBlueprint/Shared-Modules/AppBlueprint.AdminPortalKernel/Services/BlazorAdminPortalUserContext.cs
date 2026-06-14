using System.Security.Claims;
using AppBlueprint.AdminPortalKernel.Security;
using AppBlueprint.Application.Constants;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;

namespace AppBlueprint.AdminPortalKernel.Services;

/// <summary>
/// Resolves the signed-in admin from Blazor's AuthenticationStateProvider, which is
/// reliable inside interactive server circuits (unlike IHttpContextAccessor).
/// </summary>
public sealed class BlazorAdminPortalUserContext : IAdminPortalUserContext
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly AdminPortalSecurityOptions _security;

    public BlazorAdminPortalUserContext(
        AuthenticationStateProvider authenticationStateProvider,
        IOptions<AdminPortalSecurityOptions> securityOptions)
    {
        ArgumentNullException.ThrowIfNull(authenticationStateProvider);
        ArgumentNullException.ThrowIfNull(securityOptions);
        _authenticationStateProvider = authenticationStateProvider;
        _security = securityOptions.Value;
    }

    public async Task<bool> IsDeploymentManagerAdminAsync()
    {
        ClaimsPrincipal user = await GetUserAsync();
        return user.IsInRole(Roles.DeploymentManagerAdmin);
    }

    public async Task<string?> GetUserIdAsync()
    {
        ClaimsPrincipal user = await GetUserAsync();
        return user.FindFirst("sub")?.Value ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    public async Task<string?> GetEmailAsync()
    {
        ClaimsPrincipal user = await GetUserAsync();
        return user.FindFirst("email")?.Value ?? user.FindFirst(ClaimTypes.Email)?.Value;
    }

    public async Task<bool> HasCompletedMfaAsync()
    {
        ClaimsPrincipal user = await GetUserAsync();

        // Logto emits the authentication methods reference (amr) claim. Depending on the OIDC
        // handler it arrives either as multiple claims of the type or as a single JSON-array
        // string, so a substring match against any claim of the configured type covers both.
        foreach (Claim claim in user.FindAll(_security.MfaClaimType))
        {
            if (claim.Value.Contains(_security.MfaClaimValue, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    public async Task<bool> IsPrimarySuperAdminAsync()
    {
        if (_security.SuperAdminSubjects.Count == 0)
        {
            return false;
        }

        string? subject = await GetUserIdAsync();
        if (string.IsNullOrWhiteSpace(subject))
        {
            return false;
        }

        foreach (string configured in _security.SuperAdminSubjects)
        {
            if (string.Equals(configured, subject, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private async Task<ClaimsPrincipal> GetUserAsync()
    {
        AuthenticationState state = await _authenticationStateProvider.GetAuthenticationStateAsync();
        return state.User;
    }
}
