using DeploymentManager.ApiService.Domain.Entities;
using DeploymentManager.ApiService.Domain.Interfaces;
using DeploymentManager.ApiService.Infrastructure.Persistence.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DeploymentManager.ApiService.Infrastructure.Persistence.Data.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly DeploymentManagerDbContext _context;

    public ProjectRepository(DeploymentManagerDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<ProjectEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.DmProjects.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<ProjectEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.DmProjects.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ProjectEntity project, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);
        await _context.DmProjects.AddAsync(project, cancellationToken);
    }

    public Task UpdateAsync(ProjectEntity project, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);
        _context.DmProjects.Update(project);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        ProjectEntity? project = await GetByIdAsync(id, cancellationToken);
        if (project is not null)
            _context.DmProjects.Remove(project);
    }
}
