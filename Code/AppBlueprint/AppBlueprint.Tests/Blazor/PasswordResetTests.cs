using AppBlueprint.Application.Services.Users;
using AppBlueprint.UiKit.Components.Pages;
using Bunit;
using static Bunit.ComponentParameterFactory; // Added for Parameter()
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using Moq;

namespace AppBlueprint.Tests.Blazor
{
    internal class PasswordResetTests : Bunit.TestContext // Changed to internal
    {
        private readonly Mock<IUserService> _userServiceMock;

        public PasswordResetTests()
        {
            _userServiceMock = new Mock<IUserService>();
            var snackbarMock = new Mock<ISnackbar>(); // Made local

            Services.AddSingleton(_userServiceMock.Object);
            Services.AddSingleton(snackbarMock.Object); // Use local mock
            Services.AddMudServices();
        }

        [Test] // Changed from [TUnit.Core.Test]
        public void ForgotPasswordShouldRender()
        {
            // Act
            var cut = RenderComponent<ForgotPassword>(); // No parameters

            // Assert
            cut.MarkupMatches("*Forgot Password*");
            cut.MarkupMatches("*Email*");
            cut.MarkupMatches("*Reset Password*");
        }

        [Test] // Changed from [TUnit.Core.Test]
        public void ResetPasswordShouldRender()
        {
            // Act
            var cut = RenderComponent<ResetPassword>( // Using ComponentParameterFactory.Parameter
                Parameter(nameof(ResetPassword.Token), "test-token")
            );

            // Assert
            cut.MarkupMatches("*Reset Password*");
            cut.MarkupMatches("*Email*");
            cut.MarkupMatches("*New Password*");
            cut.MarkupMatches("*Confirm Password*");
        }

        [Test] // Changed from [TUnit.Core.Test]
        public void EmailVerificationShouldRender()
        {
            // Arrange
            _userServiceMock.Setup(x => x.VerifyEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var cut = RenderComponent<EmailVerification>( // Using ComponentParameterFactory.Parameter
                Parameter(nameof(EmailVerification.Token), "test-token"),
                Parameter(nameof(EmailVerification.UserId), 1)
            );

            // Assert - initially should show loading
            cut.MarkupMatches("*Verifying your email*");

            // Wait for verification to complete
            cut.WaitForState(() => cut.Find(".mud-icon-root").ClassList.Contains("mud-icon-size-large"));

            // Should now show success message
            cut.MarkupMatches("*Email Verified Successfully*");
        }
    }
}
