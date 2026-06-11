using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing.Address;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing.City;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing.Country;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing.CountryRegion;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing.GlobalRegion;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing.Region;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing.Street;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline;

public partial class BaselineDbContext
{
    public DbSet<GlobalRegionEntity> GlobalRegions { get; set; }
    public DbSet<CountryRegionEntity> CountryRegions { get; set; }
    public DbSet<AddressEntity> Addresses { get; set; }
    public DbSet<CountryEntity> Countries { get; set; }
    public DbSet<StreetEntity> Streets { get; set; }
    public DbSet<CityEntity> Cities { get; set; }

    partial void OnModelCreating_Addressing(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new GlobalRegionEntityConfiguration());
        modelBuilder.ApplyConfiguration(new AddressEntityConfiguration());
        modelBuilder.ApplyConfiguration(new CountryRegionEntityConfiguration());
        modelBuilder.ApplyConfiguration(new CountryEntityConfiguration());
        modelBuilder.ApplyConfiguration(new StreetEntityConfiguration());
        modelBuilder.ApplyConfiguration(new CityEntityConfiguration());
    }
}
