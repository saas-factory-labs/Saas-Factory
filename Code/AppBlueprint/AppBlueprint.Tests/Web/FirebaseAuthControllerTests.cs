using AppBlueprint.Infrastructure.Authentication;
using AppBlueprint.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace AppBlueprint.Tests.Web;

/// <summary>
/// Tests for the input-validation guard on the Firebase sign-in endpoint (OWASP A03/A07).
/// Invalid credentials input must be rejected before any call to the Firebase sign-in service,
/// and must redirect back to the login page with a generic error (no field-level disclosure).
/// </summary>
internal sealed class FirebaseAuthControllerTests
{
    private const string SignInErrorRedirectPrefix = "/signin/firebase?error=";

    private static (FirebaseAuthController Controller, IFirebaseSignInService SignInService) CreateController()
    {
        IFirebaseSignInService signInService = Substitute.For<IFirebaseSignInService>();
        ILogger<FirebaseAuthController> logger = Substitute.For<ILogger<FirebaseAuthController>>();

        var controller = new FirebaseAuthController(signInService, logger);
        return (controller, signInService);
    }

    [Test]
    [Arguments("", "somePassword")]
    [Arguments("   ", "somePassword")]
    [Arguments("user@example.com", "")]
    [Arguments("user@example.com", "   ")]
    public async Task SignIn_WithMissingCredentials_RedirectsWithoutCallingFirebase(string email, string password)
    {
        (FirebaseAuthController controller, IFirebaseSignInService signInService) = CreateController();
        var request = new FirebaseSignInFormRequest { Email = email, Password = password };

        IActionResult result = await controller.SignIn(request);

        var redirect = result as RedirectResult;
        await Assert.That(redirect).IsNotNull();
        await Assert.That(redirect!.Url).StartsWith(SignInErrorRedirectPrefix);
        await signInService.DidNotReceiveWithAnyArgs().SignInAsync(default!, default!);
    }

    [Test]
    public async Task SignIn_WithOversizedEmail_RedirectsWithoutCallingFirebase()
    {
        (FirebaseAuthController controller, IFirebaseSignInService signInService) = CreateController();
        string oversizedEmail = new string('a', 250) + "@example.com"; // exceeds RFC 5321 limit of 254
        var request = new FirebaseSignInFormRequest { Email = oversizedEmail, Password = "somePassword" };

        IActionResult result = await controller.SignIn(request);

        var redirect = result as RedirectResult;
        await Assert.That(redirect).IsNotNull();
        await Assert.That(redirect!.Url).StartsWith(SignInErrorRedirectPrefix);
        await signInService.DidNotReceiveWithAnyArgs().SignInAsync(default!, default!);
    }

    [Test]
    public async Task SignIn_WithOversizedPassword_RedirectsWithoutCallingFirebase()
    {
        (FirebaseAuthController controller, IFirebaseSignInService signInService) = CreateController();
        var request = new FirebaseSignInFormRequest
        {
            Email = "user@example.com",
            Password = new string('p', 129) // exceeds Firebase's 128-character maximum
        };

        IActionResult result = await controller.SignIn(request);

        var redirect = result as RedirectResult;
        await Assert.That(redirect).IsNotNull();
        await Assert.That(redirect!.Url).StartsWith(SignInErrorRedirectPrefix);
        await signInService.DidNotReceiveWithAnyArgs().SignInAsync(default!, default!);
    }

    [Test]
    public async Task SignIn_WithRejectedCredentials_RedirectsWithError()
    {
        (FirebaseAuthController controller, IFirebaseSignInService signInService) = CreateController();
        signInService.SignInAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new FirebaseSignInResult { IsSuccess = false, Error = "INVALID_LOGIN_CREDENTIALS" });
        var request = new FirebaseSignInFormRequest { Email = "user@example.com", Password = "wrongPassword" };

        IActionResult result = await controller.SignIn(request);

        var redirect = result as RedirectResult;
        await Assert.That(redirect).IsNotNull();
        await Assert.That(redirect!.Url).StartsWith(SignInErrorRedirectPrefix);
    }
}
