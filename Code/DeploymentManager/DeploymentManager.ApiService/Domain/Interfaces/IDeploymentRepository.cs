using DeploymentManager.ApiService.Domain.Entities;

namespace DeploymentManager.ApiService.Domain.Interfaces;

public interface IDeploymentRepository
{
    Task<DeploymentEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<DeploymentEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(DeploymentEntity deployment, CancellationToken cancellationToken = default);
    Task UpdateAsync(DeploymentEntity deployment, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
