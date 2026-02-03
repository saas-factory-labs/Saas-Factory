using AppBlueprint.Domain.Entities.Notifications;

namespace AppBlueprint.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for managing UserNotification entities.
/// </summary>
public interface INotificationRepository
{
    Task<UserNotificationEntity?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<List<UserNotificationEntity>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<List<UserNotificationEntity>> GetUnreadByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task AddAsync(UserNotificationEntity notification, CancellationToken cancellationToken = default);
    Task UpdateAsync(UserNotificationEntity notification, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<int> CountUnreadAsync(string userId, CancellationToken cancellationToken = default);
    Task MarkAllAsReadAsync(string userId, CancellationToken cancellationToken = default);
}
