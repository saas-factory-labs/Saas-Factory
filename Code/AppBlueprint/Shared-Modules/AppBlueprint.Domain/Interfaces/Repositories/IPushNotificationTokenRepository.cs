using AppBlueprint.Domain.Entities.Notifications;

namespace AppBlueprint.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for managing PushNotificationToken entities.
/// </summary>
public interface IPushNotificationTokenRepository
{
    Task<PushNotificationTokenEntity?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<List<PushNotificationTokenEntity>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<List<PushNotificationTokenEntity>> GetActiveByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<List<PushNotificationTokenEntity>> GetActiveByTenantIdAsync(string tenantId, CancellationToken cancellationToken = default);
    Task<PushNotificationTokenEntity?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task AddAsync(PushNotificationTokenEntity token, CancellationToken cancellationToken = default);
    Task UpdateAsync(PushNotificationTokenEntity token, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task DeactivateByTokenAsync(string token, CancellationToken cancellationToken = default);
}
