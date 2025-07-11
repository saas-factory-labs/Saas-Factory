using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;

namespace AppBlueprint.Infrastructure.Repositories.Interfaces;

public interface ISessionRepository
{
    Task<IEnumerable<SessionEntity>> GetAllAsync(CancellationToken cancellationToken); Task<SessionEntity?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<SessionEntity?> GetByKeyAsync(string sessionKey, CancellationToken cancellationToken);
    Task AddAsync(SessionEntity session, CancellationToken cancellationToken);
    Task UpdateAsync(SessionEntity session, CancellationToken cancellationToken);
    Task DeleteAsync(string id, CancellationToken cancellationToken);
    Task DeleteExpiredAsync(CancellationToken cancellationToken);
}
