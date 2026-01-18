using System.Diagnostics;
using System.Reflection;
using AppBlueprint.Application.Interfaces.Email;
using AppBlueprint.Contracts.Email;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.Services.Email;

/// <summary>
/// Razor-based email template rendering service.
/// Templates should be located in ~/EmailTemplates/{TemplateName}.cshtml
/// </summary>
public sealed class RazorEmailTemplateService(
    IRazorViewEngine razorViewEngine,
    ITempDataProvider tempDataProvider,
    IServiceProvider serviceProvider,
    ILogger<RazorEmailTemplateService> logger) : IEmailTemplateService
{
    private const string TemplateBasePath = "~/EmailTemplates";

    public async Task<string> RenderTemplateAsync<TModel>(
        string templateName,
        TModel model,
        CancellationToken cancellationToken = default) where TModel : IEmailTemplateModel
    {
        ArgumentNullException.ThrowIfNull(templateName);
        ArgumentNullException.ThrowIfNull(model);

        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            var httpContext = new DefaultHttpContext { RequestServices = serviceProvider };
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

            string viewPath = $"{TemplateBasePath}/{templateName}.cshtml";
            ViewEngineResult? viewResult = razorViewEngine.GetView(executingFilePath: null, viewPath, isMainPage: false);

            if (!viewResult.Success)
            {
                // Try FindView as fallback
                viewResult = razorViewEngine.FindView(actionContext, templateName, isMainPage: false);
            }

            if (!viewResult.Success)
            {
                string searchedLocations = string.Join(", ", viewResult.SearchedLocations ?? []);
                throw new InvalidOperationException(
                    $"Email template '{templateName}' not found. Searched locations: {searchedLocations}");
            }

            await using var writer = new StringWriter();
            var viewDictionary = new ViewDataDictionary<TModel>(
                new EmptyModelMetadataProvider(),
                new ModelStateDictionary())
            {
                Model = model
            };

            var tempData = new TempDataDictionary(actionContext.HttpContext, tempDataProvider);
            var viewContext = new ViewContext(
                actionContext,
                viewResult.View,
                viewDictionary,
                tempData,
                writer,
                new HtmlHelperOptions()
            );

            await viewResult.View.RenderAsync(viewContext);
            string renderedContent = writer.ToString();

            stopwatch.Stop();
            logger.LogInformation(
                "Rendered email template {TemplateName} in {ElapsedMs}ms",
                templateName,
                stopwatch.ElapsedMilliseconds);

            return renderedContent;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to render email template {TemplateName}", templateName);
            throw;
        }
    }

    public Task<string> RenderTemplateAsync<TModel>(
        TModel model,
        CancellationToken cancellationToken = default) where TModel : IEmailTemplateModel
    {
        ArgumentNullException.ThrowIfNull(model);
        return RenderTemplateAsync(model.TemplateName, model, cancellationToken);
    }

    public Task<IReadOnlyList<string>> GetAvailableTemplatesAsync()
    {
        List<string> templates = [];
        
        try
        {
            // Get the assembly where email templates are embedded
            Assembly assembly = typeof(RazorEmailTemplateService).Assembly;
            string[] resourceNames = assembly.GetManifestResourceNames();
            
            // Filter for .cshtml files in EmailTemplates directory (excluding _Layout)
            templates.AddRange(resourceNames
                .Where(name => name.Contains("EmailTemplates", StringComparison.Ordinal) && 
                              name.EndsWith(".cshtml", StringComparison.Ordinal) &&
                              !name.Contains("_Layout", StringComparison.Ordinal))
                .Select(name =>
                {
                    // Extract template name from resource name
                    int lastDot = name.LastIndexOf('.');
                    int secondLastDot = name.LastIndexOf('.', lastDot - 1);
                    return name.Substring(secondLastDot + 1, lastDot - secondLastDot - 1);
                })
                .OrderBy(name => name, StringComparer.Ordinal));

            logger.LogInformation("Found {Count} email templates", templates.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error listing available email templates");
        }

        return Task.FromResult<IReadOnlyList<string>>(templates.AsReadOnly());
    }
}
