namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization;

public class RoleEntity
{
    public RoleEntity()
    {
        UserRoles = new List<UserRoleEntity>();
        RolePermissions = new List<RolePermissionEntity>();
    }

    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }

    public List<UserRoleEntity> UserRoles { get; init; }
    public List<RolePermissionEntity> RolePermissions { get; init; }
}
