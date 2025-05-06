using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;

public class PermissionRoleEntity
{
    public PermissionRoleEntity()
    {
        Permission = new PermissionEntity();
        Role = new RoleEntity
        {
            Name = "Role"
        };
    }

    public int Id { get; set; }
    public PermissionEntity Permission { get; set; }

    public int RoleId { get; set; }
    public RoleEntity Role { get; set; }
}
