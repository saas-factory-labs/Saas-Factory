using AppBlueprint.Application.Constants;
using AppBlueprint.Contracts.B2B.Contracts.Organization.Requests;
using AppBlueprint.Contracts.B2B.Contracts.Organization.Responses;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.Team;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using AppBlueprint.Application.Interfaces.UnitOfWork;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public OrganizationController(
        IConfiguration configuration,
        IOrganizationRepository organizationRepository,
        IUnitOfWork unitOfWork,
        IUserRepository userRepository)
        : base(configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _organizationRepository = organizationRepository ?? throw new ArgumentNullException(nameof(organizationRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    /// <summary>
    ///     Gets all organizations.
    /// </summary>
    /// <returns>List of organizations</returns>
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
    ///     Gets an organization by ID.
    /// </summary>
    /// <param name="id">Organization ID.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Organization details</returns>    
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
    ///     Creates a new organization.
    /// </summary>
    /// <param name="organizationDto">Organization data transfer object.</param>
    /// <param name="ownerId">ID of the user who will own the organization.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Created organization.</returns>
    [HttpPost(ApiEndpoints.Organizations.Create)]
    [ProducesResponseType(typeof(OrganizationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult> CreateOrganization(
        [FromBody] CreateOrganizationRequest organizationDto,
        [FromQuery] string ownerId,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        ArgumentNullException.ThrowIfNull(organizationDto);
        ArgumentException.ThrowIfNullOrEmpty(ownerId);

        UserEntity? owner = await _userRepository.GetByIdAsync(ownerId);
        if (owner is null) return NotFound(new { Message = $"User with ID {ownerId} not found." });

        var newOrg = new OrganizationEntity
        {
            Name = organizationDto.Name ?? "Default Organization",
            Description = organizationDto.Description ?? string.Empty,
            Owner = owner
        };

        await _organizationRepository.AddAsync(newOrg);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetOrganization), new { id = newOrg.Id }, new OrganizationResponse
        {
            Name = newOrg.Name
        });
    }

    /// <summary>
    ///     Updates an existing organization.
    /// </summary>
    /// <param name="id">Organization ID.</param>
    /// <param name="organizationDto">Organization data transfer object.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>No content.</returns>
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
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    /// <summary>
    ///     Deletes an organization by ID.
    /// </summary>
    /// <param name="id">Organization ID.</param>    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>No content.</returns>
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
