using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing.EntityConfigurations;

/// <summary>
/// Entity configuration for CityEntity defining table structure, relationships, and constraints.
/// Establishes the State→City relationship in the geographic hierarchy.
/// </summary>
public sealed class CityEntityConfiguration : IEntityTypeConfiguration<CityEntity>
{
    public void Configure(EntityTypeBuilder<CityEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Table mapping with standardized naming
        builder.ToTable("Cities");

        // Primary key - ULID as string
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .IsRequired()
            .HasMaxLength(40);

        // BaseEntity properties
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.LastUpdatedAt)
            .IsRequired();

        builder.Property(e => e.IsSoftDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Properties with validation
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.PostalCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.CountryId)
            .IsRequired()
            .HasMaxLength(40);

        builder.Property(e => e.StateId)
            .IsRequired()
            .HasMaxLength(40);

        // Relationships - Geographic hierarchy
        builder.HasOne(e => e.Country)
            .WithMany()
            .HasForeignKey(e => e.CountryId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Cities_Countries_CountryId");

        builder.HasOne(e => e.State)
            .WithMany()
            .HasForeignKey(e => e.StateId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Cities_States_StateId");

        // Performance indexes with standardized naming
        builder.HasIndex(e => e.CountryId)
            .HasDatabaseName("IX_Cities_CountryId");

        builder.HasIndex(e => e.StateId)
            .HasDatabaseName("IX_Cities_StateId");

        builder.HasIndex(e => e.Name)
            .HasDatabaseName("IX_Cities_Name");

        builder.HasIndex(e => e.PostalCode)
            .HasDatabaseName("IX_Cities_PostalCode");

        // Indexes for BaseEntity properties
        builder.HasIndex(e => e.IsSoftDeleted)
            .HasDatabaseName("IX_Cities_IsSoftDeleted");

        // Unique constraint for postal code within state
        builder.HasIndex(e => new { e.StateId, e.PostalCode })
            .IsUnique()
            .HasDatabaseName("IX_Cities_StateId_PostalCode_Unique");
    }
}
