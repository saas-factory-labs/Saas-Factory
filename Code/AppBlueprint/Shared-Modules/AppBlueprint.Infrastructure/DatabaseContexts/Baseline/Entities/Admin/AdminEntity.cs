namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Admin;

public class AdminEntity
{
    public int AccountId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
