using AppBlueprint.Domain.Baseline.Users;
using AppBlueprint.UiKit.Components.Pages;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using Moq;
using TUnit;

namespace AppBlueprint.Tests.Blazor
{
    public class PasswordResetTests : BunitContext
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<ISnackbar> _snackbarMock;

        public PasswordResetTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _snackbarMock = new Mock<ISnackbar>();

            Services.AddSingleton(_userServiceMock.Object);
            Services.AddSingleton(_snackbarMock.Object);
            Services.AddMudServices();
        }

        [Test]
        public void ForgotPasswordShouldRender()
        {
            // Act
            var cut = Render<ForgotPassword>();

            // Assert
            cut.MarkupMatches("*Forgot Password*");
            cut.MarkupMatches("*Email*");
            cut.MarkupMatches("*Reset Password*");
        }

        [Test]
        public void ResetPasswordShouldRender()
        {
            // Act
            var cut = Render<ResetPassword>(parameters => parameters
                .Add(p => p.Token, "test-token"));

            // Assert
            cut.MarkupMatches("*Reset Password*");
            cut.MarkupMatches("*Email*");
            cut.MarkupMatches("*New Password*");
            cut.MarkupMatches("*Confirm Password*");
        }

        [Test]
        public void EmailVerificationShouldRender()
        {
            // Arrange
            _userServiceMock.Setup(x => x.VerifyEmailAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var cut = Render<EmailVerification>(parameters => parameters
                .Add(p => p.Token, "test-token")
                .Add(p => p.UserId, 1));

            // Assert - initially should show loading
            cut.MarkupMatches("*Verifying your email*");

            // Wait for verification to complete
            cut.WaitForState(() => cut.Find(".mud-icon-root").ClassList.Contains("mud-icon-size-large"));

            // Should now show success message
            cut.MarkupMatches("*Email Verified Successfully*");
        }
    }
}