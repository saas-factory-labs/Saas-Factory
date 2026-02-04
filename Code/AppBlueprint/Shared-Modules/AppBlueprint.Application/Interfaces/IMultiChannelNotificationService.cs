using AppBlueprint.Domain.Entities.Notifications;

namespace AppBlueprint.Application.Interfaces;

/// <summary>
/// Unified notification service that orchestrates sending notifications across multiple channels.
/// </summary>
public interface IMultiChannelNotificationService
{
    /// <summary>
    /// Sends a notification to a user across specified channels.
    /// </summary>
    Task SendNotificationAsync(
        string tenantId,
        string userId,
        string title,
        string message,
        NotificationType type = NotificationType.Info,
        NotificationChannels channels = NotificationChannels.All,
        string? actionUrl = null,
        Dictionary<string, string>? data = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a notification to all users in a tenant across specified channels.
    /// </summary>
    Task SendTenantNotificationAsync(
        string tenantId,
        string title,
        string message,
        NotificationType type = NotificationType.Info,
        NotificationChannels channels = NotificationChannels.All,
        string? actionUrl = null,
        Dictionary<string, string>? data = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets notification statistics for a user.
    /// </summary>
    Task<NotificationStats> GetUserStatsAsync(string userId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Notification statistics.
/// </summary>
public record NotificationStats(
    int TotalCount,
    int UnreadCount,
    int ReadCount,
    int ActiveDeviceTokens
);
