namespace AppBlueprint.Contracts.B2B.Contracts;

public class TeamInviteRequest
{
    // Team that the user is invited to
    public int TeamId { get; set; }

    // User who is invited to the team
    public int UserId { get; set; }
}
