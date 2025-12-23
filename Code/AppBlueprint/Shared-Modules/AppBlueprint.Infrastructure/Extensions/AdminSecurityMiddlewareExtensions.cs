using AppBlueprint.Infrastructure.Middleware;
using Microsoft.AspNetCore.Builder;

namespace AppBlueprint.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering admin security middleware.
/// </summary>
public static class AdminSecurityMiddlewareExtensions
{
    /// <summary>
    /// Adds admin IP whitelist middleware to the application pipeline.
    /// This should be added early in the pipeline, after authentication/authorization.
    /// </summary>
    public static IApplicationBuilder UseAdminIpWhitelist(this IApplicationBuilder app)
    {
        return app.UseMiddleware<AdminIpWhitelistMiddleware>();
    }
}
