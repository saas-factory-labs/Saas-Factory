namespace AppBlueprint.Application.Interfaces;

/// <summary>
/// Service for in-app notifications (database + SignalR).
/// </summary>
public interface IInAppNotificationService
{
    /// <summary>
    /// Sends an in-app notification (stores in DB and broadcasts via SignalR).
    /// </summary>
    Task SendAsync(SendNotificationRequest request);
}
