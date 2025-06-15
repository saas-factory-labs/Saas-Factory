using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.TenantCatalog.Entities.EntityConfigurations;

public sealed class AppProjectEntityConfiguration : IEntityTypeConfiguration<AppProjectEntity>
{    public void Configure(EntityTypeBuilder<AppProjectEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("AppProjects");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).IsRequired().HasMaxLength(100);

        builder.HasMany<CustomerEntity>();
    }
}
