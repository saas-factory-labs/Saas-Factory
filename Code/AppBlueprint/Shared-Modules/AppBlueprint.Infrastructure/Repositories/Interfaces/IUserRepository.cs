using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;

namespace AppBlueprint.Infrastructure.Repositories.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<UserEntity>> GetAllAsync();
    Task<UserEntity?> GetByIdAsync(string id);
    Task<UserEntity?> GetByEmailAsync(string? email);
    Task AddAsync(UserEntity user);
    void Update(UserEntity user);
    void Delete(string id);
}
