using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;

public class PasswordResetEntity
{
    public int Id { get; set; }
    public required string Token { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
    public DateTime ExpireAt { get; set; }
    public bool IsUsed { get; set; }
    public required UserEntity User { get; set; }
    public int UserId { get; set; }
}
