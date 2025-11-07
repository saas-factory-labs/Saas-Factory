using AppBlueprint.TodoAppKernel.Domain;

namespace AppBlueprint.TodoAppKernel.Repositories;

/// <summary>
/// Repository interface for Todo operations
/// </summary>
public interface ITodoRepository
{
    /// <summary>
    /// Gets all todos for a specific tenant
    /// </summary>
    Task<IEnumerable<TodoEntity>> GetAllAsync(string tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific todo by ID and tenant
    /// </summary>
    Task<TodoEntity?> GetByIdAsync(string id, string tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new todo
    /// </summary>
    Task<TodoEntity> CreateAsync(TodoEntity todo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing todo
    /// </summary>
    Task<TodoEntity> UpdateAsync(TodoEntity todo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft deletes a todo
    /// </summary>
    Task DeleteAsync(string id, string tenantId, CancellationToken cancellationToken = default);
}

