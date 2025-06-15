using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.TeamMember;

public sealed class TeamMemberEntityConfiguration : IEntityTypeConfiguration<TeamMemberEntity>
{    public void Configure(EntityTypeBuilder<TeamMemberEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Primary Key
        builder.HasKey(e => e.Id);

        // Unique Index
        builder.HasIndex(e => e.Id).IsUnique();

        // Properties
        builder.Property(e => e.Alias)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.IsActive)
            .IsRequired();

        // Relationships
        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Team)
            .WithMany(t => t.TeamMembers)
            .HasForeignKey(e => e.TeamId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
