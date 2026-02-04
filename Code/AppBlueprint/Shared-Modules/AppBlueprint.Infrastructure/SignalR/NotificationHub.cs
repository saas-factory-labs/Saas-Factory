using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.SignalR;

/// <summary>
/// SignalR hub for real-time in-app notifications.
/// Inherits from TenantScopedHub for automatic tenant isolation and JWT authentication.
/// </summary>
// [Authorize] // Temporarily removed for debugging authentication
public sealed class NotificationHub : TenantScopedHub<NotificationHub>
{
    public NotificationHub(ILogger<NotificationHub> logger)
    {
        SetLogger(logger);
    }

    /// <summary>
    /// Sends a notification to a specific user in the current tenant.
    /// </summary>
    public async Task SendNotificationToUser(string userId, string title, string message, string type, string? actionUrl = null)
    {
        var notification = new {
            id = Guid.NewGuid().ToString(),
            title,
            message,
            type,
            actionUrl,
            timestamp = DateTime.UtcNow
        };

        await SendToUserAsync(userId, "ReceiveNotification", notification);
        Logger?.LogInformation("Sent notification to user {UserId} in tenant {TenantId}", userId, GetCurrentTenantId());
    }

    /// <summary>
    /// Sends a notification to all users in the current tenant.
    /// </summary>
    public async Task SendNotificationToTenant(string title, string message, string type, string? actionUrl = null)
    {
        var notification = new
        {
            id = Guid.NewGuid().ToString(),
            title,
            message,
            type,
            actionUrl,
            timestamp = DateTime.UtcNow
        };

        await SendToTenantAsync("ReceiveNotification", notification);
        Logger?.LogInformation("Sent broadcast notification to tenant {TenantId}", GetCurrentTenantId());
    }

    /// <summary>
    /// Marks a notification as read (client-side confirmation).
    /// </summary>
    public async Task MarkNotificationAsRead(string notificationId)
    {
        Logger?.LogDebug("User {UserId} marked notification {NotificationId} as read", GetCurrentUserId(), notificationId);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Called when a client connects to the hub.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        Logger?.LogInformation("User {UserId} connected to notifications hub (Tenant: {TenantId})", GetCurrentUserId(), GetCurrentTenantId());
    }

    /// <summary>
    /// Called when a client disconnects from the hub.
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
        if (exception != null)
        {
            Logger?.LogWarning(exception, "User {UserId} disconnected from notifications hub with error", GetCurrentUserId());
        }
        else
        {
            Logger?.LogInformation("User {UserId} disconnected from notifications hub", GetCurrentUserId());
        }
    }
}
