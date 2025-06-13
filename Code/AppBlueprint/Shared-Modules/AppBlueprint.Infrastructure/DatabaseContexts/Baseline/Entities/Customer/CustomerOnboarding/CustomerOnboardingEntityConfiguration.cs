using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.CustomerOnboarding;

public class CustomerOnboardingEntityConfiguration : IEntityTypeConfiguration<CustomerOnboardingEntity>
{
    public void Configure(EntityTypeBuilder<CustomerOnboardingEntity> builder)
    {
        // Define table name (if it needs to be different from default)
        builder.ToTable("CustomerOnboardings");

        // Define primary key
        builder.HasKey(e => e.Id); // Assuming the entity has an "Id" property

        // Define properties
        // Add property configurations when the entity is extended

        // Define relationships
        // Add relationships as needed, for example:
        // builder.HasMany(e => e.RelatedEntities)
        //        .WithOne(re => re.CustomerOnboardingEntity)
        //        .HasForeignKey(re => re.CustomerOnboardingEntityId)
        //        .OnDelete(DeleteBehavior.Cascade);
    }
}
