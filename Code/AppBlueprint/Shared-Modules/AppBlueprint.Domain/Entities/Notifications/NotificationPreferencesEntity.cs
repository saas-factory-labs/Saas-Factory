using AppBlueprint.SharedKernel;

namespace AppBlueprint.Domain.Entities.Notifications;

/// <summary>
/// Represents user notification channel preferences within a tenant.
/// </summary>
public sealed class NotificationPreferencesEntity : BaseEntity, ITenantScoped
{
    public NotificationPreferencesEntity()
    {
        Id = PrefixedUlid.Generate("ntp");
        UserId = string.Empty;
        TenantId = string.Empty;
        EmailEnabled = true;
        InAppEnabled = true;
        PushEnabled = true;
        SmsEnabled = false;
    }

    private NotificationPreferencesEntity(
        string tenantId,
        string userId,
        bool emailEnabled,
        bool inAppEnabled,
        bool pushEnabled,
        bool smsEnabled) : this()
    {
        TenantId = tenantId;
        UserId = userId;
        EmailEnabled = emailEnabled;
        InAppEnabled = inAppEnabled;
        PushEnabled = pushEnabled;
        SmsEnabled = smsEnabled;
    }

    public string TenantId { get; set; }
    public string UserId { get; set; }
    public bool EmailEnabled { get; set; }
    public bool InAppEnabled { get; set; }
    public bool PushEnabled { get; set; }
    public bool SmsEnabled { get; set; }
    public TimeSpan? QuietHoursStart { get; set; }
    public TimeSpan? QuietHoursEnd { get; set; }

    /// <summary>
    /// Creates default notification preferences for a user.
    /// </summary>
    public static NotificationPreferencesEntity CreateDefault(string tenantId, string userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tenantId);
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        return new NotificationPreferencesEntity(tenantId, userId, true, true, true, false);
    }

    /// <summary>
    /// Updates email notification preference.
    /// </summary>
    public void UpdateEmailPreference(bool enabled)
    {
        EmailEnabled = enabled;
        LastUpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates in-app notification preference.
    /// </summary>
    public void UpdateInAppPreference(bool enabled)
    {
        InAppEnabled = enabled;
        LastUpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates push notification preference.
    /// </summary>
    public void UpdatePushPreference(bool enabled)
    {
        PushEnabled = enabled;
        LastUpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates SMS notification preference.
    /// </summary>
    public void UpdateSmsPreference(bool enabled)
    {
        SmsEnabled = enabled;
        LastUpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets quiet hours for notifications.
    /// </summary>
    public void SetQuietHours(TimeSpan start, TimeSpan end)
    {
        QuietHoursStart = start;
        QuietHoursEnd = end;
        LastUpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Clears quiet hours.
    /// </summary>
    public void ClearQuietHours()
    {
        QuietHoursStart = null;
        QuietHoursEnd = null;
        LastUpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Checks if the current time is within quiet hours.
    /// </summary>
    public bool IsInQuietHours()
    {
        if (QuietHoursStart == null || QuietHoursEnd == null)
            return false;

        TimeSpan currentTime = DateTime.Now.TimeOfDay;

        if (QuietHoursStart < QuietHoursEnd)
        {
            return currentTime >= QuietHoursStart && currentTime <= QuietHoursEnd;
        }
        else
        {
            return currentTime >= QuietHoursStart || currentTime <= QuietHoursEnd;
        }
    }
}
