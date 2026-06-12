using DeploymentManager.ApiService.Domain.Entities;

namespace DeploymentManager.ApiService.Domain.Interfaces;

public interface IProjectService
{
    Task<ProjectEntity> CreateProjectAsync(ProjectEntity project, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProjectEntity>> GetAllProjectsAsync(CancellationToken cancellationToken = default);
    Task<ProjectEntity?> GetProjectByIdAsync(int id, CancellationToken cancellationToken = default);
    Task UpdateProjectAsync(ProjectEntity project, CancellationToken cancellationToken = default);
    Task DeleteProjectAsync(int id, CancellationToken cancellationToken = default);
}
