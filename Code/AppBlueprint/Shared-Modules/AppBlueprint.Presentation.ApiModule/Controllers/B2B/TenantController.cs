using AppBlueprint.Contracts.B2B.Contracts.Tenant.Requests;
using AppBlueprint.Contracts.B2B.Contracts.Tenant.Responses;
using AppBlueprint.Contracts.B2B.Tenant.Requests;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Tenant.Tenant;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using AppBlueprint.Application.Interfaces.UnitOfWork;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppBlueprint.Presentation.ApiModule.Controllers.B2B;

[Authorize]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/tenant")]
[Produces("application/json")]
public class TenantController : BaseController
{
    private readonly IConfiguration _configuration;
    private readonly ITenantRepository _tenantRepository;
    // Removed IUnitOfWork dependency for repository DI pattern

    public TenantController(
        IConfiguration configuration,
        ITenantRepository tenantRepository)
        : base(configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _tenantRepository = tenantRepository ?? throw new ArgumentNullException(nameof(tenantRepository));
        // Removed IUnitOfWork assignment
    }

    /// <summary>
    ///     Gets all tenants.
    /// </summary>
    /// <returns>List of tenants</returns>
    [HttpGet(ApiEndpoints.Tenants.GetAll)]
    [ProducesResponseType(typeof(IEnumerable<TenantResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult<IEnumerable<TenantResponse>>> GetTenants(CancellationToken cancellationToken)
    {
        IEnumerable<TenantEntity> tenants = await _tenantRepository.GetAllAsync();        if (!tenants.Any()) return NotFound(new { Message = "No tenants found." });

        IEnumerable<TenantResponse> response = tenants.Select(tenant => new TenantResponse
        {
            Id = tenant.Id,
            Name = tenant.Name,
            Description = tenant.Description,
            CreatedAt = tenant.CreatedAt,
            LastUpdatedAt = tenant.LastUpdatedAt
        });

        return Ok(response);
    }

    /// <summary>
    ///     Gets a tenant by ID.
    /// </summary>
    /// <param name="id">Tenant ID.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Tenant details</returns>
    [HttpGet(ApiEndpoints.Tenants.GetById)]
    [ProducesResponseType(typeof(TenantResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult<TenantResponse>> GetTenant(string id, CancellationToken cancellationToken)
    {
        TenantEntity? tenant = await _tenantRepository.GetByIdAsync(id);
        if (tenant is null) return NotFound(new { Message = $"Tenant with ID {id} not found." });

        var response = new TenantResponse
        {
            Id = tenant.Id,
            Name = tenant.Name,
            Description = tenant.Description,
            CreatedAt = tenant.CreatedAt,
            LastUpdatedAt = tenant.LastUpdatedAt
        };

        return Ok(response);
    }

    /// <summary>
    ///     Creates a new tenant.
    /// </summary>
    /// <param name="tenantDto">Tenant data transfer object.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Created tenant.</returns>
    [HttpPost(ApiEndpoints.Tenants.Create)]
    [ProducesResponseType(typeof(TenantResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult> CreateTenant([FromBody] CreateTenantRequest tenantDto,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(tenantDto);
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var newTenant = new TenantEntity
        {
            Name = tenantDto.Name,
            Description = tenantDto.Description,
            CreatedAt = DateTime.UtcNow
        };

        await _tenantRepository.AddAsync(newTenant);
        // If SaveChangesAsync is required, inject a service for it or handle in repository.
        var response = new TenantResponse
        {
            Id = newTenant.Id,
            Name = newTenant.Name,
            Description = newTenant.Description,
            CreatedAt = newTenant.CreatedAt,
            LastUpdatedAt = newTenant.LastUpdatedAt
        };

        return CreatedAtAction(nameof(GetTenant), new { id = newTenant.Id }, response);
    }

    /// <summary>
    ///     Updates an existing tenant.
    /// </summary>
    /// <param name="id">Tenant ID.</param>
    /// <param name="tenantDto">Tenant data transfer object.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>No content.</returns>
    [HttpPut(ApiEndpoints.Tenants.UpdateById)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult> UpdateTenant(string id, [FromBody] UpdateTenantRequest tenantDto,
        CancellationToken cancellationToken)
    {
        TenantEntity? existingTenant = await _tenantRepository.GetByIdAsync(id);
        if (existingTenant is null) return NotFound(new { Message = $"Tenant with ID {id} not found." });

        if (tenantDto.Name != null)
            existingTenant.Name = tenantDto.Name;
        if (tenantDto.Description != null)
            existingTenant.Description = tenantDto.Description;
        existingTenant.LastUpdatedAt = DateTime.UtcNow;

        _tenantRepository.Update(existingTenant);
        // If SaveChangesAsync is required, inject a service for it or handle in repository.

        return NoContent();
    }

    /// <summary>
    ///     Deletes a tenant by ID.
    /// </summary>
    /// <param name="id">Tenant ID.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>No content.</returns>
    [HttpDelete(ApiEndpoints.Tenants.DeleteById)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult> DeleteTenant(string id, CancellationToken cancellationToken)
    {
        TenantEntity? existingTenant = await _tenantRepository.GetByIdAsync(id);
        if (existingTenant is null) return NotFound(new { Message = $"Tenant with ID {id} not found." });

        _tenantRepository.Delete(existingTenant.Id);
        // If SaveChangesAsync is required, inject a service for it or handle in repository.

        return NoContent();
    }
}
