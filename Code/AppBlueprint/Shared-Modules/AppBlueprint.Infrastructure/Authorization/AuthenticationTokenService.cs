using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.Authorization;

/// <summary>
/// Interface for retrieving authentication tokens from the current HTTP context
/// </summary>
public interface IAuthenticationTokenService
{
    /// <summary>
    /// Retrieves the access token from the authentication properties
    /// </summary>
    /// <returns>The access token or null if not available</returns>
    Task<string?> GetAccessTokenAsync();
    
    /// <summary>
    /// Retrieves the ID token from the authentication properties
    /// </summary>
    /// <returns>The ID token or null if not available</returns>
    Task<string?> GetIdTokenAsync();
    
    /// <summary>
    /// Retrieves the refresh token from the authentication properties
    /// </summary>
    /// <returns>The refresh token or null if not available</returns>
    Task<string?> GetRefreshTokenAsync();
}

/// <summary>
/// Service for retrieving authentication tokens from the current HTTP context.
/// This is specifically designed to work with Blazor Server where tokens are stored
/// in authentication properties during the OIDC callback.
/// </summary>
public sealed class AuthenticationTokenService : IAuthenticationTokenService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthenticationTokenService> _logger;

    /// <summary>
    /// Initializes a new instance of the AuthenticationTokenService class
    /// </summary>
    /// <param name="httpContextAccessor">HTTP context accessor to retrieve the current context</param>
    /// <param name="logger">Logger for diagnostic information</param>
    public AuthenticationTokenService(
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuthenticationTokenService> logger)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<string?> GetAccessTokenAsync()
    {
        HttpContext? httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            _logger.LogWarning("[AuthenticationTokenService] HttpContext is null - cannot retrieve access token");
            return null;
        }

        _logger.LogInformation("[AuthenticationTokenService] Attempting to retrieve access token from authentication properties");

        // Try the cookie scheme first (most common for Blazor Server)
        string? token = await httpContext.GetTokenAsync("Logto.Cookie", "access_token");
        
        if (!string.IsNullOrEmpty(token))
        {
            _logger.LogInformation("[AuthenticationTokenService] ✅ Found access token in 'Logto.Cookie' scheme (length: {Length})", token.Length);
            return token;
        }
        
        _logger.LogWarning("[AuthenticationTokenService] No token in 'Logto.Cookie' scheme, trying 'Logto' scheme");
        
        // Fallback to the OIDC scheme
        token = await httpContext.GetTokenAsync("Logto", "access_token");
        
        if (!string.IsNullOrEmpty(token))
        {
            _logger.LogInformation("[AuthenticationTokenService] ✅ Found access token in 'Logto' scheme (length: {Length})", token.Length);
            return token;
        }
        
        _logger.LogWarning("[AuthenticationTokenService] No token in 'Logto' scheme, trying default scheme");
        
        // Last fallback - try without specifying scheme
        token = await httpContext.GetTokenAsync("access_token");
        
        if (!string.IsNullOrEmpty(token))
        {
            _logger.LogInformation("[AuthenticationTokenService] ✅ Found access token in default scheme (length: {Length})", token.Length);
            return token;
        }

        _logger.LogError("[AuthenticationTokenService] ❌ Failed to retrieve access token from any authentication scheme");
        _logger.LogInformation("[AuthenticationTokenService] User authenticated: {IsAuthenticated}", httpContext.User?.Identity?.IsAuthenticated ?? false);
        _logger.LogInformation("[AuthenticationTokenService] User name: {UserName}", httpContext.User?.Identity?.Name ?? "null");
        
        return null;
    }

    /// <inheritdoc />
    public async Task<string?> GetIdTokenAsync()
    {
        HttpContext? httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            return null;
        }

        // Try both the OpenID Connect scheme and the cookie scheme
        string? token = await httpContext.GetTokenAsync("Logto.Cookie", "id_token");
        
        if (string.IsNullOrEmpty(token))
        {
            token = await httpContext.GetTokenAsync("Logto", "id_token");
        }
        
        if (string.IsNullOrEmpty(token))
        {
            token = await httpContext.GetTokenAsync("id_token");
        }

        return token;
    }

    /// <inheritdoc />
    public async Task<string?> GetRefreshTokenAsync()
    {
        HttpContext? httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            return null;
        }

        // Try both the OpenID Connect scheme and the cookie scheme
        string? token = await httpContext.GetTokenAsync("Logto.Cookie", "refresh_token");
        
        if (string.IsNullOrEmpty(token))
        {
            token = await httpContext.GetTokenAsync("Logto", "refresh_token");
        }
        
        if (string.IsNullOrEmpty(token))
        {
            token = await httpContext.GetTokenAsync("refresh_token");
        }

        return token;
    }
}
