using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.Team;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.TeamInvite;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.TeamMember;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2B.Partials;

public partial class B2BDbContext
{
    public DbSet<TeamEntity> Teams { get; set; }
    public DbSet<TeamInviteEntity> TeamInvites { get; set; }
    public DbSet<TeamMemberEntity> TeamMembers { get; set; }

    partial void OnModelCreating_Team(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TeamEntityConfiguration());
        modelBuilder.ApplyConfiguration(new TeamInviteEntityConfiguration());
        modelBuilder.ApplyConfiguration(new TeamMemberEntityConfiguration());
    }
}
