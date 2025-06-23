namespace AppBlueprint.SharedKernel;

public class TodoEntity : IEntity
{
    public TodoEntity()
    {
        Id = PrefixedUlid.Generate("todo");
    }    public string? Title { get; set; }
    public bool IsCompleted { get; set; }
    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
    public bool IsSoftDeleted { get; set; }
}
