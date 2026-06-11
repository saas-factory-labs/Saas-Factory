using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization.Permission;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization.Role;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization.RolePermission;

public class RolePermissionEntity : BaseEntity
{
    public RolePermissionEntity()
    {
        Id = PrefixedUlid.Generate("role_perm");
    }

    public required string RoleId { get; set; }
    public required RoleEntity Role { get; set; }

    public required string PermissionId { get; set; }
    public required PermissionEntity Permission { get; set; }
}
