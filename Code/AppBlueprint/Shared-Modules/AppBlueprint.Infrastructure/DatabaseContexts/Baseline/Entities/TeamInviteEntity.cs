using AppBlueprint.Application.Attributes;
using AppBlueprint.Application.Enums;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.Team;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;

// Represents an invitation to join a team
public class TeamInviteEntity: BaseEntity
{
    // Team that the user is invited to
    public int TeamId { get; set; }
    public required TeamEntity Team { get; set; }

    // User who is invited
    [DataClassification(GDPRType.DirectlyIdentifiable)]
    public required string Email { get; set; }

    public required UserEntity Owner { get; set; }
    public required DateTime ExpireAt { get; set; }
    public required bool IsActive { get; set; }
    public required DateTime CreatedAt { get; set; }
}
