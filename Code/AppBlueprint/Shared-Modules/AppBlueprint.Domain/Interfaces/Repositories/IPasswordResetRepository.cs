using AppBlueprint.Domain.Entities.User;

namespace AppBlueprint.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for PasswordReset entity operations
/// </summary>
public interface IPasswordResetRepository
{
    Task<PasswordResetEntity?> GetByUserIdAndTokenAsync(string userId, string token, CancellationToken cancellationToken = default);
    Task<PasswordResetEntity> AddAsync(PasswordResetEntity passwordReset, CancellationToken cancellationToken = default);
    Task<PasswordResetEntity> UpdateAsync(PasswordResetEntity passwordReset, CancellationToken cancellationToken = default);
    void Update(PasswordResetEntity passwordReset);
}
