using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.Authorization.Providers.Logto;

public class LogtoProvider : BaseAuthenticationProvider
{
    private readonly HttpClient _httpClient;
    private readonly LogtoConfiguration _configuration;
    private readonly ILogger<LogtoProvider> _logger;

    public LogtoProvider(
        ITokenStorageService tokenStorage,
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<LogtoProvider> logger) : base(tokenStorage)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _configuration = new LogtoConfiguration();
        configuration.GetSection("Authentication:Logto").Bind(_configuration);
        
        ValidateConfiguration();
    }

    public override async Task<AuthenticationResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            // Logto uses OAuth2 Resource Owner Password Credentials Grant
            var logtoRequest = new
            {
                client_id = _configuration.ClientId,
                client_secret = _configuration.ClientSecret,
                grant_type = "password",
                username = request.Email,
                password = request.Password,
                scope = _configuration.Scope ?? "openid profile email"
            };

            var formContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = _configuration.ClientId,
                ["client_secret"] = _configuration.ClientSecret,
                ["grant_type"] = "password",
                ["username"] = request.Email,
                ["password"] = request.Password,
                ["scope"] = _configuration.Scope ?? "openid profile email"
            });

            var response = await _httpClient.PostAsync($"{_configuration.Endpoint}/oidc/token", formContent, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var tokenResponse = JsonSerializer.Deserialize<LogtoTokenResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                });

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

            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("Logto login failed with status: {StatusCode}, Content: {Content}", 
                response.StatusCode, errorContent);
            
            return new AuthenticationResult
            {
                IsSuccess = false,
                Error = "Login failed. Please check your credentials."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Logto login");
            return new AuthenticationResult
            {
                IsSuccess = false,
                Error = "An error occurred during login. Please try again."
            };
        }
    }

    public override async Task<AuthenticationResult> RefreshTokenAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_refreshToken))
        {
            return new AuthenticationResult
            {
                IsSuccess = false,
                Error = "No refresh token available"
            };
        }

        try
        {
            var formContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = _configuration.ClientId,
                ["client_secret"] = _configuration.ClientSecret,
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = _refreshToken
            });

            var response = await _httpClient.PostAsync($"{_configuration.Endpoint}/oidc/token", formContent, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var tokenResponse = JsonSerializer.Deserialize<LogtoTokenResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                });

                if (tokenResponse?.AccessToken != null)
                {
                    var result = new AuthenticationResult
                    {
                        IsSuccess = true,
                        AccessToken = tokenResponse.AccessToken,
                        RefreshToken = tokenResponse.RefreshToken ?? _refreshToken,
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Logto token refresh");
            return new AuthenticationResult
            {
                IsSuccess = false,
                Error = "Token refresh failed"
            };
        }
    }

    protected override async Task TryRestoreFromStoredToken(string storedToken)
    {
        try
        {
            // For Logto, you would typically validate the token and extract expiration
            // This is a simplified version - in production you'd decode the JWT
            _accessToken = storedToken;
            _tokenExpiration = DateTime.UtcNow.AddHours(1); // Default expiration
            
            NotifyAuthenticationStateChanged();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring Logto token from storage");
            await _tokenStorage.RemoveTokenAsync();
        }
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