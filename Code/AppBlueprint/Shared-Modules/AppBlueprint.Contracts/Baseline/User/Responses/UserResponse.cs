namespace AppBlueprint.Contracts.Baseline.User.Responses;

public class UserResponse
{
    public string Id { get; init; } = string.Empty;

    public string? FirstName { get; init; }

    public string? LastName { get; init; }

    public string? UserName { get; init; }
    public string? Email { get; init; }
}
