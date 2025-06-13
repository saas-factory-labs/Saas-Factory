using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing.EntityConfigurations;

public class CountryEntityConfiguration : IEntityTypeConfiguration<CountryEntity>
{
    public void Configure(EntityTypeBuilder<CountryEntity> builder)
    {
        // Define table name (if it needs to be different from default)
        builder.ToTable("Countries");

        // Define primary key
        builder.HasKey(e => e.Id); // Assuming the entity has an "Id" property

        // Define properties
        builder.Property(e => e.Name)
            .IsRequired() // Example property requirement
            .HasMaxLength(100); // Example max length

        builder.Property(c => c.Name)
            .IsRequired().HasMaxLength(200);

        builder.Property(c => c.IsoCode)
            .IsRequired().HasMaxLength(2);

        builder.HasMany<CityEntity>();

        builder.HasIndex(e => e.Name) // Example index
            .IsUnique();

        // Define relationships
        // Add relationships as needed, for example:
        // builder.HasMany(e => e.RelatedEntities)
        //        .WithOne(re => re.CountryEntity)
        //        .HasForeignKey(re => re.CountryEntityId)
        //        .OnDelete(DeleteBehavior.Cascade);
    }
}
