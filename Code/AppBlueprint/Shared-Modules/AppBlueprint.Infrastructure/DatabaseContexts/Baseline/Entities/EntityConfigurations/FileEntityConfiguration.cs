using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.FileManagement.EntityConfigurations;

public class FileEntityConfiguration : IEntityTypeConfiguration<FileEntity>
{
    public void Configure(EntityTypeBuilder<FileEntity> builder)
    {
        // Define table name (if it needs to be different from default)
        builder.ToTable("Files");

        // Define primary key
        builder.HasKey(e => e.Id); // Assuming the entity has an "Id" property

        // Define properties
        builder.Property(e => e.FileName) // Example property
            .IsRequired() // Example property requirement
            .HasMaxLength(255) // Example max length
            .HasAnnotation("SensitiveData", true); // Handle SensitiveData attribute

        // Define relationships
        // Add relationships as needed, for example:
        // builder.HasMany(e => e.RelatedEntities)
        //        .WithOne(re => re.FileEntity)
        //        .HasForeignKey(re => re.FileEntityId)
        //        .OnDelete(DeleteBehavior.Cascade);
    }
}
