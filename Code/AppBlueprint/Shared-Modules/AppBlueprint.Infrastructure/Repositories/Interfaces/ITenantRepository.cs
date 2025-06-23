using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Tenant.Tenant;

namespace AppBlueprint.Infrastructure.Repositories.Interfaces;

public interface ITenantRepository
{
    Task<IEnumerable<TenantEntity>> GetAllAsync();
    Task<TenantEntity?> GetByIdAsync(string id);
    Task AddAsync(TenantEntity tenant);
    void Update(TenantEntity tenant);
    void Delete(string id);
}
