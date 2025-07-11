namespace AppBlueprint.Domain.Interfaces.Services;

/// <summary>
/// Domain service interface for email operations
/// </summary>
public interface IEmailService
{
    Task SendEmailAsync(string recipient, string subject, string body, CancellationToken cancellationToken = default);
    Task SendWelcomeEmailAsync(string recipient, string userName, CancellationToken cancellationToken = default);
    Task SendPasswordResetEmailAsync(string recipient, string resetToken, CancellationToken cancellationToken = default);
    Task SendSignUpWelcomeEmail(string fromEmail, string recipient, string subject, CancellationToken cancellationToken = default);
}
