using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

public class StateEntityConfiguration : IEntityTypeConfiguration<StateEntity>
{
    public void Configure(EntityTypeBuilder<StateEntity> builder)
    {
        // Define table name (if it needs to be different from default)
        builder.ToTable("States");

        // Define primary key
        builder.HasKey(e => e.Id); // Assuming the entity has an "Id" property

        // Define properties
        builder.Property(e => e.Name)
            .IsRequired() // Example property requirement
            .HasMaxLength(100); // Example max length

        // Define relationships
        // Add relationships as needed, for example:
        // builder.HasMany(e => e.RelatedEntities)
        //        .WithOne(re => re.StateEntity)
        //        .HasForeignKey(re => re.StateEntityId)
        //        .OnDelete(DeleteBehavior.Cascade);
    }
}
