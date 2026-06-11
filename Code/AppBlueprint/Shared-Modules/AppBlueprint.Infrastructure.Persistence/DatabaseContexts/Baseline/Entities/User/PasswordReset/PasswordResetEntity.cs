using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User.PasswordReset;

public sealed class PasswordResetEntity : BaseEntity, ITenantScoped
{
    public PasswordResetEntity()
    {
        Id = PrefixedUlid.Generate("pwr");
    }

    public required string Token { get; init; }
    public DateTime ExpireAt { get; set; }
    public bool IsUsed { get; set; }
    public required UserEntity User { get; set; }
    public required string UserId { get; set; }
    public required string TenantId { get; set; }
}
