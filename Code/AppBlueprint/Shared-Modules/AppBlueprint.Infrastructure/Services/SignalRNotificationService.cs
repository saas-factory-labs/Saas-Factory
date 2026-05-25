using AppBlueprint.Application.Interfaces;
using AppBlueprint.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.Services;

/// <summary>
/// SignalR-based implementation of the notification service for real-time communication.
/// </summary>
public sealed class SignalRNotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<SignalRNotificationService> _logger;

    public SignalRNotificationService(
        IHubContext<NotificationHub> hubContext,
        ILogger<SignalRNotificationService> logger)
    {
        ArgumentNullException.ThrowIfNull(hubContext);
        ArgumentNullException.ThrowIfNull(logger);

        _hubContext = hubContext;
        _logger = logger;
    }

    /// <summary>
    /// Sends a notification to all users in a specific tenant.
    /// </summary>
    public async Task SendToTenantAsync(string tenantId, string message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(tenantId);
        ArgumentNullException.ThrowIfNull(message);

        _logger.LogInformation("Sending notification to tenant {TenantId}: {MessagePreview}", 
            tenantId, 
            string.Concat(message.AsSpan(0, Math.Min(50, message.Length)), "..."));

        await _hubContext.Clients
            .Group($"tenant-{tenantId}")
            .SendAsync("ReceiveNotification", message, cancellationToken);
    }

    /// <summary>
    /// Sends a notification to a specific user.
    /// </summary>
    public async Task SendToUserAsync(string userId, string message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(message);

        _logger.LogInformation("Sending notification to user {UserId}: {MessagePreview}", 
            userId, 
            string.Concat(message.AsSpan(0, Math.Min(50, message.Length)), "..."));

        await _hubContext.Clients
            .User(userId)
            .SendAsync("ReceiveNotification", message, cancellationToken);
    }

    /// <summary>
    /// Sends a notification to all connected clients (bypasses tenant isolation - use with caution).
    /// </summary>
    public async Task SendToAllAsync(string message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        _logger.LogWarning("Broadcasting notification to ALL clients (bypassing tenant isolation): {MessagePreview}", 
            string.Concat(message.AsSpan(0, Math.Min(50, message.Length)), "..."));

        await _hubContext.Clients
            .All
            .SendAsync("ReceiveNotification", message, cancellationToken);
    }
}
