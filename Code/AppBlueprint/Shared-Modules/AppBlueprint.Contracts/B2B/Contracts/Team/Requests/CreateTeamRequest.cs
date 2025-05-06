namespace AppBlueprint.Contracts.B2B.Contracts.Team.Requests;

public class CreateTeamRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}
