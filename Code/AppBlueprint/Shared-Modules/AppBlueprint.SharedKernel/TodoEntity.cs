namespace AppBlueprint.SharedKernel;

/// <summary>
/// DEPRECATED: This TodoEntity is being replaced by the new multi-tenant TodoEntity in AppBlueprint.Domain.B2B.Todos.
/// Use AppBlueprint.Domain.B2B.Todos.TodoEntity instead for proper multi-tenancy support.
/// This class will be removed in a future version.
/// </summary>
[Obsolete("Use AppBlueprint.Domain.B2B.Todos.TodoEntity instead for proper multi-tenancy support. This class will be removed in a future version.")]
public class TodoEntity : IEntity
{
    public TodoEntity()
    {
        Id = PrefixedUlid.Generate("todo");
    }
    public string? Title { get; set; }
    public bool IsCompleted { get; set; }
    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
    public bool IsSoftDeleted { get; set; }
}
