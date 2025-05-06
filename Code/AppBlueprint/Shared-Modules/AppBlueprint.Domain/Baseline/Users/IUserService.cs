using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;

namespace AppBlueprint.Domain.Baseline.Users;

public interface IUserService
{
    Task<UserEntity> RegisterAsync(string firstName, string lastName, string email, string userName, CancellationToken cancellationToken);
    Task<UserEntity> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<UserEntity> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task UpdateProfileAsync(int userId, string firstName, string lastName, string? phoneNumber, string? bio, CancellationToken cancellationToken);
    Task DeactivateUserAsync(int userId, CancellationToken cancellationToken);
    
    // Email verification
    Task<string> GenerateEmailVerificationTokenAsync(int userId, CancellationToken cancellationToken);
    Task<bool> VerifyEmailAsync(int userId, string token, CancellationToken cancellationToken);
    
    // Password reset
    Task<string> InitiatePasswordResetAsync(string email, CancellationToken cancellationToken);
    Task<bool> CompletePasswordResetAsync(string email, string token, string newPassword, CancellationToken cancellationToken);
}