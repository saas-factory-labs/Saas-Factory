using AppBlueprint.Contracts.Baseline.DataExport.Requests;
using AppBlueprint.Contracts.Baseline.DataExport.Responses;

namespace AppBlueprint.Application.Services.DataExport;

/// <summary>
/// Application service implementation for data export operations.
/// Encapsulates business logic and coordinates between domain and infrastructure layers.
/// NOTE: This is the interface implementation moved to Infrastructure layer for proper Clean Architecture.
/// </summary>
public sealed class DataExportService : IDataExportService
{
    private const string ImplementationMovedMessage = "Implementation moved to Infrastructure layer";

    // Implementation moved to Infrastructure layer to maintain Clean Architecture principles
    // This file serves as a placeholder and will be moved
    public Task<DataExportResponse> CreateDataExportAsync(CreateDataExportRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException(ImplementationMovedMessage);
    }

    public Task<IEnumerable<DataExportResponse>> GetAllDataExportsAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException(ImplementationMovedMessage);
    }

    public Task<DataExportResponse?> GetDataExportByIdAsync(string id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException(ImplementationMovedMessage);
    }

    public Task<bool> UpdateDataExportAsync(string id, UpdateDataExportRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException(ImplementationMovedMessage);
    }

    public Task<bool> DeleteDataExportAsync(string id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException(ImplementationMovedMessage);
    }
}
