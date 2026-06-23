using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.Realtime.SignalR;

/// <summary>
/// SignalR hub for real-time in-app notifications.
/// Inherits from TenantScopedHub for automatic tenant isolation and JWT authentication.
/// </summary>
// [Authorize] // Temporarily removed for debugging authentication
public sealed class NotificationHub : TenantScopedHub<NotificationHub>
{
    public const string HubPath = "/hubs/notifications";

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        SetLogger(logger);
    }

    /// <summary>
    /// Sends a notification to a specific user in the current tenant.
    /// </summary>
    public async Task SendNotificationToUser(string userId, string title, string message, string type, Uri? actionUrl = null)
    {
        var notification = new
        {
            id = Guid.NewGuid().ToString(),
            title,
            message,
            type,
            actionUrl = actionUrl?.ToString(),
            timestamp = DateTime.UtcNow
        };

        await SendToUserAsync(userId, "ReceiveNotification", notification);
        Logger?.LogInformation("Sent tenant-scoped notification to a user");
    }

    /// <summary>
    /// Sends a notification to all users in the current tenant.
    /// </summary>
    public async Task SendNotificationToTenant(string title, string message, string type, Uri? actionUrl = null)
    {
        var notification = new
        {
            id = Guid.NewGuid().ToString(),
            title,
            message,
            type,
            actionUrl = actionUrl?.ToString(),
            timestamp = DateTime.UtcNow
        };

        await SendToTenantAsync("ReceiveNotification", notification);
        Logger?.LogInformation("Sent tenant-scoped broadcast notification");
    }

    /// <summary>
    /// Marks a notification as read (client-side confirmation).
    /// </summary>
    public async Task MarkNotificationAsRead(string notificationId)
    {
        Logger?.LogDebug("Notification marked as read");
        await Task.CompletedTask;
    }

    /// <summary>
    /// Called when a client connects to the hub.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        Logger?.LogInformation("Notifications hub connection established");
    }

    /// <summary>
    /// Called when a client disconnects from the hub.
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
        if (exception != null)
        {
            Logger?.LogWarning(exception, "Notifications hub disconnected with error");
        }
        else
        {
            Logger?.LogInformation("Notifications hub connection closed");
        }
    }
}
