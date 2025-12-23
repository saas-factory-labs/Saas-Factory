namespace AppBlueprint.Presentation.ApiModule.Middleware;

public class TenantMiddleware(RequestDelegate next)
{    // Paths that should be excluded from tenant ID requirement
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
        "/api/AuthDebug" // Debug endpoints for troubleshooting
    ];

    public async Task Invoke(HttpContext? context)
    {
        ArgumentNullException.ThrowIfNull(context);

        string requestPath = context.Request.Path.Value ?? string.Empty;

        // Check if the request should bypass tenant validation
        bool shouldBypassTenantValidation = ShouldBypassTenantValidation(requestPath);

        if (!shouldBypassTenantValidation)
        {
            // SECURITY: For authenticated requests, ALWAYS extract tenant from JWT claims
            // JWT tokens are cryptographically signed and cannot be tampered with
            // Headers, subdomains, and path segments can all be forged by malicious clients
            if (context.User.Identity?.IsAuthenticated == true)
            {
                // Try primary tenant claim name
                string? tenantId = context.User.FindFirst("tenant_id")?.Value;
                
                // Fallback to alternative claim name
                tenantId ??= context.User.FindFirst("tid")?.Value;
                
                if (string.IsNullOrEmpty(tenantId))
                {
                    context.Response.StatusCode = 401; // Unauthorized
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync("{\"error\":\"Tenant claim missing from JWT\",\"message\":\"Authenticated users must have a tenant_id or tid claim in their JWT token.\"}\n");
                    return;
                }
                
                context.Items["TenantId"] = tenantId;
            }
            else
            {
                // Unauthenticated requests should not access tenant-scoped data
                // Exception: Pre-authentication scenarios (login, registration) should be excluded via ShouldBypassTenantValidation
                context.Response.StatusCode = 401; // Unauthorized
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"error\":\"Authentication required\",\"message\":\"This endpoint requires authentication with a valid tenant context.\"}\n");
                return;
            }
        }

        await next(context);
    }
    private static bool ShouldBypassTenantValidation(string requestPath)
    {
        // Check if path is in the excluded list (case-insensitive)
        bool isExcludedPath = ExcludedPaths.Any(excludedPath =>
            requestPath.StartsWith(excludedPath, StringComparison.OrdinalIgnoreCase));

        if (isExcludedPath) return true;

        // Check if it's a documentation or static file request
        bool isDocumentationOrStatic = requestPath.Contains("swagger", StringComparison.OrdinalIgnoreCase) ||
                                      requestPath.Contains("openapi", StringComparison.OrdinalIgnoreCase) ||
                                      requestPath.Contains("nswag", StringComparison.OrdinalIgnoreCase) ||
                                      requestPath.Contains("docs", StringComparison.OrdinalIgnoreCase) ||
                                      requestPath.EndsWith("/v1", StringComparison.OrdinalIgnoreCase) ||
                                      requestPath.StartsWith("/v1/", StringComparison.OrdinalIgnoreCase) ||
                                      HasStaticFileExtension(requestPath);

        if (isDocumentationOrStatic) return true;

        // Check if it's a system or test endpoint
        bool isSystemEndpoint = requestPath.StartsWith("/api/test", StringComparison.OrdinalIgnoreCase) ||
                               requestPath.StartsWith("/api/system", StringComparison.OrdinalIgnoreCase) ||
                               requestPath.StartsWith("/health", StringComparison.OrdinalIgnoreCase) ||
                               requestPath.StartsWith("/metrics", StringComparison.OrdinalIgnoreCase);

        if (isSystemEndpoint) return true;

        // If it's not an API endpoint, bypass validation
        bool isApiEndpoint = requestPath.StartsWith("/api", StringComparison.OrdinalIgnoreCase);
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
