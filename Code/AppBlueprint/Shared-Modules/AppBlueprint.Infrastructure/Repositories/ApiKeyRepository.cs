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
        return await _context.ApiKeys.ToListAsync();
    }

    public async Task<ApiKeyEntity?> GetByIdAsync(string id)
    {
        return await _context.ApiKeys.FindAsync(id);
    }

    public async Task AddAsync(ApiKeyEntity apiKey)
    {
        await _context.ApiKeys.AddAsync(apiKey);
    }
    public void Update(ApiKeyEntity apiKey)
    {
        _context.ApiKeys.Update(apiKey);
    }

    public void Delete(string id)
    {
        ApiKeyEntity? apiKey = _context.ApiKeys.Find(id);
        if (apiKey is not null) _context.ApiKeys.Remove(apiKey);
    }
}
