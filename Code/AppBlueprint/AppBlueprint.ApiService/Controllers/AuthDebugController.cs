using AppBlueprint.Application.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppBlueprint.ApiService.Controllers;

/// <summary>
/// Debug controller to help diagnose authentication issues.
/// WARNING: Should be disabled in production or restricted to DeploymentManagerAdmin only.
/// </summary>
[Authorize(Roles = Roles.DeploymentManagerAdmin)]
[ApiController]
[Route("api/[controller]")]
internal sealed class AuthDebugController(ILogger<AuthDebugController> logger) : ControllerBase
{
    private readonly ILogger<AuthDebugController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Test endpoint that doesn't require authentication
    /// </summary>
    [HttpGet("ping")]
    [AllowAnonymous]
    public IActionResult Ping()
    {
        _logger.LogInformation("Ping endpoint called - no authentication required");
        return Ok(new { message = "Pong! API is reachable.", timestamp = DateTime.UtcNow });
    }

    /// <summary>
    /// Test endpoint that requires authentication
    /// </summary>
    [HttpGet("secure-ping")]
    [Authorize]
    public IActionResult SecurePing()
    {
        string userId = User.FindFirst("sub")?.Value ?? "Unknown";
        string userName = User.Identity?.Name ?? "Unknown";
        string issuer = User.FindFirst("iss")?.Value ?? "Unknown";
        bool isAuthenticated = User.Identity?.IsAuthenticated ?? false;

        _logger.LogInformation(
            "Secure ping endpoint called. User: {User}, UserId: {UserId}, Issuer: {Issuer}, IsAuthenticated: {IsAuthenticated}",
            userName, userId, issuer, isAuthenticated);

        return Ok(new
        {
            message = "Authenticated successfully!",
            timestamp = DateTime.UtcNow,
            user = new
            {
                id = userId,
                name = userName,
                issuer,
                isAuthenticated,
                claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
            }
        });
    }

    /// <summary>
    /// Get authentication headers from request (DeploymentManagerAdmin only)
    /// </summary>
    [HttpGet("headers")]
    public IActionResult GetHeaders()
    {
        var authHeader = Request.Headers.Authorization.ToString();
        var tenantIdHeader = Request.Headers["tenant-id"].ToString();

        var hasAuth = !string.IsNullOrEmpty(authHeader);
        var hasTenantId = !string.IsNullOrEmpty(tenantIdHeader);

        // Log to console for debugging, don't return sensitive data
        Console.WriteLine($"Headers endpoint called. HasAuth: {hasAuth}, HasTenantId: {hasTenantId}");
        Console.WriteLine($"Authorization header: {authHeader}");
        Console.WriteLine($"Tenant ID: {tenantIdHeader}");

        foreach (var header in Request.Headers)
        {
            Console.WriteLine($"{header.Key}: {header.Value}");
        }

        return Ok(new
        {
            message = "Headers logged to console. Check application logs for details.",
            timestamp = DateTime.UtcNow
        });
    }
}

