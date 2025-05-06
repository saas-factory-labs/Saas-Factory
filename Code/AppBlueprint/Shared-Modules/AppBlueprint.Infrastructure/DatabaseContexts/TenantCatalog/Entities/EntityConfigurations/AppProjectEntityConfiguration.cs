using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.TenantCatalog.Entities.EntityConfigurations;

public class AppProjectEntityConfiguration : IEntityTypeConfiguration<AppProjectEntity>
{
    public void Configure(EntityTypeBuilder<AppProjectEntity> builder)
    {
        builder.ToTable("AppProjects");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).IsRequired().HasMaxLength(100);

        builder.HasMany<CustomerEntity>();
    }
}
