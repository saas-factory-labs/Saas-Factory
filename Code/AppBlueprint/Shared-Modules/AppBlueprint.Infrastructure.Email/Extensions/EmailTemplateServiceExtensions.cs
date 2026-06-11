using System.Reflection;
using AppBlueprint.Application.Interfaces;
using AppBlueprint.Infrastructure.Services.Email;
using Microsoft.Extensions.DependencyInjection;
using RazorLight;
using Resend;

namespace AppBlueprint.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering email template services.
/// </summary>
public static class EmailTemplateServiceExtensions
{
    /// <summary>
    /// Registers the email template service with support for template overrides.
    /// Deployed applications can provide custom templates in their own /Templates/ folder
    /// which will override the framework's generic templates.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="customTemplatesPath">Optional path to custom templates folder in the deployed application. 
    /// If not provided, only framework templates will be available.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddEmailTemplateService(
        this IServiceCollection services,
        string? customTemplatesPath = null)
    {
        // Configure RazorLight engine
        IRazorLightEngine razorEngine = BuildRazorLightEngine(customTemplatesPath);

        services.AddSingleton(razorEngine);

        // Register a factory that resolves IResend? from the service provider
        // This allows the service to work even when IResend is not registered
        services.AddScoped<IEmailTemplateService>(sp =>
        {
            IResend? resend = sp.GetService<IResend>(); // Returns null if not registered
            var logger = sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<RazorEmailTemplateService>>();
            return new RazorEmailTemplateService(razorEngine, resend, logger);
        });

        return services;
    }

    private static IRazorLightEngine BuildRazorLightEngine(string? customTemplatesPath)
    {
        var builder = new RazorLightEngineBuilder();

        // If deployed app provides custom templates path, add it first (higher priority)
        if (!string.IsNullOrEmpty(customTemplatesPath) && Directory.Exists(customTemplatesPath))
        {
            builder.UseFileSystemProject(customTemplatesPath);
        }
        else
        {
            // Use embedded resources from framework (fallback)
            // Templates are embedded in AppBlueprint.Infrastructure assembly
            Assembly infrastructureAssembly = typeof(RazorEmailTemplateService).Assembly;
            const string templateNamespace = "AppBlueprint.Infrastructure.Services.Email.Templates";

            builder.UseEmbeddedResourcesProject(infrastructureAssembly, templateNamespace);
        }

        builder.UseMemoryCachingProvider();

        return builder.Build();
    }
}
