using AppBlueprint.Contracts.Baseline.Permissions.Responses;

namespace AppBlueprint.Contracts.Baseline.Role.Responses;

public class RoleResponse(IReadOnlyList<PermissionResponse> permissions)
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
    public IReadOnlyList<PermissionResponse> Permissions { get; set; } = permissions;
}
