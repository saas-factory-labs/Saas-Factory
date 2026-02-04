using AppBlueprint.SharedKernel;

namespace AppBlueprint.Domain.Entities.Notifications;

/// <summary>
/// Represents an in-app notification for a user within a tenant.
/// </summary>
public sealed class UserNotificationEntity : BaseEntity, ITenantScoped
{
    public UserNotificationEntity()
    {
        Id = PrefixedUlid.Generate("ntf");
        Title = string.Empty;
        Message = string.Empty;
        UserId = string.Empty;
        TenantId = string.Empty;
        Type = NotificationType.Info;
        IsRead = false;
        CreatedAt = DateTime.UtcNow;
    }

    private UserNotificationEntity(
        string tenantId,
        string userId,
        string title,
        string message,
        NotificationType type,
        string? actionUrl) : this()
    {
        TenantId = tenantId;
        UserId = userId;
        Title = title;
        Message = message;
        Type = type;
        ActionUrl = actionUrl;
    }

    public string TenantId { get; set; }
    public string UserId { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public NotificationType Type { get; set; }
    public string? ActionUrl { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// Creates a new notification for a user.
    /// </summary>
    public static UserNotificationEntity Create(
        string tenantId,
        string userId,
        string title,
        string message,
        NotificationType type = NotificationType.Info,
        string? actionUrl = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tenantId);
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        return new UserNotificationEntity(tenantId, userId, title, message, type, actionUrl);
    }

    /// <summary>
    /// Marks the notification as read.
    /// </summary>
    public void MarkAsRead()
    {
        if (!IsRead)
        {
            IsRead = true;
            ReadAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Marks the notification as unread.
    /// </summary>
    public void MarkAsUnread()
    {
        if (IsRead)
        {
            IsRead = false;
            ReadAt = null;
        }
    }
}
