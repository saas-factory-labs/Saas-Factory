using AppBlueprint.Application.Interfaces;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using RazorLight;
using Resend;

namespace AppBlueprint.Infrastructure.Services.Email;

/// <summary>
/// Email template service using RazorLight for rendering Razor templates.
/// Supports template override pattern: deployed apps can provide custom templates
/// that override the framework's generic templates.
/// </summary>
public sealed class RazorEmailTemplateService : IEmailTemplateService
{
    private readonly IRazorLightEngine _razorEngine;
    private readonly IResend? _resend;
    private readonly ILogger<RazorEmailTemplateService> _logger;

    public RazorEmailTemplateService(
        IRazorLightEngine razorEngine,
        IResend? resend,
        ILogger<RazorEmailTemplateService> logger)
    {
        ArgumentNullException.ThrowIfNull(razorEngine);
        ArgumentNullException.ThrowIfNull(logger);

        _razorEngine = razorEngine;
        _resend = resend;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<string> RenderTemplateAsync<TModel>(
        string templateName,
        TModel model,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(templateName);
        ArgumentNullException.ThrowIfNull(model);

        try
        {
            // RazorLight will automatically search for templates in configured paths
            // 1. First checks deployed app's custom templates folder
            // 2. Falls back to framework's embedded templates
            string templateKey = $"{templateName}.cshtml";
            string result = await _razorEngine.CompileRenderAsync(templateKey, model);

            _logger.LogDebug("Successfully rendered email template {TemplateName}", templateName);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to render email template {TemplateName}", templateName);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<Guid> SendTemplatedEmailAsync<TModel>(
        string from,
        string to,
        string subject,
        string templateName,
        TModel model,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(from);
        ArgumentNullException.ThrowIfNull(to);
        ArgumentNullException.ThrowIfNull(subject);
        ArgumentNullException.ThrowIfNull(templateName);
        ArgumentNullException.ThrowIfNull(model);

        if (_resend is null)
        {
            const string errorMessage = "Cannot send email: Resend is not configured. Please set one of these environment variables: APPBLUEPRINT_RESEND_APIKEY, RESEND_API_KEY, or APPBLUEPRINT_Resend__ApiKey (and corresponding FROM_EMAIL variables).";
            _logger.LogError("{ErrorMessage}", errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        try
        {
            // Render the email HTML from template
            string htmlBody = await RenderTemplateAsync(templateName, model, cancellationToken);

            // Send email using Resend
            var message = new EmailMessage
            {
                From = from,
                To = to,
                Subject = subject,
                HtmlBody = htmlBody
            };

            ResendResponse<Guid> response = await _resend.EmailSendAsync(message, cancellationToken);

            if (response.Content != Guid.Empty)
            {
                _logger.LogInformation(
                    "Sent templated email {TemplateName} to {Recipient} with ID {EmailId}",
                    templateName,
                    to,
                    response.Content);

                return response.Content;
            }

            _logger.LogError("Failed to send email - no email ID returned");
            throw new InvalidOperationException("Failed to send email");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send templated email {TemplateName} to {Recipient}",
                templateName,
                to);
            throw;
        }
    }
}
