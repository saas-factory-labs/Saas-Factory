using AppBlueprint.Domain.Entities.Notifications;

namespace AppBlueprint.Application.Interfaces;

/// <summary>
/// Service for push notifications (Firebase Cloud Messaging).
/// </summary>
public interface IPushNotificationService
{
    /// <summary>
    /// Sends a push notification to all user devices.
    /// </summary>
    Task SendAsync(PushNotificationRequest request);
    
    /// <summary>
    /// Sends a push notification to all users in a tenant.
    /// </summary>
    Task<int> SendToTenantAsync(string tenantId, string title, string body, Dictionary<string, string>? data = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a push notification token for the current user.
    /// </summary>
    Task RegisterTokenAsync(RegisterPushTokenRequest request);

    /// <summary>
    /// Unregisters a push notification token.
    /// </summary>
    Task UnregisterTokenAsync(string token);
}

/// <summary>
/// Request to send a push notification.
/// </summary>
public sealed record PushNotificationRequest(
    string UserId,
    string Title,
    string Body,
    Uri? ImageUrl = null,
    Uri? ActionUrl = null,
    Dictionary<string, string>? Data = null
);

/// <summary>
/// Request to register a push notification token.
/// </summary>
public sealed record RegisterPushTokenRequest(
    string TenantId,
    string UserId,
    string Token,
    DeviceType Platform
);
