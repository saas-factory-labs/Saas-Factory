using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

/// <summary>
/// Entity configuration for PermissionEntity defining authorization permission schema and constraints.
/// </summary>
public sealed class PermissionEntityConfiguration : IEntityTypeConfiguration<PermissionEntity>
{
    public void Configure(EntityTypeBuilder<PermissionEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);        // Table mapping with standardized naming
        builder.ToTable("Permissions");

        // Primary key - ULID as string
        builder.HasKey(e => e.Id)
            .HasName("PK_Permissions");

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
            .HasComment("The permission name (e.g., 'read:users', 'write:documents')");

        builder.Property(e => e.Description)
            .HasMaxLength(500)
            .HasComment("Optional description of what this permission allows");

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasComment("Timestamp when the permission was created");

        builder.Property(e => e.LastUpdatedAt)
            .HasComment("Timestamp when the permission was last modified");

        // Performance indexes with standardized naming
        builder.HasIndex(e => e.Name)
            .IsUnique()
            .HasDatabaseName("IX_Permissions_Name");

        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("IX_Permissions_CreatedAt");
    }
}