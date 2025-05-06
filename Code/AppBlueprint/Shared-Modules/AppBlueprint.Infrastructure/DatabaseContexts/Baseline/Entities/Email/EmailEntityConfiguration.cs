using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Email;

public class EmailEntityConfiguration : IEntityTypeConfiguration<EmailEntity>
{
    public void Configure(EntityTypeBuilder<EmailEntity> builder)
    {
        // Define table name (if it needs to be different from default)
        builder.ToTable("Emails");

        // Define primary key
        builder.HasKey(e => e.Id); // Assuming the entity has an "Id" property

        // Define properties
        builder.Property(e => e.Address)
            .IsRequired() // Example property requirement
            .HasMaxLength(100); // Example max length

        // builder.HasIndex(e => e.Address).IsUnique; // Example index

        // Define relationships
        // Add relationships as needed, for example:
        // builder.HasMany(e => e.RelatedEntities)
        //        .WithOne(re => re.EmailEntity)
        //        .HasForeignKey(re => re.EmailEntityId)
        //        .OnDelete(DeleteBehavior.Cascade);
    }
}
