using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.Team;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.TeamMember;

public class TeamMemberEntity : BaseEntity, ITenantScoped
{    public TeamMemberEntity()
    {
        Id = PrefixedUlid.Generate("team-member");
        Alias = string.Empty;
        TenantId = string.Empty;
        UserId = string.Empty;
        TeamId = string.Empty;
    }

    // Alias name like in Discord
    public string Alias { get; set; }
    public bool IsActive { get; set; }

    // ITenantScoped implementation
    public string TenantId { get; set; }

    // User relationship
    public string UserId { get; set; }
    public UserEntity? User { get; set; }

    // Team relationship
    public string TeamId { get; set; }
    public TeamEntity? Team { get; set; }
}
