using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.FileManagement;

namespace AppBlueprint.Infrastructure.Repositories.Interfaces;

/// <summary>
/// Repository interface for file metadata entities.
/// </summary>
public interface IFileMetadataRepository
{
    Task<IEnumerable<FileMetadataEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<FileMetadataEntity?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<FileMetadataEntity?> GetByFileKeyAsync(string fileKey, CancellationToken cancellationToken = default);
    Task<IEnumerable<FileMetadataEntity>> GetByTenantAsync(string tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<FileMetadataEntity>> GetByFolderAsync(string tenantId, string folder, CancellationToken cancellationToken = default);
    Task AddAsync(FileMetadataEntity file, CancellationToken cancellationToken = default);
    Task UpdateAsync(FileMetadataEntity file, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}
