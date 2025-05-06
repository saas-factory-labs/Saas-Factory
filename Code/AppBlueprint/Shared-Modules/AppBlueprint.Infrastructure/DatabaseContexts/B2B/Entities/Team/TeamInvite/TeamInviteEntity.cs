using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.Team;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.TeamInvite;

// Represents an invitation to join a team
public class TeamInviteEntity
{
    public int Id { get; set; }

    // Team that the user is invited to
    public int TeamId { get; set; }
    public TeamEntity Team { get; set; }

    // User who is invited

    public UserEntity Owner { get; set; }
    public DateTime ExpireAt { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
