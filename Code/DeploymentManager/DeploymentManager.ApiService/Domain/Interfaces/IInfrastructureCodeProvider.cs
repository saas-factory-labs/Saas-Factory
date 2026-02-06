using DeploymentManager.ApiService.Domain.DTOs.Project;
using DeploymentManager.ApiService.Domain.Entities;

namespace DeploymentManager.ApiService.Domain.Interfaces;

public interface IInfrastructureCodeProvider : IDisposable
{
    public Task<bool> CreateOrUpdateProject(ProjectRequestDto projectRequestDto);

    public Task<bool> CreateOrUpdateProjectInfrastructure(ProjectEntity projectEntity);

    public Task<bool> CreateOrUpdateProjectEnvironmentDatabase(ProjectEntity projectEntity);

    public Task<bool> CreateOrUpdateContainerWorkload(string appName, string imageName);
}
