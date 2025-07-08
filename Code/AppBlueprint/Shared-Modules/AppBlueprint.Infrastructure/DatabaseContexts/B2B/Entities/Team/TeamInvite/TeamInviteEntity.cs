using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.Team;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.TeamInvite;

// Represents an invitation to join a team
public class TeamInviteEntity : BaseEntity, ITenantScoped
{
    public TeamInviteEntity()
    {
        Id = PrefixedUlid.Generate("team-invite");
        TeamId = string.Empty;
        OwnerId = string.Empty;
        TenantId = string.Empty;
    }

    // Team that the user is invited to
    public string TeamId { get; set; }
    public TeamEntity? Team { get; set; }

    // User who is invited
    public string OwnerId { get; set; }
    public UserEntity? Owner { get; set; }

    public DateTime ExpireAt { get; set; }
    public bool IsActive { get; set; }

    // ITenantScoped implementation
    public string TenantId { get; set; }
}
