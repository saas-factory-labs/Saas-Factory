using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization.Permission;

namespace AppBlueprint.Infrastructure.Repositories.Interfaces;

public interface IPermissionRepository
{
    Task<IEnumerable<PermissionEntity>> GetAllAsync(); Task<PermissionEntity?> GetByIdAsync(string id);
    Task AddAsync(PermissionEntity permission);
    void Update(PermissionEntity permission);
    void Delete(string id);
}
