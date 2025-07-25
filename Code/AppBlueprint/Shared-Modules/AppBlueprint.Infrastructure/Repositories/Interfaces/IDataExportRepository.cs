using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.DataExport;

namespace AppBlueprint.Infrastructure.Repositories.Interfaces;

public interface IDataExportRepository
{
    public Task<IEnumerable<DataExportEntity>> GetAllAsync(CancellationToken cancellationToken); public Task<DataExportEntity> GetByIdAsync(string id, CancellationToken cancellationToken);
    public Task AddAsync(DataExportEntity account, CancellationToken cancellationToken);
    public Task UpdateAsync(DataExportEntity account, CancellationToken cancellationToken);
    public Task DeleteAsync(string id, CancellationToken cancellationToken);
}
