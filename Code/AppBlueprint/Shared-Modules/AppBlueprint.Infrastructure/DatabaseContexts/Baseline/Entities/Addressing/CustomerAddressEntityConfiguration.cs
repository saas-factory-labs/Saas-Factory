using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.CustomerAddress;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

public class CustomerAddressEntityConfiguration : IEntityTypeConfiguration<CustomerAddressEntity>
{
    public void Configure(EntityTypeBuilder<CustomerAddressEntity> builder)
    {
        // Define table name (if it needs to be different from default)
        builder.ToTable("CustomerAddresss");

        // Define primary key
        builder.HasKey(e => e.Id); // Assuming the entity has an "Id" property

        // Define properties
        // builder.Property(e => e)
        //     .IsRequired()           // Example property requirement
        //     .HasMaxLength(100);     // Example max length

        // Define relationships
        // Add relationships as needed, for example:
        // builder.HasMany(e => e.RelatedEntities)
        //        .WithOne(re => re.CustomerAddressEntity)
        //        .HasForeignKey(re => re.CustomerAddressEntityId)
        //        .OnDelete(DeleteBehavior.Cascade);
    }
}
