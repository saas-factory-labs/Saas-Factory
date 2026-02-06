namespace AppBlueprint.Contracts.Baseline.Account.Responses;

public class AccountResponse
{
    public string Id { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Type { get; init; }
    public string Slug { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
