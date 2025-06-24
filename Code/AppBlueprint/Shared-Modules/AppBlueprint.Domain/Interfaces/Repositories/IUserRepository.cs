using AppBlueprint.Domain.Entities.User;

namespace AppBlueprint.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for User entity operations
/// </summary>
public interface IUserRepository
{
    Task<UserEntity?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserEntity> CreateAsync(UserEntity user, CancellationToken cancellationToken = default);
    Task<UserEntity> AddAsync(UserEntity user, CancellationToken cancellationToken = default);
    Task<UserEntity> UpdateAsync(UserEntity user, CancellationToken cancellationToken = default);
    void Update(UserEntity user);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
}
