using AppBlueprint.Infrastructure.Persistence.DatabaseContexts;
using AppBlueprint.Infrastructure.Persistence.DatabaseContexts.Baseline.Entities.Authorization.Permission;
using AppBlueprint.Infrastructure.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.Persistence.Repositories;

public class PermissionRepository : IPermissionRepository
{
    private readonly ApplicationDbContext _context;

    public PermissionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PermissionEntity>> GetAllAsync()
    {
        return await _context.Permissions.ToListAsync();
    }
    public async Task<PermissionEntity?> GetByIdAsync(string id)
    {
        return await _context.Permissions.FindAsync(id);
    }

    public async Task AddAsync(PermissionEntity permission)
    {
        await _context.Permissions.AddAsync(permission);
    }

    public void Update(PermissionEntity permission)
    {
        _context.Permissions.Update(permission);
    }

    public void Delete(string id)
    {
        PermissionEntity? permission = _context.Permissions.Find(id);
        if (permission is not null) _context.Permissions.Remove(permission);
    }
}
