using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Admin;

namespace AppBlueprint.Infrastructure.Repositories.Interfaces;

public interface IAdminRepository
{
    Task<IEnumerable<AdminEntity>> GetAllAsync();
    Task<AdminEntity> GetByIdAsync(int id);
    Task AddAsync(AdminEntity admin);
    void Update(AdminEntity admin);
    void Delete(int id);
}
