using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing.EntityConfigurations;

public class StreetEntityConfiguration : IEntityTypeConfiguration<StreetEntity>
{
    public void Configure(EntityTypeBuilder<StreetEntity> builder)
    {
        // Define table name (if it needs to be different from default)
        builder.ToTable("Streets");

        // Define primary key
        builder.HasKey(e => e.Id); // Assuming the entity has an "Id" property

        // Define properties
        builder.Property(e => e.Name)
            .IsRequired() // Example property requirement
            .HasMaxLength(100); // Example max length

        // Define relationships
        // Add relationships as needed, for example:
        // builder.HasMany(e => e.RelatedEntities)
        //        .WithOne(re => re.StreetEntity)
        //        .HasForeignKey(re => re.StreetEntityId)
        //        .OnDelete(DeleteBehavior.Cascade);
    }
}
