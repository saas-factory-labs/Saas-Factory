namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authentication.AuthenticationProvider;

public class AuthenticationProviderEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
}
