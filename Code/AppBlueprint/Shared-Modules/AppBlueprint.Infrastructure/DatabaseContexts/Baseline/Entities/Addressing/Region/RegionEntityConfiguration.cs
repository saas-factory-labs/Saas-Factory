//
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
//
// public class RegionEntityConfiguration : IEntityTypeConfiguration<RegionEntity>
// {
//     public void Configure(EntityTypeBuilder<RegionEntity> builder)
//     {
//         // Define table name (if it needs to be different from default)
//         builder.ToTable("Regions");
//
//         // Define primary key
//         builder.HasKey(e => e.Id); // Assuming the entity has an "Id" property
//
//         // Define properties
//         builder.Property(e => e.SomeProperty)
//             .IsRequired()           // Example property requirement
//             .HasMaxLength(100);     // Example max length
//
//         // Define relationships
//         // Add relationships as needed, for example:
//         // builder.HasMany(e => e.RelatedEntities)
//         //        .WithOne(re => re.RegionEntity)
//         //        .HasForeignKey(re => re.RegionEntityId)
//         //        .OnDelete(DeleteBehavior.Cascade);
//     }
// }
//



