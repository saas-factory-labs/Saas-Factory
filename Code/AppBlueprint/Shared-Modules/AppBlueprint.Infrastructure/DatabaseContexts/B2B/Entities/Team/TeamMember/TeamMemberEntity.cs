using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.Team;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.TeamMember;

public class TeamMemberEntity
{
    public int Id { get; set; }

    // Alias name like in Discord
    public string Alias { get; set; }
    public bool IsActive { get; set; }

    // User relationship
    public int UserId { get; set; }
    public UserEntity User { get; set; }

    // Team relationship
    public int TeamId { get; set; }
    public TeamEntity Team { get; set; }
}
