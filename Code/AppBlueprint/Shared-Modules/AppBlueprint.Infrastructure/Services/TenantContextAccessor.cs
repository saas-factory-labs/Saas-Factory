using Microsoft.AspNetCore.Http;

namespace AppBlueprint.Infrastructure.Services;

/// <summary>
/// Provides access to the current tenant context from JWT claims.
/// Thread-safe accessor for multi-tenant operations.
/// </summary>
public interface ITenantContextAccessor
{
    /// <summary>
    /// Gets the current tenant ID from JWT claims.
    /// Returns null if no tenant context is available (e.g., during migrations, background jobs).
    /// </summary>
    string? TenantId { get; }
}

/// <summary>
/// Implementation of ITenantContextAccessor that extracts tenant ID from HttpContext JWT claims.
/// Used by EF Core Named Query Filters for automatic tenant isolation.
/// </summary>
public sealed class TenantContextAccessor : ITenantContextAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        ArgumentNullException.ThrowIfNull(httpContextAccessor);
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Extracts tenant ID from JWT "tenant_id" claim.
    /// Returns null if:
    /// - No HttpContext exists (background jobs, migrations)
    /// - User is not authenticated
    /// - Tenant claim is missing
    /// </summary>
    public string? TenantId
    {
        get
        {
            HttpContext? context = _httpContextAccessor.HttpContext;
            
            if (context?.User?.Identity?.IsAuthenticated != true)
                return null;

            string? tenantId = context.User.FindFirst("tenant_id")?.Value;
            return tenantId;
        }
    }
}
