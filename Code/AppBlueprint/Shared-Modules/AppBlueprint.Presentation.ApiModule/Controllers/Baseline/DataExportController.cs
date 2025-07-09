using AppBlueprint.Application.Services.DataExport;
using AppBlueprint.Contracts.Baseline.DataExport.Requests;
using AppBlueprint.Contracts.Baseline.DataExport.Responses;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppBlueprint.Presentation.ApiModule.Controllers.Baseline;

[Authorize]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/data-export")]
[Produces("application/json")]
public class DataExportController : BaseController
{
    private readonly IDataExportService _dataExportService;

    public DataExportController(
        IConfiguration configuration,
        IDataExportService dataExportService)
        : base(configuration)
    {
        _dataExportService = dataExportService ?? throw new ArgumentNullException(nameof(dataExportService));
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
        var dataExports = await _dataExportService.GetAllDataExportsAsync(cancellationToken);

        if (!dataExports.Any())
            return NotFound(new { Message = "No data exports found." });

        return Ok(dataExports);
    }

    /// <summary>
    ///     Gets a data export by ID.
    /// </summary>
    /// <param name="id">Data export ID.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Data export</returns>
    [HttpGet(ApiEndpoints.DataExports.GetById)]
    [ProducesResponseType(typeof(DataExportResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult<DataExportResponse>> GetDataExport(string id, CancellationToken cancellationToken)
    {
        var dataExport = await _dataExportService.GetDataExportByIdAsync(id, cancellationToken);

        if (dataExport is null)
            return NotFound(new { Message = $"Data export with ID {id} not found." });

        return Ok(dataExport);
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
        ArgumentNullException.ThrowIfNull(dataExportDto);

        if (!ModelState.IsValid) return BadRequest(ModelState);

        var response = await _dataExportService.CreateDataExportAsync(dataExportDto, cancellationToken);

        return CreatedAtAction(nameof(GetDataExport), new { id = response.Id }, response);
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
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult> UpdateDataExport(string id, [FromBody] UpdateDataExportRequest dataExportDto,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(dataExportDto);

        var success = await _dataExportService.UpdateDataExportAsync(id, dataExportDto, cancellationToken);

        if (!success)
            return NotFound(new { Message = $"Data export with ID {id} not found." });

        return NoContent();
    }

    /// <summary>
    ///     Deletes a data export by ID.
    /// </summary>
    /// <param name="id">Data export ID.</param>
    /// /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>No content.</returns>
    [HttpDelete(ApiEndpoints.DataExports.DeleteById)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult> DeleteDataExport(string id, CancellationToken cancellationToken)
    {
        var success = await _dataExportService.DeleteDataExportAsync(id, cancellationToken);

        if (!success)
            return NotFound(new { Message = $"Data export with ID {id} not found." });

        return NoContent();
    }
}
