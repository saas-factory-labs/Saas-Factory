using AppBlueprint.Application.Interfaces;
using AppBlueprint.Infrastructure.Services.Notifications;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AppBlueprint.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering notification services.
/// </summary>
public static class NotificationServiceExtensions
{
    /// <summary>
    /// Adds Firebase Cloud Messaging notification services to the DI container.
    /// </summary>
    public static IServiceCollection AddFirebaseNotifications(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IPushNotificationService, FirebasePushNotificationService>();
        return services;
    }

    /// <summary>
    /// Adds multi-channel notification orchestration service to the DI container.
    /// </summary>
    public static IServiceCollection AddMultiChannelNotifications(
        this IServiceCollection services)
    {
        services.AddScoped<IMultiChannelNotificationService, MultiChannelNotificationService>();
        return services;
    }
}
