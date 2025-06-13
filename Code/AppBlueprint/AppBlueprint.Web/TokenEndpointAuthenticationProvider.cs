using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;

namespace AppBlueprint.Web;

internal class TokenEndpointAuthenticationProvider(
    HttpClient httpClient,
    string tokenEndpoint,
    string clientId,
    string clientSecret,
    string scope) : IAuthenticationProvider, IDisposable
{
    private readonly string _clientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
    private readonly string _clientSecret = clientSecret ?? throw new ArgumentNullException(nameof(clientSecret));
    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    private readonly string _scope = scope ?? throw new ArgumentNullException(nameof(scope));
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly string _tokenEndpoint = tokenEndpoint ?? throw new ArgumentNullException(nameof(tokenEndpoint));

    private string? _accessToken;
    private DateTime _tokenExpiration = DateTime.MinValue;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task AuthenticateRequestAsync(HttpRequestMessage request,
        Dictionary<string, object>? additionalAuthenticationContext = null,
        CancellationToken cancellationToken = new())
    {
        ArgumentNullException.ThrowIfNull(request);
        await EnsureAccessTokenAsync(cancellationToken).ConfigureAwait(false);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
    }

    public async Task AuthenticateRequestAsync(
        RequestInformation request,
        Dictionary<string, object>? additionalAuthenticationContext = null,
        CancellationToken cancellationToken = new())
    {
        ArgumentNullException.ThrowIfNull(request);

        await EnsureAccessTokenAsync(cancellationToken).ConfigureAwait(false);

        request.Headers.TryAdd("Authorization", $"Bearer {_accessToken}");
    }


    private async Task EnsureAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        if (DateTime.UtcNow < _tokenExpiration) return; // Token is still valid

        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (DateTime.UtcNow < _tokenExpiration) // Double-check inside lock
                return;

            TokenResponse? tokenResponse = await RequestNewAccessTokenAsync(cancellationToken).ConfigureAwait(false);
            _accessToken = tokenResponse.AccessToken;
            _tokenExpiration = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 30); // Buffer before expiry
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task<TokenResponse> RequestNewAccessTokenAsync(CancellationToken cancellationToken)
    {
        using var content = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                { "client_id", _clientId },
                { "client_secret", _clientSecret },
                { "grant_type", "client_credentials" },
                { "scope", _scope }
            });
        HttpResponseMessage response = await _httpClient.PostAsync(new Uri(_tokenEndpoint), content, cancellationToken).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
        TokenResponse? tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: cancellationToken).ConfigureAwait(false);
        return tokenResponse ?? throw new InvalidOperationException("Failed to deserialize token response");
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _semaphore?.Dispose();
            // HttpClient is provided by the host and should not be disposed here
        }
    }


    private sealed class TokenResponse
    {
        public required string AccessToken { get; set; }
        public required int ExpiresIn { get; set; } // Expiry time in seconds
    }
}
