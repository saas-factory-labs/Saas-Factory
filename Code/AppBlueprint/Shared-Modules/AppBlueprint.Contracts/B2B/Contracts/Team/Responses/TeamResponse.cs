namespace AppBlueprint.Contracts.B2B.Contracts.Team.Responses;

public class TeamResponse(IReadOnlyList<TeamMemberResponse>? members)
{
    public string Id { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Description { get; set; }
    public IReadOnlyList<TeamMemberResponse>? Members { get; init; } = members;
}
