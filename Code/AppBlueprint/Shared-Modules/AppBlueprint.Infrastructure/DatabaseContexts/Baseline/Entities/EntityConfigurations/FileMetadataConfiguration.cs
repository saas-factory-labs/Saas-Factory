using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.FileManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

public sealed class FileMetadataConfiguration : IEntityTypeConfiguration<FileMetadataEntity>
{
    public void Configure(EntityTypeBuilder<FileMetadataEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("FileMetadata");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.FileKey)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(f => f.FileKey)
            .IsUnique();

        builder.Property(f => f.OriginalFileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(f => f.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.SizeInBytes)
            .IsRequired();

        builder.Property(f => f.UploadedBy)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(f => f.Folder)
            .HasMaxLength(200);

        builder.Property(f => f.IsPublic)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(f => f.PublicUrl)
            .HasMaxLength(1000)
            .HasConversion(
                v => v != null ? v.ToString() : null,
                v => !string.IsNullOrWhiteSpace(v) ? new Uri(v) : null);

        // Store CustomMetadata as JSONB in PostgreSQL
        builder.Property(f => f.CustomMetadata)
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'{}'");

        builder.Property(f => f.TenantId)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(f => f.TenantId);

        // Index for fast file lookups
        builder.HasIndex(f => new { f.TenantId, f.Folder });
        builder.HasIndex(f => new { f.TenantId, f.UploadedBy });
        builder.HasIndex(f => f.CreatedAt);
    }
}
