using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing.Region;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing.EntityConfigurations;

/// <summary>
/// Entity configuration for GlobalRegionEntity defining table structure, relationships, and constraints.
/// Configures the top-level geographic hierarchy for GlobalRegion→Country relationships.
/// </summary>
public sealed class GlobalRegionEntityConfiguration : IEntityTypeConfiguration<GlobalRegionEntity>
{
    public void Configure(EntityTypeBuilder<GlobalRegionEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Table mapping with standardized naming
        builder.ToTable("GlobalRegions");

        // Primary key
        builder.HasKey(e => e.Id);        // Properties with proper validation
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        // Note: Code property not configured as GlobalRegionEntity doesn't currently have Code property
        // TODO: Add Code property to GlobalRegionEntity if region codes are needed (e.g., "NA", "EU", "AS")

        // Geographic hierarchy relationships - GlobalRegion to Country (one-to-many)
        builder.HasMany(gr => gr.Countries)
            .WithOne(c => c.GlobalRegion)
            .HasForeignKey(c => c.GlobalRegionId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Countries_GlobalRegions_GlobalRegionId");        // Performance indexes for geographic queries
        builder.HasIndex(e => e.Name)
            .IsUnique()
            .HasDatabaseName("IX_GlobalRegions_Name");
    }
}
