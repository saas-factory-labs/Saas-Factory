namespace AppBlueprint.Contracts.Baseline.User.Requests;

public class UpdateUserRequest
{
    public required string Name { get; set; }
    public required string Email { get; set; }
}
