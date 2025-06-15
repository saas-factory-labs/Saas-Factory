using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization.ResourcePermissionType;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

/// <summary>
/// Entity configuration for ResourcePermissionTypeEntity defining table structure, relationships, and constraints.
/// Manages resource permission type definitions and authorization rules.
/// </summary>
public sealed class ResourcePermissionTypeEntityConfiguration : IEntityTypeConfiguration<ResourcePermissionTypeEntity>
{
    public void Configure(EntityTypeBuilder<ResourcePermissionTypeEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("ResourcePermissionTypes");

        builder.HasKey(e => e.Id);

        // builder.HasIndex(e => new { e.ResourcePermissionId, e.Permission })
        //        .IsUnique();

        // Add additional property/relationship configs here as needed
    }
}
