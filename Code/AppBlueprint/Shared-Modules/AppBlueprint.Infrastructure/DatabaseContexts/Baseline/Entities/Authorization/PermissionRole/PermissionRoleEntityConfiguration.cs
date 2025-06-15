using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

public sealed class PermissionRoleEntityConfiguration : IEntityTypeConfiguration<PermissionRoleEntity>
{
    public void Configure(EntityTypeBuilder<PermissionRoleEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Table mapping with standardized naming
        builder.ToTable("PermissionRoles");

        // Primary key
        builder.HasKey(e => e.Id);

        // Properties with validation
        builder.Property(e => e.RoleId)
            .IsRequired();

        // Relationships for many-to-many Permission-Role mapping
        builder.HasOne(pr => pr.Permission)
            .WithMany()
            .HasForeignKey("PermissionId")
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_PermissionRoles_Permissions_PermissionId");

        builder.HasOne(pr => pr.Role)
            .WithMany()
            .HasForeignKey(pr => pr.RoleId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_PermissionRoles_Roles_RoleId");

        // Performance indexes with standardized naming
        builder.HasIndex("PermissionId")
            .HasDatabaseName("IX_PermissionRoles_PermissionId");

        builder.HasIndex(pr => pr.RoleId)
            .HasDatabaseName("IX_PermissionRoles_RoleId");

        // Unique constraint to prevent duplicate permission-role assignments
        builder.HasIndex(new string[] { "PermissionId", "RoleId" })
            .IsUnique()
            .HasDatabaseName("UX_PermissionRoles_PermissionId_RoleId");
    }
}
