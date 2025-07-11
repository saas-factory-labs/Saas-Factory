using System.Linq.Expressions;
using AppBlueprint.Application.Attributes;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Organization;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.Team;
using AppBlueprint.Infrastructure.DatabaseContexts.B2C;
using AppBlueprint.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.DatabaseContexts;

public class ApplicationDbContext : B2CdbContext
{
    public ApplicationDbContext(
        DbContextOptions options,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor,
        ILogger<B2CdbContext> logger
    ) : base(options, configuration, logger)
    {
    }

    #region B2B DbSets
    public DbSet<ApiKeyEntity> ApiKeys { get; set; }
    public DbSet<B2B.Entities.Organization.OrganizationEntity> Organizations { get; set; }
    public DbSet<TeamEntity> Teams { get; set; }
    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        base.OnModelCreating(modelBuilder);

        ConfigureGdprDataClassification(modelBuilder);
        ConfigureSoftDeleteFilters(modelBuilder);

        // Optional: add multi-tenancy query filters here when ready
    }

    private static void ConfigureGdprDataClassification(ModelBuilder modelBuilder)
    {
        // Mark properties with [DataClassification] as sensitive
        foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (IMutableProperty property in entityType.GetProperties())
            {
                var sensitiveDataAttribute = property.PropertyInfo?
                    .GetCustomAttributes(typeof(DataClassificationAttribute), false)
                    .FirstOrDefault();

                if (sensitiveDataAttribute is not null)
                {
                    property.SetAnnotation("SensitiveData", true);
                }
            }
        }
    }

    private static void ConfigureSoftDeleteFilters(ModelBuilder modelBuilder)
    {
        // Configure soft delete filters for entities that implement IEntity
        modelBuilder.Model.GetEntityTypes()
            .Where(et => typeof(IEntity).IsAssignableFrom(et.ClrType))
            .ToList()
            .ForEach(entityType =>
            {
                var queryFilter = CreateIsNotSoftDeletedFilter(entityType.ClrType);
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(queryFilter);
            });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<IEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                // Set to null for new entities - will be populated on first update
                entry.Entity.LastUpdatedAt = null;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.LastUpdatedAt = now;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
    private static LambdaExpression CreateIsNotSoftDeletedFilter(Type entityType)
    {
        var parameter = Expression.Parameter(entityType, "e");
        var property = Expression.Property(parameter, nameof(IEntity.IsSoftDeleted));
        var condition = Expression.Equal(property, Expression.Constant(false));
        return Expression.Lambda(condition, parameter);
    }
}


