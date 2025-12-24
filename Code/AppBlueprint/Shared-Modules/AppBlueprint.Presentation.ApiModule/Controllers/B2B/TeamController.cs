using AppBlueprint.Contracts.B2B.Contracts.Team.Requests;
using AppBlueprint.Contracts.B2B.Contracts.Team.Responses;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.Team;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using AppBlueprint.Application.Interfaces.UnitOfWork;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppBlueprint.Presentation.ApiModule.Controllers.B2B;

[Authorize]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}")]
[Produces("application/json")]
public class TeamController : BaseController
{
    private readonly IConfiguration _configuration;
    private readonly ITeamRepository _teamRepository;

    public TeamController(
        IConfiguration configuration,
        ITeamRepository teamRepository)
        : base(configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
    }

    /// <summary>
    ///     Retrieves all teams in the system.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>List of teams with their details and members.</returns>
    /// <remarks>
    ///     Sample response:
    ///     <code>
    ///     [
    ///         {
    ///             "id": "team_01HX...",
    ///             "name": "Engineering Team",
    ///             "description": "Core development team",
    ///             "teamMembers": [...]
    ///         }
    ///     ]
    ///     </code>
    /// </remarks>
    /// <response code="200">Returns the list of teams successfully.</response>
    /// <response code="404">No teams found in the system.</response>
    [HttpGet(ApiEndpoints.Teams.GetAll)]
    [ProducesResponseType(typeof(IEnumerable<TeamResponse>), StatusCodes.Status200OK)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult<IEnumerable<TeamResponse>>> GetTeams(CancellationToken cancellationToken)
    {
        try
        {
            IEnumerable<TeamEntity> teams = await _teamRepository.GetAllAsync();

            // Return empty list instead of 404 - more RESTful
            if (!teams.Any())
            {
                return Ok(Enumerable.Empty<TeamResponse>());
            }

            var response = teams.Select(team => new TeamResponse(
                team.TeamMembers?.Select(m => new TeamMemberResponse
                {
                    Id = m.Id,
                    UserId = m.UserId,
                    Role = string.Empty,
                    JoinedAt = DateTime.UtcNow
                }).ToList()
            )
            {
                Id = team.Id,
                Name = team.Name,
                Description = team.Description
            });

            return Ok(response);
        }
        catch (Exception ex)
        {
            // Log the error and return 500 with generic message
            return StatusCode(500, new { Message = "Error retrieving teams. Please try again or contact support." });
        }
    }

    /// <summary>
    ///     Retrieves a specific team by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the team (e.g., "team_01HX...").</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>Team details including name, description, and team members.</returns>
    /// <remarks>
    ///     Sample request:
    ///     <code>GET /api/v1/team/team_01HX...</code>
    /// </remarks>
    /// <response code="200">Returns the team details successfully.</response>
    /// <response code="404">Team with the specified ID was not found.</response>
    [ProducesResponseType(typeof(TeamResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    [HttpGet(ApiEndpoints.Teams.GetById)]
    public async Task<ActionResult<TeamResponse>> GetTeam(string id, CancellationToken cancellationToken)
    {
        TeamEntity? team = await _teamRepository.GetByIdAsync(id);
        if (team is null) return NotFound(new { Message = $"Team with ID {id} not found." });

        var response = new TeamResponse(team.TeamMembers?.Select(m => new TeamMemberResponse
        {
            // Id = m.Id,
            // UserId = m.UserId,
            // Role = m.Role,
            // JoinedAt = m.JoinedAt
        }).ToList())
        {
            Id = team.Id,
            Name = team.Name,
            Description = team.Description,
        };

        return Ok(response);
    }

    /// <summary>
    ///     Creates a new team with the specified details.
    /// </summary>
    /// <param name="teamDto">Team creation request containing name and description.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>The newly created team with generated ID.</returns>
    /// <remarks>
    ///     Sample request:
    ///     <code>
    ///     POST /api/v1/team
    ///     {
    ///         "name": "Engineering Team",
    ///         "description": "Core development team"
    ///     }
    ///     </code>
    /// </remarks>
    /// <response code="201">Team created successfully. Returns the created team with its ID.</response>
    /// <response code="400">Invalid request data or validation failed.</response>
    [HttpPost(ApiEndpoints.Teams.Create)]
    [ProducesResponseType(typeof(TeamResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult> CreateTeam([FromBody] CreateTeamRequest teamDto,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(teamDto);

        if (!ModelState.IsValid) return BadRequest(ModelState);

        var newTeam = new TeamEntity
        {
            Name = teamDto.Name,
            Description = teamDto.Description
        };

        await _teamRepository.AddAsync(newTeam);
        // If SaveChangesAsync is required, inject a service for it or handle in repository.

        return CreatedAtAction(nameof(GetTeam), new { id = newTeam.Id }, new TeamResponse(new List<TeamMemberResponse>())
        {
            Id = newTeam.Id,
            Name = newTeam.Name,
            Description = newTeam.Description,
        });
    }

    /// <summary>
    ///     Updates an existing team's details.
    /// </summary>
    /// <param name="id">The unique identifier of the team to update.</param>
    /// <param name="teamDto">Updated team data containing name and/or description.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>No content on success.</returns>
    /// <remarks>
    ///     Sample request:
    ///     <code>
    ///     PUT /api/v1/team/team_01HX...
    ///     {
    ///         "name": "Updated Team Name",
    ///         "description": "Updated description"
    ///     }
    ///     </code>
    ///     Note: Only provided fields will be updated. Null values are ignored.
    /// </remarks>
    /// <response code="204">Team updated successfully.</response>
    /// <response code="404">Team with the specified ID was not found.</response>
    [HttpPut(ApiEndpoints.Teams.UpdateById)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult> UpdateTeam(string id, [FromBody] UpdateTeamRequest teamDto,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(teamDto);

        TeamEntity? existingTeam = await _teamRepository.GetByIdAsync(id);
        if (existingTeam is null) return NotFound(new { Message = $"Team with ID {id} not found." });

        if (teamDto.Name != null)
            existingTeam.Name = teamDto.Name;
        if (teamDto.Description != null)
            existingTeam.Description = teamDto.Description;

        _teamRepository.Update(existingTeam);
        // If SaveChangesAsync is required, inject a service for it or handle in repository.

        return NoContent();
    }

    /// <summary>
    ///     Deletes a team by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the team to delete.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>No content on success.</returns>
    /// <remarks>
    ///     Sample request:
    ///     <code>DELETE /api/v1/team/team_01HX...</code>
    ///     Warning: This operation cannot be undone. All team data and associations will be removed.
    /// </remarks>
    /// <response code="204">Team deleted successfully.</response>
    /// <response code="404">Team with the specified ID was not found.</response>
    [HttpDelete(ApiEndpoints.Teams.DeleteById)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult> DeleteTeam(string id, CancellationToken cancellationToken)
    {
        TeamEntity? existingTeam = await _teamRepository.GetByIdAsync(id);
        if (existingTeam is null) return NotFound(new { Message = $"Team with ID {id} not found." });

        _teamRepository.Delete(existingTeam.Id);

        return NoContent();
    }
}
