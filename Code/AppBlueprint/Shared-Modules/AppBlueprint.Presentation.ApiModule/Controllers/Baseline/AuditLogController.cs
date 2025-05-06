using AppBlueprint.Contracts.Baseline.AuditLog.Requests;
using AppBlueprint.Contracts.Baseline.AuditLog.Responses;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using AppBlueprint.Infrastructure.UnitOfWork;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppBlueprint.Presentation.ApiModule.Controllers.Baseline;

[Authorize(Roles = Roles.SaaSProviderAdmin)]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/auditlog")]
[Produces("application/json")]
public class AuditLogController : BaseController
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;

    public AuditLogController(IConfiguration configuration, IAuditLogRepository auditLogRepository,
        IUnitOfWork unitOfWork) : base(configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _auditLogRepository = auditLogRepository ?? throw new ArgumentNullException(nameof(auditLogRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <summary>
    ///     Gets all audit logs.
    /// </summary>
    /// <returns>List of audit logs</returns>
    [HttpGet(ApiEndpoints.AuditLogs.GetAll)]
    [ProducesResponseType(typeof(IEnumerable<AuditLogResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult> GetAuditLogs(CancellationToken cancellationToken)
    {
        IEnumerable<AuditLogEntity>? auditLogs = await _auditLogRepository.GetAllAsync(cancellationToken);
        if (!auditLogs.Any()) return NotFound(new { Message = "No audit logs found." });

        IEnumerable<AuditLogResponse>? response = auditLogs.Select(auditLog => new AuditLogResponse
        {
            Id = auditLog.Id,
            Action = auditLog.Action
        });

        return Ok(response);
    }

    /// <summary>
    ///     Gets an audit log by ID.
    /// </summary>
    /// <param name="id">Audit log ID.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Audit log</returns>
    [HttpGet(ApiEndpoints.AuditLogs.GetById)]
    [ProducesResponseType(typeof(AuditLogResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult> GetAuditLog(int id, CancellationToken cancellationToken)
    {
        AuditLogEntity? auditLog = await _auditLogRepository.GetByIdAsync(id, cancellationToken);
        if (auditLog is null) return NotFound(new { Message = $"Audit log with ID {id} not found." });

        var response = new AuditLogResponse
        {
            Id = auditLog.Id,
            Action = auditLog.Action
        };

        return Ok(response);
    }

    // /// <summary>
    // /// Creates a new audit log.
    // /// </summary>
    // /// <param name="auditLogDto">Audit log data transfer object.</param>
    // /// <returns>Created audit log.</returns>
    // [HttpPost("CreateAuditLog")]
    // [ProducesResponseType(typeof(AuditLogResponseDto), StatusCodes.Status201Created)]
    // [MapToApiVersion(ApiVersions.V1)]
    // public async Task<ActionResult> CreateAuditLog([FromBody] AuditLogRequestDto auditLogDto, CancellationToken cancellationToken)
    // {
    //     if (!ModelState.IsValid) return BadRequest(ModelState);
    //
    //     var newAuditLog = new AuditLogRequestDto()
    //     {
    //         Action = auditLogDto.Action,
    //     };
    //     
    //     await _auditLogRepository.GetByIdAsync(auditLogDto, cancellationToken);
    //
    //     await _auditLogRepository.AddAsync(, cancellationToken);
    //     await _unitOfWork.SaveChangesAsync();
    //
    //     return CreatedAtAction(nameof(GetAuditLog), new { id = newAuditLog. }, newAuditLog);
    // }

    /// <summary>
    ///     Updates an existing audit log.
    /// </summary>
    /// <param name="id">Audit log ID.</param>
    /// <param name="auditLogDto">Audit log data transfer object.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>No content.</returns>
    [HttpPut(ApiEndpoints.AuditLogs.UpdateById)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult> UpdateAuditLog(int id, [FromBody] UpdateAuditLogRequest auditLogDto,
        CancellationToken cancellationToken)
    {
        AuditLogEntity? existingAuditLog = await _auditLogRepository.GetByIdAsync(id, cancellationToken);
        if (existingAuditLog is null) return NotFound(new { Message = $"Audit log with ID {id} not found." });

        // existingAuditLog.Action = auditLogDto.Action      

        _auditLogRepository.Update(existingAuditLog, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    ///     Deletes an audit log by ID.
    /// </summary>
    /// <param name="id">Audit log ID.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>No content.</returns>
    [HttpDelete(ApiEndpoints.AuditLogs.DeleteById)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult> DeleteAuditLog(int id, CancellationToken cancellationToken)
    {
        AuditLogEntity? existingAuditLog = await _auditLogRepository.GetByIdAsync(id, cancellationToken);
        if (existingAuditLog is null) return NotFound(new { Message = $"Audit log with ID {id} not found." });

        _auditLogRepository.Delete(existingAuditLog.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
}
