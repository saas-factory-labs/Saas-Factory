using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Tenant.Tenant;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.Repositories;

public class TenantRepository : ITenantRepository
{
    private readonly ApplicationDbContext _context;

    public TenantRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TenantEntity>> GetAllAsync()
    {
        return await _context.Set<TenantEntity>().ToListAsync();
    }

    public async Task<TenantEntity> GetByIdAsync(int id)
    {
        return await _context.Set<TenantEntity>().FindAsync(id);
    }

    public async Task AddAsync(TenantEntity tenant)
    {
        await _context.Set<TenantEntity>().AddAsync(tenant);
    }

    public void Update(TenantEntity tenant)
    {
        _context.Set<TenantEntity>().Update(tenant);
    }

    public void Delete(int id)
    {
        TenantEntity? tenant = _context.Set<TenantEntity>().Find(id);
        if (tenant is not null) _context.Set<TenantEntity>().Remove(tenant);
    }
}
