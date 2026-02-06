using AppBlueprint.TodoAppKernel.Controllers.Dto;
using AppBlueprint.TodoAppKernel.Domain;
using AppBlueprint.TodoAppKernel.Repositories;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.TodoAppKernel.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/todo")]
[Produces("application/json")]
public class TodoController(
    ILogger<TodoController> logger,
    ITodoRepository todoRepository) : ControllerBase
{
    private const string TenantIdKey = "TenantId";
    private const string DefaultTenantId = "default-tenant";

    private readonly ILogger<TodoController> _logger =
        logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ITodoRepository _todoRepository =
        todoRepository ?? throw new ArgumentNullException(nameof(todoRepository));

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TodoEntity>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<IEnumerable<TodoEntity>>> GetTodosAsync(
        CancellationToken cancellationToken)
    {
        string tenantId = HttpContext.Items[TenantIdKey]?.ToString() ?? DefaultTenantId;
        string userId = User.FindFirst("sub")?.Value ?? User.FindFirst("userId")?.Value ?? "unknown-user";

        _logger.LogInformation("Getting todos for tenant {TenantId} and user {UserId}", tenantId, userId);

        var todos = await _todoRepository.GetAllAsync(tenantId, cancellationToken);

        return Ok(todos);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TodoEntity), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TodoEntity>> CreateTodoAsync(
        [FromBody] CreateTodoRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        string tenantId = HttpContext.Items[TenantIdKey]?.ToString() ?? DefaultTenantId;
        string userId = User.FindFirst("sub")?.Value ?? User.FindFirst("userId")?.Value ?? "unknown-user";

        _logger.LogInformation("Creating new todo: {Title} for tenant {TenantId} and user {UserId}",
            request.Title, tenantId, userId);

        var todo = new TodoEntity(request.Title, request.Description, tenantId, userId);
        var createdTodo = await _todoRepository.CreateAsync(todo, cancellationToken);

        _logger.LogInformation("Todo created successfully with ID: {TodoId}", createdTodo.Id);

        return Ok(createdTodo);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TodoEntity), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoEntity>> GetTodoByIdAsync(
        string id,
        CancellationToken cancellationToken)
    {
        string tenantId = HttpContext.Items[TenantIdKey]?.ToString() ?? DefaultTenantId;

        _logger.LogInformation("Getting todo by ID: {Id} for tenant {TenantId}", id, tenantId);

        var todo = await _todoRepository.GetByIdAsync(id, tenantId, cancellationToken);

        if (todo is null)
        {
            return NotFound($"Todo with ID {id} not found");
        }

        return Ok(todo);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(TodoEntity), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TodoEntity>> UpdateTodoAsync(
        string id,
        [FromBody] UpdateTodoRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        string tenantId = HttpContext.Items[TenantIdKey]?.ToString() ?? DefaultTenantId;

        _logger.LogInformation("Updating todo: {Id} for tenant {TenantId}", id, tenantId);

        var todo = await _todoRepository.GetByIdAsync(id, tenantId, cancellationToken);

        if (todo is null)
        {
            return NotFound($"Todo with ID {id} not found");
        }

        todo.Update(request.Title, request.Description);
        var updatedTodo = await _todoRepository.UpdateAsync(todo, cancellationToken);

        _logger.LogInformation("Todo updated successfully: {TodoId}", updatedTodo.Id);

        return Ok(updatedTodo);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteTodoAsync(
        string id,
        CancellationToken cancellationToken)
    {
        string tenantId = HttpContext.Items[TenantIdKey]?.ToString() ?? DefaultTenantId;

        _logger.LogInformation("Deleting todo: {Id} for tenant {TenantId}", id, tenantId);

        await _todoRepository.DeleteAsync(id, tenantId, cancellationToken);

        _logger.LogInformation("Todo soft-deleted successfully: {TodoId}", id);

        return NoContent();
    }

    [HttpPatch("{id}/complete")]
    [ProducesResponseType(typeof(TodoEntity), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoEntity>> CompleteTodoAsync(
        string id,
        CancellationToken cancellationToken)
    {
        string tenantId = HttpContext.Items[TenantIdKey]?.ToString() ?? DefaultTenantId;

        _logger.LogInformation("Completing todo: {Id} for tenant {TenantId}", id, tenantId);

        var todo = await _todoRepository.GetByIdAsync(id, tenantId, cancellationToken);

        if (todo is null)
        {
            return NotFound($"Todo with ID {id} not found");
        }

        todo.MarkAsCompleted();
        var updatedTodo = await _todoRepository.UpdateAsync(todo, cancellationToken);

        _logger.LogInformation("Todo marked as completed: {TodoId}", updatedTodo.Id);

        return Ok(updatedTodo);
    }
}
