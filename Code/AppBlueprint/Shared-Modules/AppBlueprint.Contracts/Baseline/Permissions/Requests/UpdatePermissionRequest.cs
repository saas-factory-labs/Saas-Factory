namespace AppBlueprint.Contracts.Baseline.Permissions.Requests;

public class UpdatePermissionRequest
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
}
