using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.TeamInvite;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.TeamMember;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Tenant.Tenant;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.Team;

public class TeamEntity
{
    public TeamEntity()
    {
        TeamMembers = new List<TeamMemberEntity>();
        TeamInvites = new List<TeamInviteEntity>();
    }

    public int Id { get; set; }

    public UserEntity? Owner { get; set; }

    public string? Name { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }

    // Owner relationship


    // Team Members and Invites
    public List<TeamMemberEntity>? TeamMembers { get; }
    public List<TeamInviteEntity>? TeamInvites { get; }

    // Tenant relationship
    public TenantEntity? Tenant { get; set; }
    public int TenantId { get; set; } // Assuming TenantId as FK
}
