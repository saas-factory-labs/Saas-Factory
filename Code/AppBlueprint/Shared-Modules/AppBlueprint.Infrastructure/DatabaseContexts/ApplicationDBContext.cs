using System.Linq.Expressions;
using AppBlueprint.Application.Attributes;
using AppBlueprint.Infrastructure.DatabaseContexts.B2C;
using AppBlueprint.Infrastructure.Services;
using AppBlueprint.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.DatabaseContexts;

public class ApplicationDbContext : B2CdbContext
{
    private readonly ITenantContextAccessor? _tenantContextAccessor;

    // Public constructor for direct DI registration
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor,
        ILogger<ApplicationDbContext> logger,
        ITenantContextAccessor tenantContextAccessor
    ) : base((DbContextOptions)options, configuration, logger)
    {
        _tenantContextAccessor = tenantContextAccessor;
    }

    // Protected constructor for derived module DbContext classes
    protected ApplicationDbContext(
        DbContextOptions options,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor,
        ILogger<ApplicationDbContext> logger,
        ITenantContextAccessor? tenantContextAccessor = null
    ) : base(options, configuration, logger)
    {
        _tenantContextAccessor = tenantContextAccessor;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        base.OnModelCreating(modelBuilder);

        ConfigureGdprDataClassification(modelBuilder);
        ConfigureSoftDeleteFilters(modelBuilder);
        ConfigureTenantQueryFilters(modelBuilder);
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

    /// <summary>
    /// Configures Named Query Filters for automatic tenant isolation.
    /// This is Layer 1 of defense-in-depth (application-level).
    /// Layer 2 is PostgreSQL Row-Level Security (database-level).
    /// 
    /// CRITICAL SECURITY: These filters prevent data leaks at the application level.
    /// Even if developers forget to add tenant filters, EF Core applies them automatically.
    /// 
    /// Named Query Filters (.NET 10) combine multiple filters using AND logic:
    /// - Soft delete filter: !IsSoftDeleted
    /// - Tenant filter: TenantId == currentTenantId
    /// 
    /// Result query: WHERE !IsSoftDeleted AND TenantId = @p0
    /// </summary>
    private void ConfigureTenantQueryFilters(ModelBuilder modelBuilder)
    {
        if (_tenantContextAccessor is null)
        {
            // No tenant context available (e.g., during migrations)
            // Skip tenant filtering - only soft delete filters will apply
            return;
        }

        // Configure tenant isolation for all tenant-scoped entities
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(ITenantScoped).IsAssignableFrom(entityType.ClrType))
                continue;

            // Build expression: entity => entity.TenantId == _tenantContextAccessor.TenantId
            var parameter = Expression.Parameter(entityType.ClrType, "entity");
            var tenantIdProperty = Expression.Property(parameter, nameof(ITenantScoped.TenantId));
            
            // Get current tenant ID from accessor
            var tenantContextAccessor = Expression.Constant(_tenantContextAccessor);
            var currentTenantId = Expression.Property(tenantContextAccessor, nameof(ITenantContextAccessor.TenantId));
            
            // Create comparison: entity.TenantId == currentTenantId
            var comparison = Expression.Equal(tenantIdProperty, currentTenantId);
            var lambda = Expression.Lambda(comparison, parameter);

            // Apply named query filter (combines with existing filters using AND)
            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
        }
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
