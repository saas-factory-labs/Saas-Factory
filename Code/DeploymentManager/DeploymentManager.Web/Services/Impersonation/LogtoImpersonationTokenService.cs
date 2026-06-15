using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace DeploymentManager.Web.Services.Impersonation;

/// <summary>
/// Logto-native impersonation via RFC 8693 token exchange. Three steps, all against Logto:
/// <list type="number">
///   <item>client_credentials grant for the Management API (M2M app);</item>
///   <item>POST /api/subject-tokens to create a one-time subject token for the target user, with the
///         impersonating admin recorded in its context;</item>
///   <item>token-exchange at /oidc/token to swap the subject token for a Logto-signed access token
///         scoped (read-only) to the target app.</item>
/// </list>
/// The impersonating admin is carried in the subject-token <c>context</c>; to expose it as the standard
/// OIDC <c>act</c> claim, supply the admin's access token as an <c>actor_token</c> in step 3 (not done
/// here to avoid coupling the mint to the admin's token audience) or map the context via a Logto
/// custom-JWT-claims script. The authoritative impersonator record is always the dm_admin_audit log.
/// </summary>
public sealed class LogtoImpersonationTokenService : ILogtoImpersonationTokenService
{
    private const string TokenExchangeGrantType = "urn:ietf:params:oauth:grant-type:token-exchange";
    private const string SubjectTokenType = "urn:ietf:params:oauth:token-type:access_token";

    private readonly HttpClient _httpClient;
    private readonly ImpersonationOptions _options;
    private readonly IConfiguration _configuration;
    private readonly ILogger<LogtoImpersonationTokenService> _logger;

    public LogtoImpersonationTokenService(
        HttpClient httpClient,
        IOptions<ImpersonationOptions> options,
        IConfiguration configuration,
        ILogger<LogtoImpersonationTokenService> logger)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(logger);

        // Boot-safe: no config validation in the constructor. The service is registered even when
        // impersonation is not configured (and the host runs ValidateOnBuild=true, which instantiates
        // every registration). Config is validated lazily in IssueReadOnlyTokenAsync - fail closed at
        // point of use, never at startup.
        _httpClient = httpClient;
        _options = options.Value;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ImpersonationTokenResult> IssueReadOnlyTokenAsync(
        string targetUserId,
        string impersonatorAdminId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(targetUserId);
        ArgumentException.ThrowIfNullOrWhiteSpace(impersonatorAdminId);
        ArgumentException.ThrowIfNullOrWhiteSpace(_options.ManagementClientId, "Impersonation:ManagementClientId");
        ArgumentException.ThrowIfNullOrWhiteSpace(_options.ManagementClientSecret, "Impersonation:ManagementClientSecret");

        string? endpoint = _configuration["Authentication:Logto:Endpoint"];
        ArgumentException.ThrowIfNullOrWhiteSpace(endpoint, "Authentication:Logto:Endpoint");
        endpoint = endpoint.TrimEnd('/');

        string? appClientId = _configuration["Authentication:Logto:ClientId"];
        ArgumentException.ThrowIfNullOrWhiteSpace(appClientId, "Authentication:Logto:ClientId");

        string managementToken = await GetManagementTokenAsync(endpoint, cancellationToken);
        string subjectToken = await CreateSubjectTokenAsync(endpoint, managementToken, targetUserId, impersonatorAdminId, cancellationToken);
        return await ExchangeSubjectTokenAsync(endpoint, appClientId, subjectToken, cancellationToken);
    }

