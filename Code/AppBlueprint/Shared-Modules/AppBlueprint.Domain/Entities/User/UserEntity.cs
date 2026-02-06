using AppBlueprint.SharedKernel;

namespace AppBlueprint.Domain.Entities.User;

/// <summary>
/// Domain entity representing a User in the system
/// </summary>
public class UserEntity : BaseEntity
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; init; }
    public required string UserName { get; set; }

    // Computed property for display name
    public string Name => $"{FirstName} {LastName}";

    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Additional properties needed by services
    public string TenantId { get; set; } = string.Empty; // Default tenant

    // Navigation property to Profile
    public ProfileEntity? Profile { get; init; }
}
