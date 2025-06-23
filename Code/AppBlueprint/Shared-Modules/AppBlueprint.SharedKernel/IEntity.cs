namespace AppBlueprint.SharedKernel;

public interface IEntity
{
    string Id { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime? LastUpdatedAt { get; set; }
    bool IsSoftDeleted { get; set; }
}
