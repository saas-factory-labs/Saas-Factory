using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.Authorization.Providers.Mock;

public class MockProvider : BaseAuthenticationProvider
{
    private readonly ILogger<MockProvider> _logger;

    public MockProvider(
        ITokenStorageService tokenStorage,
        ILogger<MockProvider> logger) : base(tokenStorage)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<AuthenticationResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            // Simulate network delay
            await Task.Delay(500, cancellationToken);

            // Mock validation - accept any non-empty credentials
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return new AuthenticationResult
                {
                    IsSuccess = false,
                    Error = "Email and password are required"
                };
            }

            // Generate mock token
            var mockToken = GenerateMockToken(request.Email);
            var result = new AuthenticationResult
            {
                IsSuccess = true,
                AccessToken = mockToken,
                RefreshToken = $"refresh_{Guid.NewGuid()}",
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };

            await StoreTokens(result);
            
            _logger.LogInformation("Mock login successful for user: {Email}", request.Email);
            return result;
        }
#pragma warning disable CA1031 // Mock provider - returns error result instead of throwing for test scenarios
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during mock login");
            return new AuthenticationResult
            {
                IsSuccess = false,
                Error = "An error occurred during login. Please try again."
            };
        }
#pragma warning restore CA1031
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
            // Simulate network delay
            await Task.Delay(300, cancellationToken);

            // Extract email from current token for mock refresh
            var email = ExtractEmailFromToken(AccessToken);
            string mockToken = GenerateMockToken(email ?? "unknown@example.com");

            var result = new AuthenticationResult
            {
                IsSuccess = true,
                AccessToken = mockToken,
                RefreshToken = RefreshToken, // Keep same refresh token
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };

            await StoreTokens(result);
            
            _logger.LogInformation("Mock token refresh successful");
            return result;
        }
#pragma warning disable CA1031 // Mock provider - returns error result instead of throwing for test scenarios
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during mock token refresh");
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
        ArgumentNullException.ThrowIfNull(storedToken);

        try
        {
            var tokenParts = storedToken.Split('.');
            if (tokenParts.Length >= 2)
            {
                var payload = JsonSerializer.Deserialize<MockTokenPayload>(
                    System.Text.Encoding.UTF8.GetString(
                        Convert.FromBase64String(tokenParts[1].PadBase64())
                    )
                );

                if (payload != null)
                {
                    var expiration = DateTimeOffset.FromUnixTimeSeconds(payload.exp).DateTime;

                    if (DateTime.UtcNow < expiration)
                    {
                        AccessToken = storedToken;
                        TokenExpiration = expiration;
                        NotifyAuthenticationStateChanged();
                        return;
                    }
                }
            }

            // Token is invalid or expired
            await TokenStorage.RemoveTokenAsync();
        }
#pragma warning disable CA1031 // Generic catch for graceful degradation - token restoration is optional in mock provider
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring mock token from storage");
            await TokenStorage.RemoveTokenAsync();
        }
#pragma warning restore CA1031
    }

    private static string GenerateMockToken(string email)
    {
        var header = new { alg = "HS256", typ = "JWT" };
        var payload = new
        {
            sub = Guid.NewGuid().ToString(),
            name = email.Split('@')[0],
            email = email,
            iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            exp = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds()
        };

        var headerJson = JsonSerializer.Serialize(header);
        var payloadJson = JsonSerializer.Serialize(payload);

        var headerBase64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(headerJson))
            .TrimEnd('=').Replace('+', '-').Replace('/', '_');
        var payloadBase64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(payloadJson))
            .TrimEnd('=').Replace('+', '-').Replace('/', '_');

        return $"{headerBase64}.{payloadBase64}.MOCK_SIGNATURE";
    }

    private static string? ExtractEmailFromToken(string? token)
    {
        if (string.IsNullOrEmpty(token)) return null;

        try
        {
            var tokenParts = token.Split('.');
            if (tokenParts.Length >= 2)
            {
                var payload = JsonSerializer.Deserialize<MockTokenPayload>(
                    System.Text.Encoding.UTF8.GetString(
                        Convert.FromBase64String(tokenParts[1].PadBase64())
                    )
                );
                return payload?.email;
            }
        }
        catch
        {
            // Ignore parsing errors
        }

        return null;
    }

    private sealed class MockTokenPayload
    {
        public string sub { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public long iat { get; set; }
        public long exp { get; set; }
    }
}

public static class StringExtensions
{
    public static string PadBase64(this string base64)
    {
        ArgumentNullException.ThrowIfNull(base64);
        
        switch (base64.Length % 4)
        {
            case 2: return base64 + "==";
            case 3: return base64 + "=";
            default: return base64;
        }
    }
}