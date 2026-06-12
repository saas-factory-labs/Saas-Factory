using System.Collections.Concurrent;
using AppBlueprint.AdminPortalKernel.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;

namespace AppBlueprint.AdminPortalKernel.Infrastructure;

/// <summary>
/// Singleton factory that caches one pooled <see cref="NpgsqlDataSource"/> (and the EF
/// options built on it) per module slug. Contexts default to NoTracking - the admin
/// portal is read-mostly and the single write path uses ExecuteUpdate.
/// </summary>
public sealed class AdminPortalDbContextFactory : IAdminPortalDbContextFactory, IAsyncDisposable
{
    private readonly IOptions<AdminPortalOptions> _options;
    private readonly ConcurrentDictionary<string, ModuleDatabase> _databasesBySlug = new(StringComparer.Ordinal);

    public AdminPortalDbContextFactory(IOptions<AdminPortalOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _options = options;
    }

    public AdminPortalAppDbContext CreateForModule(string slug)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);

        ModuleDatabase database = _databasesBySlug.GetOrAdd(slug, CreateModuleDatabase);
        return new AdminPortalAppDbContext(database.ContextOptions);
    }

    private ModuleDatabase CreateModuleDatabase(string slug)
    {
        if (!_options.Value.Modules.TryGetValue(slug, out AdminPortalModuleOptions? moduleOptions)
            || string.IsNullOrWhiteSpace(moduleOptions.ConnectionString))
        {
            throw new InvalidOperationException(
                $"No connection string configured for admin portal module '{slug}'. " +
                $"Set AdminPortal:Modules:{slug}:ConnectionString.");
        }

        NpgsqlDataSource dataSource = new NpgsqlDataSourceBuilder(moduleOptions.ConnectionString).Build();

        DbContextOptions<AdminPortalAppDbContext> contextOptions =
            new DbContextOptionsBuilder<AdminPortalAppDbContext>()
                .UseNpgsql(dataSource)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .Options;

        return new ModuleDatabase(dataSource, contextOptions);
    }

    public async ValueTask DisposeAsync()
    {
        foreach (ModuleDatabase database in _databasesBySlug.Values)
        {
            await database.DataSource.DisposeAsync();
        }

        _databasesBySlug.Clear();
    }

    private sealed record ModuleDatabase(
        NpgsqlDataSource DataSource,
        DbContextOptions<AdminPortalAppDbContext> ContextOptions);
}
