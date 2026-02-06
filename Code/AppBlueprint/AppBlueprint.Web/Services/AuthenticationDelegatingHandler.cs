using System.Net.Http.Headers;
using AppBlueprint.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.JSInterop;

namespace AppBlueprint.Web.Services;

/// <summary>
/// Delegating handler that adds JWT authentication token and tenant-id to HTTP requests
/// </summary>
internal class AuthenticationDelegatingHandler : DelegatingHandler
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
                        // CRITICAL: Opaque tokens indicate missing API Resource configuration
                        _logger.LogError("[AuthHandler] ❌ CRITICAL: Received OPAQUE access_token (length: {Length}). This means LOGTO_RESOURCE is not configured!", accessToken.Length);
                        _logger.LogError("[AuthHandler] ❌ Opaque tokens cannot be validated by the API. Add LOGTO_RESOURCE environment variable.");
                        _logger.LogError("[AuthHandler] ❌ Expected: JWT with 3 parts (header.payload.signature), Got: {TokenPreview}...",
                            accessToken.Length > 20 ? accessToken[..20] : accessToken);

                        // DO NOT fall back to id_token - this is a configuration error that must be fixed
                        throw new InvalidOperationException(
                            "Authentication failed: Received opaque access token instead of JWT. " +
                            "Configure LOGTO_RESOURCE environment variable to receive JWT tokens with proper scopes. " +
                            "See LOGTO-ENVIRONMENT-VARIABLES.md for setup instructions.");
                    }
                }
                else
                {
                    _logger.LogError("[AuthHandler] ❌ No access_token in HttpContext - authentication may have failed");
                    throw new InvalidOperationException(
                        "Authentication failed: No access token available. " +
                        "Ensure Logto authentication completed successfully and tokens were saved.");
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
                string tokenPreview = token.Length > 50 ? string.Concat(token.AsSpan(0, 50), "...") : token;
                _logger.LogInformation("[AuthHandler] ✅ Added Bearer token to request: {Method} {Uri}, Token preview: {Preview}",
                    request.Method, request.RequestUri, tokenPreview);
            }
            else
            {
                _logger.LogError("[AuthHandler] ❌ NO VALID JWT TOKEN FOUND for request: {Method} {Uri}. User authenticated: {IsAuth}",
                    request.Method, request.RequestUri, httpContext?.User?.Identity?.IsAuthenticated ?? false);
                _logger.LogError("[AuthHandler] API calls will fail with 401 Unauthorized");
            }

            // Note: Tenant ID is now extracted from JWT claims by TenantMiddleware on the API side
            // We no longer need to send the tenant-id header - it's derived from the Bearer token
            // This eliminates a potential security vulnerability where the header could be forged
        }
#pragma warning disable CA1031 // Generic catch for graceful degradation - authentication errors should not prevent request from being sent
        catch (Exception ex)
        {
            // Log any errors but don't fail the request
            _logger.LogError(ex, "[AuthHandler] Error adding authentication to request: {Method} {Uri}", request.Method, request.RequestUri);
        }
#pragma warning restore CA1031

        return await base.SendAsync(request, cancellationToken);
    }


}

