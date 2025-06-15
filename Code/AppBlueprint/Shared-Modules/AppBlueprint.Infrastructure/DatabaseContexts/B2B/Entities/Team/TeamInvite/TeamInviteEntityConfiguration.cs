using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.TeamInvite;

/// <summary>
/// Entity configuration for TeamInviteEntity defining table structure, relationships, and constraints.
/// Manages team invitation system for B2B scenarios.
/// </summary>
public sealed class TeamInviteEntityConfiguration : IEntityTypeConfiguration<TeamInviteEntity>
{
    public void Configure(EntityTypeBuilder<TeamInviteEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Primary Key
        builder.HasKey(ti => ti.Id);        // Unique Index
        builder.HasIndex(ti => ti.Id).IsUnique();

        // Relationships
        builder.HasOne(ti => ti.Team)
            .WithMany(t => t.TeamInvites)
            .HasForeignKey(ti => ti.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        // builder.HasOne(ti => ti.User)
        //     .WithMany()
        //     .HasForeignKey(ti => ti.UserId)
        //     .OnDelete(DeleteBehavior.Restrict);

        // Properties
        builder.Property(ti => ti.ExpireAt)
            .IsRequired();

        builder.Property(ti => ti.IsActive)
            .IsRequired();

        builder.Property(ti => ti.CreatedAt)
            .IsRequired();
    }
}
