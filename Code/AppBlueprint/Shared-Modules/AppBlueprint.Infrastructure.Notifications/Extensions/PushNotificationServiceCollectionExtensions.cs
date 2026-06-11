using AppBlueprint.Application.Interfaces;
using AppBlueprint.Infrastructure.Services.Notifications;
using Microsoft.Extensions.DependencyInjection;

namespace AppBlueprint.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering AppBlueprint notification services.
/// </summary>
public static class PushNotificationServiceCollectionExtensions
{
    /// <summary>
    /// Registers Firebase Cloud Messaging as the push notification provider.
    /// Requires an <see cref="AppBlueprint.Domain.Interfaces.Repositories.IPushNotificationTokenRepository"/>
    /// implementation to be registered by the host.
    /// </summary>
    public static IServiceCollection AddFirebasePushNotifications(this IServiceCollection services)
    {
        services.AddScoped<IPushNotificationService, FirebasePushNotificationService>();
        return services;
    }

    /// <summary>
    /// Registers the core notification orchestration service.
    /// Requires notification repositories and an <see cref="IInAppNotificationService"/>
    /// implementation to be registered by the host.
    /// </summary>
    public static IServiceCollection AddCoreNotificationService(this IServiceCollection services)
    {
        services.AddScoped<INotificationService, NotificationService>();
        return services;
    }
}
