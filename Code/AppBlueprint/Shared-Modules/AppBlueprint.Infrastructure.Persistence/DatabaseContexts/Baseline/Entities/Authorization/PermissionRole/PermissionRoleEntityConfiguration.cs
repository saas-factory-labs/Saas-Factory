using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization.PermissionRole;

public sealed class PermissionRoleEntityConfiguration : IEntityTypeConfiguration<PermissionRoleEntity>
{
    public void Configure(EntityTypeBuilder<PermissionRoleEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Table mapping with standardized naming
        builder.ToTable("PermissionRoles");

        // Primary key
        builder.HasKey(e => e.Id);

        // Configure ULID ID with proper length for prefixed ULID
        builder.Property(e => e.Id)
            .HasMaxLength(40)
            .IsRequired();

        // Configure foreign key properties
        builder.Property(e => e.PermissionId)
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(e => e.RoleId)
            .HasMaxLength(40)
            .IsRequired();

        // BaseEntity properties
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.LastUpdatedAt);

        builder.Property(e => e.IsSoftDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Relationships for many-to-many Permission-Role mapping
        builder.HasOne(pr => pr.Permission)
            .WithMany()
            .HasForeignKey(pr => pr.PermissionId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_PermissionRoles_Permissions_PermissionId");

        builder.HasOne(pr => pr.Role)
            .WithMany()
            .HasForeignKey(pr => pr.RoleId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_PermissionRoles_Roles_RoleId");

        // Performance indexes with standardized naming
        builder.HasIndex(pr => pr.PermissionId)
            .HasDatabaseName("IX_PermissionRoles_PermissionId");

        builder.HasIndex(pr => pr.RoleId)
            .HasDatabaseName("IX_PermissionRoles_RoleId");

        builder.HasIndex(pr => pr.IsSoftDeleted)
            .HasDatabaseName("IX_PermissionRoles_IsSoftDeleted");

        // Unique constraint to prevent duplicate permission-role assignments
        builder.HasIndex(pr => new { pr.PermissionId, pr.RoleId })
            .IsUnique()
            .HasDatabaseName("UX_PermissionRoles_PermissionId_RoleId");
    }
}
