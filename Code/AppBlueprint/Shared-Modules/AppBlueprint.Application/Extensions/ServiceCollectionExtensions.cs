using AppBlueprint.Application.Services;
using AppBlueprint.Application.Services.DataExport;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

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
