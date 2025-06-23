using AppBlueprint.Application.Attributes;
using AppBlueprint.Application.Enums;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;

public sealed class PasswordResetEntity : BaseEntity, ITenantScoped
{
    public PasswordResetEntity()
    {
        Id = PrefixedUlid.Generate("pwr");
    }
    
    public required string Token { get; set; }
    public DateTime ExpireAt { get; set; }
    public bool IsUsed { get; set; }
    public required UserEntity User { get; set; }
    public required string UserId { get; set; }
    public required string TenantId { get; set; }
}
