using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

public class PermissionEntityConfiguration : IEntityTypeConfiguration<PermissionEntity>
{
    public void Configure(EntityTypeBuilder<PermissionEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Define table name (if it needs to be different from default)
        // builder.ToTable("Permissions");

        builder.ToTable("RolePermissions");

        // Define primary key
        builder.HasKey(e => e.Id); // Assuming the entity has an "Id" property

        // Define properties
        builder.Property(e => e.Name)
            .IsRequired() // Example property requirement
            .HasMaxLength(100); // Example max length

        // Define relationships
        // Add relationships as needed, for example:
        // builder.HasMany(e => e.RelatedEntities)
        //        .WithOne(re => re.PermissionEntity)
        //        .HasForeignKey(re => re.PermissionEntityId)
        //        .OnDelete(DeleteBehavior.Cascade);
    }
}