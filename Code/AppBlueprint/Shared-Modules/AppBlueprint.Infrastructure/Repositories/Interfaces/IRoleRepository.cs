using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization;

namespace AppBlueprint.Infrastructure.Repositories.Interfaces;

public interface IRoleRepository
{
    Task<IEnumerable<RoleEntity>> GetAllAsync();
    Task<RoleEntity> GetByIdAsync(int id);
    Task AddAsync(RoleEntity role);
    void Update(RoleEntity role);
    void Delete(int id);
}
