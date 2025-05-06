using AppBlueprint.Infrastructure.DatabaseContexts.TenantCatalog.Entities;

namespace AppBlueprint.Infrastructure.Repositories.Interfaces;

public interface IAppProjectRepository
{
    public Task<IEnumerable<AppProjectEntity>> GetAllAsync(CancellationToken cancellationToken);
    public Task<AppProjectEntity> GetByIdAsync(int id, CancellationToken cancellationToken);
    public Task AddAsync(AppProjectEntity account, CancellationToken cancellationToken);
    public Task UpdateAsync(AppProjectEntity account, CancellationToken cancellationToken);
    public Task DeleteAsync(int id, CancellationToken cancellationToken);
}
