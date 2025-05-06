using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization.ResourcePermissionType;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

public class ResourcePermissionTypeEntityConfiguration : IEntityTypeConfiguration<ResourcePermissionTypeEntity>
{
    public void Configure(EntityTypeBuilder<ResourcePermissionTypeEntity> builder)
    {
        builder.ToTable("ResourcePermissionTypes");

        builder.HasKey(e => e.Id);

        // builder.HasIndex(e => new { e.ResourcePermissionId, e.Permission })
        //        .IsUnique();

        // Add additional property/relationship configs here as needed
    }
}
