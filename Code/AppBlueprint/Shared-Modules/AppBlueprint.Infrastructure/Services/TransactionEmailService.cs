using AppBlueprint.Application.Interfaces.Email;
using AppBlueprint.Contracts.Email;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;
using Microsoft.Extensions.Logging;
using Resend;

namespace AppBlueprint.Infrastructure.Services;

public sealed class TransactionEmailService(
    ILogger<TransactionEmailService> logger, 
    IResend resend,
    IEmailTemplateService templateService)
{
    public async Task SendSignUpWelcomeEmail(
        string from, 
        string to, 
        string userName,
        string siteName,
        string loginUrl,
        string supportEmail,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(from);
        ArgumentNullException.ThrowIfNull(to);
        ArgumentNullException.ThrowIfNull(userName);
        ArgumentNullException.ThrowIfNull(siteName);
        ArgumentNullException.ThrowIfNull(loginUrl);
        ArgumentNullException.ThrowIfNull(supportEmail);

        var model = new WelcomeEmailModel(userName, siteName, loginUrl, supportEmail);
        string htmlBody = await templateService.RenderTemplateAsync(model, cancellationToken);

        var message = new EmailMessage
        {
            From = from,
            To = to,
            Subject = $"Welcome to {siteName}!",
            HtmlBody = htmlBody
        };

        ResendResponse<Guid> resp = await resend.EmailSendAsync(message);
        logger.LogInformation("Sent welcome email to {Email}, Id = {EmailId}", to, resp.Content);
    }

    public async Task SendPasswordResetEmail(
        string from,
        string to,
        string userName,
        string resetUrl,
        string supportEmail,
        string expirationHours = "24",
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(from);
        ArgumentNullException.ThrowIfNull(to);
        ArgumentNullException.ThrowIfNull(userName);
        ArgumentNullException.ThrowIfNull(resetUrl);
        ArgumentNullException.ThrowIfNull(supportEmail);

        var model = new PasswordResetEmailModel(userName, resetUrl, expirationHours, supportEmail);
        string htmlBody = await templateService.RenderTemplateAsync(model, cancellationToken);

        var message = new EmailMessage
        {
            From = from,
            To = to,
            Subject = "Reset Your Password",
            HtmlBody = htmlBody
        };

        ResendResponse<Guid> resp = await resend.EmailSendAsync(message);
        logger.LogInformation("Sent password reset email to {Email}, Id = {EmailId}", to, resp.Content);
    }

    public async Task SendEmailVerification(
        string from,
        string to,
        string userName,
        string verificationUrl,
        string siteName,
        string expirationHours = "24",
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(from);
        ArgumentNullException.ThrowIfNull(to);
        ArgumentNullException.ThrowIfNull(userName);
        ArgumentNullException.ThrowIfNull(verificationUrl);
        ArgumentNullException.ThrowIfNull(siteName);

        var model = new EmailVerificationModel(userName, verificationUrl, siteName, expirationHours);
        string htmlBody = await templateService.RenderTemplateAsync(model, cancellationToken);

        var message = new EmailMessage
        {
            From = from,
            To = to,
            Subject = $"Verify Your Email - {siteName}",
            HtmlBody = htmlBody
        };

        ResendResponse<Guid> resp = await resend.EmailSendAsync(message);
        logger.LogInformation("Sent email verification to {Email}, Id = {EmailId}", to, resp.Content);
    }

    public async Task SendOrderConfirmationEmail(
        string from, 
        string to, 
        string customerName,
        string orderId, 
        decimal totalAmount,
        List<OrderLineItemModel> items,
        string orderDetailsUrl,
        string siteName,
        string supportEmail,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(from);
        ArgumentNullException.ThrowIfNull(to);
        ArgumentNullException.ThrowIfNull(customerName);
        ArgumentNullException.ThrowIfNull(orderId);
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(orderDetailsUrl);
        ArgumentNullException.ThrowIfNull(siteName);
        ArgumentNullException.ThrowIfNull(supportEmail);

        var model = new OrderConfirmationEmailModel(
            customerName, 
            orderId, 
            totalAmount, 
            items, 
            orderDetailsUrl, 
            siteName, 
            supportEmail);
        
        string htmlBody = await templateService.RenderTemplateAsync(model, cancellationToken);

        var message = new EmailMessage
        {
            From = from,
            To = to,
            Subject = $"Order Confirmation - {orderId}",
            HtmlBody = htmlBody
        };

        ResendResponse<Guid> resp = await resend.EmailSendAsync(message);
        logger.LogInformation("Sent order confirmation email to {Email}, OrderId = {OrderId}, EmailId = {EmailId}", 
            to, orderId, resp.Content);
    }

    public async Task SendTeamInvitation(
        string from,
        string to,
        string inviteeName,
        string inviterName,
        string teamName,
        string acceptInvitationUrl,
        string siteName,
        string expirationDays = "7",
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(from);
        ArgumentNullException.ThrowIfNull(to);
        ArgumentNullException.ThrowIfNull(inviteeName);
        ArgumentNullException.ThrowIfNull(inviterName);
        ArgumentNullException.ThrowIfNull(teamName);
        ArgumentNullException.ThrowIfNull(acceptInvitationUrl);
        ArgumentNullException.ThrowIfNull(siteName);

        var model = new TeamInvitationEmailModel(
            inviteeName, 
            inviterName, 
            teamName, 
            acceptInvitationUrl, 
            siteName, 
            expirationDays);
        
        string htmlBody = await templateService.RenderTemplateAsync(model, cancellationToken);

        var message = new EmailMessage
        {
            From = from,
            To = to,
            Subject = $"You've been invited to join {teamName}",
            HtmlBody = htmlBody
        };

        ResendResponse<Guid> resp = await resend.EmailSendAsync(message);
        logger.LogInformation("Sent team invitation to {Email}, Team = {Team}, EmailId = {EmailId}", 
            to, teamName, resp.Content);
    }
}

