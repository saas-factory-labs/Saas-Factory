using AppBlueprint.Contracts.Baseline.Permissions.Responses;

namespace AppBlueprint.Contracts.Baseline.Role.Responses;

public class RoleResponse(IReadOnlyList<PermissionResponse> permissions)
{
    public string Id { get; init; } = string.Empty;
    public string? Name { get; init; }
    public string? Description { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? LastUpdatedAt { get; init; }
    public IReadOnlyList<PermissionResponse> Permissions { get; init; } = permissions;
}
