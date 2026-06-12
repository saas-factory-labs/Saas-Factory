using System.ComponentModel.DataAnnotations;

namespace AppBlueprint.Contracts.Baseline.User.Requests;

public class CreateUserRequest
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public required string FirstName { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public required string LastName { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public required string UserName { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(254)]
    public required string Email { get; set; }

    [Required]
    [StringLength(64, MinimumLength = 1)]
    public required string TenantId { get; set; }

    /// <summary>
    /// External authentication provider user ID (e.g., Logto 'sub' claim).
    /// </summary>
    [StringLength(256)]
    public string? ExternalAuthId { get; set; }

    public bool IsActive { get; set; } = true;
}
