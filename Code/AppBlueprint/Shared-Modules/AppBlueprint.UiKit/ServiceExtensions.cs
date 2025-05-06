using AppBlueprint.UiKit.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AppBlueprint.UiKit;

public static class ServiceExtensions
{
    public static IServiceCollection AddUiKit(this IServiceCollection services)
    {
        services.AddScoped<NavigationService>();
        return services;
    }
}
