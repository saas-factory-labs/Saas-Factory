using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization.ResourcePermissionType;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization;

public class ResourcePermissionEntity : BaseEntity
{
    public ResourcePermissionEntity()
    {
        Id = PrefixedUlid.Generate("resource_permission");
        PermissionTypes = new List<ResourcePermissionTypeEntity>();
        ResourceId = string.Empty;
        UserId = string.Empty;
    }

    public string UserId { get; set; }
    public required UserEntity User { get; set; }
    public string ResourceId { get; set; }
    public ICollection<ResourcePermissionTypeEntity> PermissionTypes { get; set; }
}
