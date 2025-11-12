using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.TeamMember;

public sealed class TeamMemberEntityConfiguration : BaseEntityConfiguration<TeamMemberEntity>
{
    public override void Configure(EntityTypeBuilder<TeamMemberEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        
        // Apply base configuration including named soft delete filter
        base.Configure(builder);
builder.ToTable("TeamMembers");

        // Primary Key
        builder.HasKey(e => e.Id);

        // Configure ULID ID with proper length for prefixed ULID (prefix + underscore + 26 char ULID)
        builder.Property(e => e.Id)
            .HasMaxLength(40)
            .IsRequired();

        // BaseEntity properties
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.LastUpdatedAt);

        builder.Property(e => e.IsSoftDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // ITenantScoped property
        builder.Property(e => e.TenantId)
            .IsRequired()
            .HasMaxLength(40);

        // Entity-specific properties
        builder.Property(e => e.Alias)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.IsActive)
            .IsRequired();

        builder.Property(e => e.UserId)
            .IsRequired()
            .HasMaxLength(40);

        builder.Property(e => e.TeamId)
            .IsRequired()
            .HasMaxLength(40);

        // Relationships
        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_TeamMembers_Users_UserId");

        builder.HasOne(e => e.Team)
            .WithMany(t => t.TeamMembers)
            .HasForeignKey(e => e.TeamId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_TeamMembers_Teams_TeamId");

        // Indexes
        builder.HasIndex(e => e.Id)
            .IsUnique()
            .HasDatabaseName("IX_TeamMembers_Id");

        builder.HasIndex(e => e.TenantId)
            .HasDatabaseName("IX_TeamMembers_TenantId");

        builder.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_TeamMembers_UserId");

        builder.HasIndex(e => e.TeamId)
            .HasDatabaseName("IX_TeamMembers_TeamId");

        builder.HasIndex(e => e.IsActive)
            .HasDatabaseName("IX_TeamMembers_IsActive");

        builder.HasIndex(e => e.IsSoftDeleted)
            .HasDatabaseName("IX_TeamMembers_IsSoftDeleted");
    }
}
