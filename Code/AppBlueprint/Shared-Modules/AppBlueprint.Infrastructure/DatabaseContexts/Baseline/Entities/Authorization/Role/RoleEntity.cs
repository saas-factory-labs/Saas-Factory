using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization;

public sealed class RoleEntity : BaseEntity
{
    private readonly List<UserRoleEntity> _userRoles = new();
    private readonly List<RolePermissionEntity> _rolePermissions = new();

    public RoleEntity()
    {
        Id = PrefixedUlid.Generate("role");
    }

    public required string Name { get; set; }
    public string? Description { get; set; }

    public IReadOnlyCollection<UserRoleEntity> UserRoles => _userRoles.AsReadOnly();
    public IReadOnlyCollection<RolePermissionEntity> RolePermissions => _rolePermissions.AsReadOnly();

    // Domain methods for controlled collection management
    public void AddUserRole(UserRoleEntity userRole)
    {
        ArgumentNullException.ThrowIfNull(userRole);

        if (_userRoles.Any(ur => ur.Id == userRole.Id))
            return; // User role already exists

        _userRoles.Add(userRole);
    }

    public void RemoveUserRole(UserRoleEntity userRole)
    {
        ArgumentNullException.ThrowIfNull(userRole);
        _userRoles.Remove(userRole);
    }

    public void AddRolePermission(RolePermissionEntity rolePermission)
    {
        ArgumentNullException.ThrowIfNull(rolePermission);

        if (_rolePermissions.Any(rp => rp.Id == rolePermission.Id))
            return; // Role permission already exists

        _rolePermissions.Add(rolePermission);
    }

    public void RemoveRolePermission(RolePermissionEntity rolePermission)
    {
        ArgumentNullException.ThrowIfNull(rolePermission);
        _rolePermissions.Remove(rolePermission);
    }
}
