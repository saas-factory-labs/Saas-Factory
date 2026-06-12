using AppBlueprint.Application.Services;
using AppBlueprint.SharedKernel.Enums;

namespace DeploymentManager.Web.Services;

internal sealed class NullCurrentTenantService : ICurrentTenantService
{
    public string? TenantId => null;
    public Task<string?> GetTenantNameAsync() => Task.FromResult<string?>(null);
    public Task<TenantType?> GetTenantTypeAsync() => Task.FromResult<TenantType?>(null);
}
