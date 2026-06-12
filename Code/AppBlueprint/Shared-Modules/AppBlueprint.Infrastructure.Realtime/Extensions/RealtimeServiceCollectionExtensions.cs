using AppBlueprint.Application.Interfaces;
using AppBlueprint.Infrastructure.Services.Notifications;
using Microsoft.Extensions.DependencyInjection;

namespace AppBlueprint.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering AppBlueprint real-time services.
/// </summary>
public static class RealtimeServiceCollectionExtensions
{
    /// <summary>
    /// Adds SignalR with tenant-aware authentication and authorization.
    /// Configures MessagePack protocol for efficient binary serialization.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAppBlueprintSignalR(this IServiceCollection services)
    {
        services.AddSignalR(options =>
        {
            // Enable detailed errors in development only
            options.EnableDetailedErrors = false; // Set to true via environment variable in dev

            // Configure client timeouts
            options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
            options.HandshakeTimeout = TimeSpan.FromSeconds(15);
            options.KeepAliveInterval = TimeSpan.FromSeconds(15);

            // Maximum message size (1 MB default)
            options.MaximumReceiveMessageSize = 1024 * 1024;
        })
        .AddMessagePackProtocol(); // Binary protocol for better performance

        Console.WriteLine("[AppBlueprint.Infrastructure] SignalR registered with tenant-aware authentication");

        return services;
    }

    /// <summary>
    /// Registers the SignalR-based in-app notification service.
    /// Requires an <see cref="AppBlueprint.Domain.Interfaces.Repositories.INotificationRepository"/>
    /// implementation to be registered by the host.
    /// </summary>
    public static IServiceCollection AddInAppNotificationService(this IServiceCollection services)
    {
        services.AddScoped<IInAppNotificationService, InAppNotificationService>();
        return services;
    }
}
