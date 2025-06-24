namespace AppBlueprint.Domain.Interfaces.Services;

/// <summary>
/// Domain service interface for email operations
/// </summary>
public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
    Task SendWelcomeEmailAsync(string to, string userName, CancellationToken cancellationToken = default);
    Task SendPasswordResetEmailAsync(string to, string resetToken, CancellationToken cancellationToken = default);
    Task SendSignUpWelcomeEmail(string fromEmail, string toEmail, string subject, CancellationToken cancellationToken = default);
}
