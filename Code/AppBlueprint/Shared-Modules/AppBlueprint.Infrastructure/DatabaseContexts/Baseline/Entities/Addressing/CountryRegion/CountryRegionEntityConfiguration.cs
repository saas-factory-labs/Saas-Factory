using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

/// <summary>
/// Entity configuration for CountryRegionEntity defining geographical regions within countries.
/// Examples: Syddanmark, Midtjylland, Nordjylland, Sjælland, Hovedstaden for Denmark.
/// </summary>
public sealed class CountryRegionEntityConfiguration : BaseEntityConfiguration<CountryRegionEntity>
{
    public override void Configure(EntityTypeBuilder<CountryRegionEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        
        // Apply base configuration including named soft delete filter
        base.Configure(builder);
// Define table name
        builder.ToTable("CountryRegions");

        // Define primary key
        builder.HasKey(e => e.Id)
            .HasName("PK_CountryRegions");

        // Configure Id property - using ULID pattern
        builder.Property(e => e.Id)
            .HasMaxLength(40)
            .IsRequired()
            .HasComment("Primary key for country region using ULID format");

        // Configure Name property - populated from dictionary at startup
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("Name of the region within the country (e.g., Syddanmark, Midtjylland)");

        // Configure foreign key property
        builder.Property(e => e.CountryId)
            .HasMaxLength(40)
            .IsRequired()
            .HasComment("Foreign key to the country this region belongs to");

        // BaseEntity properties
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.LastUpdatedAt);

        builder.Property(e => e.IsSoftDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Configure relationship to Country
        builder.HasOne(cr => cr.Country)
            .WithMany() // CountryEntity doesn't have Regions navigation property
            .HasForeignKey(cr => cr.CountryId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_CountryRegions_Countries_CountryId");

        // Create indexes for performance
        builder.HasIndex(cr => cr.CountryId)
            .HasDatabaseName("IX_CountryRegions_CountryId")
            .HasFilter(null);

        builder.HasIndex(cr => cr.Name)
            .HasDatabaseName("IX_CountryRegions_Name")
            .HasFilter(null);

        // Ensure unique region names within each country
        builder.HasIndex(cr => new { cr.CountryId, cr.Name })
            .IsUnique()
            .HasDatabaseName("IX_CountryRegions_CountryId_Name_Unique")
            .HasFilter(null);
    }
}
