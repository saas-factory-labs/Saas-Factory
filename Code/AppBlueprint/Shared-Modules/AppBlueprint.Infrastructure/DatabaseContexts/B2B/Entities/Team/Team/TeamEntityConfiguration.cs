using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.Team;

public sealed class TeamEntityConfiguration : IEntityTypeConfiguration<TeamEntity>
{
    public void Configure(EntityTypeBuilder<TeamEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Table mapping
        builder.ToTable("Teams");

        // Primary key
        builder.HasKey(t => t.Id);

        // Properties
        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Description)
            .HasMaxLength(500);

        builder.Property(t => t.IsActive)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.LastUpdatedAt);        // Relationships with proper foreign key constraints
        builder.HasOne(t => t.Tenant)
            .WithMany(tenant => tenant.Teams)
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

        // Performance indexes with standardized naming
        builder.HasIndex(t => t.Name)
            .HasDatabaseName("IX_Teams_Name");
            
        builder.HasIndex(t => t.TenantId)
            .HasDatabaseName("IX_Teams_TenantId");
            
        builder.HasIndex(t => t.IsActive)
            .HasDatabaseName("IX_Teams_IsActive");
    }
}
