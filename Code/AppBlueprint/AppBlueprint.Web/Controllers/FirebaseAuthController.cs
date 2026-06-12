using System.Security.Claims;
using AppBlueprint.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

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

    // SECURITY (OWASP A03): bound credential input before it reaches the Firebase HTTP call.
    // RFC 5321 caps e-mail addresses at 254 characters; Firebase rejects passwords over 128.
    private const int MaxEmailLength = 254;
    private const int MaxPasswordLength = 128;

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
    [EnableRateLimiting("auth-signin")]
    public async Task<IActionResult> SignIn([FromForm] FirebaseSignInFormRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        // SECURITY (OWASP A03/A07): validate input manually (not via DataAnnotations) so the
        // browser form flow keeps its redirect-with-error UX instead of an automatic JSON 400.
        // Use a generic error message to avoid disclosing which field was rejected.
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password)
            || request.Email.Length > MaxEmailLength || request.Password.Length > MaxPasswordLength)
        {
            _logger.LogWarning("Firebase sign-in rejected: invalid form input");
            string encodedValidationError = Uri.EscapeDataString("Login failed. Please check your credentials.");
            return Redirect($"/signin/firebase?error={encodedValidationError}");
        }

        FirebaseSignInResult result = await _firebaseSignIn.SignInAsync(request.Email, request.Password);

        if (!result.IsSuccess)
        {
            // SECURITY (GDPR/OWASP A09): do not log e-mail addresses or other PII.
            _logger.LogWarning("Firebase sign-in rejected: {Error}", result.Error);
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

        // SECURITY (GDPR/OWASP A09): log the opaque uid only, never the e-mail.
        _logger.LogInformation("Firebase sign-in successful (uid: {Uid})", uid);
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
