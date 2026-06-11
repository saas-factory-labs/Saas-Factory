using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization.Permission;

public class PermissionEntity : BaseEntity
{
    public PermissionEntity()
    {
        Id = PrefixedUlid.Generate("permission");
    }

    public required string Name { get; set; }
    public string? Description { get; set; }
    //public RoleModel Role { get; set; }
    //public int RoleId { get; set; }
}
