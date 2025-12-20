using System.Net.Http.Headers;
using AppBlueprint.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.JSInterop;

namespace AppBlueprint.Web.Services;

/// <summary>
/// Delegating handler that adds JWT authentication token and tenant-id to HTTP requests
/// </summary>
public class AuthenticationDelegatingHandler : DelegatingHandler
{
    private readonly ITokenStorageService _tokenStorageService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthenticationDelegatingHandler> _logger;

    public AuthenticationDelegatingHandler(
        ITokenStorageService tokenStorageService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuthenticationDelegatingHandler> logger)
    {
        ArgumentNullException.ThrowIfNull(tokenStorageService);
        ArgumentNullException.ThrowIfNull(httpContextAccessor);
        ArgumentNullException.ThrowIfNull(logger);

        _tokenStorageService = tokenStorageService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            string? token = null;
            
            // For OpenID Connect / Blazor Server, try to get token from HttpContext
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                _logger.LogInformation("[AuthHandler] User is authenticated: {User}", httpContext.User.Identity.Name);

                // Try to get the access token from authentication properties
                // Explicitly specify "Logto" scheme to get tokens saved by Logto authentication
                var accessToken = await httpContext.GetTokenAsync("Logto", "access_token");
                
                if (!string.IsNullOrEmpty(accessToken))
                {
                    // Validate that it's a proper JWT (has 3 parts separated by dots)
                    if (accessToken.Split('.').Length == 3)
                    {
                        _logger.LogInformation("[AuthHandler] ✅ Retrieved VALID JWT access_token from HttpContext (length: {Length})", accessToken.Length);
                        token = accessToken;
                    }
                    else
                    {
                        _logger.LogWarning("[AuthHandler] ⚠️ access_token in HttpContext is NOT a valid JWT (length: {Length}, format invalid). Ignoring it. Token: {Token}", 
                            accessToken.Length, accessToken);
                    }
                }
                
                // If no valid access_token, try id_token
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("[AuthHandler] ❌ No valid access_token in HttpContext, trying id_token");

                    // Also explicitly specify "Logto" scheme for id_token
                    var idToken = await httpContext.GetTokenAsync("Logto", "id_token");
                    
                    if (!string.IsNullOrEmpty(idToken))
                    {
                        // Validate that it's a proper JWT
                        if (idToken.Split('.').Length == 3)
                        {
                            _logger.LogInformation("[AuthHandler] ✅ Retrieved VALID JWT id_token from HttpContext (length: {Length})", idToken.Length);
                            token = idToken;
                        }
                        else
                        {
                            _logger.LogWarning("[AuthHandler] ⚠️ id_token in HttpContext is NOT a valid JWT (length: {Length}). Ignoring it.", idToken.Length);
                        }
                    }
                    else
                    {
                        _logger.LogError("[AuthHandler] ❌ CRITICAL: No valid JWT tokens (access_token OR id_token) available in HttpContext!");
                        _logger.LogError("[AuthHandler] OpenID Connect authentication completed but tokens were not saved properly");
                        
                        // Try to get all available token names for debugging
                        try
                        {
                            var authResult = await httpContext.AuthenticateAsync();
                            if (authResult?.Properties?.Items != null)
                            {
                                var tokenNames = authResult.Properties.Items
                                    .Where(x => x.Key.StartsWith(".Token."))
                                    .Select(x => x.Key.Replace(".Token.", ""));
                                _logger.LogInformation("[AuthHandler] Available tokens in HttpContext: {Tokens}", 
                                    string.Join(", ", tokenNames));
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "[AuthHandler] Error checking available tokens");
                        }
                    }
                }
            }
            else
            {
                _logger.LogWarning("[AuthHandler] User is NOT authenticated or HttpContext is null");
            }
            
            // Fallback: Try to get token from localStorage (for backward compatibility) - DISABLED for now
            // We're disabling this because localStorage has invalid legacy tokens
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("[AuthHandler] ⚠️ No valid JWT token found in HttpContext. localStorage fallback is disabled.");
                _logger.LogWarning("[AuthHandler] This means OpenID Connect is not properly saving tokens in authentication properties.");
            }

            if (!string.IsNullOrEmpty(token))
            {
                // Add the Bearer token to the Authorization header
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var tokenPreview = token.Length > 50 ? token.Substring(0, 50) + "..." : token;
                _logger.LogInformation("[AuthHandler] ✅ Added Bearer token to request: {Method} {Uri}, Token preview: {Preview}", 
                    request.Method, request.RequestUri, tokenPreview);
            }
            else
            {
                _logger.LogError("[AuthHandler] ❌ NO VALID JWT TOKEN FOUND for request: {Method} {Uri}. User authenticated: {IsAuth}", 
                    request.Method, request.RequestUri, httpContext?.User?.Identity?.IsAuthenticated ?? false);
                _logger.LogError("[AuthHandler] API calls will fail with 401 Unauthorized");
            }
            
            // Add tenant-id header (required by TenantMiddleware)
            if (!request.Headers.Contains("tenant-id"))
            {
                var tenantId = await GetTenantIdAsync();
                request.Headers.Add("tenant-id", tenantId);
                _logger.LogDebug("[AuthHandler] Added tenant-id header: {TenantId}", tenantId);
            }
        }
        catch (Exception ex)
        {
            // Log any errors but don't fail the request
            _logger.LogError(ex, "[AuthHandler] Error adding authentication to request: {Method} {Uri}", request.Method, request.RequestUri);
        }

        return await base.SendAsync(request, cancellationToken);
    }

    private async Task<string> GetTenantIdAsync()
    {
        // First, try to get tenant ID from JWT claims in HttpContext
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            // Try to get tenant_id from JWT claims
            var tenantClaim = httpContext.User.FindFirst("tenant_id")?.Value;
            
            if (!string.IsNullOrEmpty(tenantClaim))
            {
                _logger.LogDebug("Retrieved tenant ID from JWT claims: {TenantId}", tenantClaim);
                return tenantClaim;
            }
            
            // Also check for "tid" claim (common alternative)
            tenantClaim = httpContext.User.FindFirst("tid")?.Value;
            if (!string.IsNullOrEmpty(tenantClaim))
            {
                _logger.LogDebug("Retrieved tenant ID from JWT 'tid' claim: {TenantId}", tenantClaim);
                return tenantClaim;
            }
            
            _logger.LogWarning("User is authenticated but no tenant_id or tid claim found in JWT");
        }
        
        // Fallback: Try to get tenant ID from storage
        try
        {
            var tenantId = await _tokenStorageService.GetValueAsync("tenant_id");
            
            if (!string.IsNullOrEmpty(tenantId))
            {
                _logger.LogDebug("Retrieved tenant ID from storage: {TenantId}", tenantId);
                return tenantId;
            }
        }
        catch (Exception ex)
        {
            _logger.LogDebug("Could not retrieve tenant ID from storage: {Message}", ex.Message);
        }
        
        // SECURITY: Fail-fast if tenant cannot be resolved to prevent tenant isolation bypass
        string errorMessage = "Tenant ID could not be resolved from JWT claims or storage. This is required for tenant isolation.";
        _logger.LogError("[AuthHandler] {ErrorMessage}", errorMessage);
        throw new InvalidOperationException(errorMessage);
    }
}

