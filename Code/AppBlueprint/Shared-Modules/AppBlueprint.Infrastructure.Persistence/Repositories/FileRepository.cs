using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.Repositories;

public class FileRepository : IFileRepository
{
    private readonly ApplicationDbContext _context;

    public FileRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<FileEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Set<FileEntity>().ToListAsync(cancellationToken);
    }
    public async Task<FileEntity?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        return await _context.Set<FileEntity>().FindAsync([id], cancellationToken);
    }

    public async Task AddAsync(FileEntity file, CancellationToken cancellationToken)
    {
        await _context.Files.AddAsync(file, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task UpdateAsync(FileEntity file, CancellationToken cancellationToken)
    {
        _context.Set<FileEntity>().Update(file);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(FileEntity file, CancellationToken cancellationToken)
    {
        _context.Set<FileEntity>().Remove(file);
        return Task.CompletedTask;
    }
}
