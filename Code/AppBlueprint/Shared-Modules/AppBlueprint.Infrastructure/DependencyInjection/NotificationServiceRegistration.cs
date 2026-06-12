using AppBlueprint.Application.Interfaces;
using AppBlueprint.Domain.Interfaces.Repositories;
using AppBlueprint.Infrastructure.Notifications.Extensions;
using AppBlueprint.Infrastructure.Persistence.Repositories;
using AppBlueprint.Infrastructure.Realtime.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace AppBlueprint.Infrastructure.DependencyInjection;

/// <summary>
/// Extension methods for registering notification services in the DI container.
/// </summary>
public static class NotificationServiceRegistration
{
    /// <summary>
    /// Registers all notification-related services (repositories, SignalR hub, notification services).
    /// </summary>
    public static IServiceCollection AddNotificationServices(this IServiceCollection services)
    {
        // Register repositories
        services.AddScoped<INotificationRepository, UserNotificationRepository>();
        services.AddScoped<INotificationPreferencesRepository, UserNotificationPreferencesRepository>();
        services.AddScoped<IPushNotificationTokenRepository, UserPushNotificationTokenRepository>();

        // Register notification services (delegates to the Notifications and Realtime modules)
        services.AddCoreNotificationService();
        services.AddFirebasePushNotifications();
        services.AddInAppNotificationService();

        // Register SignalR hub (if not already registered)
        services.AddSignalR();

        return services;
    }
}
