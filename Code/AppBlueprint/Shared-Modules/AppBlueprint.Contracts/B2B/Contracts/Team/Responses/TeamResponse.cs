namespace AppBlueprint.Contracts.B2B.Contracts.Team.Responses;

public class TeamResponse(List<TeamMemberResponse>? members)
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public IReadOnlyList<TeamMemberResponse>? Members { get; set; } = members;
}
