using System.Linq.Expressions;
using System.Reflection;
using AppBlueprint.Application.Enums;
using AppBlueprint.Infrastructure.DatabaseContexts.B2C;
using AppBlueprint.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace AppBlueprint.Infrastructure.DatabaseContexts;

public class ApplicationDbContext : B2CdbContext
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly int _tenantId = 1; // Replace with tenantProvider.GetTenantId() when ready

    public ApplicationDbContext(
        DbContextOptions options,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor
    ) : base(options, configuration)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Optional: You can add fallback logic or logging here
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Mark properties with [GDPRType] as sensitive
        foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (IMutableProperty property in entityType.GetProperties())
            {
                var sensitiveDataAttribute = property.PropertyInfo?
                    .GetCustomAttributes(typeof(GDPRType), false)
                    .FirstOrDefault();

                if (sensitiveDataAttribute != null)
                {
                    property.SetAnnotation("SensitiveData", true);
                }
            }

            if (typeof(IEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(ApplicationDbContext)
                    .GetMethod(nameof(SetSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)
                    ?.MakeGenericMethod(entityType.ClrType);

                method?.Invoke(null, new object[] { modelBuilder });
            }
        }
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(IEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(
                    CreateIsNotSoftDeletedFilter(entityType.ClrType));
            }
        }
        // Optional: add multi-tenancy query filters here when ready
    }

    private static void SetSoftDeleteFilter<T>(ModelBuilder modelBuilder) where T : class, IEntity
    {
        modelBuilder.Entity<T>().HasQueryFilter(e => !e.IsSoftDeleted);
    }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<IEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
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


