namespace AppBlueprint.Application.Interfaces;

/// <summary>
/// Service for rendering and sending templated emails using Razor templates.
/// </summary>
public interface IEmailTemplateService
{
    /// <summary>
    /// Renders an email template to HTML using the specified model.
    /// </summary>
    /// <typeparam name="TModel">The type of the model containing template data.</typeparam>
    /// <param name="templateName">The name of the template file (without .cshtml extension).</param>
    /// <param name="model">The model data to populate the template.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The rendered HTML content.</returns>
    Task<string> RenderTemplateAsync<TModel>(string templateName, TModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Renders and sends a templated email to the specified recipient.
    /// </summary>
    /// <typeparam name="TModel">The type of the model containing template data.</typeparam>
    /// <param name="from">The sender email address.</param>
    /// <param name="to">The recipient email address.</param>
    /// <param name="subject">The email subject line.</param>
    /// <param name="templateName">The name of the template file (without .cshtml extension).</param>
    /// <param name="model">The model data to populate the template.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The unique ID of the sent email.</returns>
    Task<Guid> SendTemplatedEmailAsync<TModel>(string from, string to, string subject, string templateName, TModel model, CancellationToken cancellationToken = default);
}
