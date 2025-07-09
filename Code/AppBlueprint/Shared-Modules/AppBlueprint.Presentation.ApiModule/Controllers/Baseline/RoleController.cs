using AppBlueprint.Contracts.Baseline.Permissions.Responses;
using AppBlueprint.Contracts.Baseline.Role.Requests;
using AppBlueprint.Contracts.Baseline.Role.Responses;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using AppBlueprint.Application.Interfaces.UnitOfWork;
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
    private readonly IConfiguration _configuration;
    private readonly IRoleRepository _roleRepository;
    // Removed IUnitOfWork dependency for repository DI pattern

    public RoleController(IConfiguration configuration, IRoleRepository roleRepository) :
        base(configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        // Removed IUnitOfWork assignment
    }

    /// <summary>
    ///     Gets all roles.
    /// </summary>
    /// <returns>List of roles</returns>
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
    ///     Gets a role by ID.
    /// </summary>
    /// <param name="id">Role ID.</param>    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Role</returns>
    [HttpGet(ApiEndpoints.Roles.GetById)]
    [ProducesResponseType(typeof(RoleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoleResponse>> Get(string id, CancellationToken cancellationToken)
    {
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
    ///     Creates a new role.
    /// </summary>
    /// <param name="roleDto">Role data transfer object.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Created role.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(RoleResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Post([FromBody] CreateRoleRequest roleDto, CancellationToken cancellationToken)
    {
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
    ///     Updates an existing role.
    /// </summary>
    /// <param name="id">Role ID.</param>
    /// <param name="request">Role data transfer object.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>No content.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]    public async Task<ActionResult> Put(string id, [FromBody] UpdateRoleRequest request,
        CancellationToken cancellationToken)
    {
        RoleEntity? existingRole = await _roleRepository.GetByIdAsync(id);
        if (existingRole is null) return NotFound(new { Message = $"Role with ID {id} not found." });

        existingRole.Name = request.Name;

        _roleRepository.Update(existingRole);
        // If SaveChangesAsync is required, inject a service for it or handle in repository.

        return NoContent();
    }

    /// <summary>
    ///     Deletes a role by ID.
    /// </summary>    /// <param name="id">Role ID.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>No content.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        RoleEntity? existingRole = await _roleRepository.GetByIdAsync(id);
        if (existingRole is null) return NotFound(new { Message = $"Role with ID {id} not found." });

        // _roleRepository.Delete(existingRole);
        // await _unitOfWork.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}

//public class RoleRequestDto
//{
//    public string Name { get; set; }
//}

//public class RoleResponseDto
//{
//    public int Id { get; set; }
//    public string Name { get; set; }
//}
