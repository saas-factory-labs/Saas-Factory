using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.Authorization.Providers.Logto;

/// <summary>
/// Logto provider using Authorization Code Flow with PKCE (recommended approach)
/// This is more secure than ROPC and supports all Logto features
/// </summary>
public class LogtoAuthorizationCodeProvider : BaseAuthenticationProvider
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    private readonly HttpClient _httpClient;
    private readonly LogtoConfiguration _configuration;
    private readonly ILogger<LogtoAuthorizationCodeProvider> _logger;

    public LogtoAuthorizationCodeProvider(
        ITokenStorageService tokenStorage,
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<LogtoAuthorizationCodeProvider> logger) : base(tokenStorage)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _configuration = new LogtoConfiguration();
        configuration.GetSection("Authentication:Logto").Bind(_configuration);
        
        ValidateConfiguration();
    }

    /// <summary>
    /// Generate the Logto logout URL to properly end the session
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1054:URI parameters should not be strings", Justification = "OAuth redirect URIs are provided as strings by authentication protocols")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1055:URI return values should not be strings", Justification = "OAuth logout URLs are returned as strings for authentication protocols")]
    public string GetLogoutUrl(string postLogoutRedirectUri)
    {
        var logoutUrl = $"{_configuration.Endpoint}/oidc/session/end?" +
            $"client_id={Uri.EscapeDataString(_configuration.ClientId)}&" +
            $"post_logout_redirect_uri={Uri.EscapeDataString(postLogoutRedirectUri)}";

        _logger.LogInformation("Generated Logto logout URL");
        return logoutUrl;
    }

    /// <summary>
    /// Override logout to support proper OIDC logout flow
    /// </summary>
    public override async Task LogoutAsync()
    {
        _logger.LogInformation("LogoutAsync called - clearing local tokens");
        await base.LogoutAsync();
        _logger.LogInformation("Local tokens cleared - caller should redirect to GetLogoutUrl()");
    }

    /// <summary>
    /// For Authorization Code Flow, this method generates the authorization URL
    /// Users should be redirected to this URL to log in via Logto's hosted page
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1054:URI parameters should not be strings", Justification = "OAuth redirect URIs are provided as strings by authentication protocols")]
    public async Task<string> GetAuthorizationUrlAsync(string redirectUri)
    {
        // Generate PKCE parameters
        var codeVerifier = GenerateCodeVerifier();
        var codeChallenge = GenerateCodeChallenge(codeVerifier);
        
        // Encode the code_verifier into the state parameter so it survives the redirect
        var stateData = new
        {
            nonce = GenerateState(),
            cv = codeVerifier  // Store code verifier in state
        };
        var stateJson = JsonSerializer.Serialize(stateData);
        var state = Base64UrlEncode(Encoding.UTF8.GetBytes(stateJson));

        _logger.LogInformation("Generated PKCE parameters - CodeVerifier length: {Length}, State length: {StateLength}", 
            codeVerifier.Length, state.Length);

        var authUrl = $"{_configuration.Endpoint}/oidc/auth?" +
            $"client_id={Uri.EscapeDataString(_configuration.ClientId)}&" +
            $"redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
            $"response_type=code&" +
            $"scope={Uri.EscapeDataString(_configuration.Scope ?? "openid profile email offline_access")}&" +
            $"state={state}&" +
            $"code_challenge={codeChallenge}&" +
            $"code_challenge_method=S256&" +
            $"prompt=consent";

        _logger.LogInformation("Generated Logto authorization URL with embedded code verifier in state");
        return authUrl;
    }

    /// <summary>
    /// Exchange the authorization code for tokens after user is redirected back
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1054:URI parameters should not be strings", Justification = "OAuth redirect URIs are provided as strings by authentication protocols")]
    public async Task<AuthenticationResult> ExchangeCodeForTokensAsync(
        string code, 
        string state,
        string redirectUri, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(state);

        try
        {
            _logger.LogInformation("Starting token exchange for authorization code");
            
            // Extract code_verifier from the state parameter
            string? codeVerifier = null;
            try
            {
                var stateBytes = Base64UrlDecode(state);
                var stateJson = Encoding.UTF8.GetString(stateBytes);
                var stateData = JsonSerializer.Deserialize<JsonElement>(stateJson);
                
                if (stateData.TryGetProperty("cv", out var cvElement))
                {
                    codeVerifier = cvElement.GetString();
                    _logger.LogDebug("Code verifier extracted from state parameter");
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error extracting code verifier from state parameter");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Invalid operation extracting code verifier from state parameter");
            }
            
            if (string.IsNullOrEmpty(codeVerifier))
            {
                _logger.LogError("Code verifier not found in state parameter");
                return new AuthenticationResult
                {
                    IsSuccess = false,
                    Error = "Code verifier not found. Please restart the login flow."
                };
            }

            _logger.LogDebug("Preparing token exchange request to {Endpoint}", $"{_configuration.Endpoint}/oidc/token");

            using var formContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = _configuration.ClientId,
                ["client_secret"] = _configuration.ClientSecret,
                ["grant_type"] = "authorization_code",
                ["code"] = code,
                ["redirect_uri"] = redirectUri,
                ["code_verifier"] = codeVerifier
            });

            var response = await _httpClient.PostAsync(
                new Uri($"{_configuration.Endpoint}/oidc/token", UriKind.Absolute), 
                formContent, 
                cancellationToken);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            
            _logger.LogInformation("Token exchange response status: {StatusCode}", response.StatusCode);

            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = JsonSerializer.Deserialize<LogtoTokenResponse>(
                    responseContent, 
                    JsonOptions);

                if (tokenResponse?.AccessToken != null)
                {
                    var result = new AuthenticationResult
                    {
                        IsSuccess = true,
                        AccessToken = tokenResponse.AccessToken,
                        RefreshToken = tokenResponse.RefreshToken,
                        ExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn)
                    };

                    await StoreTokens(result);
                    _logger.LogInformation("Successfully exchanged authorization code for tokens");
                    return result;
                }
                
                _logger.LogError("Token response did not contain access token");
            }

            _logger.LogError("Token exchange failed with status: {StatusCode}, Response: {Response}", 
                response.StatusCode, responseContent);

            return new AuthenticationResult
            {
                IsSuccess = false,
                Error = $"Failed to exchange authorization code for tokens. Status: {response.StatusCode}"
            };
        }
