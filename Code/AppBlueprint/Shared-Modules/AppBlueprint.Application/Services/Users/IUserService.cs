using AppBlueprint.Domain.Entities.User;

namespace AppBlueprint.Application.Services.Users;

public interface IUserService
{
    Task<UserEntity> RegisterAsync(string firstName, string lastName, string email, string userName, CancellationToken cancellationToken);
    Task<UserEntity?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task UpdateProfileAsync(string userId, string firstName, string lastName, string? phoneNumber, string? bio, CancellationToken cancellationToken);
    Task DeactivateUserAsync(string userId, CancellationToken cancellationToken);

    // Email verification
    Task<string> GenerateEmailVerificationTokenAsync(string userId, CancellationToken cancellationToken);
    Task<bool> VerifyEmailAsync(string userId, string token, CancellationToken cancellationToken);

    // Password reset
    Task<string> InitiatePasswordResetAsync(string email, CancellationToken cancellationToken);
    Task<bool> CompletePasswordResetAsync(string email, string token, string newPassword, CancellationToken cancellationToken);
}
