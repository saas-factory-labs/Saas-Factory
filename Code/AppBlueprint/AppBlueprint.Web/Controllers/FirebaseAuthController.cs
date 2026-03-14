using System.Security.Claims;
using AppBlueprint.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppBlueprint.Web.Controllers;

/// <summary>
/// Handles Firebase email/password sign-in for the Blazor Server frontend.
/// Accepts a form POST, calls Firebase Identity Toolkit, and issues an ASP.NET Core
/// auth cookie (Firebase.Cookie scheme) so that Blazor's CascadingAuthenticationState
/// reflects the authenticated user without requiring JSInterop.
/// </summary>
[ApiController]
[Route("auth")]
[AllowAnonymous]
public sealed class FirebaseAuthController : ControllerBase
{
    private const string FirebaseCookieScheme = "Firebase.Cookie";

    private readonly IFirebaseSignInService _firebaseSignIn;
    private readonly ILogger<FirebaseAuthController> _logger;

    public FirebaseAuthController(
        IFirebaseSignInService firebaseSignIn,
        ILogger<FirebaseAuthController> logger)
    {
        ArgumentNullException.ThrowIfNull(firebaseSignIn);
        ArgumentNullException.ThrowIfNull(logger);

        _firebaseSignIn = firebaseSignIn;
        _logger = logger;
    }

    /// <summary>
    /// Accepts a form POST from the Firebase login page, signs the user in via cookie,
    /// then redirects to the application root.
    /// </summary>
    [HttpPost("firebase-signin")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IActionResult> SignIn([FromForm] FirebaseSignInFormRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        FirebaseSignInResult result = await _firebaseSignIn.SignInAsync(request.Email, request.Password);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Firebase sign-in rejected for {Email}: {Error}", request.Email, result.Error);
            string encodedError = Uri.EscapeDataString(result.Error ?? "Login failed. Please check your credentials.");
            return Redirect($"/signin/firebase?error={encodedError}");
        }

        string uid = result.Uid ?? string.Empty;
        string email = result.Email ?? request.Email;

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, uid),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Name, email),
            new Claim("iss", "Firebase"),
        };

        var identity = new ClaimsIdentity(claims, FirebaseCookieScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(FirebaseCookieScheme, principal);

        _logger.LogInformation("Firebase sign-in successful for {Email} (uid: {Uid})", email, uid);
        return Redirect("/");
    }

    /// <summary>
    /// Signs the user out by clearing the Firebase.Cookie and redirecting to the login page.
    /// </summary>
    [HttpPost("firebase-signout")]
    [HttpGet("firebase-signout")]
    public async Task<IActionResult> HandleSignOut()
    {
        await HttpContext.SignOutAsync(FirebaseCookieScheme);
        _logger.LogInformation("Firebase sign-out completed");
        return Redirect("/signin/firebase");
    }
}

public sealed class FirebaseSignInFormRequest
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
