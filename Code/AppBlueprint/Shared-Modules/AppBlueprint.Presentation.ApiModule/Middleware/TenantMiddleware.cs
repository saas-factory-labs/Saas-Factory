using System.Security.Claims;
using AppBlueprint.Infrastructure.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Presentation.ApiModule.Middleware;

/// <summary>
/// Simple DTO for tenant lookup query result.
/// </summary>
internal sealed record UserTenantLookup(string Id, string? TenantId);

public class TenantMiddleware(RequestDelegate next)
{
    // Paths that should be excluded from tenant ID requirement
    private static readonly string[] ExcludedPaths = [
        "/swagger",
        "/openapi",
        "/nswag",
        "/api-docs",
        "/swagger.json",
        "/swagger/v1/swagger.json", // NSwag generated OpenAPI document
        "/swagger/index.html",
        "/swagger-ui",
        "/swagger-resources",
        "/webjars",
        "/docs",
        "/v1/swagger.json", // Alternative NSwag path
        "/health",
        "/metrics",
        "/_framework",
        "/_vs",
        "/favicon.ico",
        "/api/test",
        "/api/system", // System endpoints should not require tenant
        "/api/AuthDebug", // Debug endpoints for troubleshooting
        "/api/v1/webhooks", // Webhook endpoints (external services)
        "/api/v2/webhooks" // Webhook endpoints (future versions)
    ];

    public async Task Invoke(HttpContext? context)
    {
        ArgumentNullException.ThrowIfNull(context);

        string requestPath = context.Request.Path.Value ?? string.Empty;

        // Check if the request should bypass tenant validation
        bool shouldBypassTenantValidation = ShouldBypassTenantValidation(requestPath);

        // Early return for bypass scenarios
        if (shouldBypassTenantValidation)
        {
            await next(context);
            return;
        }

        // Guard clause: Require authentication
        if (context.User.Identity?.IsAuthenticated != true)
        {
            // Unauthenticated requests should not access tenant-scoped data
            // Exception: Pre-authentication scenarios (login, registration) should be excluded via ShouldBypassTenantValidation
            context.Response.StatusCode = 401; // Unauthorized
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"error\":\"Authentication required\",\"message\":\"This endpoint requires authentication with a valid tenant context.\"}\n");
            return;
        }

        // SECURITY: For authenticated requests, ALWAYS extract tenant from JWT claims
        // JWT tokens are cryptographically signed and cannot be tampered with
        // Headers, subdomains, and path segments can all be forged by malicious clients
        
        // Try primary tenant claim name
        string? tenantId = context.User.FindFirst("tenant_id")?.Value;

        // Fallback to alternative claim name
        tenantId ??= context.User.FindFirst("tid")?.Value;

        // If tenant claim is not in JWT, look it up from database based on user's external auth ID (sub claim)
        // This handles external IdPs like Logto that don't include custom tenant claims in access tokens
        if (string.IsNullOrEmpty(tenantId))
        {
            // Get the 'sub' claim (external auth provider user ID)
            string? externalAuthId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? context.User.FindFirst("sub")?.Value;

            Console.WriteLine($"[TenantMiddleware] No tenant claim in JWT. Looking up by ExternalAuthId (sub): {externalAuthId ?? "(null)"}");
            Console.WriteLine($"[TenantMiddleware] JWT Claims ({context.User.Claims.Count()}):");
            foreach (var claim in context.User.Claims)
            {
                Console.WriteLine($"[TenantMiddleware]   - {claim.Type}: {claim.Value}");
            }

            if (!string.IsNullOrEmpty(externalAuthId))
            {
                // Resolve DbContextFactory from request services (not constructor - middleware is singleton)
                var dbContextFactory = context.RequestServices.GetService<IDbContextFactory<ApplicationDbContext>>();

                if (dbContextFactory is not null)
                {
                    await using ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();

                    // Query database by ExternalAuthId (Logto 'sub' claim)
                    // Use quoted column names to match exact casing in PostgreSQL
                    FormattableString query = $"SELECT \"Id\", \"TenantId\" FROM \"Users\" WHERE \"ExternalAuthId\" = {externalAuthId} LIMIT 1";

                    var result = await dbContext.Database.SqlQuery<UserTenantLookup>(query).FirstOrDefaultAsync();
                    tenantId = result?.TenantId;

                    Console.WriteLine($"[TenantMiddleware] Database lookup result: TenantId={tenantId ?? "(null)"}");
                }
                else
                {
                    Console.WriteLine("[TenantMiddleware] ⚠️ IDbContextFactory not available in DI container");
                }
            }
            else
            {
                Console.WriteLine("[TenantMiddleware] ⚠️ No sub/NameIdentifier claim found in JWT - cannot look up tenant");
            }
        }

