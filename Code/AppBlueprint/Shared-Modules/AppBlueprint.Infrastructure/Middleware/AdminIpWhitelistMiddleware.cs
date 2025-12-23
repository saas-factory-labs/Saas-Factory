using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.Middleware;

/// <summary>
/// Middleware that restricts admin access to whitelisted IP addresses.
/// Applies only to admin operations (SuperAdmin role required routes).
/// </summary>
public sealed class AdminIpWhitelistMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AdminIpWhitelistMiddleware> _logger;
    private readonly HashSet<IPAddress> _allowedIps;
    private readonly bool _isEnabled;

    public AdminIpWhitelistMiddleware(
        RequestDelegate next,
        IConfiguration configuration,
        ILogger<AdminIpWhitelistMiddleware> logger)
    {
        _next = next;
        _logger = logger;

        // Read configuration
        _isEnabled = configuration.GetValue<bool>("Security:AdminIpWhitelist:Enabled", true);
        
        string[]? allowedIpsConfig = configuration.GetSection("Security:AdminIpWhitelist:AllowedIps")
            .Get<string[]>();

        _allowedIps = new HashSet<IPAddress>();

        if (allowedIpsConfig != null)
        {
            foreach (string ipString in allowedIpsConfig)
            {
                if (IPAddress.TryParse(ipString, out IPAddress? ipAddress))
                {
                    _allowedIps.Add(ipAddress);
                    _logger.LogInformation("Admin IP whitelist: Added {IpAddress}", ipAddress);
                }
                else
                {
                    _logger.LogWarning("Invalid IP address in whitelist configuration: {IpString}", ipString);
                }
            }
        }

        // Always allow localhost for development
        _allowedIps.Add(IPAddress.Loopback); // 127.0.0.1
        _allowedIps.Add(IPAddress.IPv6Loopback); // ::1

        _logger.LogInformation(
            "Admin IP whitelist initialized. Enabled={IsEnabled}, AllowedIps={Count}",
            _isEnabled,
            _allowedIps.Count);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip if middleware is disabled
        if (!_isEnabled)
        {
            await _next(context);
            return;
        }

        // Only check admin routes (can be customized based on your routing)
        bool isAdminRoute = context.Request.Path.StartsWithSegments("/api/admin", StringComparison.OrdinalIgnoreCase) ||
                           context.Request.Path.Value?.Contains("/tenant-data", StringComparison.OrdinalIgnoreCase) == true;

        if (!isAdminRoute)
        {
            await _next(context);
            return;
        }

        // Check if user has SuperAdmin role
        bool isAdmin = context.User.IsInRole("SuperAdmin");

        if (!isAdmin)
        {
            // Not an admin, let other middleware handle authorization
            await _next(context);
            return;
        }

        // Get client IP address
        IPAddress? remoteIp = context.Connection.RemoteIpAddress;

        if (remoteIp == null)
        {
            _logger.LogWarning("ADMIN_IP_BLOCKED | Unable to determine client IP address");
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Unable to determine IP address",
                message = "Admin access denied: IP address could not be determined"
            });
            return;
        }

        // Check if IP is whitelisted
        bool isAllowed = _allowedIps.Contains(remoteIp);

        // Handle IPv4-mapped IPv6 addresses (::ffff:192.168.1.1)
        if (!isAllowed && remoteIp.IsIPv4MappedToIPv6)
        {
            IPAddress ipv4 = remoteIp.MapToIPv4();
            isAllowed = _allowedIps.Contains(ipv4);
        }

        if (!isAllowed)
        {
            _logger.LogWarning(
                "ADMIN_IP_BLOCKED | AdminUserId={UserId} | IpAddress={IpAddress} | Path={Path} | Reason=IP_NOT_WHITELISTED",
                context.User.Identity?.Name,
                remoteIp,
                context.Request.Path);

            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Forbidden",
                message = "Admin access denied: Your IP address is not whitelisted for admin operations"
            });
            return;
        }

        // IP is allowed, log and continue
        _logger.LogInformation(
            "ADMIN_IP_ALLOWED | AdminUserId={UserId} | IpAddress={IpAddress} | Path={Path}",
            context.User.Identity?.Name,
            remoteIp,
            context.Request.Path);

        await _next(context);
    }
}
