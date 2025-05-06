using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization.UserRole;

public class UserRoleEntityConfiguration : IEntityTypeConfiguration<UserRoleEntity>
{
    public void Configure(EntityTypeBuilder<UserRoleEntity> builder)
    {
        // Define table name (if it needs to be different from default)
        builder.ToTable("UserRoles");

        // Define primary key
        builder.HasKey(e => e.Id); // Assuming the entity has an "Id" property


        // Define properties
        // builder.Property(e => e)
        //     .IsRequired()           // Example property requirement
        //     .HasMaxLength(100);     // Example max length

        // Define relationships
        // Add relationships as needed, for example:
        // builder.HasMany(e => e.RelatedEntities)
        //        .WithOne(re => re.UserRoleEntity)
        //        .HasForeignKey(re => re.UserRoleEntityId)
        //        .OnDelete(DeleteBehavior.Cascade);
    }
}
