namespace AppBlueprint.Contracts.B2B.Contracts.Team.Responses;

public class TeamResponse(IReadOnlyList<TeamMemberResponse>? members)
{
    public string Id { get; init; } = string.Empty;
    public string? Name { get; init; }
    public string? Description { get; init; }
    public IReadOnlyList<TeamMemberResponse>? Members { get; init; } = members;
}
