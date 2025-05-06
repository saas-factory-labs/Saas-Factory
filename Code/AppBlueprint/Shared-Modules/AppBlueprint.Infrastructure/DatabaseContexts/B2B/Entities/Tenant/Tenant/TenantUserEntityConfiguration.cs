using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Tenant.Tenant;

public class TenantUserEntityConfiguration : IEntityTypeConfiguration<TenantUserEntity>
{
    public void Configure(EntityTypeBuilder<TenantUserEntity> builder)
    {
        // builder.ToTable("Teams");
        // builder.HasKey(t => t.Id);
        // builder.Property(t => t.TeamName).IsRequired().HasMaxLength(100);
        // builder.HasOne(t => t.Manager)
        //     .WithMany()
        //     .HasForeignKey(t => t.ManagerId)
        //     .OnDelete(DeleteBehavior.Restrict);
    }
}
