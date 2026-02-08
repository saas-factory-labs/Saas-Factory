using DeploymentManager.ApiService.Domain.DTOs.Project;
using DeploymentManager.ApiService.Domain.Entities;

namespace DeploymentManager.ApiService.Domain.Interfaces;

public interface IInfrastructureCodeProvider : IDisposable
{
    Task<bool> CreateOrUpdateProject(ProjectRequestDto projectRequestDto);

    Task<bool> CreateOrUpdateProjectInfrastructure(ProjectEntity projectEntity);

    Task<bool> CreateOrUpdateProjectEnvironmentDatabase(ProjectEntity projectEntity);

    Task<bool> CreateOrUpdateContainerWorkload(string appName, string imageName);
}
