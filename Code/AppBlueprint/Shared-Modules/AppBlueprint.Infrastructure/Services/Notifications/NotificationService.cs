using AppBlueprint.Application.Interfaces;
using AppBlueprint.Domain.Entities.Notifications;
using AppBlueprint.Domain.Interfaces.Repositories;

namespace AppBlueprint.Infrastructure.Services.Notifications;

/// <summary>
/// Main notification service that coordinates multi-channel delivery.
/// </summary>
public sealed class NotificationService(
    INotificationRepository notificationRepository,
    INotificationPreferencesRepository preferencesRepository,
    IInAppNotificationService inAppService,
    IPushNotificationService pushService) : INotificationService
{
    public async Task SendAsync(SendNotificationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        Console.WriteLine($"[NotificationService] SendAsync - TenantId: '{request.TenantId}', UserId: '{request.UserId}', Channels: {request.Channels}");

        // Get user preferences
        NotificationPreferencesEntity? preferences = await preferencesRepository.GetByUserIdAsync(request.UserId);
        
        // Create default preferences if none exist
        if (preferences is null)
        {
            preferences = NotificationPreferencesEntity.CreateDefault(
                request.TenantId,
                request.UserId);
            await preferencesRepository.AddAsync(preferences);
        }

        // Check quiet hours
        if (preferences.IsInQuietHours())
        {
            return;
        }

        // Send to enabled channels
        List<Task> tasks = new();

        if (request.Channels.HasFlag(NotificationChannels.InApp) && preferences.InAppEnabled)
        {
            tasks.Add(inAppService.SendAsync(request));
        }

        if (request.Channels.HasFlag(NotificationChannels.Push) && preferences.PushEnabled)
        {
            tasks.Add(pushService.SendAsync(new PushNotificationRequest(
                request.UserId,
                request.Title,
                request.Message,
                ImageUrl: null,
                request.ActionUrl,
                Data: new Dictionary<string, string>
                {
                    ["type"] = request.Type.ToString()
                })));
        }

        await Task.WhenAll(tasks);
    }

    public async Task<IEnumerable<UserNotificationEntity>> GetUserNotificationsAsync(string userId, int count = 20)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        
        List<UserNotificationEntity> notifications = await notificationRepository.GetByUserIdAsync(userId);
        return notifications.Take(count);
    }

    public async Task<int> GetUnreadCountAsync(string userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        return await notificationRepository.CountUnreadAsync(userId);
    }

    public async Task MarkAsReadAsync(string notificationId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(notificationId);
        
        UserNotificationEntity? notification = await notificationRepository.GetByIdAsync(notificationId);
        if (notification is not null)
        {
            notification.MarkAsRead();
            await notificationRepository.UpdateAsync(notification);
        }
    }

    public async Task MarkAllAsReadAsync(string userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        await notificationRepository.MarkAllAsReadAsync(userId);
    }
}
