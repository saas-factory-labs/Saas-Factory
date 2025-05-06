using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User.Person;

public class PersonEntityConfiguration : IEntityTypeConfiguration<
    PersonEntity>
{
    public void Configure(EntityTypeBuilder<PersonEntity> builder)
    {
        // Define table name (if it needs to be different from default)
        builder.ToTable("Persons");

        // Define primary key
        builder.HasKey(e => e.Id); // Assuming the entity has an "Id" property

        // Define properties
        builder.Property(e => e.FirstName)
            .IsRequired() // Example property requirement
            .HasMaxLength(100); // Example max length

        // Define relationships
        // Add relationships as needed, for example:
        // builder.HasMany(e => e.RelatedEntities)
        //        .WithOne(re => re.PersonEntity)
        //        .HasForeignKey(re => re.PersonEntityId)
        //        .OnDelete(DeleteBehavior.Cascade);
    }
}
