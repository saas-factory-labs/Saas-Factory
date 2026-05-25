using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.Hubs;

/// <summary>
/// SignalR hub for real-time notifications with tenant isolation support.
/// </summary>
[Authorize]
public sealed class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        _logger = logger;
    }

    /// <summary>
    /// Called when a client connects to the hub. Automatically adds the connection to the tenant-specific group.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        string? tenantId = Context.User?.FindFirst("tenant_id")?.Value;
        string connectionId = Context.ConnectionId;

        _logger.LogInformation("Client connected: ConnectionId={ConnectionId}, TenantId={TenantId}", connectionId, tenantId);

        // Add connection to tenant-specific group for tenant isolation
        if (!string.IsNullOrEmpty(tenantId))
        {
            await Groups.AddToGroupAsync(connectionId, $"tenant-{tenantId}");
            _logger.LogInformation("Added connection {ConnectionId} to tenant group {TenantId}", connectionId, tenantId);
        }

        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Called when a client disconnects from the hub.
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string connectionId = Context.ConnectionId;

        if (exception is not null)
        {
            _logger.LogError(exception, "Client disconnected with error: ConnectionId={ConnectionId}", connectionId);
        }
        else
        {
            _logger.LogInformation("Client disconnected: ConnectionId={ConnectionId}", connectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Sends a notification to all clients in the caller's tenant.
    /// </summary>
    /// <param name="message">The notification message to broadcast.</param>
    /// <exception cref="HubException">Thrown when tenant ID is not found in user claims.</exception>
    public async Task SendNotificationToTenant(string message)
    {
        ArgumentNullException.ThrowIfNull(message);

        string? tenantId = Context.User?.FindFirst("tenant_id")?.Value;

        if (string.IsNullOrEmpty(tenantId))
        {
            throw new HubException("Tenant ID not found in user claims.");
        }

        _logger.LogInformation("Broadcasting message to tenant {TenantId}: {Message}", tenantId, string.Concat(message.AsSpan(0, Math.Min(50, message.Length)), "..."));

        // Send to all connections in the tenant group
        await Clients.Group($"tenant-{tenantId}").SendAsync("ReceiveNotification", message);
    }

    /// <summary>
    /// Sends a notification to a specific user within the caller's tenant.
    /// </summary>
    /// <param name="userId">The ID of the user to send the notification to.</param>
    /// <param name="message">The notification message.</param>
    /// <exception cref="HubException">Thrown when tenant ID is not found in user claims.</exception>
    public async Task SendNotificationToUser(string userId, string message)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(message);

        string? tenantId = Context.User?.FindFirst("tenant_id")?.Value;

        if (string.IsNullOrEmpty(tenantId))
        {
            throw new HubException("Tenant ID not found in user claims.");
        }

        _logger.LogInformation("Sending message to user {UserId} in tenant {TenantId}", userId, tenantId);

        // Send to specific user
        await Clients.User(userId).SendAsync("ReceiveNotification", message);
    }
}
