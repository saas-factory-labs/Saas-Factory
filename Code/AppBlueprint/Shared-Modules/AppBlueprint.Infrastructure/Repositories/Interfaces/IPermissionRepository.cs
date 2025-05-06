using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;

namespace AppBlueprint.Infrastructure.Repositories.Interfaces;

public interface IPermissionRepository
{
    Task<IEnumerable<PermissionEntity>> GetAllAsync();
    Task<PermissionEntity> GetByIdAsync(int id);
    Task AddAsync(PermissionEntity permission);
    void Update(PermissionEntity permission);
    void Delete(int id);
}
