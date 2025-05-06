namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;

public class PermissionEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }

    public DateTime? LastUpdatedAt { get; set; }
    //public RoleModel Role { get; set; }
    //public int RoleId { get; set; }
}
