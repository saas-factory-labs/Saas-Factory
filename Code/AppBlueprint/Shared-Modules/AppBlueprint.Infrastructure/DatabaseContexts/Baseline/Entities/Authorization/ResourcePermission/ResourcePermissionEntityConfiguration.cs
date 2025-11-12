using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

/// <summary>
/// Entity configuration for ResourcePermissionEntity defining table structure, relationships, and constraints.
/// Manages resource-based permissions and access control rules.
/// </summary>
public sealed class ResourcePermissionEntityConfiguration : BaseEntityConfiguration<ResourcePermissionEntity>
{
    public override void Configure(EntityTypeBuilder<ResourcePermissionEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        
        // Apply base configuration including named soft delete filter
        base.Configure(builder);
builder.ToTable("ResourcePermissions");

        builder.HasKey(e => e.Id);

        // Configure ULID ID with proper length for prefixed ULID
        builder.Property(e => e.Id)
            .HasMaxLength(40)
            .IsRequired();

        // Configure foreign key properties
        builder.Property(e => e.UserId)
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(e => e.ResourceId)
            .HasMaxLength(40)
            .IsRequired();

        // BaseEntity properties
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.LastUpdatedAt);

        builder.Property(e => e.IsSoftDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Relationships
        builder.HasOne(rp => rp.User)
            .WithMany(u => u.ResourcePermissions)
            .HasForeignKey(rp => rp.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_ResourcePermissions_Users_UserId");

        // Indexes for performance
        builder.HasIndex(rp => rp.UserId)
            .HasDatabaseName("IX_ResourcePermissions_UserId");

        builder.HasIndex(rp => rp.ResourceId)
            .HasDatabaseName("IX_ResourcePermissions_ResourceId");

        builder.HasIndex(rp => new { rp.UserId, rp.ResourceId })
            .IsUnique()
            .HasDatabaseName("IX_ResourcePermissions_UserId_ResourceId_Unique");

        builder.HasIndex(rp => rp.IsSoftDeleted)
            .HasDatabaseName("IX_ResourcePermissions_IsSoftDeleted");
    }
}
