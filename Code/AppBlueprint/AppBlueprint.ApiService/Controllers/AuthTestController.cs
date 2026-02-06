using System.Security.Claims;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppBlueprint.ApiService.Controllers;

/// <summary>
/// Test controller for validating JWT authentication
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
internal sealed class AuthTestController(ILogger<AuthTestController> logger) : ControllerBase
{
    private readonly ILogger<AuthTestController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Public endpoint - no authentication required
    /// </summary>
    [HttpGet("public")]
    [AllowAnonymous]
    public IActionResult GetPublic()
    {
        _logger.LogInformation("Public endpoint accessed");

        return Ok(new
        {
            Message = "This is a public endpoint - no authentication required",
            Timestamp = DateTime.UtcNow,
            IsAuthenticated = User.Identity?.IsAuthenticated ?? false
        });
    }

    /// <summary>
    /// Protected endpoint - requires valid JWT token
    /// </summary>
    [HttpGet("protected")]
    [Authorize]
    public IActionResult GetProtected()
    {
        _logger.LogInformation("Protected endpoint accessed by user: {User}", User.Identity?.Name);

        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();

        return Ok(new
        {
            Message = "Successfully authenticated!",
            Timestamp = DateTime.UtcNow,
            User = new
            {
                User.Identity?.IsAuthenticated,
                User.Identity?.Name,
                UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                Email = User.FindFirst(ClaimTypes.Email)?.Value,
                Roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList(),
                User.Identity?.AuthenticationType
            },
            AllClaims = claims
        });
    }

    /// <summary>
    /// Admin-only endpoint - requires Admin role
    /// </summary>
    [HttpGet("admin")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult GetAdmin()
    {
        _logger.LogInformation("Admin endpoint accessed by user: {User}", User.Identity?.Name);

        return Ok(new
        {
            Message = "You have admin access!",
            Timestamp = DateTime.UtcNow,
            User = User.Identity?.Name
        });
    }

    /// <summary>
    /// User or Admin endpoint - requires User or Admin role
    /// </summary>
    [HttpGet("user")]
    [Authorize(Policy = "UserOrAdmin")]
    public IActionResult GetUserEndpoint()
    {
        _logger.LogInformation("User endpoint accessed by user: {User}", User.Identity?.Name);

        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

        return Ok(new
        {
            Message = "You have user or admin access!",
            Timestamp = DateTime.UtcNow,
            User = User.Identity?.Name,
            Roles = roles
        });
    }

    /// <summary>
    /// Development endpoint - returns the JWT token from Authorization header
    /// WARNING: FOR DEVELOPMENT/TESTING ONLY - DO NOT USE IN PRODUCTION
    /// </summary>
    [HttpGet("get-token")]
    [Authorize]
    public IActionResult GetToken()
    {
#if !DEBUG
        return BadRequest(new { Error = "This endpoint is only available in DEBUG mode" });
#endif

        _logger.LogInformation("Token extraction endpoint accessed by user: {User}", User.Identity?.Name);

        // Try to get token from Authorization header
        var authHeader = Request.Headers.Authorization.ToString();

        if (string.IsNullOrEmpty(authHeader))
        {
            return Ok(new
            {
                Message = "No Authorization header found in this request",
                Note = "Your Blazor Server app uses cookie-based authentication. The JWT token may not be passed in HTTP headers.",
                User.Identity?.IsAuthenticated,
                User = User.Identity?.Name,
                Claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList(),
                Suggestion = "Try calling your API directly from a REST client (Postman, curl) with a JWT token, or check the Logto callback handler."
            });
        }

        // Extract token (remove "Bearer " prefix)
        string token = authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            ? authHeader.AsSpan(7).ToString()
            : authHeader;

        return Ok(new
        {
            Message = "JWT Token extracted successfully",
            Token = token,
            User = User.Identity?.Name,
            User.Identity?.IsAuthenticated,
            Warning = "FOR DEVELOPMENT ONLY - Never expose tokens in production"
        });
    }

    /// <summary>
    /// Echo endpoint - returns information about the current request
    /// </summary>
    [HttpGet("echo")]
    [Authorize]
    public IActionResult Echo()
    {
        var authHeader = Request.Headers.Authorization.ToString();
        var tokenPreview = string.Empty;

        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            var token = authHeader["Bearer ".Length..];
            tokenPreview = token.Length > 50 ? $"{token[..50]}..." : token;
        }

        return Ok(new
        {
            Message = "Echo - Request received",
            Timestamp = DateTime.UtcNow,
            Request = new
            {
                Request.Method,
                Path = Request.Path.Value,
                HasAuthHeader = !string.IsNullOrEmpty(authHeader),
                TokenPreview = tokenPreview
            },
            User = new
            {
                User.Identity?.IsAuthenticated,
                User.Identity?.Name,
                Claims = User.Claims.Select(c => new { c.Type, c.Value })
            }
        });
    }
}