    // Step 1: M2M access token for the Logto Management API.
    private async Task<string> GetManagementTokenAsync(string endpoint, CancellationToken cancellationToken)
    {
        string managementResource = string.IsNullOrWhiteSpace(_options.ManagementApiResource)
            ? $"{endpoint}/api"
            : _options.ManagementApiResource;

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{endpoint}/oidc/token")
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = _options.ManagementClientId!,
                ["client_secret"] = _options.ManagementClientSecret!,
                ["resource"] = managementResource,
                ["scope"] = "all"
            })
        };

        TokenResponse token = await SendForTokenAsync(request, "client_credentials", cancellationToken);
        return token.AccessToken;
    }

    // Step 2: one-time subject token for the target user, tagging the impersonating admin.
    private async Task<string> CreateSubjectTokenAsync(
        string endpoint,
        string managementToken,
        string targetUserId,
        string impersonatorAdminId,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"{endpoint}/api/subject-tokens");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", managementToken);
        request.Content = JsonContent.Create(new
        {
            userId = targetUserId,
            // Records who is impersonating. Logto preserves this context on the grant; it surfaces as
            // a custom claim via a getCustomJwtClaims() script (or as the standard `act` claim if an
            // actor_token is supplied to the exchange). The dm_admin_audit log is the authoritative record.
            context = new { actorId = impersonatorAdminId, reason = "control-plane-impersonation" }
        });

        using HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Logto subject-token creation failed with status {Status}.", (int)response.StatusCode);
            throw new InvalidOperationException(
                $"Logto subject-token creation failed ({(int)response.StatusCode}).");
        }

        SubjectTokenResponse? body = await response.Content
            .ReadFromJsonAsync<SubjectTokenResponse>(cancellationToken);
        if (body is null || string.IsNullOrWhiteSpace(body.SubjectToken))
        {
            throw new InvalidOperationException("Logto subject-token response did not contain a subjectToken.");
        }

        return body.SubjectToken;
    }

    // Step 3: exchange the subject token for a read-only access token for the target app.
    private async Task<ImpersonationTokenResult> ExchangeSubjectTokenAsync(
        string endpoint,
        string appClientId,
        string subjectToken,
        CancellationToken cancellationToken)
    {
        var form = new Dictionary<string, string>
        {
            ["grant_type"] = TokenExchangeGrantType,
            ["client_id"] = appClientId,
            ["subject_token"] = subjectToken,
            ["subject_token_type"] = SubjectTokenType,
            ["scope"] = _options.ReadOnlyScope
        };

        if (!string.IsNullOrWhiteSpace(_options.TargetApiResource))
        {
            form["resource"] = _options.TargetApiResource;
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{endpoint}/oidc/token")
        {
            Content = new FormUrlEncodedContent(form)
        };

        TokenResponse token = await SendForTokenAsync(request, "token-exchange", cancellationToken);

        // Prefer Logto's expires_in; cap to the configured session lifetime so a long-lived grant
        // can never outlive the policy window.
        int lifetimeSeconds = Math.Min(
            token.ExpiresIn > 0 ? token.ExpiresIn : int.MaxValue,
            _options.EffectiveLifetimeMinutes * 60);

        return new ImpersonationTokenResult(
            token.AccessToken,
            string.IsNullOrWhiteSpace(token.TokenType) ? "Bearer" : token.TokenType,
            DateTime.UtcNow.AddSeconds(lifetimeSeconds));
    }

    private async Task<TokenResponse> SendForTokenAsync(
        HttpRequestMessage request,
        string stepName,
        CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            // OWASP A09: never log token material; status + step are enough to diagnose.
            _logger.LogError("Logto {Step} request failed with status {Status}.", stepName, (int)response.StatusCode);
            throw new InvalidOperationException($"Logto {stepName} request failed ({(int)response.StatusCode}).");
        }

        TokenResponse? token = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken);
        if (token is null || string.IsNullOrWhiteSpace(token.AccessToken))
        {
            throw new InvalidOperationException($"Logto {stepName} response did not contain an access_token.");
        }

        return token;
    }

    private sealed record TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; init; } = string.Empty;

        [JsonPropertyName("token_type")]
        public string TokenType { get; init; } = string.Empty;

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; init; }
    }

    private sealed record SubjectTokenResponse
    {
        [JsonPropertyName("subjectToken")]
        public string SubjectToken { get; init; } = string.Empty;
    }
}
