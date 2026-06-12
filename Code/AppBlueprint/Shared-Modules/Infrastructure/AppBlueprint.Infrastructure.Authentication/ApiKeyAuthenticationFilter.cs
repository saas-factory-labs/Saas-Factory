using AppBlueprint.Application.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace AppBlueprint.Infrastructure.Authentication;

/// <summary>
/// Simple header-based API-key authorization filter.
/// NOTE: The primary API-key path is <see cref="ApiKeyAuthenticationHandler"/>. This filter is
/// retained for callers that wire it up explicitly and MUST fail closed: any missing, unconfigured,
/// or mismatched key results in a 401 so it can never silently allow a request through.
/// </summary>
public sealed class ApiKeyAuthenticationFilter : IAuthorizationFilter
{
    private const string ApiKeyConfigPath = "Authentication:ApiKey";   // <-- adjust if your appsettings path differs
    private readonly IConfiguration _configuration;

    public ApiKeyAuthenticationFilter(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        // 1. Reject when the API key header is missing.
        if (!context.HttpContext.Request.Headers.TryGetValue(ApiHeaderNames.ApiKeyHeaderName,
                out StringValues extractedApiKey))
        {
            context.Result = new UnauthorizedObjectResult(new { error = "API key is missing" });
            return;
        }

        // 2. Reject when no key is configured server-side (fail closed, do not allow).
        string? expectedApiKey = _configuration[ApiKeyConfigPath];

        if (string.IsNullOrWhiteSpace(expectedApiKey))
        {
            context.Result = new UnauthorizedObjectResult(new { error = "API key is not configured" });
            return;
        }

        // 3. Reject on mismatch using a fixed-time comparison to avoid timing oracles.
        bool matches = CryptographicEquals(expectedApiKey, extractedApiKey.ToString());
        if (!matches)
        {
            context.Result = new UnauthorizedObjectResult(new { error = "Invalid API key" });
        }
        // If the keys match, do nothing; MVC will continue down the pipeline.
    }

    private static bool CryptographicEquals(string expected, string actual)
    {
        byte[] expectedBytes = System.Text.Encoding.UTF8.GetBytes(expected);
        byte[] actualBytes = System.Text.Encoding.UTF8.GetBytes(actual);
        return System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(expectedBytes, actualBytes);
    }
}
