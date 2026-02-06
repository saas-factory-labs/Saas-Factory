using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization.Role;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly ApplicationDbContext _context;

    public RoleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RoleEntity>> GetAllAsync()
    {
        return await _context.Roles.ToListAsync();
    }
    public async Task<RoleEntity?> GetByIdAsync(string id)
    {
        return await _context.Roles.FindAsync(id);
    }

    public async Task AddAsync(RoleEntity role)
    {
        await _context.Roles.AddAsync(role);
    }

    public void Update(RoleEntity role)
    {
        _context.Roles.Update(role);
    }

    public void Delete(string id)
    {
        RoleEntity? role = _context.Roles.Find(id);
        if (role is not null) _context.Roles.Remove(role);
    }
}
