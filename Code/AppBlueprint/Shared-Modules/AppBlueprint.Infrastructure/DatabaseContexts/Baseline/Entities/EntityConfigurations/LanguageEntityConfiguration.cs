using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

public class LanguageEntityConfiguration : IEntityTypeConfiguration<LanguageEntity>
{
    public void Configure(EntityTypeBuilder<LanguageEntity> builder)
    {
        // Define table name (if it needs to be different from default)
        builder.ToTable("Languages");

        // Define primary key
        builder.HasKey(e => e.Id); // Assuming the entity has an "Id" property

        // Define properties
        builder.Property(e => e.Name)
            .IsRequired() // Example property requirement
            .HasMaxLength(200); // Example max length

        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(e => e.Code) // Example index
            .IsUnique();

        builder.HasIndex(e => e.Name) // Example index
            .IsUnique();


        // Define relationships
        // Add relationships as needed, for example:
        // builder.HasMany(e => e.RelatedEntities)
        //        .WithOne(re => re.LanguageEntity)
        //        .HasForeignKey(re => re.LanguageEntityId)
        //        .OnDelete(DeleteBehavior.Cascade);
    }
}
