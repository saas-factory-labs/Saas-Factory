using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Admin;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.Repositories;

public class AdminRepository : IAdminRepository
{
    private readonly ApplicationDbContext _context;

    public AdminRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AdminEntity>> GetAllAsync()
    {
        return await _context.Admins.ToListAsync();
    }

    public async Task<AdminEntity?> GetByIdAsync(string id)
    {
        return await _context.Admins.FindAsync(id);
    }

    public async Task AddAsync(AdminEntity admin)
    {
        await _context.Admins.AddAsync(admin);
    }

    public void Update(AdminEntity admin)
    {
        _context.Admins.Update(admin);
    }

    public void Delete(int id)
    {
        AdminEntity? admin = _context.Admins.Find(id);
        if (admin is not null) _context.Admins.Remove(admin);
    }
}
