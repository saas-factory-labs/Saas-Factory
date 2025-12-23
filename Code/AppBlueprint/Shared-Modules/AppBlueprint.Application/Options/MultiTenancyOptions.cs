namespace AppBlueprint.Application.Options;

/// <summary>
/// Configuration options for multi-tenancy behavior
/// </summary>
public sealed class MultiTenancyOptions
{
    /// <summary>
    /// The section name in appsettings.json
    /// </summary>
    public const string SectionName = "MultiTenancy";

    /// <summary>
    /// Multi-tenancy strategy (SharedDatabase is the only supported option)
    /// </summary>
    public MultiTenancyStrategy Strategy { get; set; } = MultiTenancyStrategy.SharedDatabase;

    /// <summary>
    /// Tenant resolution strategy (how to determine which tenant the request belongs to)
    /// </summary>
    public TenantResolutionStrategy ResolutionStrategy { get; set; } = TenantResolutionStrategy.JwtClaim;

    /// <summary>
    /// JWT claim name containing tenant ID (when using JwtClaim resolution)
    /// </summary>
    public string TenantClaimName { get; set; } = "tenant_id";

    /// <summary>
    /// HTTP header name containing tenant ID (when using Header resolution)
    /// </summary>
    public string TenantHeaderName { get; set; } = "X-Tenant-ID";

    /// <summary>
    /// Path segment index for tenant ID (when using PathSegment resolution)
    /// Example: /tenant/{tenantId}/api/... would use index 2
    /// </summary>
    public int TenantPathSegmentIndex { get; set; } = 2;

    /// <summary>
    /// Enable Row-Level Security (RLS) enforcement at database level
    /// </summary>
    public bool EnableRowLevelSecurity { get; set; } = true;

    /// <summary>
    /// Enable application-level query filters as defense-in-depth
    /// (in addition to database RLS)
    /// </summary>
    public bool EnableQueryFilters { get; set; } = true;

    /// <summary>
    /// Validate that tenant exists and is active before processing requests
    /// </summary>
    public bool ValidateTenantExists { get; set; } = true;

    /// <summary>
    /// Default tenant ID for system operations or development
    /// </summary>
    public string? DefaultTenantId { get; set; }

    /// <summary>
    /// Allow missing tenant context for specific endpoints (e.g., health checks, metrics)
    /// </summary>
    public bool AllowMissingTenantForSystemEndpoints { get; set; } = true;

    /// <summary>
    /// Endpoints that don't require tenant context (regex patterns)
    /// </summary>
    public string[] ExcludedPaths { get; set; } = new[]
    {
        "/health",
        "/metrics",
        "/swagger.*",
        "/_framework/.*",
        "/_content/.*"
    };

    /// <summary>
    /// Enable tenant audit logging
    /// </summary>
    public bool EnableTenantAuditLogging { get; set; } = true;
}

/// <summary>
/// Multi-tenancy isolation strategy
/// </summary>
public enum MultiTenancyStrategy
{
    /// <summary>
    /// Shared database with Row-Level Security (default and recommended)
    /// All tenants share the same database with RLS policies enforcing isolation
    /// </summary>
    SharedDatabase = 0,

    /// <summary>
    /// Database-per-tenant (NOT SUPPORTED)
    /// Each tenant gets their own database
    /// </summary>
    DatabasePerTenant = 1,

    /// <summary>
    /// Schema-per-tenant (NOT SUPPORTED)
    /// Each tenant gets their own database schema
    /// </summary>
    SchemaPerTenant = 2
}

/// <summary>
/// Strategy for resolving tenant from HTTP request
/// </summary>
public enum TenantResolutionStrategy
{
    /// <summary>
    /// Extract tenant ID from JWT claim (recommended for B2C)
    /// Example: JWT contains "tenant_id": "acme-corp"
    /// </summary>
    JwtClaim = 0,

    /// <summary>
    /// Extract tenant ID from subdomain (recommended for B2B)
    /// Example: acme.yourapp.com -> tenant ID is "acme"
    /// </summary>
    Subdomain = 1,

    /// <summary>
    /// Extract tenant ID from custom HTTP header
    /// Example: X-Tenant-ID: acme-corp
    /// </summary>
    Header = 2,

    /// <summary>
    /// Extract tenant ID from URL path segment
    /// Example: /tenant/acme-corp/api/users
    /// </summary>
    PathSegment = 3,

    /// <summary>
    /// Combination of multiple strategies (fallback chain)
    /// Try JwtClaim -> Header -> Subdomain -> PathSegment
    /// </summary>
    Multiple = 4
}
