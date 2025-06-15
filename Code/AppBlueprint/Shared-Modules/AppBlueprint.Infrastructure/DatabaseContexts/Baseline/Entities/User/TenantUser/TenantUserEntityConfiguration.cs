using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User.TenantUser;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

/// <summary>
/// Entity configuration for TenantUserEntity defining table structure, relationships, and constraints.
/// Uses data attributes from entity properties rather than duplicating in EF configuration.
/// </summary>
public sealed class TenantUserEntityConfiguration : IEntityTypeConfiguration<TenantUserEntity>
{
    public void Configure(EntityTypeBuilder<TenantUserEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Table mapping with standardized naming
        builder.ToTable("TenantUsers");

        // Composite primary key for many-to-many relationship
        builder.HasKey(e => new { e.TenantId, e.UserId });

        // Properties (data attributes on entity will be automatically applied)
        builder.Property(e => e.UserId)
            .IsRequired();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        // Relationships
        builder.HasOne(e => e.Tenant)
            .WithMany()
            .HasForeignKey(e => e.TenantId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_TenantUsers_Tenants_TenantId");

        // Performance indexes with standardized naming
        builder.HasIndex(e => e.TenantId)
            .HasDatabaseName("IX_TenantUsers_TenantId");
            
        builder.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_TenantUsers_UserId");
            
        builder.HasIndex(e => e.Name)
            .HasDatabaseName("IX_TenantUsers_Name");
    }
    }
}