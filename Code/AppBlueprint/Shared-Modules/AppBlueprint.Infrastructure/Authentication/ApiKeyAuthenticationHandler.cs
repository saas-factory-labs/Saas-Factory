using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using AppBlueprint.Application.Constants;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AppBlueprint.Infrastructure.Authentication;

/// <summary>
/// Options for the API key authentication scheme. No configuration needed beyond registering the scheme.
/// </summary>
public sealed class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions { }

/// <summary>
/// ASP.NET Core authentication handler that validates inbound <c>x-api-key</c> header values
/// against hashed keys stored in the database.
/// </summary>
public sealed class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    public const string SchemeName = "ApiKey";

    private readonly IApiKeyRepository _apiKeyRepository;

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IApiKeyRepository apiKeyRepository)
        : base(options, logger, encoder)
    {
        _apiKeyRepository = apiKeyRepository;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(ApiHeaderNames.ApiKeyHeaderName, out var headerValues)
            || string.IsNullOrWhiteSpace(headerValues))
        {
            return AuthenticateResult.NoResult();
        }

        string rawKey = headerValues.ToString().Trim();
        string keyHash = ComputeSha256Hex(rawKey);

        ApiKeyEntity? apiKey = await _apiKeyRepository.GetByKeyHashAsync(keyHash);

        if (apiKey is null)
        {
            Logger.LogWarning("API key authentication failed: key not found or revoked.");
            return AuthenticateResult.Fail("Invalid API key.");
        }

        if (apiKey.ExpiresAt.HasValue && apiKey.ExpiresAt.Value < DateTimeOffset.UtcNow)
        {
            Logger.LogWarning("API key authentication failed: key '{KeyId}' has expired.", apiKey.Id);
            return AuthenticateResult.Fail("API key has expired.");
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, apiKey.UserId),
            new("sub", apiKey.UserId),
            new("tenant_id", apiKey.TenantId),
            new("api_key_id", apiKey.Id),
            new("api_key_name", apiKey.Name)
        };

        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        Logger.LogInformation(
            "API key authentication succeeded for key '{KeyId}' (tenant: {TenantId}).",
            apiKey.Id,
            apiKey.TenantId);

        return AuthenticateResult.Success(ticket);
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = 401;
        Response.Headers["WWW-Authenticate"] = "ApiKey realm=\"AppBlueprint\"";
        return Task.CompletedTask;
    }

    /// <summary>Computes a lowercase hex SHA-256 hash of <paramref name="input"/>.</summary>
    private static string ComputeSha256Hex(string input)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
