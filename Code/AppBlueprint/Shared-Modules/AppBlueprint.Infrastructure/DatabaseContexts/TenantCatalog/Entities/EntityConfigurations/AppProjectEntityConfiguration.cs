using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.TenantCatalog.Entities.EntityConfigurations;

public sealed class AppProjectEntityConfiguration : BaseEntityConfiguration<AppProjectEntity>
{
    public override void Configure(EntityTypeBuilder<AppProjectEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        
        // Apply base configuration including named soft delete filter
        base.Configure(builder);
builder.ToTable("AppProjects");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).IsRequired().HasMaxLength(100);

        builder.HasMany<CustomerEntity>();
    }
}
