using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization.EntityConfigurations;

public class PermissionRoleEntityConfiguration : IEntityTypeConfiguration<PermissionRoleEntity>
{
    public void Configure(EntityTypeBuilder<PermissionRoleEntity> builder)
    {
        builder.ToTable("PermissionRoles");

        builder.HasKey(e => e.Id);

        // Define relationships
        // Add relationships as needed, for example:
        // builder.HasMany(e => e.RelatedEntities)
        //        .WithOne(re => re.PermissionRoleEntity)
        //        .HasForeignKey(re => re.PermissionRoleEntityId)
        //        .OnDelete(DeleteBehavior.Cascade);
    }
}

