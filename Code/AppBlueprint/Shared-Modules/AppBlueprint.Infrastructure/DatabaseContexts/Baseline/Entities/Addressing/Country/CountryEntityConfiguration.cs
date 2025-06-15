using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing.EntityConfigurations;

/// <summary>
/// Entity configuration for CountryEntity defining table structure, relationships, and constraints.
/// Configures the geographic hierarchy for Country→State→City relationships.
/// </summary>
public sealed class CountryEntityConfiguration : IEntityTypeConfiguration<CountryEntity>
{
    public void Configure(EntityTypeBuilder<CountryEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Table mapping with standardized naming
        builder.ToTable("Countries");

        // Primary key
        builder.HasKey(e => e.Id);

        // Properties with proper validation
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.IsoCode)
            .IsRequired()
            .HasMaxLength(3); // ISO 3166-1 alpha-3 codes

        builder.Property(e => e.CityId)
            .HasMaxLength(1024);

        // Geographic hierarchy relationships
        builder.HasOne(e => e.GlobalRegion)
            .WithMany(gr => gr.Countries)
            .HasForeignKey(e => e.GlobalRegionId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Countries_GlobalRegions_GlobalRegionId");

        builder.HasMany<CityEntity>()
            .WithOne(c => c.Country)
            .HasForeignKey(c => c.CountryId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Cities_Countries_CountryId");

        // Performance indexes for geographic queries
        builder.HasIndex(e => e.Name)
            .IsUnique()
            .HasDatabaseName("IX_Countries_Name");

        builder.HasIndex(e => e.IsoCode)
            .IsUnique()
            .HasDatabaseName("IX_Countries_IsoCode");

        builder.HasIndex(e => e.GlobalRegionId)
            .HasDatabaseName("IX_Countries_GlobalRegionId");
    }
}
