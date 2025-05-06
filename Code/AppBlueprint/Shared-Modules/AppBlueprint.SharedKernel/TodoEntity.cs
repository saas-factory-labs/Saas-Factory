namespace AppBlueprint.SharedKernel;

public class TodoEntity : IEntity
{
    public string? Title { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
    public bool IsSoftDeleted { get; set; }
}
