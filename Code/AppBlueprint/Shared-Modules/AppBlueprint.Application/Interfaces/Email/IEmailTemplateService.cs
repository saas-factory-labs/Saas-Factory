using AppBlueprint.Contracts.Email;

namespace AppBlueprint.Application.Interfaces.Email;

/// <summary>
/// Service for rendering email templates using Razor.
/// </summary>
public interface IEmailTemplateService
{
    /// <summary>
    /// Renders an email template with the provided model.
    /// </summary>
    /// <typeparam name="TModel">The type of the template model.</typeparam>
    /// <param name="templateName">The name of the template (without .cshtml extension).</param>
    /// <param name="model">The data model to pass to the template.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The rendered HTML string.</returns>
    /// <exception cref="InvalidOperationException">Thrown when template is not found.</exception>
    Task<string> RenderTemplateAsync<TModel>(
        string templateName,
        TModel model,
        CancellationToken cancellationToken = default) where TModel : IEmailTemplateModel;

    /// <summary>
    /// Renders an email template using the model's built-in template name.
    /// </summary>
    /// <typeparam name="TModel">The type of the template model.</typeparam>
    /// <param name="model">The data model to pass to the template.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The rendered HTML string.</returns>
    Task<string> RenderTemplateAsync<TModel>(
        TModel model,
        CancellationToken cancellationToken = default) where TModel : IEmailTemplateModel;

    /// <summary>
    /// Gets a list of all available email templates.
    /// </summary>
    /// <returns>List of template names.</returns>
    Task<IReadOnlyList<string>> GetAvailableTemplatesAsync();
}
