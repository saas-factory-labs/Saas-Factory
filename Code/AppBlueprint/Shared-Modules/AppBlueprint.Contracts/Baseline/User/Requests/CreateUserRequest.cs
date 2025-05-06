namespace AppBlueprint.Contracts.Baseline.User.Requests;

public class CreateUserRequest
{
    public required string Name { get; set; }
    public required string? Email { get; set; }
}
