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
    ///     Gets all teams.
    /// </summary>
    /// <returns>List of teams</returns>
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
    ///     Gets a team by ID.
    /// </summary>
    /// <param name="id">Team ID.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Team details</returns>    
    /// [HttpGet(ApiEndpoints.Teams.GetById)]
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
    ///     Creates a new team.
    /// </summary>
    /// <param name="teamDto">Team data transfer object.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Created team.</returns>
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
    ///     Updates an existing team.
    /// </summary>
    /// <param name="id">Team ID.</param>
    /// <param name="teamDto">Team data transfer object.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>No content.</returns>
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
    ///     Deletes a team by ID.
    /// </summary>
    /// <param name="id">Team ID.</param>
    /// <param name="cancellationToken">Cancellation Token</param>    /// <returns>No content.</returns>
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
