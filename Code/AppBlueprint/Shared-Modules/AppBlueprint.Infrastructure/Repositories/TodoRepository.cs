using AppBlueprint.Infrastructure.DatabaseContexts.B2B;
using AppBlueprint.TodoAppKernel.Domain;
using AppBlueprint.TodoAppKernel.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Todo operations using Entity Framework
/// </summary>
public class TodoRepository : ITodoRepository
{
    private readonly B2BDbContext _dbContext;
    private readonly ILogger<TodoRepository> _logger;

    public TodoRepository(B2BDbContext dbContext, ILogger<TodoRepository> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<TodoEntity>> GetAllAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(tenantId);

        _logger.LogInformation("Getting all todos for tenant {TenantId}", tenantId);

        return await _dbContext.Todos
            .Where(t => t.TenantId == tenantId && !t.IsSoftDeleted)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<TodoEntity?> GetByIdAsync(string id, string tenantId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        ArgumentException.ThrowIfNullOrEmpty(tenantId);

        _logger.LogInformation("Getting todo {TodoId} for tenant {TenantId}", id, tenantId);

        return await _dbContext.Todos
            .FirstOrDefaultAsync(t => t.Id == id && t.TenantId == tenantId && !t.IsSoftDeleted, cancellationToken);
    }

    public async Task<TodoEntity> CreateAsync(TodoEntity todo, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(todo);

        _logger.LogInformation("Creating todo {TodoTitle} for tenant {TenantId}", todo.Title, todo.TenantId);

        _dbContext.Todos.Add(todo);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Todo created with ID {TodoId}", todo.Id);

        return todo;
    }

    public async Task<TodoEntity> UpdateAsync(TodoEntity todo, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(todo);

        _logger.LogInformation("Updating todo {TodoId} for tenant {TenantId}", todo.Id, todo.TenantId);

        _dbContext.Todos.Update(todo);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Todo {TodoId} updated successfully", todo.Id);

        return todo;
    }

    public async Task DeleteAsync(string id, string tenantId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        ArgumentException.ThrowIfNullOrEmpty(tenantId);

        _logger.LogInformation("Deleting todo {TodoId} for tenant {TenantId}", id, tenantId);

        var todo = await GetByIdAsync(id, tenantId, cancellationToken);
        
        if (todo is not null)
        {
            todo.IsSoftDeleted = true;
            todo.LastUpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Todo {TodoId} soft-deleted successfully", id);
        }
        else
        {
            _logger.LogWarning("Todo {TodoId} not found for deletion", id);
        }
    }
}

