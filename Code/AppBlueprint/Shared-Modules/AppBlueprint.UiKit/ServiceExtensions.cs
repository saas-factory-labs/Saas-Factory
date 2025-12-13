using AppBlueprint.UiKit.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AppBlueprint.UiKit;

/// <summary>
/// Extension methods for registering AppBlueprint UiKit services.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Adds AppBlueprint UiKit services.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddUiKit(this IServiceCollection services)
    {
        // Register core UiKit services
        services.AddScoped<NavigationService>();
        services.AddSingleton<BreadcrumbService>();

        return services;
    }
}
