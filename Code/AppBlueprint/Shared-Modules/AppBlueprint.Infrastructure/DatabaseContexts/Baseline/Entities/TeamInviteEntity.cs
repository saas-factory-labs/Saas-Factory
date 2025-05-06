using AppBlueprint.Application.Attributes;
using AppBlueprint.Application.Enums;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.Team;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;

// Represents an invitation to join a team
public class TeamInviteEntity
{
    public int Id { get; set; }

    // Team that the user is invited to
    public int TeamId { get; set; }
    public TeamEntity Team { get; set; }

    // User who is invited
    [DataClassification(GDPRType.DirectlyIdentifiable)]
    public string Email { get; set; }

    public UserEntity Owner { get; set; }
    public DateTime ExpireAt { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
