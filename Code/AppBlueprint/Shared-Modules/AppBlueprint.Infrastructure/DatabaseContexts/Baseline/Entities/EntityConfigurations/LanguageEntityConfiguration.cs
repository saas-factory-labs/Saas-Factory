using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

/// <summary>
/// Entity configuration for LanguageEntity defining table structure, relationships, and constraints.
/// </summary>
public sealed class LanguageEntityConfiguration : IEntityTypeConfiguration<LanguageEntity>
{
    public void Configure(EntityTypeBuilder<LanguageEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Table mapping with standardized naming
        builder.ToTable("Languages");

        // Primary key
        builder.HasKey(e => e.Id);

        // Properties with validation
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(20);

        // Performance indexes with standardized naming and uniqueness constraints
        builder.HasIndex(e => e.Code)
            .IsUnique()
            .HasDatabaseName("IX_Languages_Code");

        builder.HasIndex(e => e.Name)
            .IsUnique()
            .HasDatabaseName("IX_Languages_Name");
    }
}
