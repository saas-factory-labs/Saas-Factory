using AppBlueprint.SharedKernel;

namespace AppBlueprint.Domain.Entities.User;

/// <summary>
/// Domain entity representing a User's profile information
/// </summary>
public class ProfileEntity : BaseEntity
{
    public string? Bio { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Avatar { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Foreign key to User
    public Guid UserId { get; set; }
    
    // Navigation property back to User
    public UserEntity? User { get; set; }
}
