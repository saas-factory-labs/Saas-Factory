using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;

namespace AppBlueprint.Infrastructure.Authorization;

/// <summary>
/// Kiota IAuthenticationProvider implementation for ASP.NET Core cookie-based authentication.
/// This provider is used with Kiota's HttpClientRequestAdapter for API calls.
/// With cookie-based authentication (Logto SDK), the browser automatically sends authentication cookies,
/// so this provider doesn't need to add Authorization headers.
/// </summary>
public class AspNetCoreKiotaAuthenticationProvider : IAuthenticationProvider
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly ITokenStorageService? _tokenStorageService;

    /// <summary>
    /// Initializes a new instance of the AspNetCoreKiotaAuthenticationProvider class.
    /// </summary>
    /// <param name="authenticationStateProvider">The ASP.NET Core authentication state provider</param>
    /// <param name="tokenStorageService">Optional token storage service for accessing JWT tokens if available</param>
    public AspNetCoreKiotaAuthenticationProvider(
        AuthenticationStateProvider authenticationStateProvider,
        ITokenStorageService? tokenStorageService = null)
    {
        _authenticationStateProvider = authenticationStateProvider ?? throw new ArgumentNullException(nameof(authenticationStateProvider));
        _tokenStorageService = tokenStorageService;
    }

    /// <summary>
    /// Authenticates the API request.
    /// With cookie-based authentication, the browser automatically sends authentication cookies,
    /// so this method is primarily a pass-through. If a JWT token is available in storage,
    /// it will be added to the Authorization header for API calls.
    /// </summary>
    public async Task AuthenticateRequestAsync(
        RequestInformation request,
        Dictionary<string, object>? additionalAuthenticationContext = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Check if user is authenticated
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        if (authState.User.Identity?.IsAuthenticated != true)
        {
            // Not authenticated - don't add any headers
            return;
        }

        // If token storage is available, try to get JWT token for API authentication
        if (_tokenStorageService is not null)
        {
            try
            {
                var token = await _tokenStorageService.GetTokenAsync();
                if (!string.IsNullOrEmpty(token))
                {
                    // Add Bearer token for API authentication
                    request.Headers.Add("Authorization", $"Bearer {token}");
                }
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"[AspNetCoreKiotaAuthenticationProvider] Error retrieving token: {ex.Message}");
                // Continue without token - cookies may still work
            }
        }

        // With cookie-based authentication, the HttpClient will automatically include cookies
        // No need to manually add Authorization header
    }

    /// <summary>
    /// Returns the allowed hosts validator.
    /// For local development and API calls, we allow all hosts.
    /// </summary>
    public AllowedHostsValidator AllowedHostsValidator => new AllowedHostsValidator();
}

