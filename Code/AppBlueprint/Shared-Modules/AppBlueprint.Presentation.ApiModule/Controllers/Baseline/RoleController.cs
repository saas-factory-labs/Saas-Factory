using AppBlueprint.Contracts.Baseline.Permissions.Responses;
using AppBlueprint.Contracts.Baseline.Role.Requests;
using AppBlueprint.Contracts.Baseline.Role.Responses;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization.Role;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppBlueprint.Presentation.ApiModule.Controllers.Baseline;

[Authorize]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/role")]
[Produces("application/json")]
public class RoleController : BaseController
{
    private readonly IRoleRepository _roleRepository;
    // Removed IUnitOfWork dependency for repository DI pattern

    public RoleController(IConfiguration configuration, IRoleRepository roleRepository) :
        base(configuration)
    {
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        // Removed IUnitOfWork assignment
    }

    /// <summary>
    ///     Retrieves all roles in the system.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>List of roles with their names and associated permissions.</returns>
    /// <remarks>
    ///     Returns all defined roles including system roles and custom roles.
    ///     Requires user authentication.
    /// </remarks>
    /// <response code="200">Returns the list of roles successfully.</response>
    /// <response code="404">No roles found in the system.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet(ApiEndpoints.Roles.GetAll)]
    [ProducesResponseType(typeof(IEnumerable<RoleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<RoleResponse>>> Get(CancellationToken cancellationToken)
    {
        IEnumerable<RoleEntity>? roles = await _roleRepository.GetAllAsync();
        if (!roles.Any()) return NotFound(new { Message = "No roles found." });

        IEnumerable<RoleResponse>? response = roles.Select(role => new RoleResponse(new List<PermissionResponse>())
        {
            Id = role.Id,
            Name = role.Name
        });

        return Ok(response);
    }

    /// <summary>
    ///     Retrieves a specific role by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the role (e.g., "role_01HX...").</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>Role details including name and permissions.</returns>
    /// <remarks>
    ///     Sample request:
    ///     <code>GET /api/v1/role/role_01HX...</code>
    /// </remarks>
    /// <response code="200">Returns the role details successfully.</response>
    /// <response code="404">Role with the specified ID was not found.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet(ApiEndpoints.Roles.GetById)]
    [ProducesResponseType(typeof(RoleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoleResponse>> Get(string id, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(id);

        RoleEntity? role = await _roleRepository.GetByIdAsync(id);
        if (role is null) return NotFound(new { Message = $"Role with ID {id} not found." });

        var response = new RoleResponse(new List<PermissionResponse>())
        {
            Id = role.Id,
            Name = role.Name
        };

        return Ok(response);
    }

    /// <summary>
    ///     Creates a new role with the specified name.
    /// </summary>
    /// <param name="roleDto">Role creation request containing the role name.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>The newly created role with generated ID.</returns>
    /// <remarks>
    ///     Sample request:
    ///     <code>
    ///     POST /api/v1/role
    ///     {
    ///         "name": "Content Manager"
    ///     }
    ///     </code>
    ///     Role names should be unique and descriptive.
    /// </remarks>
    /// <response code="201">Role created successfully. Returns the created role with its ID.</response>
    /// <response code="400">Invalid request data or validation failed.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpPost]
    [ProducesResponseType(typeof(RoleResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Post([FromBody] CreateRoleRequest roleDto, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(roleDto);

        if (!ModelState.IsValid) return BadRequest(ModelState);

        var newRole = new RoleEntity
        {
            Name = roleDto.Name
        };

        await _roleRepository.AddAsync(newRole);
        // If SaveChangesAsync is required, inject a service for it or handle in repository.

        return CreatedAtAction(nameof(Get), new { id = newRole.Id }, newRole);
    }

    /// <summary>
    ///     Updates an existing role's name.
    /// </summary>
    /// <param name="id">The unique identifier of the role to update.</param>
    /// <param name="request">Updated role data containing the new name.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>No content on success.</returns>
    /// <remarks>
    ///     Sample request:
    ///     <code>
    ///     PUT /api/v1/role/role_01HX...
    ///     {
    ///         "name": "Senior Content Manager"
    ///     }
    ///     </code>
    /// </remarks>
    /// <response code="204">Role updated successfully.</response>
    /// <response code="404">Role with the specified ID was not found.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Put(string id, [FromBody] UpdateRoleRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(request);

        RoleEntity? existingRole = await _roleRepository.GetByIdAsync(id);
        if (existingRole is null) return NotFound(new { Message = $"Role with ID {id} not found." });

        existingRole.Name = request.Name;

        _roleRepository.Update(existingRole);
        // If SaveChangesAsync is required, inject a service for it or handle in repository.

        return NoContent();
    }

    /// <summary>
    ///     Deletes a role by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the role to delete.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>No content on success.</returns>
    /// <remarks>
    ///     Sample request:
    ///     <code>DELETE /api/v1/role/role_01HX...</code>
    ///     Warning: Deleting a role may affect users who have been assigned this role.
    ///     Note: Currently commented out in implementation - needs to be uncommented.
    /// </remarks>
    /// <response code="204">Role deleted successfully.</response>
    /// <response code="404">Role with the specified ID was not found.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(id);

        RoleEntity? existingRole = await _roleRepository.GetByIdAsync(id);
        if (existingRole is null) return NotFound(new { Message = $"Role with ID {id} not found." });

        return NoContent();
    }
}
