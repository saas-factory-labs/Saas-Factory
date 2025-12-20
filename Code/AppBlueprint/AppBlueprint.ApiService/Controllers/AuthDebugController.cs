using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppBlueprint.ApiService.Controllers;

/// <summary>
/// Debug controller to help diagnose authentication issues
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthDebugController : ControllerBase
{
    private readonly ILogger<AuthDebugController> _logger;

    public AuthDebugController(ILogger<AuthDebugController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

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
                issuer = issuer,
                isAuthenticated = isAuthenticated,
                claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
            }
        });
    }

    /// <summary>
    /// Get authentication headers from request
    /// </summary>
    [HttpGet("headers")]
    [AllowAnonymous]
    public IActionResult GetHeaders()
    {
        var authHeader = Request.Headers.Authorization.ToString();
        var tenantIdHeader = Request.Headers["tenant-id"].ToString();
        
        var hasAuth = !string.IsNullOrEmpty(authHeader);
        var hasTenantId = !string.IsNullOrEmpty(tenantIdHeader);
        
        _logger.LogInformation(
            "Headers endpoint called. HasAuth: {HasAuth}, HasTenantId: {HasTenantId}",
            hasAuth, hasTenantId);

        return Ok(new
        {
            hasAuthorizationHeader = hasAuth,
            authorizationHeaderPreview = hasAuth ? authHeader[..Math.Min(30, authHeader.Length)] + "..." : null,
            hasTenantIdHeader = hasTenantId,
            tenantId = tenantIdHeader,
            allHeaders = Request.Headers.Select(h => new { h.Key, Value = h.Value.ToString() }).ToList()
        });
    }
}

