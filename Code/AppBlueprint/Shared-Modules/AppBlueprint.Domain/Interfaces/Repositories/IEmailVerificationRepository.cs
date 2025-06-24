using AppBlueprint.Domain.Entities.User;

namespace AppBlueprint.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for EmailVerification entity operations
/// </summary>
public interface IEmailVerificationRepository
{
    Task<EmailVerificationEntity?> GetByUserIdAndTokenAsync(string userId, string token, CancellationToken cancellationToken = default);
    Task<EmailVerificationEntity> CreateAsync(EmailVerificationEntity emailVerification, CancellationToken cancellationToken = default);
    Task<EmailVerificationEntity> AddAsync(EmailVerificationEntity emailVerification, CancellationToken cancellationToken = default);
    Task<EmailVerificationEntity> UpdateAsync(EmailVerificationEntity emailVerification, CancellationToken cancellationToken = default);
    void Update(EmailVerificationEntity emailVerification);
}
