using Domain.Entities;

namespace DeploymentPortal.ApiService.Domain.Interfaces;

public interface IProjectService
{
    //  ProjectDTO projectDto
    Task<ProjectEntity> CreateProjectAsync(ProjectEntity project);
    // Other operations related to projects
}
