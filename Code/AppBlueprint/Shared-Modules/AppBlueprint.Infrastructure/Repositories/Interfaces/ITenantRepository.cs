using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Tenant;

namespace AppBlueprint.Infrastructure.Repositories.Interfaces;

public interface ITenantRepository
{
    Task<IEnumerable<TenantEntity>> GetAllAsync();
    Task<TenantEntity?> GetByIdAsync(string id);
    Task AddAsync(TenantEntity tenant);
    void Update(TenantEntity tenant);
    void Delete(string id);
}
