using AppBlueprint.SharedKernel;

namespace AppBlueprint.Domain.Entities.User;

/// <summary>
/// Entity representing a password reset request
/// </summary>
public class PasswordResetEntity : BaseEntity
{
    public required string Token { get; set; }
    public DateTime ExpireAt { get; init; }
    public bool IsUsed { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;

    // Navigation property
    public UserEntity? User { get; set; }
}
