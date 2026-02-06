using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization.Permission;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.Repositories;

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
