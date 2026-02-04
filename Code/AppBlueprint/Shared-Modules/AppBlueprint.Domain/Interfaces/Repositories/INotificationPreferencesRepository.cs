using AppBlueprint.Domain.Entities.Notifications;

namespace AppBlueprint.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for managing NotificationPreferences entities.
/// </summary>
public interface INotificationPreferencesRepository
{
    Task<NotificationPreferencesEntity?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task AddAsync(NotificationPreferencesEntity preferences, CancellationToken cancellationToken = default);
    Task UpdateAsync(NotificationPreferencesEntity preferences, CancellationToken cancellationToken = default);
    Task DeleteAsync(string userId, CancellationToken cancellationToken = default);
}
