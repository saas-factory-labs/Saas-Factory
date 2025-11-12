using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing.EntityConfigurations;

/// <summary>
/// Entity configuration for AddressEntity defining table structure, relationships, and constraints.
/// Manages address information with geographic and postal data.
/// </summary>
public sealed class AddressEntityConfiguration : BaseEntityConfiguration<AddressEntity>
{
    public override void Configure(EntityTypeBuilder<AddressEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        
        // Apply base configuration including named soft delete filter
        base.Configure(builder);
// Table Mapping
        builder.ToTable("Addresses");

        // Primary Key
        builder.HasKey(e => e.Id);

        // Properties
        builder.Property(e => e.StreetNumber)
            .IsRequired()
            .HasMaxLength(10)
            .HasAnnotation("SensitiveData", true);

        builder.Property(e => e.Floor)
            .HasMaxLength(10);

        builder.Property(e => e.Latitude)
            .HasMaxLength(20);

        builder.Property(e => e.Longitude)
            .HasMaxLength(20);

        builder.Property(e => e.UnitNumber)
            .HasMaxLength(10)
            .HasAnnotation("SensitiveData", true);

        builder.Property(e => e.State)
            .HasMaxLength(100);

        builder.Property(e => e.PostalCode)
            .IsRequired()
            .HasMaxLength(20);

        // Relationships
        builder.HasOne(e => e.City)
            .WithMany()
            .HasForeignKey(e => e.CityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Country)
            .WithMany()
            .HasForeignKey(e => e.CountryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Street)
            .WithMany()
            .HasForeignKey(e => e.StreetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Customer)
            .WithMany()
            .HasForeignKey(e => e.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Tenant)
            .WithMany()
            .HasForeignKey(e => e.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        // Adding indexes
        builder.HasIndex(e => e.CityId);
        builder.HasIndex(e => e.CountryId);
        builder.HasIndex(e => e.PostalCode);
    }
}
