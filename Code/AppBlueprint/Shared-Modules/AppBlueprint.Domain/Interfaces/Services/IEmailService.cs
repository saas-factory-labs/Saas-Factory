namespace AppBlueprint.Domain.Interfaces.Services;

/// <summary>
/// Domain service interface for email operations
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends a generic email with the specified subject and body.
    /// </summary>
    /// <param name="recipient">Email address of the recipient.</param>
    /// <param name="subject">Email subject line.</param>
    /// <param name="body">Email body content (HTML or plain text).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SendEmailAsync(string recipient, string subject, string body, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a welcome email to a new user.
    /// </summary>
    /// <param name="recipient">Email address of the new user.</param>
    /// <param name="userName">Name of the user to personalize the email.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SendWelcomeEmailAsync(string recipient, string userName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a password reset email with a reset token.
    /// </summary>
    /// <param name="recipient">Email address of the user requesting password reset.</param>
    /// <param name="resetToken">Secure token for password reset verification.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SendPasswordResetEmailAsync(string recipient, string resetToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a welcome email after user signs up.
    /// </summary>
    /// <param name="fromEmail">Email address of the sender.</param>
    /// <param name="recipient">Email address of the new user.</param>
    /// <param name="subject">Email subject line.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SendSignUpWelcomeEmail(string fromEmail, string recipient, string subject, CancellationToken cancellationToken = default);
}
