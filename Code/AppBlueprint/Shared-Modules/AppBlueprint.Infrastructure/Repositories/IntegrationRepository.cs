using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Integration;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.Repositories;

public class IntegrationRepository : IIntegrationRepository
{
    private readonly ApplicationDbContext _context;

    public IntegrationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<IntegrationEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Set<IntegrationEntity>().ToListAsync(cancellationToken);
    }
    public async Task<IntegrationEntity?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        return await _context.Set<IntegrationEntity>().FindAsync([id], cancellationToken);
    }

    public async Task AddAsync(IntegrationEntity integration, CancellationToken cancellationToken)
    {
        await _context.Set<IntegrationEntity>().AddAsync(integration, cancellationToken);
    }

    public Task UpdateAsync(IntegrationEntity integration, CancellationToken cancellationToken)
    {
        _context.Set<IntegrationEntity>().Update(integration);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        IntegrationEntity? integration =
            await _context.Set<IntegrationEntity>().FindAsync([id], cancellationToken);
        if (integration is not null) _context.Set<IntegrationEntity>().Remove(integration);
    }
}
