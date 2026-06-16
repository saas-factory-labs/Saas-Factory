using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.AdminPortalKernel.Domain;

/// <summary>EF mapping for the dm_admin_audit table, shared by the DeploymentManager
/// context (which owns the migration) and the kernel's audit context (which never migrates).</summary>
public sealed class AdminAuditEntryConfiguration : IEntityTypeConfiguration<AdminAuditEntryEntity>
{
    public void Configure(EntityTypeBuilder<AdminAuditEntryEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("dm_admin_audit");
        builder.HasKey(entry => entry.Id);

        builder.Property(entry => entry.Id).HasMaxLength(40);
        builder.Property(entry => entry.AppSlug).HasMaxLength(100).IsRequired();
        builder.Property(entry => entry.AdminUserId).HasMaxLength(256).IsRequired();
        builder.Property(entry => entry.AdminEmail).HasMaxLength(320).IsRequired();
        builder.Property(entry => entry.Action).HasMaxLength(100).IsRequired();
        builder.Property(entry => entry.TargetType).HasMaxLength(100);
        builder.Property(entry => entry.TargetId).HasMaxLength(40);
        builder.Property(entry => entry.TenantId).HasMaxLength(40);
        builder.Property(entry => entry.Reason).HasMaxLength(500).IsRequired();
        builder.Property(entry => entry.Details).HasMaxLength(4000);
        builder.Property(entry => entry.OccurredAtUtc).IsRequired();

        builder.HasIndex(entry => new { entry.AppSlug, entry.OccurredAtUtc })
            .HasDatabaseName("IX_dm_admin_audit_AppSlug_OccurredAtUtc");
        builder.HasIndex(entry => entry.AdminUserId)
            .HasDatabaseName("IX_dm_admin_audit_AdminUserId");
    }
}
