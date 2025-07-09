using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization.ResourcePermissionType;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization;

public class ResourcePermissionEntity
{
    public ResourcePermissionEntity()
    {
        PermissionTypes = new List<ResourcePermissionTypeEntity>();
    }

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required UserEntity User { get; set; }
    public Guid ResourceId { get; set; }
    public ICollection<ResourcePermissionTypeEntity> PermissionTypes { get; set; }
}
