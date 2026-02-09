using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.SharedKernel;
using AppBlueprint.SharedKernel.Attributes;
using AppBlueprint.SharedKernel.Enums;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Email.EmailInvite;

public class EmailInviteEntity : BaseEntity
{
    public EmailInviteEntity()
    {
        Id = PrefixedUlid.Generate("email_invite");
    }

    public required string Token { get; init; }

    [DataClassification(GDPRType.DirectlyIdentifiable)]
    public required string ReferredEmailAddress { get; init; }

    public DateTime ExpireAt { get; set; }
    public bool InviteIsUsed { get; set; }

    public string? UserEntityId { get; set; }
    public UserEntity? User { get; set; }
}
