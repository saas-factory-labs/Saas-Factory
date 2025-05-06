// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;

// namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

// public class EmailVerificationEntityConfiguration : IEntityTypeConfiguration<EmailVerificationEntity>
// {
//     public void Configure(EntityTypeBuilder<EmailVerificationEntity> builder)
//     {
//         // Define table name (if it needs to be different from default)
//         builder.ToTable("EmailVerifications");

//         // Define primary key
//         builder.HasKey(e => e.Id); // Assuming the entity has an "Id" property

//         // Define properties
//         builder.Property(e => e.Token)
//             .IsRequired()           // Example property requirement
//             .HasMaxLength(100)      // Example max length
//             .HasAnnotation("SensitiveData", true); // Handle SensitiveData attribute

//         // Define relationships
//         // Add relationships as needed, for example:
//         // builder.HasMany(e => e.RelatedEntities)
//         //        .WithOne(re => re.EmailVerificationEntity)
//         //        .HasForeignKey(re => re.EmailVerificationEntityId)
//         //        .OnDelete(DeleteBehavior.Cascade);
//     }
// }



