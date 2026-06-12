using AppBlueprint.Infrastructure.Persistence.DatabaseContexts;
using AppBlueprint.Infrastructure.Persistence.DatabaseContexts.Baseline.Entities.Tenant;
using AppBlueprint.Infrastructure.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository for tenant entities using DbContextFactory pattern for thread-safe Blazor Server access.
/// </summary>
public class TenantRepository : ITenantRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public TenantRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        ArgumentNullException.ThrowIfNull(contextFactory);
        _contextFactory = contextFactory;
    }

    public async Task<IEnumerable<TenantEntity>> GetAllAsync()
    {
        await using ApplicationDbContext context = await _contextFactory.CreateDbContextAsync();
        return await context.Set<TenantEntity>().ToListAsync();
    }

    public async Task<TenantEntity?> GetByIdAsync(string id)
    {
        ArgumentNullException.ThrowIfNull(id);
        await using ApplicationDbContext context = await _contextFactory.CreateDbContextAsync();
        return await context.Set<TenantEntity>().FindAsync(id);
    }

    public async Task AddAsync(TenantEntity tenant)
    {
        ArgumentNullException.ThrowIfNull(tenant);
        await using ApplicationDbContext context = await _contextFactory.CreateDbContextAsync();
        await context.Set<TenantEntity>().AddAsync(tenant);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TenantEntity tenant)
    {
        ArgumentNullException.ThrowIfNull(tenant);
        await using ApplicationDbContext context = await _contextFactory.CreateDbContextAsync();
        context.Set<TenantEntity>().Update(tenant);
        await context.SaveChangesAsync();
    }

    public void Update(TenantEntity tenant)
    {
        throw new NotSupportedException(
            "Synchronous Update is not supported with DbContextFactory pattern. Use UpdateAsync instead.");
    }

    public async Task DeleteAsync(string id)
    {
        ArgumentNullException.ThrowIfNull(id);
        await using ApplicationDbContext context = await _contextFactory.CreateDbContextAsync();
        TenantEntity? tenant = await context.Set<TenantEntity>().FindAsync(id);
        if (tenant is not null)
        {
            context.Set<TenantEntity>().Remove(tenant);
            await context.SaveChangesAsync();
        }
    }

    public void Delete(string id)
    {
        throw new NotSupportedException(
            "Synchronous Delete is not supported with DbContextFactory pattern. Use DeleteAsync instead.");
    }
}
