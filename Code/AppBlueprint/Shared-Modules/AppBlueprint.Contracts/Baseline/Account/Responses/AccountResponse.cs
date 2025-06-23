namespace AppBlueprint.Contracts.Baseline.Account.Responses;

public class AccountResponse
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Type { get; set; }
    public string Slug { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
