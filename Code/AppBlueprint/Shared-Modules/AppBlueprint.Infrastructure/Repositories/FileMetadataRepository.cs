using AppBlueprint.Infrastructure.DatabaseContexts.Baseline;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.FileManagement;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.Repositories;

public sealed class FileMetadataRepository : IFileMetadataRepository
{
    private readonly BaselineDbContext _context;

    public FileMetadataRepository(BaselineDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<IEnumerable<FileMetadataEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<FileMetadataEntity>()
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<FileMetadataEntity?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        return await _context.Set<FileMetadataEntity>()
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
    }

    public async Task<FileMetadataEntity?> GetByFileKeyAsync(string fileKey, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileKey);
        return await _context.Set<FileMetadataEntity>()
            .FirstOrDefaultAsync(f => f.FileKey == fileKey, cancellationToken);
    }

    public async Task<IEnumerable<FileMetadataEntity>> GetByTenantAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tenantId);
        return await _context.Set<FileMetadataEntity>()
            .Where(f => f.TenantId == tenantId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<FileMetadataEntity>> GetByFolderAsync(string tenantId, string folder, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tenantId);
        ArgumentException.ThrowIfNullOrWhiteSpace(folder);
        
        return await _context.Set<FileMetadataEntity>()
            .Where(f => f.TenantId == tenantId && f.Folder == folder)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(FileMetadataEntity file, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);
        await _context.Set<FileMetadataEntity>().AddAsync(file, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(FileMetadataEntity file, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);
        _context.Set<FileMetadataEntity>().Update(file);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        FileMetadataEntity? file = await GetByIdAsync(id, cancellationToken);
        if (file is not null)
        {
            _context.Set<FileMetadataEntity>().Remove(file);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
