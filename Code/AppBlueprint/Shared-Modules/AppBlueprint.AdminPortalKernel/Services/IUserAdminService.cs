using AppBlueprint.AdminPortalKernel.Domain;
using AppBlueprint.AdminPortalKernel.Domain.Dtos;

namespace AppBlueprint.AdminPortalKernel.Services;

/// <summary>User administration across all tenants of a target app.</summary>
public interface IUserAdminService
{
    Task<PagedResult<AdminUserRecord>> SearchAsync(string slug, UserSearchRequest request);

    Task<AdminUserRecord?> GetAsync(string slug, string userId);

    /// <summary>
    /// Activates or deactivates a user in the target app. Writes an audit entry before
    /// touching the app database; returns false when the user does not exist.
    /// </summary>
    Task<bool> SetActiveAsync(string slug, string userId, bool isActive, string reason);
}
