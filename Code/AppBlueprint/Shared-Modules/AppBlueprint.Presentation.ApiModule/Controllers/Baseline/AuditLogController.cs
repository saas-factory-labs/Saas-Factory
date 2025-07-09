using AppBlueprint.Contracts.Baseline.AuditLog.Requests;
using AppBlueprint.Contracts.Baseline.AuditLog.Responses;
using AppBlueprint.Contracts.Baseline.User.Responses;
using AppBlueprint.Contracts.B2B.Contracts.Tenant.Responses;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using AppBlueprint.Application.Interfaces.UnitOfWork;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppBlueprint.Presentation.ApiModule.Controllers.Baseline;

[Authorize(Roles = Roles.SaaSProviderAdmin)]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/audit-log")]
[Produces("application/json")]
public class AuditLogController : BaseController
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IConfiguration _configuration;

    public AuditLogController(
        IConfiguration configuration,
        IAuditLogRepository auditLogRepository)
        : base(configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _auditLogRepository = auditLogRepository ?? throw new ArgumentNullException(nameof(auditLogRepository));
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
            Action = auditLog.Action,
            Category = auditLog.Category ?? "General",
            NewValue = auditLog.NewValue,
            OldValue = auditLog.OldValue,
            ModifiedBy = $"{auditLog.ModifiedBy.FirstName} {auditLog.ModifiedBy.LastName}",
            ModifiedAt = auditLog.ModifiedAt,
            Tenant = new TenantResponse
            {
                Id = auditLog.Tenant.Id,
                Name = auditLog.Tenant.Name ?? "Default Tenant",
                CreatedAt = auditLog.Tenant.CreatedAt
            },
            User = new UserResponse
            {
                Id = auditLog.User.Id,
                Email = auditLog.User.Email,
                FirstName = auditLog.User.FirstName,
                LastName = auditLog.User.LastName
            }
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
    public async Task<ActionResult> GetAuditLog(string id, CancellationToken cancellationToken)
    {
        AuditLogEntity? auditLog = await _auditLogRepository.GetByIdAsync(id, cancellationToken);
        if (auditLog is null) return NotFound(new { Message = $"Audit log with ID {id} not found." });

        var response = new AuditLogResponse
        {
            Id = auditLog.Id,
            Action = auditLog.Action,
            Category = auditLog.Category ?? "General",
            NewValue = auditLog.NewValue,
            OldValue = auditLog.OldValue,
            ModifiedBy = $"{auditLog.ModifiedBy.FirstName} {auditLog.ModifiedBy.LastName}",
            ModifiedAt = auditLog.ModifiedAt,
            Tenant = new TenantResponse
            {
                Id = auditLog.Tenant.Id,
                Name = auditLog.Tenant.Name ?? "Default Tenant",
                CreatedAt = auditLog.Tenant.CreatedAt
            },
            User = new UserResponse
            {
                Id = auditLog.User.Id,
                Email = auditLog.User.Email,
                FirstName = auditLog.User.FirstName,
                LastName = auditLog.User.LastName
            }
        };

        return Ok(response);
    }

    /// <summary>
    /// Creates a new audit log.
    /// </summary>
    /// <param name="auditLogDto">Audit log data transfer object.</param>
    /// <returns>Created audit log.</returns>
    // [HttpPost("CreateAuditLog")]
    // [ProducesResponseType(typeof(AuditLogResponse), StatusCodes.Status201Created)]
    // [MapToApiVersion(ApiVersions.V1)]
    // public async Task<ActionResult> CreateAuditLog([FromBody] CreateAuditLogRequest auditLogDto, CancellationToken cancellationToken)
    // {
    //     if (!ModelState.IsValid) return BadRequest(ModelState);

    //     var newAuditLog = new AuditLogEntity
    //     {
    //         Action = auditLogDto.Action,
    //         TenantId = "",
    //         UserId = auditLogDto.UserId,
    //     };

    //     await _auditLogRepository.AddAsync(newAuditLog, cancellationToken);

    //     var response = new AuditLogResponse
    //     {
    //         Id = newAuditLog.Id,
    //         Action = newAuditLog.Action
    //     };

    //     return CreatedAtAction(nameof(GetAuditLog), new { id = newAuditLog.Id }, response);
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
    public async Task<ActionResult> UpdateAuditLog(string id, [FromBody] UpdateAuditLogRequest auditLogDto,
        CancellationToken cancellationToken)
    {
        AuditLogEntity? existingAuditLog = await _auditLogRepository.GetByIdAsync(id, cancellationToken);
        if (existingAuditLog is null) return NotFound(new { Message = $"Audit log with ID {id} not found." });

        // existingAuditLog.Action = auditLogDto.Action      

        _auditLogRepository.Update(existingAuditLog, cancellationToken);
        // If SaveChangesAsync is required, inject a service for it or handle in repository.

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
    public async Task<ActionResult> DeleteAuditLog(string id, CancellationToken cancellationToken)
    {
        AuditLogEntity? existingAuditLog = await _auditLogRepository.GetByIdAsync(id, cancellationToken);
        if (existingAuditLog is null) return NotFound(new { Message = $"Audit log with ID {id} not found." });

        _auditLogRepository.Delete(existingAuditLog.Id, cancellationToken);
        // If SaveChangesAsync is required, inject a service for it or handle in repository.

        return NoContent();
    }
}
