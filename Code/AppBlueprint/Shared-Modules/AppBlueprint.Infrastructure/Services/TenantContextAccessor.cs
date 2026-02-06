using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

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
    private readonly ILogger<TenantContextAccessor> _logger;

    public TenantContextAccessor(IHttpContextAccessor httpContextAccessor, ILogger<TenantContextAccessor> logger)
    {
        ArgumentNullException.ThrowIfNull(httpContextAccessor);
        ArgumentNullException.ThrowIfNull(logger);
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    /// <summary>
    /// Extracts tenant ID from JWT "tenant_id" claim or HttpContext.Items (set by TenantMiddleware).
    /// Returns null if:
    /// - No HttpContext exists (background jobs, migrations)
    /// - User is not authenticated
    /// - Tenant is not available from claims or middleware lookup
    /// </summary>
    public string? TenantId
    {
        get
        {
            HttpContext? context = _httpContextAccessor.HttpContext;

            if (context is null)
            {
                _logger.LogDebug("TenantId access - No HttpContext available");
                return null;
            }

            // First check HttpContext.Items (set by TenantMiddleware after database lookup)
            if (context.Items.TryGetValue("TenantId", out object? tenantIdObj) && tenantIdObj is string tenantIdFromItems)
            {
                _logger.LogDebug("TenantId from HttpContext.Items: {TenantId}", tenantIdFromItems);
                return tenantIdFromItems;
            }

            // Fallback to JWT claims (for cases where middleware hasn't run yet)
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                string? tenantId = context.User.FindFirst("tenant_id")?.Value;

                if (tenantId is not null)
                {
                    _logger.LogDebug("TenantId from JWT claim 'tenant_id': {TenantId}", tenantId);
                }
                else
                {
                    // Log all available claims for debugging
                    string claims = string.Join(", ", context.User.Claims.Select(c => $"{c.Type}={c.Value}"));
                    _logger.LogWarning("TenantId not found. User authenticated: {IsAuth}. Available claims: {Claims}",
                        context.User.Identity.IsAuthenticated, claims);
                }

                return tenantId;
            }

            _logger.LogDebug("TenantId access - User not authenticated");
            return null;
        }
    }
}
