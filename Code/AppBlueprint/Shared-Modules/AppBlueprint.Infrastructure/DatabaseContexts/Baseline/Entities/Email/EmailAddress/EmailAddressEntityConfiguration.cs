using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Email.EmailAddress;

public class EmailAddressEntityConfiguration : IEntityTypeConfiguration<EmailAddressEntity>
{
    public void Configure(EntityTypeBuilder<EmailAddressEntity> builder)
    {
        // Define table name (if it needs to be different from default)
        builder.ToTable("EmailAddresses");

        // Define primary key
        builder.HasKey(e => e.Id); // Assuming the entity has an "Id" property

        // Define properties
        builder.Property(e => e.Address) // Example property
            .IsRequired() // Example property requirement
            .HasMaxLength(100) // Example max length
            .HasAnnotation("SensitiveData", true); // Handle SensitiveData attribute

        // Define relationships
        // Add relationships as needed, for example:
        // builder.HasMany(e => e.RelatedEntities)
        //        .WithOne(re => re.AccountEntity)
        //        .HasForeignKey(re => re.AccountEntityId)
        //        .OnDelete(DeleteBehavior.Cascade);
    }
}
