using AppBlueprint.Application.Interfaces.UnitOfWork;
using AppBlueprint.Application.Services.DataExport;
using AppBlueprint.Contracts.Baseline.DataExport.Requests;
using AppBlueprint.Contracts.Baseline.DataExport.Responses;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.DataExport;
using AppBlueprint.Infrastructure.Repositories.Interfaces;

namespace AppBlueprint.Infrastructure.Services.DataExport;

/// <summary>
/// Infrastructure implementation of the DataExport application service.
/// This follows Clean Architecture by implementing the application interface in the infrastructure layer.
/// </summary>
public sealed class DataExportService : IDataExportService
{
    private readonly IDataExportRepository _dataExportRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DataExportService(
        IDataExportRepository dataExportRepository,
        IUnitOfWork unitOfWork)
    {
        _dataExportRepository = dataExportRepository ?? throw new ArgumentNullException(nameof(dataExportRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<DataExportResponse> CreateDataExportAsync(CreateDataExportRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Business logic: Create domain entity
        var dataExport = new DataExportEntity
        {
            FileName = request.FileName,
            FileSize = request.FileSize,
            CreatedAt = DateTime.UtcNow
        };

        // Persist to repository
        await _dataExportRepository.AddAsync(dataExport, cancellationToken);

        // Commit transaction
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Map to response DTO
        return MapToResponse(dataExport);
    }

    public async Task<IEnumerable<DataExportResponse>> GetAllDataExportsAsync(CancellationToken cancellationToken)
    {
        var dataExports = await _dataExportRepository.GetAllAsync(cancellationToken);

        return dataExports.Select(MapToResponse);
    }

    public async Task<DataExportResponse?> GetDataExportByIdAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(id);

        var dataExport = await _dataExportRepository.GetByIdAsync(id, cancellationToken);

        return dataExport is not null ? MapToResponse(dataExport) : null;
    }

    public async Task<bool> UpdateDataExportAsync(string id, UpdateDataExportRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(request);

        var existingDataExport = await _dataExportRepository.GetByIdAsync(id, cancellationToken);
        if (existingDataExport is null)
            return false;

        // Business logic: Update entity
        existingDataExport.FileName = request.FileName;
        // Add other update logic as needed

        await _dataExportRepository.UpdateAsync(existingDataExport, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteDataExportAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(id);

        var existingDataExport = await _dataExportRepository.GetByIdAsync(id, cancellationToken);
        if (existingDataExport is null)
            return false;

        await _dataExportRepository.DeleteAsync(existingDataExport.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <summary>
    /// Maps a domain entity to a response DTO.
    /// </summary>
    private static DataExportResponse MapToResponse(DataExportEntity entity)
    {
        return new DataExportResponse
        {
            Id = entity.Id,
            FileName = entity.FileName,
            FileSize = entity.FileSize.ToString(System.Globalization.CultureInfo.InvariantCulture),
            DownloadUrl = string.Empty, // TODO: Generate actual download URL based on business rules
            CreatedAt = entity.CreatedAt
        };
    }
}
