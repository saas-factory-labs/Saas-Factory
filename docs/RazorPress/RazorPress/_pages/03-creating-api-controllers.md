---
title: Creating API Controllers
---

Learn how to create RESTful API controllers following ASP.NET Core best practices.

## Overview

API Controllers in AppBlueprint follow these conventions:
- Inherit from `BaseController` or `ControllerBase`
- Use attribute routing with API versioning
- Return `ActionResult<T>` for type-safe responses
- Include XML documentation comments
- Use proper HTTP status codes
- Validate inputs and handle errors gracefully

## Controller Structure

### Basic Controller Template

**Location:** `Shared-Modules/AppBlueprint.Presentation.ApiModule/Controllers/ProjectController.cs`

```csharp
using AppBlueprint.Application.DTOs;
using AppBlueprint.Application.Interfaces;
using AppBlueprint.Domain.Entities;
using AppBlueprint.SharedKernel;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Presentation.ApiModule.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class ProjectController : ControllerBase
{
    private readonly ILogger<ProjectController> _logger;
    private readonly IProjectRepository _projectRepository;

    public ProjectController(
        ILogger<ProjectController> logger,
        IProjectRepository projectRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
    }

    /// <summary>
    /// Gets all projects for a specific team.
    /// </summary>
    /// <param name="teamId">The team ID to filter projects.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of projects.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProjectDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects(
        [FromQuery] string? teamId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(teamId))
        {
            return BadRequest("TeamId is required");
        }

        if (!TeamId.TryParse(teamId, out var parsedTeamId))
        {
            return BadRequest("Invalid team ID format");
        }

        _logger.LogInformation("Getting projects for team {TeamId}", teamId);

        var projects = await _projectRepository.GetByTeamIdAsync(parsedTeamId, cancellationToken);

        var projectDtos = projects.Select(p => new ProjectDto
        {
            Id = p.Id.ToString(),
            Name = p.Name,
            Description = p.Description,
            TeamId = p.TeamId.ToString(),
            Status = p.Status.ToString(),
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        });

        return Ok(projectDtos);
    }

    /// <summary>
    /// Gets a specific project by ID.
    /// </summary>
    /// <param name="id">The project ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The project details.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<ProjectDto>> GetProject(
        string id,
        CancellationToken cancellationToken)
    {
        if (!ProjectId.TryParse(id, out var projectId))
        {
            return BadRequest("Invalid project ID format");
        }

        _logger.LogInformation("Getting project {ProjectId}", id);

        var project = await _projectRepository.GetByIdAsync(projectId, cancellationToken);

        if (project is null)
        {
            _logger.LogWarning("Project {ProjectId} not found", id);
            return NotFound($"Project with ID {id} not found");
        }

        var projectDto = new ProjectDto
        {
            Id = project.Id.ToString(),
            Name = project.Name,
            Description = project.Description,
            TeamId = project.TeamId.ToString(),
            Status = project.Status.ToString(),
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt
        };

        return Ok(projectDto);
    }

    /// <summary>
    /// Creates a new project.
    /// </summary>
    /// <param name="request">The project creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created project.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<ProjectDto>> CreateProject(
        [FromBody] CreateProjectRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!TeamId.TryParse(request.TeamId, out var teamId))
        {
            return BadRequest("Invalid team ID format");
        }

        _logger.LogInformation("Creating new project: {ProjectName} for team {TeamId}", 
            request.Name, request.TeamId);

        var project = Project.Create(request.Name, request.Description, teamId);
        await _projectRepository.AddAsync(project, cancellationToken);

        _logger.LogInformation("Project created successfully with ID: {ProjectId}", project.Id);

        var projectDto = new ProjectDto
        {
            Id = project.Id.ToString(),
            Name = project.Name,
            Description = project.Description,
            TeamId = project.TeamId.ToString(),
            Status = project.Status.ToString(),
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt
        };

        return CreatedAtAction(
            nameof(GetProject),
            new { id = project.Id.ToString() },
            projectDto);
    }

    /// <summary>
    /// Updates an existing project.
    /// </summary>
    /// <param name="id">The project ID.</param>
    /// <param name="request">The update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated project.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<ProjectDto>> UpdateProject(
        string id,
        [FromBody] UpdateProjectRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!ProjectId.TryParse(id, out var projectId))
        {
            return BadRequest("Invalid project ID format");
        }

        _logger.LogInformation("Updating project {ProjectId}", id);

        var project = await _projectRepository.GetByIdAsync(projectId, cancellationToken);

        if (project is null)
        {
            _logger.LogWarning("Project {ProjectId} not found", id);
            return NotFound($"Project with ID {id} not found");
        }

        project.UpdateDetails(request.Name, request.Description);
        await _projectRepository.UpdateAsync(project, cancellationToken);

        _logger.LogInformation("Project updated successfully: {ProjectId}", id);

        var projectDto = new ProjectDto
        {
            Id = project.Id.ToString(),
            Name = project.Name,
            Description = project.Description,
            TeamId = project.TeamId.ToString(),
            Status = project.Status.ToString(),
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt
        };

        return Ok(projectDto);
    }

    /// <summary>
    /// Archives a project (soft delete).
    /// </summary>
    /// <param name="id">The project ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content.</returns>
    [HttpPatch("{id}/archive")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult> ArchiveProject(
        string id,
        CancellationToken cancellationToken)
    {
        if (!ProjectId.TryParse(id, out var projectId))
        {
            return BadRequest("Invalid project ID format");
        }

        _logger.LogInformation("Archiving project {ProjectId}", id);

        var project = await _projectRepository.GetByIdAsync(projectId, cancellationToken);

        if (project is null)
        {
            _logger.LogWarning("Project {ProjectId} not found", id);
            return NotFound($"Project with ID {id} not found");
        }

        project.Archive();
        await _projectRepository.UpdateAsync(project, cancellationToken);

        _logger.LogInformation("Project archived successfully: {ProjectId}", id);

        return NoContent();
    }

    /// <summary>
    /// Deletes a project permanently.
    /// </summary>
    /// <param name="id">The project ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult> DeleteProject(
        string id,
        CancellationToken cancellationToken)
    {
        if (!ProjectId.TryParse(id, out var projectId))
        {
            return BadRequest("Invalid project ID format");
        }

        _logger.LogInformation("Deleting project {ProjectId}", id);

        await _projectRepository.DeleteAsync(projectId, cancellationToken);

        _logger.LogInformation("Project deleted successfully: {ProjectId}", id);

        return NoContent();
    }
}
```

