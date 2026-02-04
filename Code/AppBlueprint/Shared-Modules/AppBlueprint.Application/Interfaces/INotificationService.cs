using AppBlueprint.Domain.Entities.Notifications;

namespace AppBlueprint.Application.Interfaces;

/// <summary>
/// Service for sending multi-channel notifications.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Sends a notification through specified channels.
    /// </summary>
    Task SendAsync(SendNotificationRequest request);

    /// <summary>
    /// Gets recent notifications for the current user.
    /// </summary>
    Task<IEnumerable<UserNotificationEntity>> GetUserNotificationsAsync(string userId, int count = 20);

    /// <summary>
    /// Gets unread notification count for the current user.
    /// </summary>
    Task<int> GetUnreadCountAsync(string userId);

    /// <summary>
    /// Marks a notification as read.
    /// </summary>
    Task MarkAsReadAsync(string notificationId);

    /// <summary>
    /// Marks all notifications as read for the current user.
    /// </summary>
    Task MarkAllAsReadAsync(string userId);
}

/// <summary>
/// Request to send a notification.
/// </summary>
public sealed record SendNotificationRequest(
    string TenantId,
    string UserId,
    string Title,
    string Message,
    NotificationType Type = NotificationType.Info,
    string? ActionUrl = null,
    NotificationChannels Channels = NotificationChannels.All
);

/// <summary>
/// Available notification channels.
/// </summary>
[Flags]
public enum NotificationChannels
{
    None = 0,
    InApp = 1,
    Email = 2,
    Push = 4,
    Sms = 8,
    All = InApp | Email | Push | Sms
}
