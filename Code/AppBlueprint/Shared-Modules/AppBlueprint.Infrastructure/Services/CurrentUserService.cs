using AppBlueprint.Application.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AppBlueprint.Infrastructure.Services;

/// <summary>
/// Implementation of ICurrentUserService that extracts user information from HttpContext JWT claims.
/// Used by AdminTenantAccessService and other components requiring user context.
/// Thread-safe accessor for authenticated user operations.
/// </summary>
public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        ArgumentNullException.ThrowIfNull(httpContextAccessor);
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Gets the current user's unique identifier from JWT "sub" claim.
    /// Returns null if no authenticated user exists.
    /// </summary>
    public string? UserId
    {
        get
        {
            HttpContext? context = _httpContextAccessor.HttpContext;
            
            if (context?.User?.Identity?.IsAuthenticated != true)
                return null;

            // Try "sub" claim first (standard JWT claim), fallback to "userId"
            string? userId = context.User.FindFirst("sub")?.Value 
                ?? context.User.FindFirst("userId")?.Value
                ?? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            return userId;
        }
    }

    /// <summary>
    /// Gets the current user's email address from JWT "email" claim.
    /// Returns null if no authenticated user exists or email is not available.
    /// </summary>
    public string? Email
    {
        get
        {
            HttpContext? context = _httpContextAccessor.HttpContext;
            
            if (context?.User?.Identity?.IsAuthenticated != true)
                return null;

            string? email = context.User.FindFirst("email")?.Value 
                ?? context.User.FindFirst(ClaimTypes.Email)?.Value;
            
            return email;
        }
    }

    /// <summary>
    /// Checks if the current user has the specified role.
    /// Checks both standard "role" claims and ClaimTypes.Role.
    /// </summary>
    /// <param name="role">The role name to check (e.g., "SuperAdmin", "TenantAdmin")</param>
    /// <returns>True if the user has the role, false otherwise</returns>
    public bool IsInRole(string role)
    {
        ArgumentNullException.ThrowIfNull(role);

        HttpContext? context = _httpContextAccessor.HttpContext;
        
        if (context?.User?.Identity?.IsAuthenticated != true)
            return false;

        // Check if user has the role claim
        // JWT tokens may use "role" claim, while ASP.NET uses ClaimTypes.Role
        bool hasRole = context.User.HasClaim(c => 
            (c.Type == "role" || c.Type == ClaimTypes.Role) && 
            c.Value.Equals(role, StringComparison.OrdinalIgnoreCase));

        return hasRole;
    }

    /// <summary>
    /// Gets all roles assigned to the current user.
    /// Returns empty collection if no user is authenticated.
    /// </summary>
    public IEnumerable<string> Roles
    {
        get
        {
            HttpContext? context = _httpContextAccessor.HttpContext;
            
            if (context?.User?.Identity?.IsAuthenticated != true)
                return Enumerable.Empty<string>();

            // Extract all role claims (may be multiple "role" claims in JWT)
            IEnumerable<string> roles = context.User.Claims
                .Where(c => c.Type == "role" || c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            return roles;
        }
    }
}
