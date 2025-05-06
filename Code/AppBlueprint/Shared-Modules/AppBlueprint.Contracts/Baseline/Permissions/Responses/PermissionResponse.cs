namespace AppBlueprint.Contracts.Baseline.Permissions.Responses;

public class PermissionResponse
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
}
