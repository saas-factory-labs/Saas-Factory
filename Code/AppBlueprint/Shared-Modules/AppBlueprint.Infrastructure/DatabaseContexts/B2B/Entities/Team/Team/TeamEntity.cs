using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Tenant.Tenant;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.TeamMember;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.TeamInvite;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.Team;

public sealed class TeamEntity : BaseEntity, ITenantScoped
{
    private readonly List<TeamMemberEntity> _teamMembers = new();
    private readonly List<TeamInviteEntity> _teamInvites = new();

    public TeamEntity()
    {
        Id = PrefixedUlid.Generate("team");
        TenantId = string.Empty;
    }

    public string? Name { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }

    public string TenantId { get; set; }
    public TenantEntity? Tenant { get; set; }

    public UserEntity? Owner { get; set; }

    public IReadOnlyCollection<TeamMemberEntity> TeamMembers => _teamMembers.AsReadOnly();
    public IReadOnlyCollection<TeamInviteEntity> TeamInvites => _teamInvites.AsReadOnly();

    public string? OrganizationId { get; set; }

    // Domain methods for controlled collection management
    public void AddTeamMember(TeamMemberEntity teamMember)
    {
        ArgumentNullException.ThrowIfNull(teamMember);
        
        if (_teamMembers.Any(tm => tm.Id == teamMember.Id))
            return; // Team member already exists
            
        _teamMembers.Add(teamMember);
    }

    public void RemoveTeamMember(TeamMemberEntity teamMember)
    {
        ArgumentNullException.ThrowIfNull(teamMember);
        _teamMembers.Remove(teamMember);
    }

    public void AddTeamInvite(TeamInviteEntity teamInvite)
    {
        ArgumentNullException.ThrowIfNull(teamInvite);
        
        if (_teamInvites.Any(ti => ti.Id == teamInvite.Id))
            return; // Team invite already exists
            
        _teamInvites.Add(teamInvite);
    }

    public void RemoveTeamInvite(TeamInviteEntity teamInvite)
    {
        ArgumentNullException.ThrowIfNull(teamInvite);
        _teamInvites.Remove(teamInvite);
    }
}
