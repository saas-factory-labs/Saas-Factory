using System.ComponentModel.DataAnnotations;

namespace AppBlueprint.Contracts.Baseline.User.Requests;

public class CreateUserRequest
{
    [Required]
    public required string FirstName { get; set; }
    
    [Required]
    public required string LastName { get; set; }
    
    [Required]
    public required string UserName { get; set; }
    
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    
    [Required]
    public required string TenantId { get; set; }
    
    /// <summary>
    /// External authentication provider user ID (e.g., Logto 'sub' claim).
    /// </summary>
    public string? ExternalAuthId { get; set; }
    
    public bool IsActive { get; set; } = true;
}
