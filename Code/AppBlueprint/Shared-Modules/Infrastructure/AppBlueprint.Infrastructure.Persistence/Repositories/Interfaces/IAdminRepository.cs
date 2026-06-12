using AppBlueprint.Infrastructure.Persistence.DatabaseContexts.Baseline.Entities.Admin;

namespace AppBlueprint.Infrastructure.Persistence.Repositories.Interfaces;

public interface IAdminRepository
{
    Task<IEnumerable<AdminEntity>> GetAllAsync();
    Task<AdminEntity?> GetByIdAsync(string id);
    Task AddAsync(AdminEntity admin);
    void Update(AdminEntity admin);
    void Delete(int id);
}
