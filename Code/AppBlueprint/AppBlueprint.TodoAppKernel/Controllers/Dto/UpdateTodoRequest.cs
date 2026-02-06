using System.ComponentModel.DataAnnotations;

namespace AppBlueprint.TodoAppKernel.Controllers.Dto;

/// <summary>
/// Request model for updating an existing todo item
/// </summary>
public record UpdateTodoRequest
{
    /// <summary>
    /// The title of the todo item
    /// </summary>
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// The description of the todo item
    /// </summary>
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; init; }
}
