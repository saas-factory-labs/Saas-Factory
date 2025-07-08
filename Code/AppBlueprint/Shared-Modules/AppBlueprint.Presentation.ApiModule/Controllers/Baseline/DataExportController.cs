using AppBlueprint.Contracts.Baseline.DataExport.Requests;
using AppBlueprint.Contracts.Baseline.DataExport.Responses;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.DataExport;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using AppBlueprint.Application.Interfaces.UnitOfWork;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppBlueprint.Presentation.ApiModule.Controllers.Baseline;

[Authorize]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/data-exports")]
[Produces("application/json")]
public class DataExportController : BaseController
{
    private readonly IConfiguration _configuration;
    private readonly IDataExportRepository _dataExportRepository;

    public DataExportController(
        IConfiguration configuration,
        IDataExportRepository dataExportRepository)
        : base(configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _dataExportRepository = dataExportRepository ?? throw new ArgumentNullException(nameof(dataExportRepository));
        // Removed IUnitOfWork assignment
    }

    /// <summary>
    ///     Gets all data exports.
    /// </summary>
    /// <returns>List of data exports</returns>
    [HttpGet(ApiEndpoints.DataExports.GetAll)]
    [ProducesResponseType(typeof(IEnumerable<DataExportResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult<IEnumerable<DataExportResponse>>> GetDataExports(CancellationToken cancellationToken)
    {
        IEnumerable<DataExportEntity>? dataExports = await _dataExportRepository.GetAllAsync(cancellationToken);
        if (!dataExports.Any()) return NotFound(new { Message = "No data exports found." });

        IEnumerable<DataExportResponse>? response = dataExports.Select(dataExport => new DataExportResponse
        {
            // Id = dataExport.Id,
            // Name = dataExport.FileName,
            // FileSize = dataExports.FileSize
        });

        return Ok(response);
    }

    /// <summary>
    ///     Gets a data export by ID.
    /// </summary>
    /// <param name="id">Data export ID.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Data export</returns>    [HttpGet(ApiEndpoints.DataExports.GetById)]
    [ProducesResponseType(typeof(DataExportResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult<DataExportResponse>> GetDataExport(string id, CancellationToken cancellationToken)
    {
        DataExportEntity? dataExport = await _dataExportRepository.GetByIdAsync(id, cancellationToken);
        if (dataExport is null) return NotFound(new { Message = $"Data export with ID {id} not found." });

        var response = new DataExportResponse
        {
            // i = dataExport.Id,
            // Name = dataExport.Name,
            // Type = dataExport.Type
        };

        return Ok(response);
    }

    /// <summary>
    ///     Creates a new data export.
    /// </summary>
    /// <param name="dataExportDto">Data export data transfer object.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Created data export.</returns>
    [HttpPost(ApiEndpoints.DataExports.Create)]
    [ProducesResponseType(typeof(DataExportResponse), StatusCodes.Status201Created)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult<DataExportResponse>> CreateDataExport(
        [FromBody] CreateDataExportRequest dataExportDto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var newDataExport = new DataExportEntity
        {
            FileName = dataExportDto.FileName,
            FileSize = dataExportDto.FileSize
        };

        // await _dataExportRepository.AddAsync(newDataExport);
        // If SaveChangesAsync is required, inject a service for it or handle in repository.

        return CreatedAtAction(nameof(GetDataExport), new { id = newDataExport.Id }, newDataExport);
    }

    /// <summary>
    ///     Updates an existing data export.
    /// </summary>
    /// <param name="id">Data export ID.</param>
    /// <param name="dataExportDto">Data export data transfer object.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>No content.</returns>
    [HttpPut(ApiEndpoints.DataExports.UpdateById)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]    public async Task<ActionResult> UpdateDataExport(string id, [FromBody] UpdateDataExportRequest dataExportDto,
        CancellationToken cancellationToken)
    {
        DataExportEntity? existingDataExport = await _dataExportRepository.GetByIdAsync(id, cancellationToken);
        if (existingDataExport is null) return NotFound(new { Message = $"Data export with ID {id} not found." });

        existingDataExport.FileName = dataExportDto.FileName;
        // existingDataExport. = dataExportDto.Type;

        await _dataExportRepository.UpdateAsync(existingDataExport, cancellationToken);
        // If SaveChangesAsync is required, inject a service for it or handle in repository.

        return NoContent();
    }

    /// <summary>
    ///     Deletes a data export by ID.
    /// </summary>
    /// <param name="id">Data export ID.</param>    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>No content.</returns>
    [HttpDelete(ApiEndpoints.DataExports.DeleteById)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult> DeleteDataExport(string id, CancellationToken cancellationToken)
    {
        DataExportEntity? existingDataExport = await _dataExportRepository.GetByIdAsync(id, cancellationToken);
        if (existingDataExport is null) return NotFound(new { Message = $"Data export with ID {id} not found." });

        // _dataExportRepository.Delete(existingDataExport.Id);
        // If SaveChangesAsync is required, inject a service for it or handle in repository.

        return NoContent();
    }
}
