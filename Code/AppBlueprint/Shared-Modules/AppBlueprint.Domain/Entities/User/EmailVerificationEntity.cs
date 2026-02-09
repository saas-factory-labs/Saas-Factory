using AppBlueprint.SharedKernel;

namespace AppBlueprint.Domain.Entities.User;

/// <summary>
/// Domain entity representing email verification tokens
/// </summary>
public class EmailVerificationEntity : BaseEntity
{
    public required string Token { get; init; }
    public required string Email { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string UserEntityId => UserId; // Alias for backward compatibility
    public DateTime ExpiresAt { get; init; }
    public DateTime ExpireAt => ExpiresAt; // Alias for backward compatibility
    public bool HasBeenVerified { get; set; }
    public bool HasBeenOpened { get; set; }

    // Navigation property
    public UserEntity? User { get; set; }
}
