using AppBlueprint.Application.Interfaces;
using AppBlueprint.Infrastructure.Services.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AppBlueprint.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering AppBlueprint search services.
/// </summary>
public static class SearchServiceCollectionExtensions
{
    /// <summary>
    /// Adds PostgreSQL full-text search service for a specific entity type.
    /// Automatically respects tenant isolation for ITenantScoped entities.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to enable search for (must have SearchVector column)</typeparam>
    /// <typeparam name="TDbContext">The DbContext containing the entity</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <example>
    /// // Register search for Users (using B2BDbContext)
    /// services.AddPostgreSqlFullTextSearch&lt;UserEntity, B2BDbContext&gt;();
    ///
    /// // Register search for Tenants (using BaselineDbContext)
    /// services.AddPostgreSqlFullTextSearch&lt;TenantEntity, BaselineDbContext&gt;();
    /// </example>
    public static IServiceCollection AddPostgreSqlFullTextSearch<TEntity, TDbContext>(this IServiceCollection services)
        where TEntity : class
        where TDbContext : DbContext
    {
        services.AddScoped<ISearchService<TEntity>, PostgreSqlSearchService<TEntity, TDbContext>>();

        Console.WriteLine($"[AppBlueprint.Infrastructure] PostgreSQL full-text search registered for {typeof(TEntity).Name}");

        return services;
    }
}
