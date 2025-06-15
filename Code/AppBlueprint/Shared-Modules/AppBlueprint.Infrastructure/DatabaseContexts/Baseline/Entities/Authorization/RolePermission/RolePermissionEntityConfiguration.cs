using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization.RolePermission;

/// <summary>
/// Entity configuration for RolePermissionEntity defining table structure, relationships, and constraints.
/// </summary>
public sealed class RolePermissionEntityConfiguration : IEntityTypeConfiguration<RolePermissionEntity>
{
    public void Configure(EntityTypeBuilder<RolePermissionEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Table mapping with standardized naming
        builder.ToTable("RolePermissions");

        // Primary key
        builder.HasKey(e => e.Id);

        // Properties with validation
        builder.Property(e => e.RoleId)
            .IsRequired();

        // Relationships
        builder.HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_RolePermissions_Roles_RoleId");

        // Performance indexes with standardized naming
        builder.HasIndex(e => e.RoleId)
            .HasDatabaseName("IX_RolePermissions_RoleId");
    }
}
