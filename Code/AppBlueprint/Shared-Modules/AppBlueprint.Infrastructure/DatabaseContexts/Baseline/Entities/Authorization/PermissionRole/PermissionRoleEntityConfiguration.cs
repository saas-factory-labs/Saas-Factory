using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

public class PermissionRoleEntityConfiguration : IEntityTypeConfiguration<PermissionRoleEntity>
{
    public void Configure(EntityTypeBuilder<PermissionRoleEntity> builder)
    {
        // Define table name (if it needs to be different from default)
        builder.ToTable("PermissionRoles");

        // Define primary key
        builder.HasKey(e => e.Id); // Assuming the entity has an "Id" property

        // Define properties
        // builder.Property(e => e.)
        //     .IsRequired()           // Example property requirement
        //     .HasMaxLength(100);     // Example max length

        // Define relationships
        // Add relationships as needed, for example:
        // builder.HasMany(e => e.RelatedEntities)
        //        .WithOne(re => re.PermissionRoleEntity)
        //        .HasForeignKey(re => re.PermissionRoleEntityId)
        //        .OnDelete(DeleteBehavior.Cascade);
    }
}
