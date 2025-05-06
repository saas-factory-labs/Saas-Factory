using AppBlueprint.Contracts.Baseline.Permissions.Requests;

namespace AppBlueprint.Contracts.Baseline.Role.Requests;

public class CreateRoleRequest
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
    public IReadOnlyList<CreatePermissionRequest>? Permissions { get; set; }
}
