using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.DataExport;

public class DataExportEntityConfiguration : IEntityTypeConfiguration<DataExportEntity>
{
    public void Configure(EntityTypeBuilder<DataExportEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("DataExports");

        // Configure ID as ULID string
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id)
            .IsRequired()
            .HasMaxLength(40)
            .ValueGeneratedNever();

        // Configure BaseEntity properties
        builder.Property(d => d.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(d => d.LastUpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(d => d.IsSoftDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Configure TenantId for multi-tenancy
        builder.Property(d => d.TenantId)
            .IsRequired()
            .HasMaxLength(40);

        // Configure other properties
        builder.Property(d => d.FileName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(d => d.FileSize)
            .IsRequired();

        builder.Property(d => d.DownloadUrl)
            .HasMaxLength(2000);

        // Add indexes
        builder.HasIndex(d => d.TenantId)
            .HasDatabaseName("IX_DataExports_TenantId");

        builder.HasIndex(d => d.IsSoftDeleted)
            .HasDatabaseName("IX_DataExports_IsSoftDeleted");

        builder.HasIndex(d => new { d.TenantId, d.IsSoftDeleted })
            .HasDatabaseName("IX_DataExports_TenantId_IsSoftDeleted");
    }
}
