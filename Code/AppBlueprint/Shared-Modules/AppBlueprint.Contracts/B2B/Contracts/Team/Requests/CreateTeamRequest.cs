namespace AppBlueprint.Contracts.B2B.Contracts.Team.Requests;

/// <summary>
/// Request to create a new team within an organization.
/// </summary>
public class CreateTeamRequest
{
    /// <summary>
    /// Name of the team.
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// Optional description of the team's purpose or responsibilities.
    /// </summary>
    public string? Description { get; set; }
}
