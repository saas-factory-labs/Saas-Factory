using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;

public class CustomerEntityConfiguration : IEntityTypeConfiguration<CustomerEntity>
{
    public void Configure(EntityTypeBuilder<CustomerEntity> builder)
    {
        // Define table name (if it needs to be different from default)
        builder.ToTable("Customers");

        // Define primary key
        builder.HasKey(e => e.Id); // Assuming the entity has an "Id" property

        // Define properties
        builder.Property(e => e.Type)
            .IsRequired() // Example property requirement
            .HasMaxLength(100); // Example max length

        // Handle SensitiveData attribute
        builder.Property(e => e.VatNumber)
            .HasAnnotation("SensitiveData", true);

        // builder.HasIndex(e => e.); // Example index
        // builder.hasIndex(e => e.PhoneNumber);

        // Define relationships
        // Add relationships as needed, for example:
        // builder.HasMany(e => e.RelatedEntities)
        //        .WithOne(re => re.CustomerEntity)
        //        .HasForeignKey(re => re.CustomerEntityId)
        //        .OnDelete(DeleteBehavior.Cascade);
    }
}
