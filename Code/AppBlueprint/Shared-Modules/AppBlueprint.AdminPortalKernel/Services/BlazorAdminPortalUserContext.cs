using System.Security.Claims;
using AppBlueprint.Application.Constants;
using Microsoft.AspNetCore.Components.Authorization;

namespace AppBlueprint.AdminPortalKernel.Services;

/// <summary>
/// Resolves the signed-in admin from Blazor's AuthenticationStateProvider, which is
/// reliable inside interactive server circuits (unlike IHttpContextAccessor).
/// </summary>
public sealed class BlazorAdminPortalUserContext : IAdminPortalUserContext
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public BlazorAdminPortalUserContext(AuthenticationStateProvider authenticationStateProvider)
    {
        ArgumentNullException.ThrowIfNull(authenticationStateProvider);
        _authenticationStateProvider = authenticationStateProvider;
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

    private async Task<ClaimsPrincipal> GetUserAsync()
    {
        AuthenticationState state = await _authenticationStateProvider.GetAuthenticationStateAsync();
        return state.User;
    }
}
