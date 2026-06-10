using DeploymentManager.ApiService.Domain.Entities;

namespace DeploymentManager.ApiService.Domain.Interfaces;

public interface IProjectRepository
{
    Task<ProjectEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProjectEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(ProjectEntity project, CancellationToken cancellationToken = default);
    Task UpdateAsync(ProjectEntity project, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
