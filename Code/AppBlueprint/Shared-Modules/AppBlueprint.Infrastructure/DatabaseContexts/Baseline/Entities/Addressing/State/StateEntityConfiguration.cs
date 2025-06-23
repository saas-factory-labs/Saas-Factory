using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

/// <summary>
/// Entity configuration for StateEntity defining table structure, relationships, and constraints.
/// Establishes the Country→State relationship in the geographic hierarchy.
/// </summary>
public sealed class StateEntityConfiguration : IEntityTypeConfiguration<StateEntity>
{
    public void Configure(EntityTypeBuilder<StateEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Table mapping with standardized naming
        builder.ToTable("States");

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

        builder.Property(e => e.IsoCode)
            .HasMaxLength(10);

        builder.Property(e => e.CountryId)
            .IsRequired()
            .HasMaxLength(40);

        // Relationships - Country→State hierarchy
        builder.HasOne(e => e.Country)
            .WithMany()
            .HasForeignKey(e => e.CountryId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_States_Countries_CountryId");

        // Performance indexes with standardized naming
        builder.HasIndex(e => e.CountryId)
            .HasDatabaseName("IX_States_CountryId");

        builder.HasIndex(e => e.Name)
            .HasDatabaseName("IX_States_Name");

        builder.HasIndex(e => e.IsoCode)
            .HasDatabaseName("IX_States_IsoCode");

        // Indexes for BaseEntity properties
        builder.HasIndex(e => e.IsSoftDeleted)
            .HasDatabaseName("IX_States_IsSoftDeleted");

        // Unique constraint for ISO code within country
        builder.HasIndex(e => new { e.CountryId, e.IsoCode })
            .IsUnique()
            .HasDatabaseName("IX_States_CountryId_IsoCode_Unique");
    }
}
