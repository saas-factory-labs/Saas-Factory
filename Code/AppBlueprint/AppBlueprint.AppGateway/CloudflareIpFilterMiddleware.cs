using System.Net;

namespace AppBlueprint.AppGateway;

/// <summary>
/// Middleware that restricts incoming requests to those originating from Cloudflare's
/// published IP ranges. Requests missing the CF-Ray header or arriving from a non-Cloudflare
/// IP are rejected with 403 Forbidden.
///
/// Health-check paths are exempt so that Railway/Kubernetes probes continue to work.
///
/// Configuration (via environment variables or appsettings):
///   CloudflareFilter__Enabled   – set to "false" to bypass (default: true in production)
///   CloudflareFilter__IpRanges  – JSON array of CIDR strings to override defaults
///
/// Cloudflare IP reference: https://www.cloudflare.com/ips/
/// </summary>
internal sealed partial class CloudflareIpFilterMiddleware
{
    // Cloudflare published IP ranges (last verified 2025-03).
    // https://www.cloudflare.com/ips-v4  /  https://www.cloudflare.com/ips-v6
    private static readonly string[] DefaultCloudflareIpRanges =
    [
        // IPv4
        "173.245.48.0/20",
        "103.21.244.0/22",
        "103.22.200.0/22",
        "103.31.4.0/22",
        "141.101.64.0/18",
        "108.162.192.0/18",
        "190.93.240.0/20",
        "188.114.96.0/20",
        "197.234.240.0/22",
        "198.41.128.0/17",
        "162.158.0.0/15",
        "104.16.0.0/13",
        "104.24.0.0/14",
        "172.64.0.0/13",
        "131.0.72.0/22",
        // IPv6
        "2400:cb00::/32",
        "2606:4700::/32",
        "2803:f800::/32",
        "2405:b500::/32",
        "2405:8100::/32",
        "2a06:98c0::/29",
        "2c0f:f248::/32",
    ];

    // Paths that must remain reachable regardless of origin (health / readiness probes).
    private static readonly string[] ExemptPathPrefixes =
    [
        "/health",
        "/healthz",
        "/alive",
    ];

    private readonly RequestDelegate _next;
    private readonly ILogger<CloudflareIpFilterMiddleware> _logger;
    private readonly bool _enabled;
    private readonly IReadOnlyList<(IPAddress Network, int PrefixLength)> _allowedRanges;

    public CloudflareIpFilterMiddleware(
        RequestDelegate next,
        ILogger<CloudflareIpFilterMiddleware> logger,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(next);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(environment);

        _next = next;
        _logger = logger;

        // Default to disabled in Development so local Swagger / Aspire dashboard work.
        bool defaultEnabled = !environment.IsDevelopment();
        _enabled = configuration.GetValue("CloudflareFilter:Enabled", defaultEnabled);

        string[] configuredRanges = configuration.GetSection("CloudflareFilter:IpRanges")
            .Get<string[]>() ?? DefaultCloudflareIpRanges;

        _allowedRanges = ParseCidrRanges(configuredRanges);

        LogFilterInitialized(logger, _enabled, _allowedRanges.Count);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!_enabled || IsExemptPath(context.Request.Path))
        {
            await _next(context);
            return;
        }

        IPAddress? remoteIp = context.Connection.RemoteIpAddress;

        if (!IsValidCloudflareRequest(remoteIp, context.Request))
        {
            LogBlockedRequest(_logger, remoteIp?.ToString(), context.Request.Path.Value);

            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Forbidden");
            return;
        }

        await _next(context);
    }

    private bool IsValidCloudflareRequest(IPAddress? remoteIp, HttpRequest request)
        => remoteIp is not null
            && HasRequiredCloudflareHeaders(request)
            && IsCloudflareIp(remoteIp);

    private static bool HasRequiredCloudflareHeaders(HttpRequest request)
        => request.Headers.ContainsKey("CF-Ray")
            && request.Headers.ContainsKey("CF-Connecting-IP");

    private bool IsCloudflareIp(IPAddress remoteIp)
    {
        // Normalise IPv4-mapped IPv6 addresses (e.g. ::ffff:1.2.3.4).
        if (remoteIp.IsIPv4MappedToIPv6)
        {
            remoteIp = remoteIp.MapToIPv4();
        }

        foreach ((IPAddress network, int prefixLength) in _allowedRanges)
        {
            if (IsInCidrRange(remoteIp, network, prefixLength))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsInCidrRange(IPAddress ip, IPAddress network, int prefixLength)
    {
        if (ip.AddressFamily != network.AddressFamily)
        {
            return false;
        }

        byte[] ipBytes = ip.GetAddressBytes();
        byte[] networkBytes = network.GetAddressBytes();

        int fullBytes = prefixLength / 8;
        int remainingBits = prefixLength % 8;

        for (int i = 0; i < fullBytes; i++)
        {
            if (ipBytes[i] != networkBytes[i])
            {
                return false;
            }
        }

        if (remainingBits > 0 && fullBytes < ipBytes.Length)
        {
            int mask = 0xFF << (8 - remainingBits);
            if ((ipBytes[fullBytes] & mask) != (networkBytes[fullBytes] & mask))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsExemptPath(PathString path)
    {
        foreach (string prefix in ExemptPathPrefixes)
        {
            if (path.StartsWithSegments(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    [LoggerMessage(Level = LogLevel.Information,
        Message = "CloudflareIpFilter initialized – enabled: {Enabled}, ranges: {Count}")]
    private static partial void LogFilterInitialized(ILogger logger, bool enabled, int count);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "Blocked request from {RemoteIp} – not a Cloudflare IP or missing CF-Ray/CF-Connecting-IP headers. Path: {Path}")]
    private static partial void LogBlockedRequest(ILogger logger, string? remoteIp, string? path);

    private static IReadOnlyList<(IPAddress Network, int PrefixLength)> ParseCidrRanges(
        string[] ranges)
    {
        var result = new List<(IPAddress, int)>(ranges.Length);

        foreach (string range in ranges)
        {
            int slashIndex = range.IndexOf('/', StringComparison.Ordinal);
            if (slashIndex < 0)
            {
                continue;
            }

            if (IPAddress.TryParse(range.AsSpan(0, slashIndex), out IPAddress? network)
                && int.TryParse(range.AsSpan(slashIndex + 1), out int prefixLength))
            {
                result.Add((network, prefixLength));
            }
        }

        return result;
    }
}
