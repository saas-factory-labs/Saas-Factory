namespace AppBlueprint.Contracts.B2B.Contracts.Team.Responses;

public class TeamMemberResponse
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
}
