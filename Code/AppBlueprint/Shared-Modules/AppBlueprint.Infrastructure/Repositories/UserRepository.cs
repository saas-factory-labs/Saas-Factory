using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserEntity>> GetAllAsync()
    {
        return await _context.Set<UserEntity>().ToListAsync();
    }

    public async Task<UserEntity> GetByIdAsync(int id)
    {
        return await _context.Set<UserEntity>().FindAsync(id);
    }

    public async Task<UserEntity> GetByEmailAsync(string? email)
    {
        return await _context.Set<UserEntity>().FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task AddAsync(UserEntity user)
    {
        await _context.Set<UserEntity>().AddAsync(user);
    }

    public void Update(UserEntity user)
    {
        _context.Set<UserEntity>().Update(user);
    }

    public void Delete(int id)
    {
        UserEntity? user = _context.Set<UserEntity>().Find(id);
        if (user is not null) _context.Set<UserEntity>().Remove(user);
    }
}
