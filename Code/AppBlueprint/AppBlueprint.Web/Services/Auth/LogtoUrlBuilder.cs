using Microsoft.Extensions.Configuration;

namespace AppBlueprint.Web.Services.Auth;

/// <summary>
/// Helper service for building Logto authentication URLs
/// </summary>
public class LogtoUrlBuilder
{
    private readonly IConfiguration _configuration;
    private readonly string _baseUrl;

    public LogtoUrlBuilder(IConfiguration configuration, string baseUrl)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrWhiteSpace(baseUrl);

        _configuration = configuration;
        _baseUrl = baseUrl;
    }

    /// <summary>
    /// Builds a Logto sign-up URL with the specified state parameter
    /// </summary>
    /// <param name="state">State parameter to pass to Logto (e.g., "personal" or "business")</param>
    /// <param name="callbackPath">Optional callback path (defaults to "signup/callback")</param>
    /// <returns>Complete Logto sign-up URL</returns>
    public string BuildSignupUrl(string state, string callbackPath = "signup/callback")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(state);

        var logtoEndpoint = _configuration["Logto:Endpoint"];
        var appId = _configuration["Logto:AppId"];
        
        if (string.IsNullOrEmpty(logtoEndpoint) || string.IsNullOrEmpty(appId))
        {
            throw new InvalidOperationException("Logto configuration is missing. Please configure Logto:Endpoint and Logto:AppId");
        }

        var redirectUri = $"{_baseUrl}{callbackPath}";
        
        return $"{logtoEndpoint}/auth?client_id={appId}&redirect_uri={Uri.EscapeDataString(redirectUri)}&response_type=code&scope=openid%20profile%20email&state={state}";
    }

    /// <summary>
    /// Generates a username from an email address
    /// </summary>
    /// <param name="email">Email address</param>
    /// <returns>Username (email prefix)</returns>
    public static string GenerateUsernameFromEmail(string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        
        return email.Split('@')[0];
    }
}
