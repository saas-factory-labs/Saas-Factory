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

        builder.ToTable("TeamInvites");

        // Primary Key
        builder.HasKey(ti => ti.Id);

        // Configure ULID ID with proper length for prefixed ULID (prefix + underscore + 26 char ULID)
        builder.Property(ti => ti.Id)
            .HasMaxLength(40)
            .IsRequired();

        // BaseEntity properties
        builder.Property(ti => ti.CreatedAt)
            .IsRequired();

        builder.Property(ti => ti.LastUpdatedAt);

        builder.Property(ti => ti.IsSoftDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // ITenantScoped property
        builder.Property(ti => ti.TenantId)
            .IsRequired()
            .HasMaxLength(40);

        // Entity-specific properties
        builder.Property(ti => ti.TeamId)
            .IsRequired()
            .HasMaxLength(40);

        builder.Property(ti => ti.OwnerId)
            .IsRequired()
            .HasMaxLength(40);

        builder.Property(ti => ti.ExpireAt)
            .IsRequired();

        builder.Property(ti => ti.IsActive)
            .IsRequired();

        // Relationships
        builder.HasOne(ti => ti.Team)
            .WithMany(t => t.TeamInvites)
            .HasForeignKey(ti => ti.TeamId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_TeamInvites_Teams_TeamId");

        builder.HasOne(ti => ti.Owner)
            .WithMany()
            .HasForeignKey(ti => ti.OwnerId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_TeamInvites_Users_OwnerId");

        // Indexes
        builder.HasIndex(ti => ti.Id)
            .IsUnique()
            .HasDatabaseName("IX_TeamInvites_Id");

        builder.HasIndex(ti => ti.TenantId)
            .HasDatabaseName("IX_TeamInvites_TenantId");

        builder.HasIndex(ti => ti.TeamId)
            .HasDatabaseName("IX_TeamInvites_TeamId");

        builder.HasIndex(ti => ti.OwnerId)
            .HasDatabaseName("IX_TeamInvites_OwnerId");

        builder.HasIndex(ti => ti.IsActive)
            .HasDatabaseName("IX_TeamInvites_IsActive");

        builder.HasIndex(ti => ti.IsSoftDeleted)
            .HasDatabaseName("IX_TeamInvites_IsSoftDeleted");

        builder.HasIndex(ti => ti.ExpireAt)
            .HasDatabaseName("IX_TeamInvites_ExpireAt");
    }
}
