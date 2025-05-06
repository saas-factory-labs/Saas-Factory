using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Integration.EntityConfigurations;

public class IntegrationEntityConfiguration : IEntityTypeConfiguration<IntegrationEntity>
{
    public void Configure(EntityTypeBuilder<IntegrationEntity> builder)
    {
        // Define table name (if it needs to be different from default)
        builder.ToTable("Integrations");

        // Define primary key
        builder.HasKey(e => e.Id); // Assuming the entity has an "Id" property

        // Define properties
        builder.Property(e => e.Name)
            .IsRequired() // Example property requirement
            .HasMaxLength(100); // Example max length

        // Define relationships
        // Add relationships as needed, for example:
        // builder.HasMany(e => e.RelatedEntities)
        //        .WithOne(re => re.IntegrationEntity)
        //        .HasForeignKey(re => re.IntegrationEntityId)
        //        .OnDelete(DeleteBehavior.Cascade);
    }
}
