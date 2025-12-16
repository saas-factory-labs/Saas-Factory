---
title: Adding Repositories
---

Learn how to implement the Repository pattern for data access in AppBlueprint.

## Overview

Repositories provide an abstraction over data access, keeping persistence concerns out of your business logic. In AppBlueprint, repositories:
- Are defined as interfaces in the Application layer
- Are implemented in the Infrastructure layer
- Use Entity Framework Core for data access
- Return domain entities, not DTOs

## Step 1: Define Repository Interface

Create the interface in the Application layer.

**Location:** `Shared-Modules/AppBlueprint.Application/Interfaces/IProjectRepository.cs`

```csharp
using AppBlueprint.Domain.Entities;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Application.Interfaces;

public interface IProjectRepository
{
    // Query methods
    Task<Project?> GetByIdAsync(ProjectId id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> GetByTeamIdAsync(TeamId teamId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> GetActiveProjectsAsync(TeamId teamId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> GetAllAsync(CancellationToken cancellationToken = default);
    
    // Command methods
    Task AddAsync(Project project, CancellationToken cancellationToken = default);
    Task UpdateAsync(Project project, CancellationToken cancellationToken = default);
    Task DeleteAsync(ProjectId id, CancellationToken cancellationToken = default);
}
```

## Step 2: Implement Repository

Create the implementation in the Infrastructure layer.

**Location:** `Shared-Modules/AppBlueprint.Infrastructure/Repositories/ProjectRepository.cs`

```csharp
using AppBlueprint.Application.Interfaces;
using AppBlueprint.Domain.Entities;
using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.Repositories;

public sealed class ProjectRepository : IProjectRepository
{
    private readonly ApplicationDbContext _context;

    public ProjectRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Project?> GetByIdAsync(ProjectId id, CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .Include(p => p.Team) // Include related entities
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Project>> GetByTeamIdAsync(TeamId teamId, CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .Where(p => p.TeamId == teamId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Project>> GetActiveProjectsAsync(TeamId teamId, CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .Where(p => p.TeamId == teamId && p.Status == ProjectStatus.Active)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Project>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .Include(p => p.Team)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Project project, CancellationToken cancellationToken = default)
    {
        await _context.Projects.AddAsync(project, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Project project, CancellationToken cancellationToken = default)
    {
        _context.Projects.Update(project);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ProjectId id, CancellationToken cancellationToken = default)
    {
        var project = await GetByIdAsync(id, cancellationToken) 
            ?? throw new InvalidOperationException($"Project with ID {id} not found");
        
        _context.Projects.Remove(project);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
```

## Step 3: Register Repository in DI Container

Register the repository in the Infrastructure layer's service registration.

**Location:** `Shared-Modules/AppBlueprint.Infrastructure/Extensions/ServiceCollectionExtensions.cs`

```csharp
public static IServiceCollection AddAppBlueprintInfrastructure(
    this IServiceCollection services,
    IConfiguration configuration)
{
    // ... existing registrations

    // Register repositories
    services.AddScoped<IProjectRepository, ProjectRepository>();
    
    return services;
}
```

## Advanced Patterns

### Specification Pattern

For complex queries, use the Specification pattern:

```csharp
public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
    List<Expression<Func<T, object>>> Includes { get; }
}

public class ActiveProjectsSpecification : ISpecification<Project>
{
    public Expression<Func<Project, bool>> Criteria => 
        p => p.Status == ProjectStatus.Active;
    
    public List<Expression<Func<Project, object>>> Includes { get; } = new()
    {
        p => p.Team
    };
}
```

### Soft Delete

Implement soft delete for data retention:

```csharp
public async Task SoftDeleteAsync(ProjectId id, CancellationToken cancellationToken = default)
{
    var project = await GetByIdAsync(id, cancellationToken) 
        ?? throw new InvalidOperationException($"Project with ID {id} not found");
    
    project.IsDeleted = true;
    project.DeletedAt = DateTime.UtcNow;
    
    await UpdateAsync(project, cancellationToken);
}

// Update queries to filter deleted items
public async Task<IEnumerable<Project>> GetAllAsync(CancellationToken cancellationToken = default)
{
    return await _context.Projects
        .Where(p => !p.IsDeleted) // Filter out soft-deleted
        .ToListAsync(cancellationToken);
}
```

### Pagination

For large datasets, implement pagination:

```csharp
public async Task<PagedResult<Project>> GetPagedAsync(
    int pageNumber,
    int pageSize,
    CancellationToken cancellationToken = default)
{
    var totalCount = await _context.Projects.CountAsync(cancellationToken);
    
    var projects = await _context.Projects
        .OrderBy(p => p.Name)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(cancellationToken);
    
    return new PagedResult<Project>
    {
        Items = projects,
        TotalCount = totalCount,
        PageNumber = pageNumber,
        PageSize = pageSize
    };
}
```

## Best Practices

### ✅ DO
- Keep repositories simple and focused
- Use async methods with cancellation tokens
- Include related entities when needed
- Return `null` for not found items (use `FirstOrDefaultAsync`)
- Throw exceptions for unexpected errors
- Use `IQueryable` for deferred execution when appropriate

### ❌ DON'T
- Don't put business logic in repositories
- Don't return `IQueryable` directly from repositories
- Don't use `Find()` - use `FirstOrDefaultAsync()` instead
- Don't forget to call `SaveChangesAsync()`
- Don't catch and swallow exceptions

## Common Patterns

### Eager Loading

Load related entities upfront:

```csharp
return await _context.Projects
    .Include(p => p.Team)
        .ThenInclude(t => t.Members)
    .Include(p => p.Tasks)
    .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
```

### Explicit Loading

Load related entities on demand:

```csharp
var project = await _context.Projects
    .FindAsync(new object[] { id }, cancellationToken);

if (project is not null)
{
    await _context.Entry(project)
        .Collection(p => p.Tasks)
        .LoadAsync(cancellationToken);
}
```

### Projection to DTOs

Project directly to DTOs for read-only queries:

```csharp
public async Task<IEnumerable<ProjectSummaryDto>> GetProjectSummariesAsync(
    TeamId teamId,
    CancellationToken cancellationToken = default)
{
    return await _context.Projects
        .Where(p => p.TeamId == teamId)
        .Select(p => new ProjectSummaryDto
        {
            Id = p.Id.ToString(),
            Name = p.Name,
            Status = p.Status.ToString(),
            TaskCount = p.Tasks.Count
        })
        .ToListAsync(cancellationToken);
}
```

## Testing Repositories

Use in-memory database for testing:

```csharp
public class ProjectRepositoryTests
{
    private ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        return new ApplicationDbContext(options);
    }

    [Test]
    public async Task AddAsync_ShouldPersistProject()
    {
        // Arrange
        await using var context = CreateContext();
        var repository = new ProjectRepository(context);
        var project = Project.Create("Test", "Description", TeamId.NewId());

        // Act
        await repository.AddAsync(project);

        // Assert
        var retrieved = await repository.GetByIdAsync(project.Id);
        await Assert.That(retrieved).IsNotNull();
    }
}
```

## Next Steps

- [Creating API Controllers](./03-creating-api-controllers.md)
- [Writing Tests](./testing-guide.md)
- [Database Migrations](../quick-start.md#database-management)

## Examples in Codebase

See existing implementations:
- `ITodoRepository` / `TodoRepository`
- `ITeamRepository` / `TeamRepository`
- `IOrganizationRepository` / `OrganizationRepository`
