namespace AppBlueprint.Infrastructure.Email;

public interface IEmailService
{
    Task SendHtmlEmailAsync(string fromEmail, string toEmail, string subject, string htmlBody);
}
