using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2B;

public partial class B2BDbContext : BaselineDbContext
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public B2BDbContext(DbContextOptions options, IConfiguration configuration)
        : base(options, configuration)
    {
        _configuration = configuration;
        _connectionString = configuration.GetConnectionString("SUPABASE_POSTGRESQL")
                           ?? throw new InvalidOperationException("Missing connection string.");
    }

    public DbSet<ApiKeyEntity> ApiKeys { get; set; }
    public DbSet<OrganizationEntity> Organizations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseNpgsql(_connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ApiKeyEntityConfiguration());
        modelBuilder.ApplyConfiguration(new OrganizationEntityConfiguration());

        OnModelCreating_Team(modelBuilder);
        OnModelCreating_Tenant(modelBuilder);
    }

    partial void OnModelCreating_Tenant(ModelBuilder modelBuilder);
    partial void OnModelCreating_Team(ModelBuilder modelBuilder);
}
