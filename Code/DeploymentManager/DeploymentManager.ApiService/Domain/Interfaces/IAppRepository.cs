using DeploymentManager.ApiService.Domain.Entities;

namespace DeploymentManager.ApiService.Domain.Interfaces;

public interface IAppRepository
{
    Task<AppEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<AppEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(AppEntity app, CancellationToken cancellationToken = default);
    Task UpdateAsync(AppEntity app, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
