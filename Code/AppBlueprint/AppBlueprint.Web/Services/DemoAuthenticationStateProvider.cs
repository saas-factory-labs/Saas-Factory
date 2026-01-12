using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;

namespace AppBlueprint.Web.Services;

/// <summary>
/// Provides a demo user authentication state that bypasses actual authentication.
/// Allows showcasing the UI with all Cruip template pages visible.
/// Accessible via /demo route regardless of Logto configuration.
/// </summary>
public sealed class DemoAuthenticationStateProvider : RevalidatingServerAuthenticationStateProvider
{
    private readonly IServiceProvider _serviceProvider;
    
    public DemoAuthenticationStateProvider(
        ILoggerFactory loggerFactory,
        IServiceProvider serviceProvider) 
        : base(loggerFactory)
    {
        _serviceProvider = serviceProvider;
    }

    protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

    protected override Task<bool> ValidateAuthenticationStateAsync(
        AuthenticationState authenticationState, 
        CancellationToken cancellationToken)
    {
        // Demo users are always valid
        return Task.FromResult(true);
    }

    public Task<AuthenticationState> GetDemoAuthenticationStateAsync()
    {
        // Create a demo user with basic claims
        var demoIdentity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, "Demo User"),
            new Claim(ClaimTypes.NameIdentifier, "demo-user-id"),
            new Claim(ClaimTypes.Email, "demo@example.com"),
            new Claim("sub", "demo-user-id"), // Logto uses 'sub' claim
            new Claim("role", "demo")
        }, "DemoAuthentication");

        var demoPrincipal = new ClaimsPrincipal(demoIdentity);
        return Task.FromResult(new AuthenticationState(demoPrincipal));
    }
}
