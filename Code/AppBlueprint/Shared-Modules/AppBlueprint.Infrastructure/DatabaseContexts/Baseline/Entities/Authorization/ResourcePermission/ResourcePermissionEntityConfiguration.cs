using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

public class ResourcePermissionEntityConfiguration : IEntityTypeConfiguration<ResourcePermissionEntity>
{
    public void Configure(EntityTypeBuilder<ResourcePermissionEntity> builder)
    {
        builder.ToTable("ResourcePermissions");

        builder.HasKey(e => e.Id);

        // Relationships
        builder.HasOne(rp => rp.User)
            .WithMany(u => u.ResourcePermissions)
            .HasForeignKey(rp => rp.UserId);
    }
}
