namespace AppBlueprint.Application.Interfaces;

/// <summary>
/// Service for sending real-time notifications via SignalR.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Sends a notification to all users in a specific tenant.
    /// </summary>
    /// <param name="tenantId">The tenant ID to send the notification to.</param>
    /// <param name="message">The notification message.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SendToTenantAsync(string tenantId, string message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a notification to a specific user.
    /// </summary>
    /// <param name="userId">The user ID to send the notification to.</param>
    /// <param name="message">The notification message.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SendToUserAsync(string userId, string message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a notification to all connected clients (use with caution - bypasses tenant isolation).
    /// </summary>
    /// <param name="message">The notification message.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SendToAllAsync(string message, CancellationToken cancellationToken = default);
}
