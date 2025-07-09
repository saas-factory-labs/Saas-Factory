namespace AppBlueprint.Contracts.Baseline.Permissions.Requests;

public class CreatePermissionRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}
