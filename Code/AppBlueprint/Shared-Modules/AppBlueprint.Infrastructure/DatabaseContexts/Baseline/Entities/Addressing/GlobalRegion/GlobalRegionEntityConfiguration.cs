using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing.Region;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing.EntityConfigurations;

public class GlobalRegionEntityConfiguration : IEntityTypeConfiguration<GlobalRegionEntity>
{
    public void Configure(EntityTypeBuilder<GlobalRegionEntity> builder)
    {
        builder.HasKey(a => a.Id);

        // builder.HasMany<CountryEntity>().WithOne(t => t.GlobalRegion).HasForeignKey(t => t.Id).HasPrincipalKey(c => c.Id);
    }
}
