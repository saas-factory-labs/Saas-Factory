using AppBlueprint.Application.Attributes;
using AppBlueprint.Application.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

/// <summary>
/// Entity configuration for AuditLogEntity tracking all system changes for compliance and security purposes.
/// Implements GDPR compliance for sensitive audit data and optimizes for compliance reporting queries.
/// </summary>
public sealed class AuditLogEntityConfiguration : BaseEntityConfiguration<AuditLogEntity>
{
    public override void Configure(EntityTypeBuilder<AuditLogEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Apply base configuration including named soft delete filter
        base.Configure(builder);

        // Define table name
        builder.ToTable("AuditLogs");

        // Override base ID configuration to add additional settings
        builder.Property(a => a.Id)
            .ValueGeneratedNever()
            .HasComment("Primary key for audit log entry");

        // Override base CreatedAt and LastUpdatedAt to add default SQL
        builder.Property(a => a.CreatedAt)
            .HasDefaultValueSql("NOW()");

        builder.Property(a => a.LastUpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        // Configure TenantId for multi-tenancy
        builder.Property(a => a.TenantId)
            .IsRequired()
            .HasMaxLength(40)
            .HasComment("Foreign key to the tenant where the action occurred");

        // Configure Action property with GDPR compliance
        builder.Property(e => e.Action)
            .IsRequired()
            .HasMaxLength(200)
            .HasComment("Description of the action performed (GDPR sensitive)")
            .HasAnnotation("DataClassification", GDPRType.SensitiveMiscellaneous);

        // Configure Category property
        builder.Property(e => e.Category)
            .IsRequired(false)
            .HasMaxLength(100)
            .HasComment("Category classification for the audit action");

        // Configure value tracking properties with GDPR compliance
        builder.Property(e => e.NewValue)
            .IsRequired()
            .HasColumnType("text")
            .HasComment("New value after the change (JSON format)")
            .HasAnnotation("DataClassification", GDPRType.SensitiveMiscellaneous);

        builder.Property(e => e.OldValue)
            .IsRequired()
            .HasColumnType("text")
            .HasComment("Previous value before the change (JSON format)")
            .HasAnnotation("DataClassification", GDPRType.SensitiveMiscellaneous);

        // Configure timestamp property
        builder.Property(e => e.ModifiedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasComment("Timestamp when the action was performed");

        // Configure foreign key properties
        builder.Property(e => e.UserId)
            .IsRequired()
            .HasMaxLength(40)
            .HasComment("Foreign key to the user who performed the action");

        // Configure relationship to User (who performed the action)
        builder.HasOne(al => al.User)
            .WithMany() // UserEntity doesn't have AuditLogs navigation property
            .HasForeignKey(al => al.UserId)
            .OnDelete(DeleteBehavior.Restrict) // Preserve audit trail even if user is deleted
            .HasConstraintName("FK_AuditLogs_Users_UserId");

        // Configure relationship to ModifiedBy User
        builder.HasOne(al => al.ModifiedBy)
            .WithMany() // UserEntity doesn't have ModifiedAuditLogs navigation property
            .HasForeignKey("ModifiedByUserId") // Shadow property for separate relationship
            .OnDelete(DeleteBehavior.Restrict) // Preserve audit trail
            .HasConstraintName("FK_AuditLogs_Users_ModifiedByUserId");

        // Configure relationship to Tenant
        builder.HasOne(al => al.Tenant)
            .WithMany() // TenantEntity doesn't have AuditLogs navigation property
            .HasForeignKey(al => al.TenantId)
            .OnDelete(DeleteBehavior.Restrict) // Preserve audit trail even if tenant is deleted
            .HasConstraintName("FK_AuditLogs_Tenants_TenantId");

        // Add indexes
        builder.HasIndex(a => a.TenantId)
            .HasDatabaseName("IX_AuditLogs_TenantId");

        // Note: IsSoftDeleted index is configured in BaseEntityConfiguration

        builder.HasIndex(a => new { a.TenantId, a.IsSoftDeleted })
            .HasDatabaseName("IX_AuditLogs_TenantId_IsSoftDeleted");

        // Create indexes for performance and compliance queries
        builder.HasIndex(al => al.UserId)
            .HasDatabaseName("IX_AuditLogs_UserId")
            .HasFilter(null);

        builder.HasIndex(al => al.ModifiedAt)
            .HasDatabaseName("IX_AuditLogs_ModifiedAt")
            .HasFilter(null);

        builder.HasIndex(al => al.Category)
            .HasDatabaseName("IX_AuditLogs_Category")
            .HasFilter("\"Category\" IS NOT NULL");

        // Create composite indexes for common compliance reporting queries
        builder.HasIndex(al => new { al.TenantId, al.ModifiedAt })
            .HasDatabaseName("IX_AuditLogs_TenantId_ModifiedAt")
            .HasFilter(null);

        builder.HasIndex(al => new { al.UserId, al.ModifiedAt })
            .HasDatabaseName("IX_AuditLogs_UserId_ModifiedAt")
            .HasFilter(null);

        builder.HasIndex(al => new { al.Category, al.ModifiedAt })
            .HasDatabaseName("IX_AuditLogs_Category_ModifiedAt")
            .HasFilter("\"Category\" IS NOT NULL");

        // Note: Soft delete query filter is configured in BaseEntityConfiguration
    }
}
