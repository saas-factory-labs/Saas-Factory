using AppBlueprint.Application.Interfaces;
using AppBlueprint.Domain.Entities.Notifications;
using AppBlueprint.Domain.Interfaces.Repositories;
using NotificationHub = AppBlueprint.Infrastructure.SignalR.NotificationHub; // Use the correct NotificationHub
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.Services.Notifications;

/// <summary>
/// Multi-channel notification service that orchestrates sending notifications across various channels.
/// </summary>
public sealed class MultiChannelNotificationService : IMultiChannelNotificationService
{
    private readonly ILogger<MultiChannelNotificationService> _logger;
    private readonly INotificationRepository _notificationRepository;
    private readonly IPushNotificationService _pushService;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly INotificationService _inAppNotificationService;
    private readonly IPushNotificationTokenRepository _pushNotificationTokenRepository; // New dependency

    public MultiChannelNotificationService(
        ILogger<MultiChannelNotificationService> logger,
        INotificationRepository notificationRepository,
        IPushNotificationService pushService,
        IHubContext<NotificationHub> hubContext,
        INotificationService inAppNotificationService,
        IPushNotificationTokenRepository pushNotificationTokenRepository) // Inject new dependency
    {
        _logger = logger;
        _notificationRepository = notificationRepository;
        _pushService = pushService;
        _hubContext = hubContext;
        _inAppNotificationService = inAppNotificationService;
        _pushNotificationTokenRepository = pushNotificationTokenRepository;
    }

    public async Task SendNotificationAsync(
        string tenantId,
        string userId,
        string title,
        string message,
        NotificationType type = NotificationType.Info,
        NotificationChannels channels = NotificationChannels.All,
        Uri? actionUrl = null,
        Dictionary<string, string>? data = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Sending notification to user {UserId} in tenant {TenantId} via channels {Channels}: {Title}",
            userId, tenantId, channels, title);

        List<Task> tasks = new();

        // 1. In-App Database Storage (always store for history and sends SignalR real-time)
        if (channels.HasFlag(NotificationChannels.InApp))
        {
            tasks.Add(SendInAppNotificationAsync(tenantId, userId, title, message, type, actionUrl, cancellationToken));
        }

        // 2. Push Notification (Firebase Cloud Messaging)
        if (channels.HasFlag(NotificationChannels.Push))
        {
            tasks.Add(SendPushNotificationAsync(userId, title, message, data, cancellationToken));
        }

        // 4. Email (if implemented and requested)
        if (channels.HasFlag(NotificationChannels.Email))
        {
            // TODO: Integrate with email service when needed
            _logger.LogDebug("Email notifications not yet implemented");
        }

        // 5. SMS (if implemented and requested)
        if (channels.HasFlag(NotificationChannels.Sms))
        {
            // TODO: Integrate with SMS service when needed
            _logger.LogDebug("SMS notifications not yet implemented");
        }

        await Task.WhenAll(tasks);
        _logger.LogInformation("Successfully sent notification to user {UserId} via {Count} channels", userId, tasks.Count);
    }

    public async Task SendTenantNotificationAsync(
        string tenantId,
        string title,
        string message,
        NotificationType type = NotificationType.Info,
        NotificationChannels channels = NotificationChannels.All,
        Uri? actionUrl = null,
        Dictionary<string, string>? data = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Sending broadcast notification to tenant {TenantId} via channels {Channels}: {Title}",
            tenantId, channels, title);

        List<Task> tasks = new();

        // SignalR broadcast to all tenant users currently online
        if (channels.HasFlag(NotificationChannels.InApp))
        {
            tasks.Add(SendSignalRTenantNotificationAsync(tenantId, title, message, type.ToString(), actionUrl, cancellationToken));
        }

        // Push notification to all active device tokens in tenant
        if (channels.HasFlag(NotificationChannels.Push))
        {
            tasks.Add(SendTenantPushNotificationAsync(tenantId, title, message, data, cancellationToken));
        }

        await Task.WhenAll(tasks);
        _logger.LogInformation("Successfully sent tenant notification to {TenantId}", tenantId);
    }

    public async Task<NotificationStats> GetUserStatsAsync(string userId, CancellationToken cancellationToken = default)
    {
        List<UserNotificationEntity> notifications = await _notificationRepository.GetByUserIdAsync(userId, cancellationToken);
        int unreadCount = await _notificationRepository.CountUnreadAsync(userId, cancellationToken);
        List<PushNotificationTokenEntity> activeTokens = await _pushNotificationTokenRepository.GetActiveByUserIdAsync(userId, cancellationToken); // Use push token repository

        return new NotificationStats(
            TotalCount: notifications.Count,
            UnreadCount: unreadCount,
            ReadCount: notifications.Count - unreadCount,
            ActiveDeviceTokens: activeTokens.Count
        );
    }

    private async Task SendInAppNotificationAsync(
        string tenantId,
        string userId,
        string title,
        string message,
        NotificationType type,
        Uri? actionUrl,
        CancellationToken cancellationToken)
    {
        try
        {
            await _inAppNotificationService.SendAsync(new SendNotificationRequest(
                TenantId: tenantId,
                UserId: userId,
                Title: title,
                Message: message,
                Type: type,
                ActionUrl: actionUrl,
                Channels: NotificationChannels.InApp
            ));
            _logger.LogDebug("Saved in-app notification for user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save in-app notification for user {UserId}", userId);
        }
    }

    private async Task SendSignalRNotificationAsync(
        string userId,
        string title,
        string message,
        string type,
        Uri? actionUrl,
        CancellationToken cancellationToken)
    {
        try
        {
            object notification = new
            {
                title,
                message,
                type,
                actionUrl = actionUrl?.ToString(),
                timestamp = DateTime.UtcNow
            };

            await _hubContext.Clients.Group($"user:{userId}").SendAsync("ReceiveNotification", notification, cancellationToken);
            _logger.LogDebug("Sent SignalR notification to user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send SignalR notification to user {UserId}", userId);
        }
    }

    private async Task SendSignalRTenantNotificationAsync(
        string tenantId,
        string title,
        string message,
        string type,
        Uri? actionUrl,
        CancellationToken cancellationToken)
    {
        try
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

            // Note: This sends to all users in SignalR groups, would need tenant group management
            await _hubContext.Clients.Group($"tenant:{tenantId}").SendAsync("ReceiveNotification", notification, cancellationToken);
            _logger.LogDebug("Sent SignalR broadcast to tenant {TenantId}", tenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send SignalR tenant broadcast to {TenantId}", tenantId);
        }
    }

    private async Task SendPushNotificationAsync(
        string userId,
        string title,
        string message,
        Dictionary<string, string>? data,
        CancellationToken cancellationToken)
    {
        try
        {
            await _pushService.SendAsync(new PushNotificationRequest(
                UserId: userId,
                Title: title,
                Body: message,
                Data: data
            ));
            _logger.LogDebug("Sent push notification to user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send push notification to user {UserId}", userId);
        }
    }

    private async Task SendTenantPushNotificationAsync(
        string tenantId,
        string title,
        string message,
        Dictionary<string, string>? data,
        CancellationToken cancellationToken)
    {
        try
        {
            int sentCount = await _pushService.SendToTenantAsync(tenantId, title, message, data, cancellationToken);
            _logger.LogDebug("Sent push notification to {Count} devices in tenant {TenantId}", sentCount, tenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send push notification to tenant {TenantId}", tenantId);
        }
    }
}
