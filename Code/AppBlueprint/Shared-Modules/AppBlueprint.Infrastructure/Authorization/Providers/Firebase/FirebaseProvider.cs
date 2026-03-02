using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.Authorization.Providers.Firebase;

public class FirebaseProvider : BaseAuthenticationProvider
{
    private readonly HttpClient _httpClient;
    private readonly FirebaseConfiguration _configuration;
    private readonly ILogger<FirebaseProvider> _logger;

    private const string SignInUrl = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword";
    private const string RefreshUrl = "https://securetoken.googleapis.com/v1/token";

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public FirebaseProvider(
        ITokenStorageService tokenStorage,
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<FirebaseProvider> logger) : base(tokenStorage)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _configuration = new FirebaseConfiguration();
        configuration.GetSection("Authentication:Firebase").Bind(_configuration);

        ValidateConfiguration();
    }

    public override async Task<AuthenticationResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            var firebaseRequest = new
            {
                email = request.Email,
                password = request.Password,
                returnSecureToken = true
            };

            var jsonContent = JsonSerializer.Serialize(firebaseRequest);
            using var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var requestUrl = new Uri($"{SignInUrl}?key={_configuration.ApiKey}", UriKind.Absolute);
            var response = await _httpClient.PostAsync(requestUrl, content, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var options = JsonOptions;
                var tokenResponse = JsonSerializer.Deserialize<FirebaseSignInResponse>(responseContent, options);

                if (tokenResponse is not null && !string.IsNullOrEmpty(tokenResponse.IdToken))
                {
                    var expiresInSeconds = int.TryParse(tokenResponse.ExpiresIn, out var seconds) ? seconds : 3600;

                    var result = new AuthenticationResult
                    {
                        IsSuccess = true,
                        AccessToken = tokenResponse.IdToken,
                        RefreshToken = tokenResponse.RefreshToken,
                        ExpiresAt = DateTime.UtcNow.AddSeconds(expiresInSeconds)
                    };

                    await StoreTokens(result);
                    return result;
                }
            }

            // Try to parse Firebase error for better error messages
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var errorMessage = TryParseFirebaseError(errorContent);

            _logger.LogWarning("Firebase login failed with status: {StatusCode}", response.StatusCode);
            return new AuthenticationResult
            {
                IsSuccess = false,
                Error = errorMessage ?? "Login failed. Please check your credentials."
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error during Firebase login");
            return new AuthenticationResult
            {
                IsSuccess = false,
                Error = "An error occurred during login. Please check your connection."
            };
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Firebase login request timed out");
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
            var refreshRequest = new
            {
                grant_type = "refresh_token",
                refresh_token = RefreshToken
            };

            var jsonContent = JsonSerializer.Serialize(refreshRequest);
            using var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var requestUrl = new Uri($"{RefreshUrl}?key={_configuration.ApiKey}", UriKind.Absolute);
            var response = await _httpClient.PostAsync(requestUrl, content, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var options = JsonOptions;
                var tokenResponse = JsonSerializer.Deserialize<FirebaseRefreshResponse>(responseContent, options);

                if (tokenResponse is not null && !string.IsNullOrEmpty(tokenResponse.Id_token))
                {
                    var expiresInSeconds = int.TryParse(tokenResponse.Expires_in, out var seconds) ? seconds : 3600;

                    var result = new AuthenticationResult
                    {
                        IsSuccess = true,
                        AccessToken = tokenResponse.Id_token,
                        RefreshToken = tokenResponse.Refresh_token ?? RefreshToken,
                        ExpiresAt = DateTime.UtcNow.AddSeconds(expiresInSeconds)
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
#pragma warning disable CA1031 // Generic catch returns error result instead of throwing
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Firebase token refresh");
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
            TokenExpiration = ExtractExpirationFromJwt(storedToken) ?? DateTime.UtcNow.AddHours(1);

            NotifyAuthenticationStateChanged();
        }
#pragma warning disable CA1031 // Generic catch for graceful degradation
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring Firebase token from storage");
            await TokenStorage.RemoveTokenAsync();
        }
#pragma warning restore CA1031
    }

    private static DateTime? ExtractExpirationFromJwt(string token)
    {
        try
        {
            var parts = token.Split('.');
            if (parts.Length != 3) return null;

            var payload = parts[1];
            switch (payload.Length % 4)
            {
                case 2: payload += "=="; break;
                case 3: payload += "="; break;
            }

            var bytes = Convert.FromBase64String(payload.Replace('-', '+').Replace('_', '/'));
            var json = System.Text.Encoding.UTF8.GetString(bytes);

            using var doc = System.Text.Json.JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("exp", out var expElement))
            {
                var expUnix = expElement.GetInt64();
                return DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
            }
        }
#pragma warning disable CA1031
        catch
        {
            // Could not decode JWT, caller will use fallback
        }
#pragma warning restore CA1031

        return null;
    }

    private void ValidateConfiguration()
    {
        if (string.IsNullOrEmpty(_configuration.ApiKey))
            throw new InvalidOperationException("Firebase ApiKey is required in configuration");
    }

    private static string? TryParseFirebaseError(string responseContent)
    {
        try
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var errorResponse = JsonSerializer.Deserialize<FirebaseErrorResponse>(responseContent, options);

            if (errorResponse?.Error is not null)
            {
                return errorResponse.Error.Message switch
                {
                    "EMAIL_NOT_FOUND" => "No account found with this email address.",
                    "INVALID_PASSWORD" => "Incorrect password. Please try again.",
                    "USER_DISABLED" => "This account has been disabled.",
                    "TOO_MANY_ATTEMPTS_TRY_LATER" => "Too many failed attempts. Please try again later.",
                    "INVALID_LOGIN_CREDENTIALS" => "Invalid email or password.",
                    _ => $"Authentication failed: {errorResponse.Error.Message}"
                };
            }
        }
#pragma warning disable CA1031
        catch
        {
            // Could not parse error response, return null to use default message
        }
#pragma warning restore CA1031

        return null;
    }
}
