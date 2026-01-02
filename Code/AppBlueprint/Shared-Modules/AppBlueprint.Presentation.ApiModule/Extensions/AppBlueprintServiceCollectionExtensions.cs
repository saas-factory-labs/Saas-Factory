using AppBlueprint.Application.Extensions;
using AppBlueprint.Infrastructure.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AppBlueprint.Presentation.ApiModule.Extensions;

/// <summary>
/// Unified extension method for adding all AppBlueprint services at once.
/// </summary>
public static class AppBlueprintServiceCollectionExtensions
{
    /// <summary>
    /// Adds all AppBlueprint services (Infrastructure, Application, and Presentation) in one call.
    /// This is a convenience method that calls AddAppBlueprintInfrastructure, AddAppBlueprintApplication,
    /// and AddAppBlueprintPresentation.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration for infrastructure services.</param>
    /// <param name="environment">The web host environment (required for authentication setup and CORS).</param>
    /// <returns>The service collection for chaining.</returns>
    /// <example>
    /// <code>
    /// // In Program.cs
    /// builder.Services.AddAppBlueprint(builder.Configuration, builder.Environment);
    /// </code>
    /// </example>
    public static IServiceCollection AddAppBlueprint(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(environment);

        // Register all AppBlueprint layers
        services.AddAppBlueprintInfrastructure(configuration, environment);
        services.AddAppBlueprintApplication();
        services.AddAppBlueprintPresentation(environment, configuration);

        return services;
    }
}
