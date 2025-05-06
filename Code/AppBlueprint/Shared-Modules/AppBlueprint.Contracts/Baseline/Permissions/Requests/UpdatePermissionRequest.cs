namespace AppBlueprint.Contracts.Baseline.Permissions.Requests;

public class UpdatePermissionRequest
{
    public required string Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
}
