namespace AppBlueprint.Contracts.B2B.Contracts.Team.Responses;

public class TeamMemberResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Role { get; set; }
    public DateTime JoinedAt { get; set; }
}
