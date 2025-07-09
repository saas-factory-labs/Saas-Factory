using AppBlueprint.Contracts.Baseline.DataExport.Requests;
using AppBlueprint.Contracts.Baseline.DataExport.Responses;

namespace AppBlueprint.Application.Services.DataExport;

/// <summary>
/// Application service for managing data export operations.
/// Handles business logic and orchestrates domain operations.
/// </summary>
public interface IDataExportService
{
    /// <summary>
    /// Creates a new data export asynchronously.
    /// </summary>
    /// <param name="request">The data export creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created data export response.</returns>
    Task<DataExportResponse> CreateDataExportAsync(CreateDataExportRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Gets all data exports asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of data export responses.</returns>
    Task<IEnumerable<DataExportResponse>> GetAllDataExportsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets a data export by ID asynchronously.
    /// </summary>
    /// <param name="id">The data export ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The data export response if found, null otherwise.</returns>
    Task<DataExportResponse?> GetDataExportByIdAsync(string id, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing data export asynchronously.
    /// </summary>
    /// <param name="id">The data export ID.</param>
    /// <param name="request">The data export update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if updated successfully, false if not found.</returns>
    Task<bool> UpdateDataExportAsync(string id, UpdateDataExportRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a data export asynchronously.
    /// </summary>
    /// <param name="id">The data export ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if deleted successfully, false if not found.</returns>
    Task<bool> DeleteDataExportAsync(string id, CancellationToken cancellationToken);
}
