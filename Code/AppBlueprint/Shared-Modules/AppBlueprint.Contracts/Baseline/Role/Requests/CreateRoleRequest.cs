using AppBlueprint.Contracts.Baseline.Permissions.Requests;

namespace AppBlueprint.Contracts.Baseline.Role.Requests;

public class CreateRoleRequest
{
    public string Id { get; set; } = string.Empty;
    public required string Name { get; set; }
    public string? Description { get; set; }
    public IReadOnlyList<CreatePermissionRequest>? Permissions { get; set; }
}
