using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.Repositories;

public class ApiKeyRepository : IApiKeyRepository
{
    private readonly ApplicationDbContext _context;

    public ApiKeyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ApiKeyEntity>> GetAllAsync()
    {
        return await _context.Set<ApiKeyEntity>().ToListAsync();
    }

    public async Task<ApiKeyEntity> GetByIdAsync(int id)
    {
        return await _context.Set<ApiKeyEntity>().FindAsync(id);
    }

    public async Task AddAsync(ApiKeyEntity apiKey)
    {
        await _context.Set<ApiKeyEntity>().AddAsync(apiKey);
    }

    public void Update(ApiKeyEntity apiKey)
    {
        _context.Set<ApiKeyEntity>().Update(apiKey);
    }

    public void Delete(int id)
    {
        ApiKeyEntity? apiKey = _context.Set<ApiKeyEntity>().Find(id);
        if (apiKey is not null) _context.Set<ApiKeyEntity>().Remove(apiKey);
    }
}
