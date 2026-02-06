using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.Team;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.SharedKernel;
using AppBlueprint.SharedKernel.Attributes;
using AppBlueprint.SharedKernel.Enums;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;

// NOTE: This is an old version of TeamInviteEntity - renamed to avoid conflicts
// The correct version is in AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.TeamInvite
// Represents an invitation to join a team
public class OldTeamInviteEntity : BaseEntity
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
}
