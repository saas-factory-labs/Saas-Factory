using AppBlueprint.Infrastructure.DatabaseContexts.TenantCatalog.Entities;

namespace AppBlueprint.Infrastructure.Repositories.Interfaces;

public interface IAppProjectRepository
{
    Task<IEnumerable<AppProjectEntity>> GetAllAsync(CancellationToken cancellationToken);
    Task<AppProjectEntity> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task AddAsync(AppProjectEntity account, CancellationToken cancellationToken);
    Task UpdateAsync(AppProjectEntity account, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
