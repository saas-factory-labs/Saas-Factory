using AppBlueprint.SharedKernel;

namespace AppBlueprint.Domain.Entities.User;

/// <summary>
/// Domain entity representing email verification tokens
/// </summary>
public class EmailVerificationEntity : BaseEntity
{
    public required string Token { get; set; }
    public required string Email { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserEntityId => UserId; // Alias for backward compatibility
    public DateTime ExpiresAt { get; set; }
    public DateTime ExpireAt => ExpiresAt; // Alias for backward compatibility
    public bool HasBeenVerified { get; set; }
    public bool HasBeenOpened { get; set; }

    // Navigation property
    public UserEntity? User { get; set; }
}
