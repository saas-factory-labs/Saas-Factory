namespace AppBlueprint.Contracts.B2B.Contracts;

public class TeamInviteRequest
{
    // Team that the user is invited to
    public string TeamId { get; set; } = string.Empty;

    // User who is invited to the team
    public string UserId { get; set; } = string.Empty;
}
