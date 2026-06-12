using AppBlueprint.Infrastructure.Persistence.DatabaseContexts.Baseline.Entities.Admin;

namespace AppBlueprint.Presentation.ApiModule.Repositories;

public interface IAdminRepository
{
    Task<IEnumerable<AdminEntity>> GetAllAsync();
    Task<AdminEntity?> GetByIdAsync(string id);
    Task AddAsync(AdminEntity admin);
}
