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
    /// SECURITY NOTE: For authenticated requests (B2C and B2B), tenant is ALWAYS extracted from JWT claims
    /// regardless of this setting. JWT tokens are cryptographically signed and cannot be tampered with.
    /// Other strategies (Subdomain, Header, PathSegment) can only be used for PRE-AUTHENTICATION scenarios
    /// such as login page routing or public landing pages.
    /// </summary>
    public TenantResolutionStrategy ResolutionStrategy { get; set; } = TenantResolutionStrategy.JwtClaim;

    /// <summary>
    /// JWT claim name containing tenant ID (when using JwtClaim resolution)
    /// Default: "tenant_id" (fallback: "tid")
    /// SECURITY: This is the PRIMARY and ONLY secure source for authenticated requests
    /// </summary>
    public string TenantClaimName { get; set; } = "tenant_id";

    /// <summary>
    /// HTTP header name containing tenant ID (when using Header resolution)
    /// WARNING: Headers can be forged by clients and should NEVER be trusted for authenticated requests
    /// Only use for pre-authentication scenarios (e.g., determining auth provider for login)
    /// </summary>
    public string TenantHeaderName { get; set; } = "X-Tenant-ID";

    /// <summary>
    /// Path segment index for tenant ID (when using PathSegment resolution)
    /// Example: /tenant/{tenantId}/api/... would use index 2
    /// WARNING: Path segments can be manipulated by clients and should NEVER be trusted for authenticated requests
    /// Only use for pre-authentication scenarios (e.g., public landing pages)
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
    public IReadOnlyCollection<string> ExcludedPaths { get; set; } = new[]
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
    /// Database-per-tenant (NOT SUPPORTED yet)
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
/// SECURITY: For ALL authenticated requests (B2C and B2B), tenant MUST come from JWT claims.
/// Other strategies are ONLY for pre-authentication scenarios (login routing, public pages).
/// </summary>
public enum TenantResolutionStrategy
{
    /// <summary>
    /// Extract tenant ID from JWT claim (REQUIRED for all authenticated requests)
    /// Example: JWT contains "tenant_id": "acme-corp" or "tid": "acme-corp"
    /// SECURITY: This is the ONLY secure source - JWT tokens are cryptographically signed
    /// </summary>
    JwtClaim = 0,

    /// <summary>
    /// Extract tenant ID from subdomain (ONLY for pre-authentication use)
    /// Example: acme.yourapp.com -> determines which auth provider to use for login
    /// WARNING: Subdomains can be manipulated - NEVER use for authenticated requests
    /// </summary>
    Subdomain = 1,

    /// <summary>
    /// Extract tenant ID from custom HTTP header (ONLY for pre-authentication use)
    /// Example: X-Tenant-ID: acme-corp -> determines login page branding
    /// WARNING: Headers can be forged by clients - NEVER use for authenticated requests
    /// </summary>
    Header = 2,

    /// <summary>
    /// Extract tenant ID from URL path segment (ONLY for pre-authentication use)
    /// Example: /tenant/acme-corp/login -> determines login page
    /// WARNING: Path segments can be manipulated - NEVER use for authenticated requests
    /// </summary>
    PathSegment = 3,

    /// <summary>
    /// Combination of multiple strategies (fallback chain for pre-authentication)
    /// Example: Try Subdomain -> Header -> PathSegment for determining login provider
    /// SECURITY: Authenticated requests ALWAYS use JWT regardless of this setting
    /// </summary>
    Multiple = 4
}
