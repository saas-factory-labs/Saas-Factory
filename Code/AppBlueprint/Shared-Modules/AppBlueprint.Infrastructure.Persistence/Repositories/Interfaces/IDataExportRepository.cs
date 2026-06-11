using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.DataExport;

namespace AppBlueprint.Infrastructure.Repositories.Interfaces;

public interface IDataExportRepository
{
    Task<IEnumerable<DataExportEntity>> GetAllAsync(CancellationToken cancellationToken);
    Task<DataExportEntity> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task AddAsync(DataExportEntity dataExport, CancellationToken cancellationToken);
    Task UpdateAsync(DataExportEntity dataExport, CancellationToken cancellationToken);
    Task DeleteAsync(string id, CancellationToken cancellationToken);
}
