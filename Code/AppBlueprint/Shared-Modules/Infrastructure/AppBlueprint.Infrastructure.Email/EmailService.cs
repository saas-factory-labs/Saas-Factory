using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace AppBlueprint.Infrastructure.Email;

public sealed class EmailService(IOptions<SmtpOptions> options) : IEmailService
{
    private readonly SmtpOptions _options = options.Value;

    public async Task SendHtmlEmailAsync(string fromEmail, string toEmail, string subject, string htmlBody)
    {
        ArgumentException.ThrowIfNullOrEmpty(fromEmail);
        ArgumentException.ThrowIfNullOrEmpty(toEmail);
        ArgumentException.ThrowIfNullOrEmpty(subject);
        ArgumentException.ThrowIfNullOrEmpty(htmlBody);

        using var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_options.SenderName, fromEmail));
        message.To.Add(new MailboxAddress(string.Empty, toEmail));
        message.Subject = subject;
        message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_options.Host, _options.Port, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_options.Username, _options.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(quit: true);
    }
}
