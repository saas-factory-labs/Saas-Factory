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
    ///     Retrieves all tenants in the multi-tenant system.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>List of tenants with their details including creation and update timestamps.</returns>
    /// <remarks>
    ///     Returns all tenants configured in the B2B multi-tenant architecture.
    ///     Each tenant represents an isolated organizational workspace.
    ///     Requires authentication.
    /// </remarks>
    /// <response code="200">Returns the list of tenants successfully.</response>
    /// <response code="404">No tenants found in the system.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet(ApiEndpoints.Tenants.GetAll)]
    [ProducesResponseType(typeof(IEnumerable<TenantResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult<IEnumerable<TenantResponse>>> GetTenants(CancellationToken cancellationToken)
    {
        IEnumerable<TenantEntity> tenants = await _tenantRepository.GetAllAsync(); if (!tenants.Any()) return NotFound(new { Message = "No tenants found." });

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
    ///     Retrieves a specific tenant by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the tenant (e.g., "tenant_01HX...").</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>Tenant details including name, description, and timestamps.</returns>
    /// <remarks>
    ///     Sample request:
    ///     <code>GET /api/v1/tenant/tenant_01HX...</code>
    ///     Returns complete tenant information for the specified ID.
    /// </remarks>
    /// <response code="200">Returns the tenant details successfully.</response>
    /// <response code="404">Tenant with the specified ID was not found.</response>
    /// <response code="401">User is not authenticated.</response>
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
    ///     Creates a new tenant in the multi-tenant system.
    /// </summary>
    /// <param name="tenantDto">Tenant creation request containing name and description.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>The newly created tenant with generated ID and timestamps.</returns>
    /// <remarks>
    ///     Creates a new isolated tenant workspace in the B2B system.
    ///     Sample request:
    ///     <code>
    ///     POST /api/v1/tenant
    ///     {
    ///         "name": "Acme Corporation",
    ///         "description": "Tenant for Acme Corp"
    ///     }
    ///     </code>
    ///     Automatically sets creation timestamp to current UTC time.
    /// </remarks>
    /// <response code="201">Tenant created successfully. Returns the created tenant with its ID.</response>
    /// <response code="400">Invalid request data or validation failed.</response>
    /// <response code="401">User is not authenticated.</response>
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
    ///     Updates an existing tenant's information.
    /// </summary>
    /// <param name="id">The unique identifier of the tenant to update.</param>
    /// <param name="tenantDto">Updated tenant data containing name and/or description.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>No content on success.</returns>
    /// <remarks>
    ///     Sample request:
    ///     <code>
    ///     PUT /api/v1/tenant/tenant_01HX...
    ///     {
    ///         "name": "Acme Corporation - Updated",
    ///         "description": "Updated description"
    ///     }
    ///     </code>
    ///     Only provided fields will be updated. Null values are ignored.
    ///     Automatically updates LastUpdatedAt timestamp to current UTC time.
    /// </remarks>
    /// <response code="204">Tenant updated successfully.</response>
    /// <response code="404">Tenant with the specified ID was not found.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpPut(ApiEndpoints.Tenants.UpdateById)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult> UpdateTenant(string id, [FromBody] UpdateTenantRequest tenantDto,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(tenantDto);

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
    ///     Deletes a tenant by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the tenant to delete.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>No content on success.</returns>
    /// <remarks>
    ///     Sample request:
    ///     <code>DELETE /api/v1/tenant/tenant_01HX...</code>
    ///     Warning: This operation permanently removes the tenant and all associated data.
    ///     All users, teams, and resources within this tenant will be affected.
    ///     Consider tenant deactivation instead of deletion for production systems.
    /// </remarks>
    /// <response code="204">Tenant deleted successfully.</response>
    /// <response code="404">Tenant with the specified ID was not found.</response>
    /// <response code="401">User is not authenticated.</response>
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
