using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AppBlueprint.Tests.Infrastructure;

internal sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder) : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string authHeader = Request.Headers["Authorization"].ToString();

        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return Task.FromResult(AuthenticateResult.Fail("No bearer token"));

        string token = authHeader["Bearer ".Length..].Trim();
        var claims = new List<Claim>();

        // Add claims based on token
        switch (token)
        {
            case var _ when token == AuthorizationTests.AdminToken:
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                claims.Add(new Claim("permissions", "full_access"));
                break;

            case var _ when token == AuthorizationTests.UserToken:
                claims.Add(new Claim(ClaimTypes.Role, "User"));
                break;

            case var _ when token == AuthorizationTests.TenantAToken:
                claims.Add(new Claim("tenant_id", "tenant-a"));
                claims.Add(new Claim(ClaimTypes.Role, "User"));
                break;

            default:
                return Task.FromResult(AuthenticateResult.Fail("Invalid token"));
        }

        // Add common claims
        claims.Add(new Claim(ClaimTypes.Name, "test@example.com"));
        claims.Add(new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()));

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
