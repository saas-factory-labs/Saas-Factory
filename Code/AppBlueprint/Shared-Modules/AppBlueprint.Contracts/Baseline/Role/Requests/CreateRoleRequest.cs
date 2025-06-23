using AppBlueprint.Contracts.Baseline.Permissions.Requests;

namespace AppBlueprint.Contracts.Baseline.Role.Requests;

public class CreateRoleRequest
{
    public string Id { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
    public IReadOnlyList<CreatePermissionRequest>? Permissions { get; set; }
}
