using Microsoft.Kiota.Abstractions.Authentication;

namespace AppBlueprint.Infrastructure.Authorization;

/// <summary>
///     Defines an authentication provider that supports logging in, logging out,
///     and injecting authentication headers into Kiota-based HTTP requests.
/// </summary>
public interface IUserAuthenticationProvider : IAuthenticationProvider
{
    /// <summary>
    ///     Attempts to log in with the provided email and password.
    ///     Stores the resulting access token and expiration.
    /// </summary>
    Task<bool> LoginAsync(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Logs out the user by clearing the stored authentication token.
    /// </summary>
    Task LogoutAsync();

    /// <summary>
    ///     Checks if the user is currently authenticated with a valid token.
    /// </summary>
    /// <returns>True if the user has a valid access token, false otherwise.</returns>
    bool IsAuthenticated();

    /// <summary>
    ///     Initializes the authentication state from stored tokens.
    ///     Should be called when the provider is created to restore previous authentication.
    /// </summary>
    Task InitializeFromStorageAsync();

    /// <summary>
    ///     Get the logout URL for the authentication provider (for OIDC providers)
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1054:URI parameters should not be strings", Justification = "OAuth redirect URIs are provided as strings by authentication protocols")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1055:URI return values should not be strings", Justification = "OAuth redirect URIs are returned as strings for authentication protocols")]
    string? GetLogoutUrl(string postLogoutRedirectUri);
}
