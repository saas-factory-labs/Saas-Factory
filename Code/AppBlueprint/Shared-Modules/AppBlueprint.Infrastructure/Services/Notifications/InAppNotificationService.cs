using AppBlueprint.Application.Interfaces;
using AppBlueprint.Domain.Entities.Notifications;
using AppBlueprint.Domain.Interfaces.Repositories;
using AppBlueprint.Infrastructure.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace AppBlueprint.Infrastructure.Services.Notifications;

/// <summary>
/// Service for sending real-time in-app notifications via SignalR.
/// </summary>
public sealed class InAppNotificationService(
    INotificationRepository notificationRepository,
    IHubContext<NotificationHub> hubContext) : IInAppNotificationService
{
    public async Task SendAsync(SendNotificationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        Console.WriteLine($"[InAppNotificationService] SendAsync - TenantId: '{request.TenantId}', UserId: '{request.UserId}'");

        // Create and save notification
        UserNotificationEntity notification = UserNotificationEntity.Create(
            request.TenantId,
            request.UserId,
            request.Title,
            request.Message,
            request.Type,
            request.ActionUrl);

        await notificationRepository.AddAsync(notification);

        // Send real-time notification via SignalR
        await hubContext.Clients
            .Group($"user:{request.UserId}")
            .SendAsync("ReceiveNotification", new
            {
                id = notification.Id,
                title = notification.Title,
                message = notification.Message,
                type = notification.Type,
                actionUrl = notification.ActionUrl,
                timestamp = notification.CreatedAt
            });
    }
}
