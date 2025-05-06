using AppBlueprint.Application.Attributes;
using AppBlueprint.Application.Enums;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Email.EmailInvite;

public class EmailInviteEntity
{
    public int Id { get; set; }
    public required string Token { get; set; }

    [DataClassification(GDPRType.DirectlyIdentifiable)]
    public required string ReferredEmailAddress { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
    public DateTime ExpireAt { get; set; }
    public bool InviteIsUsed { get; set; }

    public int? UserEntityId { get; set; }
    public UserEntity? User { get; set; }
}
