using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing.EntityConfigurations;

/// <summary>
/// Entity configuration for StreetEntity defining table structure, relationships, and constraints.
/// Establishes the City→Street relationship in the geographic hierarchy.
/// </summary>
public sealed class StreetEntityConfiguration : BaseEntityConfiguration<StreetEntity>
{
    public override void Configure(EntityTypeBuilder<StreetEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        
        // Apply base configuration including named soft delete filter
        base.Configure(builder);
// Table mapping with standardized naming
        builder.ToTable("Streets");

        // Primary key
        builder.HasKey(e => e.Id);

        // Properties with validation
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.CityId)
            .IsRequired();

        // Relationships - City→Street hierarchy
        builder.HasOne(e => e.City)
            .WithMany()
            .HasForeignKey(e => e.CityId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Streets_Cities_CityId");

        // Performance indexes with standardized naming
        builder.HasIndex(e => e.CityId)
            .HasDatabaseName("IX_Streets_CityId");

        builder.HasIndex(e => e.Name)
            .HasDatabaseName("IX_Streets_Name");

        // Unique constraint for street name within city
        builder.HasIndex(e => new { e.CityId, e.Name })
            .IsUnique()
            .HasDatabaseName("IX_Streets_CityId_Name_Unique");
    }
}