## DTOs (Data Transfer Objects)

Create request and response DTOs in the Application layer.

**Location:** `Shared-Modules/AppBlueprint.Application/DTOs/ProjectDto.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace AppBlueprint.Application.DTOs;

public record ProjectDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string TeamId { get; init; }
    public required string Status { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public record CreateProjectRequest
{
    [Required(ErrorMessage = "Project name is required")]
    [StringLength(200, ErrorMessage = "Name must not exceed 200 characters")]
    public required string Name { get; init; }

    [Required(ErrorMessage = "Description is required")]
    [StringLength(2000, ErrorMessage = "Description must not exceed 2000 characters")]
    public required string Description { get; init; }

    [Required(ErrorMessage = "Team ID is required")]
    public required string TeamId { get; init; }
}

public record UpdateProjectRequest
{
    [Required(ErrorMessage = "Project name is required")]
    [StringLength(200, ErrorMessage = "Name must not exceed 200 characters")]
    public required string Name { get; init; }

    [Required(ErrorMessage = "Description is required")]
    [StringLength(2000, ErrorMessage = "Description must not exceed 2000 characters")]
    public required string Description { get; init; }
}
```

## HTTP Status Codes Guide

Use appropriate status codes for different scenarios:

| Status Code | When to Use | Example |
|-------------|-------------|---------|
| **200 OK** | Successful GET, PUT, PATCH | Returning data, successful update |
| **201 Created** | Successful POST | New resource created |
| **204 No Content** | Successful DELETE or action with no response | Delete succeeded |
| **400 Bad Request** | Invalid input, validation failed | Missing required fields |
| **401 Unauthorized** | Missing/invalid authentication | No JWT token |
| **403 Forbidden** | Authenticated but not authorized | User not in correct role |
| **404 Not Found** | Resource doesn't exist | Entity with ID not found |
| **409 Conflict** | Resource conflict | Duplicate entry |
| **500 Internal Server Error** | Unexpected server error | Unhandled exception |

## Best Practices

### ✅ DO
- Use async/await with cancellation tokens
- Validate strongly-typed IDs before use
- Log at appropriate levels (Information, Warning, Error)
- Return DTOs, not domain entities
- Use XML documentation comments
- Include proper ProducesResponseType attributes
- Validate ModelState for POST/PUT requests
- Use primary constructor syntax where appropriate

### ❌ DON'T
- Don't return domain entities directly
- Don't catch and hide exceptions without logging
- Don't use `string` for IDs - parse to strongly-typed IDs
- Don't forget authorization attributes
- Don't ignore cancellation tokens
- Don't expose internal implementation details

## Advanced Patterns

### Multi-Tenancy

Extract tenant ID from HttpContext:

```csharp
private string GetTenantId()
{
    return HttpContext.Items["TenantId"]?.ToString() 
        ?? throw new InvalidOperationException("Tenant ID not found");
}

[HttpGet]
public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects(
    CancellationToken cancellationToken)
{
    var tenantId = GetTenantId();
    var projects = await _projectRepository.GetByTenantIdAsync(tenantId, cancellationToken);
    // ...
}
```

### Action Filters

Create reusable filters for cross-cutting concerns:

```csharp
public class ValidateModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            context.Result = new BadRequestObjectResult(context.ModelState);
        }
    }
}

[HttpPost]
[ValidateModel]
public async Task<ActionResult> CreateProject([FromBody] CreateProjectRequest request)
{
    // ModelState already validated by filter
}
```

### Error Handling

Implement global exception handling:

```csharp
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var response = new
        {
            Message = "An error occurred processing your request",
            Details = exception.Message
        };

        return context.Response.WriteAsJsonAsync(response);
    }
}
```

## Testing Controllers

Test controller actions using integration tests:

```csharp
public class ProjectControllerTests
{
    [Test]
    public async Task CreateProject_WithValidData_ReturnsCreated()
    {
        // Arrange
        var request = new CreateProjectRequest
        {
            Name = "Test Project",
            Description = "Test Description",
            TeamId = TeamId.NewId().ToString()
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/project", request);

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);
    }
}
```

## Next Steps

- [Creating Blazor Components with Tailwind CSS](./04-creating-blazor-components.md)
- [Writing Tests](./testing-guide.md)
- [API Versioning](../quick-start.md#key-configuration-files)

## Examples in Codebase

See existing implementations:
- `TodoController` - Basic CRUD operations
- `TeamController` - Relationships and validation
- `AuthenticationController` - Complex business logic
- `Dashboard.razor` - Tailwind CSS admin dashboard
- `Todos.razor` - Full CRUD page with Tailwind CSS
