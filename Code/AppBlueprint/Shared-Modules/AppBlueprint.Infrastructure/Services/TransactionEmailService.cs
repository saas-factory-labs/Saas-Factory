using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;
using Microsoft.Extensions.Logging;
using Resend;

namespace AppBlueprint.Infrastructure.Services;

public class TransactionEmailService(ILogger logger, IResend resend)
{
    public async Task SendSignUpWelcomeEmail(string from, string to, string siteName)
    {
        ArgumentNullException.ThrowIfNull(from);
        ArgumentNullException.ThrowIfNull(to);
        ArgumentNullException.ThrowIfNull(siteName);

        var message = new EmailMessage
        {
            From = from,
            To = to,
            Subject = $"Welcome to {siteName} ",
            TextBody = $"Welcome to {siteName}. " +
                       $"Let us know " +
                       $"if you have any questions."
        };

        ResendResponse<Guid> resp = await resend.EmailSendAsync(message);

        logger.LogInformation("Sent email, with Id = {EmailId}", resp.Content);
    }

    public async Task SendOrderConfirmationEmail(string from, string to, string orderId, CustomerEntity customerEntity)
    {
        ArgumentNullException.ThrowIfNull(from);
        ArgumentNullException.ThrowIfNull(to);
        ArgumentNullException.ThrowIfNull(orderId);
        ArgumentNullException.ThrowIfNull(customerEntity);

        var message = new EmailMessage
        {
            From = from,
            To = to,
            Subject = "Order Confirmation",
            TextBody = $"Your order {orderId} has been confirmed."
        };

        ResendResponse<Guid> resp = await resend.EmailSendAsync(message);

        logger.LogInformation("Sent email, with Id = {EmailId}", resp.Content);
    }
}
