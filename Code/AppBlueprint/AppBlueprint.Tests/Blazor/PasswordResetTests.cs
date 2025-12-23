using AppBlueprint.Application.Services.Users;
using AppBlueprint.UiKit.Components.Pages;
using Bunit;
using static Bunit.ComponentParameterFactory;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace AppBlueprint.Tests.Blazor;

internal sealed class PasswordResetTests : Bunit.TestContext
{
    private readonly Mock<IUserService> _userServiceMock;

    public PasswordResetTests()
    {
        _userServiceMock = new Mock<IUserService>();
        Services.AddSingleton(_userServiceMock.Object);
    }

    // TODO: ForgotPassword component doesn't exist yet
    // [Test]
    // public void ForgotPasswordShouldRender()
    // {
    //     // Act
    //     var cut = RenderComponent<ForgotPassword>();
    //     // Assert
    //     cut.MarkupMatches("*Forgot Password*");
    //     cut.MarkupMatches("*Email*");
    //     cut.MarkupMatches("*Reset Password*");
    // }

    // TODO: ResetPassword component doesn't exist yet
    // [Test]
    // public void ResetPasswordShouldRender()
    // {
    //     // Act
    //     var cut = RenderComponent<ResetPassword>(
    //         Parameter(nameof(ResetPassword.Token), "test-token")
    //     );
    //     // Assert
    //     cut.MarkupMatches("*Reset Password*");
    //     cut.MarkupMatches("*Email*");
    //     cut.MarkupMatches("*New Password*");
    //     cut.MarkupMatches("*Confirm Password*");
    // }

    // TODO: EmailVerification component doesn't exist yet
    // [Test]
    // public void EmailVerificationShouldRender()
    // {
    //     // Arrange
    //     _userServiceMock.Setup(x => x.VerifyEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(true);
    //     // Act
    //     var cut = RenderComponent<EmailVerification>(
    //         Parameter(nameof(EmailVerification.Token), "test-token"),
    //         Parameter(nameof(EmailVerification.UserId), 1)
    //     );
    //     // Assert - initially should show loading
    //     cut.MarkupMatches("*Verifying your email*");
    //     // Should show success message after verification
    //     cut.MarkupMatches("*Email Verified Successfully*");
    // }
}
