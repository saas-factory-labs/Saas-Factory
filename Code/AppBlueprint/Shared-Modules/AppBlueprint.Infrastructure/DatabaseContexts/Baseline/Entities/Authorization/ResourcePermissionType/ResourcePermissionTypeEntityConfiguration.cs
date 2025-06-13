using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization.ResourcePermissionType;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization.EntityConfigurations;

public class ResourcePermissionTypeEntityConfiguration : IEntityTypeConfiguration<ResourcePermissionTypeEntity>
{
    public void Configure(EntityTypeBuilder<ResourcePermissionTypeEntity> builder)
    {
        builder.ToTable("ResourcePermissionTypes");

        builder.HasKey(e => e.Id);
    }
}

