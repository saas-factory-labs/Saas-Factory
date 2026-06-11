using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.Authorization.Providers.Logto;

public class LogtoProvider : BaseAuthenticationProvider
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    private readonly HttpClient _httpClient;
    private readonly LogtoConfiguration _configuration;
    private readonly ILogger<LogtoProvider> _logger;

    public LogtoProvider(
        ITokenStorageService tokenStorage,
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<LogtoProvider> logger) : base(tokenStorage)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _configuration = new LogtoConfiguration();
        configuration.GetSection("Authentication:Logto").Bind(_configuration);

        ValidateConfiguration();
    }

    public override async Task<AuthenticationResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            _logger.LogInformation("Attempting Logto login for user: {Email}", request.Email);

            // Logto uses OAuth2 Resource Owner Password Credentials Grant
            using var formContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = _configuration.ClientId,
                ["client_secret"] = _configuration.ClientSecret,
                ["grant_type"] = "password",
                ["username"] = request.Email,
                ["password"] = request.Password,
                ["scope"] = _configuration.Scope ?? "openid profile email"
            });

            _logger.LogDebug("Posting to Logto token endpoint: {Endpoint}/oidc/token", _configuration.Endpoint);

            var response = await _httpClient.PostAsync(new Uri($"{_configuration.Endpoint}/oidc/token", UriKind.Absolute), formContent, cancellationToken);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Logto login successful for user: {Email}", request.Email);

                var tokenResponse = JsonSerializer.Deserialize<LogtoTokenResponse>(responseContent, JsonOptions);

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
                    return result;
                }
            }

            // Log the full error response for debugging
            _logger.LogError("Logto login failed with status: {StatusCode}, Response: {Response}",
                response.StatusCode, responseContent);

            // Try to parse error for better user feedback
            string errorMessage = "Login failed. Please check your credentials.";
            try
            {
                var errorResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
                if (errorResponse.TryGetProperty("error_description", out var errorDesc))
                {
                    errorMessage = errorDesc.GetString() ?? errorMessage;
                }
                else if (errorResponse.TryGetProperty("error", out var error))
                {
                    var errorType = error.GetString();
                    errorMessage = errorType switch
                    {
                        "invalid_grant" => "Invalid email or password. Please check your credentials.",
                        "unsupported_grant_type" => "Password login is not enabled. Please enable ROPC grant type in Logto Console.",
                        "invalid_client" => "Application configuration error. Please check your Logto credentials.",
                        _ => $"Login failed: {errorType}"
                    };
                }
            }
#pragma warning disable CA1031 // Generic catch for error parsing - use default message if JSON parsing fails
            catch
            {
                // If we can't parse the error, use the default message
            }
#pragma warning restore CA1031

            return new AuthenticationResult
            {
                IsSuccess = false,
                Error = errorMessage
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error during Logto login for user: {Email}", request.Email);
            return new AuthenticationResult
            {
                IsSuccess = false,
                Error = "Network error occurred during login. Please check your connection and try again."
            };
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(ex, "Logto login request timed out for user: {Email}", request.Email);
            return new AuthenticationResult
            {
                IsSuccess = false,
                Error = "Login request timed out. Please try again."
            };
        }
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

            var response = await _httpClient.PostAsync(new Uri($"{_configuration.Endpoint}/oidc/token", UriKind.Absolute), formContent, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var tokenResponse = JsonSerializer.Deserialize<LogtoTokenResponse>(responseContent, JsonOptions);

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
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error during Logto token refresh");
            return new AuthenticationResult
            {
                IsSuccess = false,
                Error = "Network error during token refresh. Please check your connection."
            };
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(ex, "Logto token refresh request timed out");
            return new AuthenticationResult
            {
                IsSuccess = false,
                Error = "Token refresh timed out. Please try again."
            };
        }
    }

    protected override async Task TryRestoreFromStoredToken(string storedToken)
    {
        try
        {
            // For Logto, you would typically validate the token and extract expiration
            // This is a simplified version - in production you'd decode the JWT
            AccessToken = storedToken;
            TokenExpiration = DateTime.UtcNow.AddHours(1); // Default expiration

            NotifyAuthenticationStateChanged();
        }
#pragma warning disable CA1031 // Generic catch for graceful degradation - token restoration is optional, use re-login on any error
        catch (Exception ex)
        {
            // Keep generic catch here for graceful degradation - token restoration is optional
            _logger.LogWarning(ex, "Failed to restore Logto token from storage, will require re-login");
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
}
