using AppBlueprint.Application.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;



namespace AppBlueprint.Infrastructure.Authentication;

/// <summary>
/// Simple header‑based API‑key authorization filter.
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
        
        // 1.  Check for missing header
        if (!context.HttpContext.Request.Headers.TryGetValue(ApiHeaderNames.ApiKeyHeaderName,
                out StringValues extractedApiKey))
        {
            // context.Result = new UnauthorizedObjectResult(new { error = "API key is missing" });
            return;
        }

        // 2.  Pull expected value from configuration
        var expectedApiKey = _configuration[ApiKeyConfigPath];

        if (string.IsNullOrWhiteSpace(expectedApiKey))
        {
            // context.Result = new ObjectResult(new { error = "API key is not configured" })
            // {
            //     StatusCode = StatusCodes.Status500InternalServerError
            // };
            return;
        }

        // 3.  Compare (ordinal, case‑sensitive by default; change if needed)
        if (!expectedApiKey.Equals(extractedApiKey.ToString(), StringComparison.Ordinal))
        {




            // context.Result = new UnauthorizedObjectResult(new { error = "Invalid API key" });
        }
        // If the keys match, do nothing; MVC will continue down the pipeline.
    }

}