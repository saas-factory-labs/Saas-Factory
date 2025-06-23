using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization.EntityConfigurations;

/// <summary>
/// Entity configuration for RoleEntity defining authorization role schema, relationships and constraints.
/// </summary>
public sealed class RoleEntityConfiguration : IEntityTypeConfiguration<RoleEntity>
{
    public void Configure(EntityTypeBuilder<RoleEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);        // Table mapping with standardized naming
        builder.ToTable("Roles");

        // Primary key - ULID as string
        builder.HasKey(e => e.Id)
            .HasName("PK_Roles");

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
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("The role name (e.g., Administrator, User, Manager)");

        builder.Property(e => e.Description)
            .HasMaxLength(500)
            .HasComment("Optional description of the role's purpose and permissions");

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasComment("Timestamp when the role was created");

        builder.Property(e => e.LastUpdatedAt)
            .HasComment("Timestamp when the role was last modified");

        // Relationships
        builder.HasMany(r => r.UserRoles)
            .WithOne(ur => ur.Role)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_UserRoles_Roles_RoleId");

        builder.HasMany(r => r.RolePermissions)
            .WithOne(rp => rp.Role)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_RolePermissions_Roles_RoleId");

        // Performance indexes with standardized naming
        builder.HasIndex(e => e.Name)
            .IsUnique()
            .HasDatabaseName("IX_Roles_Name");

        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("IX_Roles_CreatedAt");
    }
}
