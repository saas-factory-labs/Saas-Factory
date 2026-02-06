using AppBlueprint.Application.Constants;
using AppBlueprint.Application.Interfaces.UnitOfWork;
using AppBlueprint.Contracts.B2B.Contracts.Organization.Requests;
using AppBlueprint.Contracts.B2B.Contracts.Organization.Responses;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.Team;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrganizationEntity = AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Organization.OrganizationEntity;

namespace AppBlueprint.Presentation.ApiModule.Controllers.B2B;

[Authorize(Roles = Roles.TenantAdmin)]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/organization")]
[Produces("application/json")]
public class OrganizationController : BaseController
{
    private readonly IConfiguration _configuration;
    private readonly IOrganizationRepository _organizationRepository;

    public OrganizationController(
        IConfiguration configuration,
        IOrganizationRepository organizationRepository)
        : base(configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _organizationRepository = organizationRepository ?? throw new ArgumentNullException(nameof(organizationRepository));
    }

    /// <summary>
    ///     Retrieves all organizations in the system.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>List of organizations with their basic information.</returns>
    /// <remarks>
    ///     This endpoint requires TenantAdmin role.
    ///     Returns organization names and basic details.
    /// </remarks>
    /// <response code="200">Returns the list of organizations successfully.</response>
    /// <response code="404">No organizations found in the system.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have TenantAdmin role.</response>
    [HttpGet(ApiEndpoints.Organizations.GetAll)]
    [ProducesResponseType(typeof(IEnumerable<OrganizationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult<IEnumerable<OrganizationResponse>>> GetOrganizations(
        CancellationToken cancellationToken)
    {
        IEnumerable<OrganizationEntity>? organizations = await _organizationRepository.GetAllAsync();
        if (!organizations.Any()) return NotFound(new { Message = "No organizations found." });

        IEnumerable<OrganizationResponse>? response = organizations.Select(o => new OrganizationResponse
        {

            Name = o.Name,
        });
        return Ok(response);
    }

    /// <summary>
    ///     Retrieves a specific organization by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the organization (e.g., "org_01HX...").</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>Organization details including name and associated data.</returns>
    /// <remarks>
    ///     Requires TenantAdmin role. Returns organization information for the specified ID.
    /// </remarks>
    /// <response code="200">Returns the organization details successfully.</response>
    /// <response code="404">Organization with the specified ID was not found.</response>
    /// <response code="403">User does not have TenantAdmin role.</response>
    [HttpGet(ApiEndpoints.Organizations.GetById)]
    [ProducesResponseType(typeof(OrganizationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult<OrganizationResponse>> GetOrganization(string id, CancellationToken cancellationToken)
    {
        OrganizationEntity? org = await _organizationRepository.GetByIdAsync(id);
        if (org is null) return NotFound(new { Message = $"Organization with ID {id} not found." });

        var response = new OrganizationResponse
        {

            Name = org.Name,

        };

        return Ok(response);
    }

    /// <summary>
    ///     Creates a new organization with the specified details.
    /// </summary>
    /// <param name="organizationDto">Organization creation request containing name and other details.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>The newly created organization with generated ID.</returns>
    /// <remarks>
    ///     Requires TenantAdmin role.
    ///     Sample request:
    ///     <code>
    ///     POST /api/v1/organization
    ///     {
    ///         "name": "Acme Corporation"
    ///     }
    ///     </code>
    /// </remarks>
    /// <response code="201">Organization created successfully. Returns the created organization with its ID.</response>
    /// <response code="400">Invalid request data or validation failed.</response>
    /// <response code="403">User does not have TenantAdmin role.</response>
    [HttpPost(ApiEndpoints.Organizations.Create)]
    [ProducesResponseType(typeof(OrganizationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult> CreateOrganization([FromBody] CreateOrganizationRequest organizationDto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        ArgumentNullException.ThrowIfNull(organizationDto);

        var owner = new UserEntity
        {
            UserName = "adkakd",
            FirstName = "asd",
            LastName = "asd",
            Email = "asd",
            Profile = new ProfileEntity()
        };

        var newOrg = new OrganizationEntity
        {
            Name = organizationDto.Name ?? "Default Organization",
            Description = "ad",
            Owner = owner
        };

        await _organizationRepository.AddAsync(newOrg);
        // If SaveChangesAsync is required, inject a service for it or handle in repository.

        return CreatedAtAction(nameof(GetOrganization), new { id = newOrg.Id }, new OrganizationResponse
        {
            Name = newOrg.Name
        });
    }

    /// <summary>
    ///     Updates an existing organization's details.
    /// </summary>
    /// <param name="id">The unique identifier of the organization to update.</param>
    /// <param name="organizationDto">Updated organization data containing name.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>No content on success.</returns>
    /// <remarks>
    ///     Requires TenantAdmin role.
    ///     Sample request:
    ///     <code>
    ///     PUT /api/v1/organization/org_01HX...
    ///     {
    ///         "name": "Updated Organization Name"
    ///     }
    ///     </code>
    /// </remarks>
    /// <response code="204">Organization updated successfully.</response>
    /// <response code="404">Organization with the specified ID was not found.</response>
    /// <response code="403">User does not have TenantAdmin role.</response>
    [HttpPut(ApiEndpoints.Organizations.UpdateById)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult> UpdateOrganization(string id, [FromBody] UpdateOrganizationRequest organizationDto,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(organizationDto);

        OrganizationEntity? existingOrg = await _organizationRepository.GetByIdAsync(id);
        if (existingOrg is null) return NotFound(new { Message = $"Organization with ID {id} not found." });

        existingOrg.Name = organizationDto.Name;

        _organizationRepository.Update(existingOrg);
        return NoContent();
    }

    /// <summary>
    ///     Deletes an organization by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the organization to delete.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>No content on success.</returns>
    /// <remarks>
    ///     Requires TenantAdmin role.
    ///     Sample request:
    ///     <code>DELETE /api/v1/organization/org_01HX...</code>
    ///     Warning: This operation cannot be undone. All organization data will be permanently removed.
    /// </remarks>
    /// <response code="204">Organization deleted successfully.</response>
    /// <response code="404">Organization with the specified ID was not found.</response>
    /// <response code="403">User does not have TenantAdmin role.</response>
    [HttpDelete(ApiEndpoints.Organizations.DeleteById)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult> DeleteOrganization(string id, CancellationToken cancellationToken)
    {
        OrganizationEntity? existingOrg = await _organizationRepository.GetByIdAsync(id);
        if (existingOrg is null) return NotFound(new { Message = $"Organization with ID {id} not found." });

        _organizationRepository.Delete(id);

        return NoContent();
    }
}
