using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

public class ApiLogEntityConfiguration : IEntityTypeConfiguration<ApiLogEntity>
{
    public void Configure(EntityTypeBuilder<ApiLogEntity> builder)
    {
        // Define table name (if it needs to be different from default)
        builder.ToTable("ApiLogs");

        // Define primary key
        builder.HasKey(e => e.Id); // Assuming the entity has an "Id" property

        // Define properties
        builder.Property(e => e.RequestMethod)
            .IsRequired() // Example property requirement
            .HasMaxLength(100); // Example max length

        // Define relationships
        // Add relationships as needed, for example:
        // builder.HasMany(e => e.RelatedEntities)
        //        .WithOne(re => re.ApiLogEntity)
        //        .HasForeignKey(re => re.ApiLogEntityId)
        //        .OnDelete(DeleteBehavior.Cascade);
    }
}
