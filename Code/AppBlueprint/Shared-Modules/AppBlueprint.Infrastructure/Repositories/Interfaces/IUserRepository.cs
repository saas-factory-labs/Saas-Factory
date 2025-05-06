using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;

namespace AppBlueprint.Infrastructure.Repositories.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<UserEntity>> GetAllAsync();
    Task<UserEntity> GetByIdAsync(int id);
    Task<UserEntity> GetByEmailAsync(string? email);
    Task AddAsync(UserEntity user);
    void Update(UserEntity user);
    void Delete(int id);
}