        // Guard clause: Tenant ID must be present
        if (string.IsNullOrEmpty(tenantId))
        {
            context.Response.StatusCode = 401; // Unauthorized
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"error\":\"Tenant claim missing from JWT\",\"message\":\"Authenticated users must have a tenant_id or tid claim in their JWT token.\"}\n");
            return;
        }

        context.Items["TenantId"] = tenantId;
        await next(context);
    }
    private static bool ShouldBypassTenantValidation(string requestPath)
    {
        // Check if path is in the excluded list (case-insensitive)
        bool isExcludedPath = ExcludedPaths.Any(excludedPath =>
            requestPath.StartsWith(excludedPath, StringComparison.OrdinalIgnoreCase));

        if (isExcludedPath) return true;

        // Check if it's an API endpoint first
        bool isApiEndpoint = requestPath.StartsWith("/api", StringComparison.OrdinalIgnoreCase);

        // Check if it's a documentation or static file request
        // IMPORTANT: Do not bypass tenant validation for API endpoints even if they have file extensions
        bool isDocumentationOrStatic = requestPath.Contains("swagger", StringComparison.OrdinalIgnoreCase) ||
                                      requestPath.Contains("openapi", StringComparison.OrdinalIgnoreCase) ||
                                      requestPath.Contains("nswag", StringComparison.OrdinalIgnoreCase) ||
                                      requestPath.Contains("docs", StringComparison.OrdinalIgnoreCase) ||
                                      requestPath.EndsWith("/v1", StringComparison.OrdinalIgnoreCase) ||
                                      requestPath.StartsWith("/v1/", StringComparison.OrdinalIgnoreCase) ||
                                      (!isApiEndpoint && HasStaticFileExtension(requestPath)); // Only check file extensions for non-API paths

        if (isDocumentationOrStatic) return true;

        // Check if it's a system or test endpoint
        bool isSystemEndpoint = requestPath.StartsWith("/api/test", StringComparison.OrdinalIgnoreCase) ||
                               requestPath.StartsWith("/api/system", StringComparison.OrdinalIgnoreCase) ||
                               requestPath.StartsWith("/health", StringComparison.OrdinalIgnoreCase) ||
                               requestPath.StartsWith("/metrics", StringComparison.OrdinalIgnoreCase);

        if (isSystemEndpoint) return true;

        // If it's not an API endpoint, bypass validation
        // Note: isApiEndpoint already computed above
        return !isApiEndpoint;
    }

    private static bool HasStaticFileExtension(string requestPath)
    {
        return requestPath.EndsWith(".json", StringComparison.OrdinalIgnoreCase) ||
               requestPath.EndsWith(".html", StringComparison.OrdinalIgnoreCase) ||
               requestPath.EndsWith(".css", StringComparison.OrdinalIgnoreCase) ||
               requestPath.EndsWith(".js", StringComparison.OrdinalIgnoreCase) ||
               requestPath.EndsWith(".ico", StringComparison.OrdinalIgnoreCase) ||
               requestPath.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
               requestPath.EndsWith(".svg", StringComparison.OrdinalIgnoreCase) ||
               requestPath.EndsWith(".woff", StringComparison.OrdinalIgnoreCase) ||
               requestPath.EndsWith(".woff2", StringComparison.OrdinalIgnoreCase);
    }
}
