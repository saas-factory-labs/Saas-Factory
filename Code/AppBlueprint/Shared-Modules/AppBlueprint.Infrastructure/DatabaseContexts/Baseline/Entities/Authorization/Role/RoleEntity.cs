using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization;

public class RoleEntity : BaseEntity
{
    public RoleEntity()
    {
        Id = PrefixedUlid.Generate("role");
        UserRoles = new List<UserRoleEntity>();
        RolePermissions = new List<RolePermissionEntity>();
    }

    public required string Name { get; set; }
    public string? Description { get; set; }

    public List<UserRoleEntity> UserRoles { get; init; }
    public List<RolePermissionEntity> RolePermissions { get; init; }
}
