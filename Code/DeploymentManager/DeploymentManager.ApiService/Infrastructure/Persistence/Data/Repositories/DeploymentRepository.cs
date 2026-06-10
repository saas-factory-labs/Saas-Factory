using DeploymentManager.ApiService.Domain.Entities;
using DeploymentManager.ApiService.Domain.Interfaces;
using DeploymentManager.ApiService.Infrastructure.Persistence.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DeploymentManager.ApiService.Infrastructure.Persistence.Data.Repositories;

public class DeploymentRepository : IDeploymentRepository
{
    private readonly DeploymentManagerDbContext _context;

    public DeploymentRepository(DeploymentManagerDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<DeploymentEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.DmDeployments.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

    public async Task<IEnumerable<DeploymentEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.DmDeployments.ToListAsync(cancellationToken);

    public async Task AddAsync(DeploymentEntity deployment, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(deployment);
        await _context.DmDeployments.AddAsync(deployment, cancellationToken);
    }

    public Task UpdateAsync(DeploymentEntity deployment, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(deployment);
        _context.DmDeployments.Update(deployment);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        DeploymentEntity? deployment = await GetByIdAsync(id, cancellationToken);
        if (deployment is not null)
            _context.DmDeployments.Remove(deployment);
    }
}
