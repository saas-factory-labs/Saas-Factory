using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

public sealed class PhoneNumberEntityConfiguration : IEntityTypeConfiguration<PhoneNumberEntity>
{
    public void Configure(EntityTypeBuilder<PhoneNumberEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("PhoneNumbers");

        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.Id).IsUnique();

        builder.Property(e => e.Number)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.CountryCode)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(e => e.IsVerified)
            .IsRequired();

        builder.Property(e => e.IsPrimary)
            .IsRequired();

        // Define relationships here if needed
        // Example:
        // builder.HasOne(e => e.ContactPerson)
        //     .WithMany(cp => cp.PhoneNumbers)
        //     .HasForeignKey(e => e.ContactPersonId);
    }
}
