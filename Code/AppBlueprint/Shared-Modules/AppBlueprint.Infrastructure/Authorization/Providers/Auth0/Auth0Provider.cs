using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.Authorization.Providers.Auth0;

public class Auth0Provider : BaseAuthenticationProvider
{
    private readonly HttpClient _httpClient;
    private readonly Auth0Configuration _configuration;
    private readonly ILogger<Auth0Provider> _logger;

    public Auth0Provider(
        ITokenStorageService tokenStorage,
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<Auth0Provider> logger) : base(tokenStorage)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _configuration = new Auth0Configuration();
        configuration.GetSection("Authentication:Auth0").Bind(_configuration);
        
        ValidateConfiguration();
    }

    public override async Task<AuthenticationResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            var auth0Request = new
            {
                client_id = _configuration.ClientId,
                client_secret = _configuration.ClientSecret,
                audience = _configuration.Audience,
                grant_type = "password",
                username = request.Email,
                password = request.Password,
                scope = "openid profile email"
            };

            var jsonContent = JsonSerializer.Serialize(auth0Request);
            using var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(new Uri($"{_configuration.Domain}/oauth/token", UriKind.Absolute), content, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var tokenResponse = JsonSerializer.Deserialize<Auth0TokenResponse>(responseContent);

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

            _logger.LogWarning("Auth0 login failed with status: {StatusCode}", response.StatusCode);
            return new AuthenticationResult
            {
                IsSuccess = false,
                Error = "Login failed. Please check your credentials."
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error during Auth0 login");
            return new AuthenticationResult
            {
                IsSuccess = false,
                Error = "An error occurred during login. Please check your connection."
            };
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Auth0 login request timed out");
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
                client_id = _configuration.ClientId,
                client_secret = _configuration.ClientSecret,
                grant_type = "refresh_token",
                refresh_token = RefreshToken
            };

            var jsonContent = JsonSerializer.Serialize(refreshRequest);
            using var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(new Uri($"{_configuration.Domain}/oauth/token", UriKind.Absolute), content, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var tokenResponse = JsonSerializer.Deserialize<Auth0TokenResponse>(responseContent);

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
#pragma warning disable CA1031 // Generic catch returns error result instead of throwing - consider catching specific HttpRequestException/TaskCanceledException
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
            // For Auth0, you would typically validate the token and extract expiration
            // This is a simplified version - in production you'd decode the JWT
            AccessToken = storedToken;
            TokenExpiration = DateTime.UtcNow.AddHours(1); // Default expiration
            
            NotifyAuthenticationStateChanged();
        }
#pragma warning disable CA1031 // Generic catch for graceful degradation - token restoration is optional, use re-login on any error
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring Auth0 token from storage");
            await TokenStorage.RemoveTokenAsync();
        }
#pragma warning restore CA1031
    }

    private void ValidateConfiguration()
    {
        if (string.IsNullOrEmpty(_configuration.Domain))
            throw new InvalidOperationException("Auth0 Domain is required in configuration");
        
        if (string.IsNullOrEmpty(_configuration.ClientId))
            throw new InvalidOperationException("Auth0 ClientId is required in configuration");
        
        if (string.IsNullOrEmpty(_configuration.ClientSecret))
            throw new InvalidOperationException("Auth0 ClientSecret is required in configuration");
    }
}