#pragma warning disable CA1031 // Returns error result instead of throwing - consider catching specific HttpRequestException/TaskCanceledException/JsonException
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exchanging authorization code for tokens");
            return new AuthenticationResult
            {
                IsSuccess = false,
                Error = "An error occurred during authentication"
            };
        }
#pragma warning restore CA1031
    }

    /// <summary>
    /// This method is not supported for Authorization Code Flow
    /// Users must be redirected to Logto's hosted login page
    /// </summary>
    public override Task<AuthenticationResult> LoginAsync(
        LoginRequest request, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("Direct password login is not supported with Authorization Code Flow. " +
            "Use GetAuthorizationUrl() to redirect users to Logto's hosted login page.");
        
        return Task.FromResult(new AuthenticationResult
        {
            IsSuccess = false,
            Error = "Direct password login is not supported. Please use the Logto hosted login page."
        });
    }

    public override async Task<AuthenticationResult> RefreshTokenAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(RefreshToken))
        {
            return new AuthenticationResult
            {
                IsSuccess = false,
                Error = "No refresh token available"
            };
        }

        try
        {
            using var formContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = _configuration.ClientId,
                ["client_secret"] = _configuration.ClientSecret,
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = RefreshToken
            });

            var response = await _httpClient.PostAsync(
                new Uri($"{_configuration.Endpoint}/oidc/token", UriKind.Absolute), 
                formContent, 
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var tokenResponse = JsonSerializer.Deserialize<LogtoTokenResponse>(
                    responseContent, 
                    JsonOptions);

                if (tokenResponse?.AccessToken != null)
                {
                    var result = new AuthenticationResult
                    {
                        IsSuccess = true,
                        AccessToken = tokenResponse.AccessToken,
                        RefreshToken = tokenResponse.RefreshToken ?? RefreshToken,
                        ExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn)
                    };

                    await StoreTokens(result);
                    return result;
                }
            }

            return new AuthenticationResult
            {
                IsSuccess = false,
                Error = "Token refresh failed"
            };
        }
#pragma warning disable CA1031 // Returns error result instead of throwing - consider catching specific HttpRequestException/TaskCanceledException
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return new AuthenticationResult
            {
                IsSuccess = false,
                Error = "Token refresh failed"
            };
        }
#pragma warning restore CA1031
    }

    protected override async Task TryRestoreFromStoredToken(string storedToken)
    {
        try
        {
            AccessToken = storedToken;
            TokenExpiration = DateTime.UtcNow.AddHours(1);
            NotifyAuthenticationStateChanged();
        }
#pragma warning disable CA1031 // Generic catch for graceful degradation - token restoration is optional, use re-login on any error
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring token from storage");
            await TokenStorage.RemoveTokenAsync();
        }
#pragma warning restore CA1031
    }

    private void ValidateConfiguration()
    {
        if (string.IsNullOrEmpty(_configuration.Endpoint))
            throw new InvalidOperationException("Logto Endpoint is required in configuration");
        
        if (string.IsNullOrEmpty(_configuration.ClientId))
            throw new InvalidOperationException("Logto ClientId is required in configuration");
        
        if (string.IsNullOrEmpty(_configuration.ClientSecret))
            throw new InvalidOperationException("Logto ClientSecret is required in configuration");
    }

    // PKCE helper methods
    private static string GenerateCodeVerifier()
    {
        var bytes = new byte[32];
        RandomNumberGenerator.Fill(bytes);
        return Base64UrlEncode(bytes);
    }

    private static string GenerateCodeChallenge(string codeVerifier)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(codeVerifier));
        return Base64UrlEncode(hash);
    }

    private static string GenerateState()
    {
        var bytes = new byte[16];
        RandomNumberGenerator.Fill(bytes);
        return Base64UrlEncode(bytes);
    }

    private static string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static byte[] Base64UrlDecode(string input)
    {
        var base64 = input.Replace('-', '+').Replace('_', '/');
        switch (input.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}
