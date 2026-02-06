using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AppBlueprint.Presentation.ApiModule.Attributes;

/// <summary>
/// Authorization filter that validates the presence of required OAuth 2.0 scopes in the access token.
/// Used to enforce fine-grained permissions for API endpoints.
/// </summary>
/// <remarks>
/// This attribute checks the "scope" claim in the JWT access token.
/// Example usage: [RequireScope("read:files", "write:files")]
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class RequireScopeAttribute : Attribute, IAuthorizationFilter
{
    private readonly string[] _requiredScopes;

    /// <summary>
    /// Initializes a new instance of the RequireScopeAttribute with one or more required scopes.
    /// </summary>
    /// <param name="scopes">One or more OAuth 2.0 scopes that must be present in the access token.</param>
    public RequireScopeAttribute(params string[] scopes)
    {
        ArgumentNullException.ThrowIfNull(scopes);

        if (scopes.Length == 0)
        {
            throw new ArgumentException("At least one scope must be specified.", nameof(scopes));
        }

        _requiredScopes = scopes;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        System.Security.Claims.ClaimsPrincipal? user = context.HttpContext.User;

        // Check if user is authenticated
        if (user?.Identity?.IsAuthenticated != true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Get scope claim(s) from token
        // Note: Logto/Auth0 typically use "scope" claim with space-separated values
        // or multiple "scope" claims
        IEnumerable<string>? scopeClaims = user.FindAll("scope").Select(c => c.Value);
        var grantedScopes = new HashSet<string>(StringComparer.Ordinal);

        foreach (string scopeClaim in scopeClaims)
        {
            // Split space-separated scopes
            string[] scopes = scopeClaim.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (string scope in scopes)
            {
                grantedScopes.Add(scope);
            }
        }

        // Check if all required scopes are present
        foreach (string requiredScope in _requiredScopes)
        {
            if (!grantedScopes.Contains(requiredScope))
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}
