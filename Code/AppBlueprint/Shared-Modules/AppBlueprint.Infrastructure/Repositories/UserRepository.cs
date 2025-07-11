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
        return await _context.Users.ToListAsync();
    }
    public async Task<UserEntity?> GetByIdAsync(string id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<UserEntity?> GetByEmailAsync(string? email)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task AddAsync(UserEntity user)
    {
        await _context.Users.AddAsync(user);
    }

    public void Update(UserEntity user)
    {
        _context.Users.Update(user);
    }
    public void Delete(string id)
    {
        UserEntity? user = _context.Users.Find(id);
        if (user is not null) _context.Users.Remove(user);
    }
}
