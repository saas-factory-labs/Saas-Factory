using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;

namespace AppBlueprint.Infrastructure.Authorization;

/// <summary>
/// Stub implementation of IUserAuthenticationProvider for compatibility with UiKit components.
/// This implementation works with ASP.NET Core's authentication state (Logto SDK).
/// UiKit components should be updated to use AuthenticationStateProvider directly instead.
/// </summary>
public class AspNetCoreAuthenticationProviderStub : IUserAuthenticationProvider
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public AspNetCoreAuthenticationProviderStub(AuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider = authenticationStateProvider ?? throw new ArgumentNullException(nameof(authenticationStateProvider));
    }

    /// <summary>
    /// Not supported - Use Logto's /signin-logto endpoint instead
    /// </summary>
    public Task<bool> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException(
            "LoginAsync is not supported with Logto authentication. " +
            "Navigate to /signin-logto or /login to authenticate via Logto.");
    }

    /// <summary>
    /// Not supported - Use Logto's /signout-logto endpoint instead
    /// </summary>
    public Task LogoutAsync()
    {
        throw new NotSupportedException(
            "LogoutAsync is not supported with Logto authentication. " +
            "Navigate to /signout-logto or /logout to sign out via Logto.");
    }

    /// <summary>
    /// Checks if the user is authenticated using ASP.NET Core's authentication state
    /// </summary>
    public bool IsAuthenticated()
    {
        var authState = _authenticationStateProvider.GetAuthenticationStateAsync().GetAwaiter().GetResult();
        return authState.User.Identity?.IsAuthenticated ?? false;
    }

    /// <summary>
    /// Not needed with ASP.NET Core authentication - state is managed by the framework
    /// </summary>
    public Task InitializeFromStorageAsync()
    {
        // No-op: ASP.NET Core manages authentication state automatically
        return Task.CompletedTask;
    }

    /// <summary>
    /// Returns the Logto logout URL
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1055:URI return values should not be strings", Justification = "OAuth redirect URIs are returned as strings for authentication protocols")]
    public string? GetLogoutUrl(string postLogoutRedirectUri)
    {
        // Logto SDK handles logout automatically via /signout-logto
        return "/signout-logto";
    }

    /// <summary>
    /// Authenticates requests - not used with cookie-based auth
    /// </summary>
    public Task AuthenticateRequestAsync(RequestInformation request, Dictionary<string, object>? additionalAuthenticationContext = null, CancellationToken cancellationToken = default)
    {
        // With cookie-based authentication, the browser automatically sends cookies
        // No need to manually add Authorization header
        return Task.CompletedTask;
    }

    /// <summary>
    /// Returns the allowed hosts list (not applicable for cookie-based auth)
    /// </summary>
    public AllowedHostsValidator AllowedHostsValidator => new AllowedHostsValidator();
}

