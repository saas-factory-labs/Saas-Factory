using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.FileManagement.EntityConfigurations;

/// <summary>
/// Entity configuration for FileEntity defining table structure, relationships, and constraints.
/// </summary>
public sealed class FileEntityConfiguration : IEntityTypeConfiguration<FileEntity>
{
    public void Configure(EntityTypeBuilder<FileEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Table mapping with standardized naming
        builder.ToTable("Files");

        // Primary key
        builder.HasKey(e => e.Id);

        // Properties with validation and GDPR compliance
        builder.Property(e => e.FileName)
            .IsRequired()
            .HasMaxLength(255)
            .HasAnnotation("SensitiveData", true);

        builder.Property(e => e.FileSize)
            .IsRequired();

        builder.Property(e => e.FileExtension)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(e => e.FilePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.OwnerId)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired(); builder.Property(e => e.LastUpdatedAt);

        // Performance indexes with standardized naming
        builder.HasIndex(e => e.FileName)
            .HasDatabaseName("IX_Files_FileName");

        builder.HasIndex(e => e.OwnerId)
            .HasDatabaseName("IX_Files_OwnerId");

        builder.HasIndex(e => e.FileExtension)
            .HasDatabaseName("IX_Files_FileExtension");

        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("IX_Files_CreatedAt");
    }
}
