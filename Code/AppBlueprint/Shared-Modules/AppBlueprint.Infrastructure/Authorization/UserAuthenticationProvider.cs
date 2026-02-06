using System.Diagnostics;
using System.Text.Json;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;

namespace AppBlueprint.Infrastructure.Authorization;

public sealed class UserAuthenticationProvider : IUserAuthenticationProvider, IDisposable
{
    private readonly ITokenStorageService _tokenStorage;
    private readonly HttpClient? _httpClient;
    private readonly string? _authEndpoint;
    private string? _accessToken;
    private DateTime _accessTokenExpiration = DateTime.MinValue;

    public UserAuthenticationProvider(ITokenStorageService tokenStorage)
    {
        _tokenStorage = tokenStorage ?? throw new ArgumentNullException(nameof(tokenStorage));
        _httpClient = null;
        _authEndpoint = null;
    }

    public UserAuthenticationProvider(HttpClient httpClient, string authEndpoint, ITokenStorageService tokenStorage)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _authEndpoint = authEndpoint ?? throw new ArgumentNullException(nameof(authEndpoint));
        _tokenStorage = tokenStorage ?? throw new ArgumentNullException(nameof(tokenStorage));
    }

    public bool IsAuthenticated()
    {
        return !string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _accessTokenExpiration;
    }

    public async Task<bool> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            // In a real implementation, this would call your authentication API
            // For demonstration purposes, we're simulating a successful login with a mock token
            var mockToken = GenerateMockToken(email);
            await _tokenStorage.StoreTokenAsync(mockToken);

            // Parse the mock token to extract expiration
            var tokenParts = mockToken.Split('.');
            if (tokenParts.Length >= 2)
            {
                var payload = JsonSerializer.Deserialize<TokenPayload>(
                    System.Text.Encoding.UTF8.GetString(
                        Convert.FromBase64String(tokenParts[1].PadBase64())
                    )
                );

                if (payload != null)
                {
                    _accessToken = mockToken;
                    _accessTokenExpiration = DateTimeOffset.FromUnixTimeSeconds(payload.exp).DateTime;
                    return true;
                }
            }

            return false;
        }
        catch (JsonException ex)
        {
            Debug.WriteLine($"Login failed due to JSON parsing error: {ex.Message}");
            return false;
        }
        catch (FormatException ex)
        {
            Debug.WriteLine($"Login failed due to format error: {ex.Message}");
            return false;
        }
        catch (ArgumentException ex)
        {
            Debug.WriteLine($"Login failed due to invalid argument: {ex.Message}");
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        _accessToken = null;
        _accessTokenExpiration = DateTime.MinValue;
        await _tokenStorage.RemoveTokenAsync();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1055:URI return values should not be strings", Justification = "OAuth redirect URIs are returned as strings for authentication protocols")]
    public string? GetLogoutUrl(string postLogoutRedirectUri)
    {
        // This provider doesn't use OIDC logout URLs
        // Return null to indicate that local logout is sufficient
        return null;
    }

    public async Task InitializeFromStorageAsync()
    {
        try
        {
            var storedToken = await _tokenStorage.GetTokenAsync();
            if (!string.IsNullOrEmpty(storedToken))
            {
                var tokenParts = storedToken.Split('.');
                if (tokenParts.Length >= 2)
                {
                    var payload = JsonSerializer.Deserialize<TokenPayload>(
                        System.Text.Encoding.UTF8.GetString(
                            Convert.FromBase64String(tokenParts[1].PadBase64())
                        )
                    );

                    if (payload != null)
                    {
                        var expiration = DateTimeOffset.FromUnixTimeSeconds(payload.exp).DateTime;

                        // Only set the token if it's still valid
                        if (DateTime.UtcNow < expiration)
                        {
                            _accessToken = storedToken;
                            _accessTokenExpiration = expiration;
                        }
                        else
                        {
                            // Token is expired, remove it
                            await _tokenStorage.RemoveTokenAsync();
                        }
                    }
                }
            }
        }
        catch (JsonException ex)
        {
            Debug.WriteLine($"Failed to initialize from storage due to JSON parsing error: {ex.Message}");
            await _tokenStorage.RemoveTokenAsync();
        }
        catch (FormatException ex)
        {
            Debug.WriteLine($"Failed to initialize from storage due to format error: {ex.Message}");
            await _tokenStorage.RemoveTokenAsync();
        }
        catch (ArgumentException ex)
        {
            Debug.WriteLine($"Failed to initialize from storage due to invalid argument: {ex.Message}");
            await _tokenStorage.RemoveTokenAsync();
        }
    }

    public Task AuthenticateRequestAsync(
        RequestInformation request,
        Dictionary<string, object>? additionalAuthenticationContext = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (IsAuthenticated())
        {
            request.Headers.Add("Authorization", $"Bearer {_accessToken}");
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Cleanup managed resources if needed
            _httpClient?.Dispose();
        }
    }

    // Helper method to generate a mock token for demonstration
    private static string GenerateMockToken(string email)
    {
        // In a real implementation, this would be a JWT token from your authentication service
        // For demonstration, we'll use a simple mock token structure
        var header = new { alg = "HS256", typ = "JWT" };
        var payload = new
        {
            sub = "123456",
            name = email,
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

        // In a real implementation, you would sign this with a secret key
        var signatureBase64 = "MOCK_SIGNATURE";

        return $"{headerBase64}.{payloadBase64}.{signatureBase64}";
    }

    // Simple class to deserialize token payload
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Class is instantiated by JSON deserialization")]
    private sealed class TokenPayload
    {
        public string sub { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public long iat { get; set; }
        public long exp { get; set; }
    }
}

// Extension methods for Base64 padding
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
