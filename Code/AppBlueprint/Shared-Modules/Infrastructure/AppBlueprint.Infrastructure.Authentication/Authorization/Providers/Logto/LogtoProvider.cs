using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.Authentication.Authorization.Providers.Logto;

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
            _logger.LogInformation("Attempting Logto login");

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
                _logger.LogInformation("Logto login successful");

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

            string errorMessage = "Login failed. Please check your credentials.";

            if (TryParseErrorDetails(responseContent, out OAuthErrorDetails errorDetails))
            {
                _logger.LogError(
                    "Logto login failed with status: {StatusCode}. Error: {Error}. ErrorDescription: {ErrorDescription}",
                    response.StatusCode,
                    errorDetails.Error,
                    errorDetails.ErrorDescription);

                errorMessage = BuildUserErrorMessage(errorDetails, errorMessage);
            }
            else
            {
                _logger.LogError(
                    "Logto login failed with status: {StatusCode}. Unable to parse error response body.",
                    response.StatusCode);
            }

            return new AuthenticationResult
            {
                IsSuccess = false,
                Error = errorMessage
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error during Logto login");
            return new AuthenticationResult
            {
                IsSuccess = false,
                Error = "Network error occurred during login. Please check your connection and try again."
            };
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(ex, "Logto login request timed out");
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
        catch (InvalidOperationException ex)
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

    private static string BuildUserErrorMessage(OAuthErrorDetails errorDetails, string defaultMessage)
    {
        if (!string.IsNullOrEmpty(errorDetails.ErrorDescription))
        {
            return errorDetails.ErrorDescription;
        }

        return errorDetails.Error switch
        {
            "invalid_grant" => "Invalid email or password. Please check your credentials.",
            "unsupported_grant_type" => "Password login is not enabled. Please enable ROPC grant type in Logto Console.",
            "invalid_client" => "Application configuration error. Please check your Logto credentials.",
            { Length: > 0 } errorType => $"Login failed: {errorType}",
            _ => defaultMessage
        };
    }

    private static bool TryParseErrorDetails(string responseContent, out OAuthErrorDetails errorDetails)
    {
        errorDetails = new OAuthErrorDetails(null, null);

        if (string.IsNullOrWhiteSpace(responseContent))
        {
            return false;
        }

#pragma warning disable CA1031 // Generic catch for error parsing - use default message if JSON parsing fails
        try
        {
            using var document = JsonDocument.Parse(responseContent);
            JsonElement root = document.RootElement;

            if (root.ValueKind != JsonValueKind.Object)
            {
                return false;
            }

            string? error = root.TryGetProperty("error", out JsonElement errorElement) && errorElement.ValueKind == JsonValueKind.String
                ? errorElement.GetString()
                : null;

            string? errorDescription = root.TryGetProperty("error_description", out JsonElement errorDescriptionElement) && errorDescriptionElement.ValueKind == JsonValueKind.String
                ? errorDescriptionElement.GetString()
                : null;

            if (string.IsNullOrEmpty(error) && string.IsNullOrEmpty(errorDescription))
            {
                return false;
            }

            errorDetails = new OAuthErrorDetails(error, errorDescription);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
#pragma warning restore CA1031
    }

    private sealed record OAuthErrorDetails(string? Error, string? ErrorDescription);
}
