using AppBlueprint.Infrastructure.Persistence.DatabaseContexts.Baseline.Entities.Authorization.Role;

namespace AppBlueprint.Infrastructure.Persistence.Repositories.Interfaces;

public interface IRoleRepository
{
    Task<IEnumerable<RoleEntity>> GetAllAsync();
    Task<RoleEntity?> GetByIdAsync(string id);
    Task AddAsync(RoleEntity role);
    void Update(RoleEntity role);
    void Delete(string id);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
