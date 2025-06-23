using AppBlueprint.SharedKernel;

namespace AppBlueprint.TodoApp.Domain;

/// <summary>
/// Domain entity representing a todo item in a multi-tenant environment.
/// Provides task management with tenant isolation and user assignment.
/// </summary>
public sealed class TodoEntity : BaseEntity, ITenantScoped
{
    public TodoEntity()
    {
        Id = PrefixedUlid.Generate("todo");
        Title = string.Empty;
        TenantId = string.Empty;
        CreatedById = string.Empty;
    }

    public TodoEntity(string title, string? description, string tenantId, string createdById) : this()
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description;
        TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
        CreatedById = createdById ?? throw new ArgumentNullException(nameof(createdById));
        AssignedToId = createdById; // Default assignment to creator
    }

    /// <summary>
    /// The title of the todo item (required).
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Optional description providing more details about the todo item.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Indicates whether the todo item has been completed.
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// The priority level of the todo item.
    /// </summary>
    public TodoPriority Priority { get; set; } = TodoPriority.Medium;

    /// <summary>
    /// Optional due date for the todo item.
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Timestamp when the todo was completed (null if not completed).
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// ITenantScoped implementation - the tenant this todo belongs to.
    /// </summary>
    public string TenantId { get; set; }

    /// <summary>
    /// The user who created this todo item.
    /// </summary>
    public string CreatedById { get; set; }

    /// <summary>
    /// The user assigned to complete this todo item (defaults to creator).
    /// </summary>
    public string? AssignedToId { get; set; }

    /// <summary>
    /// Marks the todo as completed.
    /// </summary>
    public void MarkAsCompleted()
    {
        if (IsCompleted)
        {
            return; // Already completed
        }

        IsCompleted = true;
        CompletedAt = DateTime.UtcNow;
        LastUpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the todo as not completed.
    /// </summary>
    public void MarkAsIncomplete()
    {
        if (!IsCompleted)
        {
            return; // Already incomplete
        }

        IsCompleted = false;
        CompletedAt = null;
        LastUpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the todo details.
    /// </summary>
    public void UpdateDetails(string title, string? description, TodoPriority priority, DateTime? dueDate)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description;
        Priority = priority;
        DueDate = dueDate;
        LastUpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Assigns the todo to a different user.
    /// </summary>
    public void AssignTo(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
        }

        AssignedToId = userId;
        LastUpdatedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Enumeration representing the priority levels for todo items.
/// </summary>
public enum TodoPriority
{
    Low = 0,
    Medium = 1,
    High = 2,
    Urgent = 3
}
