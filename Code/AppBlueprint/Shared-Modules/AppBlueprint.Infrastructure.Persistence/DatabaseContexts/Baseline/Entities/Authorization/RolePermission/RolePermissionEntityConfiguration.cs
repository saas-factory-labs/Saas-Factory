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
        ArgumentNullException.ThrowIfNull(builder);        // Table mapping with standardized naming
        builder.ToTable("RolePermissions");

        // Primary key - ULID as string
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .IsRequired()
            .HasMaxLength(40);

        // BaseEntity properties
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.LastUpdatedAt)
            .IsRequired();

        builder.Property(e => e.IsSoftDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Properties with validation
        builder.Property(e => e.RoleId)
            .IsRequired()
            .HasMaxLength(40);

        builder.Property(e => e.PermissionId)
            .IsRequired()
            .HasMaxLength(40);

        // Relationships
        builder.HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_RolePermissions_Roles_RoleId");

        builder.HasOne(rp => rp.Permission)
            .WithMany()
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_RolePermissions_Permissions_PermissionId");

        // Performance indexes with standardized naming
        builder.HasIndex(e => e.RoleId)
            .HasDatabaseName("IX_RolePermissions_RoleId");

        builder.HasIndex(e => e.PermissionId)
            .HasDatabaseName("IX_RolePermissions_PermissionId");

        // Unique constraint - prevent duplicate role-permission assignments
        builder.HasIndex(e => new { e.RoleId, e.PermissionId })
            .IsUnique()
            .HasDatabaseName("IX_RolePermissions_RoleId_PermissionId_Unique");
    }
}
