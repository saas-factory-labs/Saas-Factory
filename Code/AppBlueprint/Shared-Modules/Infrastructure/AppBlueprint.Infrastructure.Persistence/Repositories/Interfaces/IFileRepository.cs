using AppBlueprint.Infrastructure.Persistence.DatabaseContexts.Baseline.Entities;

namespace AppBlueprint.Infrastructure.Persistence.Repositories.Interfaces;

public interface IFileRepository
{
    Task<IEnumerable<FileEntity>> GetAllAsync(CancellationToken cancellationToken);
    Task<FileEntity?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task AddAsync(FileEntity file, CancellationToken cancellationToken);
    Task UpdateAsync(FileEntity file, CancellationToken cancellationToken);
    Task DeleteAsync(FileEntity file, CancellationToken cancellationToken);
}
