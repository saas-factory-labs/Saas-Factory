using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Tenant.Tenant;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.TeamMember;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.TeamInvite;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.Team;

public class TeamEntity : BaseEntity, ITenantScoped
{
    public TeamEntity()
    {
        Id = PrefixedUlid.Generate("team");
        TeamMembers = new List<TeamMemberEntity>();
        TeamInvites = new List<TeamInviteEntity>();
    }

    public string? Name { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }

    public string TenantId { get; set; }
    public TenantEntity? Tenant { get; set; }

    public UserEntity? Owner { get; set; }

    public List<TeamMemberEntity> TeamMembers { get; }
    public List<TeamInviteEntity> TeamInvites { get; }

    public string? OrganizationId { get; set; }
}
