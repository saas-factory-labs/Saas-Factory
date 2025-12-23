namespace AppBlueprint.Application.Services;

/// <summary>
/// Service for accessing current authenticated user information.
/// Used by AdminTenantAccessService and other components that need user context.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the current user's unique identifier.
    /// Returns null if no user is authenticated.
    /// </summary>
    string? UserId { get; }

    /// <summary>
    /// Gets the current user's email address.
    /// Returns null if no user is authenticated or email is not available.
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// Checks if the current user has the specified role.
    /// </summary>
    /// <param name="role">The role name to check (e.g., "SuperAdmin", "TenantAdmin")</param>
    /// <returns>True if the user has the role, false otherwise</returns>
    bool IsInRole(string role);

    /// <summary>
    /// Gets all roles assigned to the current user.
    /// Returns empty collection if no user is authenticated.
    /// </summary>
    IEnumerable<string> Roles { get; }
}
