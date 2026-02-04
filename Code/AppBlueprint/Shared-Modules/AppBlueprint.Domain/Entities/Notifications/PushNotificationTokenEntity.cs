using AppBlueprint.SharedKernel;

namespace AppBlueprint.Domain.Entities.Notifications;

/// <summary>
/// Represents a push notification token (FCM token) for a user device.
/// </summary>
public sealed class PushNotificationTokenEntity : BaseEntity, ITenantScoped
{
    public PushNotificationTokenEntity()
    {
        Id = PrefixedUlid.Generate("ptk");
        UserId = string.Empty;
        TenantId = string.Empty;
        Token = string.Empty;
        DeviceType = DeviceType.Web;
        IsActive = true;
    }

    private PushNotificationTokenEntity(
        string tenantId,
        string userId,
        string token,
        DeviceType deviceType,
        string? deviceInfo) : this()
    {
        TenantId = tenantId;
        UserId = userId;
        Token = token;
        DeviceType = deviceType;
        DeviceInfo = deviceInfo;
        LastUsedAt = DateTime.UtcNow;
    }

    public string TenantId { get; set; }
    public string UserId { get; set; }
    public string Token { get; set; }
    public DeviceType DeviceType { get; set; }
    public string? DeviceInfo { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastUsedAt { get; set; }

    /// <summary>
    /// Creates a new push notification token.
    /// </summary>
    public static PushNotificationTokenEntity Create(
        string tenantId,
        string userId,
        string token,
        DeviceType deviceType,
        string? deviceInfo = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tenantId);
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        return new PushNotificationTokenEntity(tenantId, userId, token, deviceType, deviceInfo);
    }

    /// <summary>
    /// Updates the last used timestamp.
    /// </summary>
    public void UpdateLastUsed()
    {
        LastUsedAt = DateTime.UtcNow;
        LastUpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates the token (e.g., when user logs out or uninstalls app).
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        LastUpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Reactivates the token.
    /// </summary>
    public void Reactivate()
    {
        IsActive = true;
        LastUsedAt = DateTime.UtcNow;
        LastUpdatedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Device type enumeration for push notifications.
/// </summary>
public enum DeviceType
{
    Web = 0,
    Android = 1,
    iOS = 2
}
