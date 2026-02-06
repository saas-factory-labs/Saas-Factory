using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;

public class PermissionRoleEntity : BaseEntity
{
    public PermissionRoleEntity()
    {
        Id = PrefixedUlid.Generate("permission_role");
        PermissionId = string.Empty;
        RoleId = string.Empty;
        Permission = new PermissionEntity
        {
            Name = "Permission"
        };
        Role = new RoleEntity
        {
            Name = "Role"
        };
    }

    public string PermissionId { get; set; }
    public PermissionEntity Permission { get; set; }

    public string RoleId { get; set; }
    public RoleEntity Role { get; set; }
}
