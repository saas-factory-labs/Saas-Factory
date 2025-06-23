namespace AppBlueprint.Contracts.Baseline.User.Responses;

public class UserResponse
{
    public string Id { get; set; } = string.Empty;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? UserName { get; set; }
    public string? Email { get; set; }
}
