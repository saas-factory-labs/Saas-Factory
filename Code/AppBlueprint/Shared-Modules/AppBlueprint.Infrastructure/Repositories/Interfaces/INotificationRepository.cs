using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;

namespace AppBlueprint.Infrastructure.Repositories.Interfaces;

public interface INotificationRepository
{
    Task<IEnumerable<NotificationEntity>> GetAllAsync();
    Task<NotificationEntity?> GetByIdAsync(string id);
    Task AddAsync(NotificationEntity notification);
    void Update(NotificationEntity notification);
    void Delete(int id);
}
