using DeploymentManager.ApiService.Domain.Entities;
using DeploymentManager.ApiService.Domain.Interfaces;

namespace DeploymentManager.ApiService.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProjectService(IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);
        _unitOfWork = unitOfWork;
    }

    public async Task<ProjectEntity> CreateProjectAsync(ProjectEntity project, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);
        await _unitOfWork.ProjectRepository.AddAsync(project, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return project;
    }

    public async Task<IEnumerable<ProjectEntity>> GetAllProjectsAsync(CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.ProjectRepository.GetAllAsync(cancellationToken);
    }

    public async Task<ProjectEntity?> GetProjectByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.ProjectRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task UpdateProjectAsync(ProjectEntity project, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);
        await _unitOfWork.ProjectRepository.UpdateAsync(project, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteProjectAsync(int id, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.ProjectRepository.DeleteAsync(id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
