using DeploymentManager.ApiService.Domain.Entities;
using DeploymentManager.ApiService.Domain.Interfaces;
using DeploymentManager.ApiService.Infrastructure.Persistence.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DeploymentManager.ApiService.Infrastructure.Persistence.Data.Repositories;

public class AppRepository : IAppRepository
{
    private readonly DeploymentManagerDbContext _context;

    public AppRepository(DeploymentManagerDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<AppEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.DmApps.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<IEnumerable<AppEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.DmApps.ToListAsync(cancellationToken);

    public async Task AddAsync(AppEntity app, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(app);
        await _context.DmApps.AddAsync(app, cancellationToken);
    }

    public Task UpdateAsync(AppEntity app, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(app);
        _context.DmApps.Update(app);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        AppEntity? app = await GetByIdAsync(id, cancellationToken);
        if (app is not null)
            _context.DmApps.Remove(app);
    }
}
