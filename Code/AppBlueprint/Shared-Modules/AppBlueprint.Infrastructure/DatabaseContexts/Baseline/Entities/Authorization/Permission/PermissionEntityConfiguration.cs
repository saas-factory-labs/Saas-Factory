using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization.EntityConfigurations;

public class PermissionEntityConfiguration : IEntityTypeConfiguration<PermissionEntity>
{
    public void Configure(EntityTypeBuilder<PermissionEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Define table name (if it needs to be different from default)
        builder.ToTable("Permissions");

        builder.HasKey(e => e.Id);

        // Define properties
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Define relationships
        // Add relationships as needed, for example:
        // builder.HasMany(e => e.RelatedEntities)
        //        .WithOne(re => re.PermissionEntity)
        //        .HasForeignKey(re => re.PermissionEntityId)
        //        .OnDelete(DeleteBehavior.Cascade);
    }
}
