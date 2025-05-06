using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

public class CountryRegionEntityConfiguration : IEntityTypeConfiguration<CountryRegionEntity>
{
    public void Configure(EntityTypeBuilder<CountryRegionEntity> builder)
    {
        // Define table name (if it needs to be different from default)
        builder.ToTable("CountryRegions");

        // Define primary key
        builder.HasKey(e => e.Id); // Assuming the entity has an "Id" property

        // Define properties
        builder.Property(e => e.Name)
            .IsRequired() // Example property requirement
            .HasMaxLength(100); // Example max length

        // Define relationships
        // Add relationships as needed, for example:
        // builder.HasMany(e => e.RelatedEntities)
        //        .WithOne(re => re.CountryRegionEntity)
        //        .HasForeignKey(re => re.CountryRegionEntityId)
        //        .OnDelete(DeleteBehavior.Cascade);
    }
}
