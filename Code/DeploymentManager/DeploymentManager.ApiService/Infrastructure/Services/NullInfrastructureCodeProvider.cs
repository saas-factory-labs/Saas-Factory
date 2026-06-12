using DeploymentManager.ApiService.Domain.DTOs.Project;
using DeploymentManager.ApiService.Domain.Entities;
using DeploymentManager.ApiService.Domain.Interfaces;

namespace DeploymentManager.ApiService.Infrastructure.Services;

/// <summary>
/// Placeholder until PulumiAutomationApiService is implemented.
/// </summary>
public sealed class NullInfrastructureCodeProvider : IInfrastructureCodeProvider
{
    public Task<bool> CreateOrUpdateProject(ProjectRequestDto projectRequestDto)
        => Task.FromResult(true);

    public Task<bool> CreateOrUpdateProjectInfrastructure(ProjectEntity projectEntity)
        => Task.FromResult(true);

    public Task<bool> CreateOrUpdateProjectEnvironmentDatabase(ProjectEntity projectEntity)
        => Task.FromResult(true);

    public Task<bool> CreateOrUpdateContainerWorkload(string appName, string imageName)
        => Task.FromResult(true);

    public void Dispose() { }
}
