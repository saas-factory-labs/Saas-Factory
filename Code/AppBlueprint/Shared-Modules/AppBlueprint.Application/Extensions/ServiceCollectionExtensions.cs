using AppBlueprint.Application.Services;
using AppBlueprint.Application.Services.Blog;
using AppBlueprint.Application.Services.DataExport;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AppBlueprint.Application.Extensions;

/// <summary>
/// Extension methods for registering AppBlueprint Application services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds AppBlueprint Application services including command handlers, query handlers,
    /// validators, and application-level services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAppBlueprintApplication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        // Register FluentValidation validators from this assembly
        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);

        // Register application services
        services.AddApplicationServices();

        // TODO: Register command handlers when implemented
        // TODO: Register query handlers when implemented

        return services;
    }

    /// <summary>
    /// Adds the shared AppBlueprint blog content service.
    /// Host applications can configure static articles here or replace IBlogContentService
    /// with a database/CMS-backed implementation.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureBlogContent">Optional content configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAppBlueprintBlogContent(
        this IServiceCollection services,
        Action<Options.BlogContentOptions>? configureBlogContent = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions<Options.BlogContentOptions>();

        if (configureBlogContent is not null)
        {
            services.Configure(configureBlogContent);
        }

        services.TryAddSingleton<IBlogContentService, OptionsBlogContentService>();

        return services;
    }

    /// <summary>
    /// Registers application-level services from AppBlueprint.Application.
    /// </summary>
    private static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IDataExportService, DataExportService>();
        services.AddScoped<ISignupService, SignupService>();
        // Add more application services as they are implemented

        return services;
    }
}
