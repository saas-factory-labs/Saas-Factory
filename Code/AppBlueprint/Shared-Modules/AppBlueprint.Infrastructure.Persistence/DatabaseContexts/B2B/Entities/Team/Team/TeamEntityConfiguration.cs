using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.Team;

public sealed class TeamEntityConfiguration : IEntityTypeConfiguration<TeamEntity>
{
    public void Configure(EntityTypeBuilder<TeamEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Teams");

        builder.HasKey(t => t.Id);

        // Configure ULID ID with proper length for prefixed ULID (prefix + underscore + 26 char ULID)
        builder.Property(t => t.Id)
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Description)
            .HasMaxLength(500);

        builder.Property(t => t.IsActive)
            .IsRequired();    // BaseEntity properties from BaseEntity
        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.LastUpdatedAt);

        builder.Property(t => t.IsSoftDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // ITenantScoped property
        builder.Property(t => t.TenantId)
            .IsRequired()
            .HasMaxLength(40);

        builder.HasOne(t => t.Tenant)
            .WithMany()
            .HasForeignKey(t => t.TenantId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Teams_Tenants_TenantId");

        builder.HasMany(t => t.TeamMembers)
            .WithOne(tm => tm.Team)
            .HasForeignKey(tm => tm.TeamId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_TeamMembers_Teams_TeamId");

        builder.HasMany(t => t.TeamInvites)
            .WithOne()
            .HasForeignKey("TeamId")
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_TeamInvites_Teams_TeamId");

        builder.HasIndex(t => t.Name)
            .HasDatabaseName("IX_Teams_Name");

        builder.HasIndex(t => t.TenantId)
            .HasDatabaseName("IX_Teams_TenantId");    // Add index for soft delete filtering
        builder.HasIndex(t => t.IsSoftDeleted)
            .HasDatabaseName("IX_Teams_IsSoftDeleted");

        builder.HasIndex(t => t.IsActive)
            .HasDatabaseName("IX_Teams_IsActive");
    }
}
