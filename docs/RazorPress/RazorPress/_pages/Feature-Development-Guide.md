---
title: Feature Development Overview
---

This guide provides an overview of adding new features to AppBlueprint using Clean Architecture and Domain-Driven Design principles.

## Development Process

Adding a new feature typically involves these steps in order:

### 1. Domain Layer - Define Business Logic
Create your entities, value objects, and business rules.

- **What:** Domain entities with strongly-typed IDs, business methods, enums
- **Where:** `Shared-Modules/AppBlueprint.Domain/Entities/`
- **Why:** Encapsulate business logic, ensure type safety

**📖 [Detailed Guide: Adding Domain Entities](./01-adding-domain-entities.md)**

### 2. Infrastructure Layer - Data Access
Implement repositories for data persistence.

- **What:** Repository interfaces and implementations using EF Core
- **Where:** `Shared-Modules/AppBlueprint.Infrastructure/Repositories/`
- **Why:** Abstract database operations, testable data access

**📖 [Detailed Guide: Adding Repositories](./02-adding-repositories.md)**

### 3. Presentation Layer - API Endpoints
Expose your feature through REST APIs.

- **What:** API Controllers with DTOs, validation, error handling
- **Where:** `Shared-Modules/AppBlueprint.Presentation.ApiModule/Controllers/`
- **Why:** Provide external interface, protect internal implementation

**📖 [Detailed Guide: Creating API Controllers](./03-creating-api-controllers.md)**

### 4. UI Layer - User Interface (Optional)
Build Blazor components for user interaction.

- **What:** Blazor components with Tailwind CSS
- **Where:** `AppBlueprint.Web/Components/`
- **Why:** User-friendly interface for your feature

**📖 [Detailed Guide: Creating Blazor Components](./04-creating-blazor-components.md)**

### 5. Tests - Quality Assurance
Write comprehensive tests for your feature.

- **What:** Unit, integration, and API tests
- **Where:** `AppBlueprint.Tests/Features/`
- **Why:** Ensure correctness, prevent regressions

**📖 [Detailed Guide: Writing Tests](./testing-guide.md)**

## Quick Example: Project Management Feature

Here's what implementing a complete "Project" feature looks like:

| Layer | Implementation |
|-------|---------------|
| **Domain** | `Project` entity with `ProjectId`, `Archive()`, `Activate()` methods |
| **Infrastructure** | `IProjectRepository` with `GetByTeamIdAsync()`, `AddAsync()`, etc. |
| **Presentation** | `ProjectController` with GET, POST, PUT, PATCH, DELETE endpoints |
| **UI** | `ProjectList.razor` and `ProjectForm.razor` with Tailwind CSS styling |
| **Tests** | Unit tests for domain logic, integration tests for repository |

## Architecture Layers

AppBlueprint follows Clean Architecture with these layers:

```
┌─────────────────────────────────────────┐
│          Presentation Layer              │ ← API Controllers, Blazor Pages
│   (Web, ApiService, ApiModule)          │
└──────────────────┬──────────────────────┘
                   │
┌──────────────────▼──────────────────────┐
│         Application Layer                │ ← Use Cases, DTOs, Interfaces
│  (Commands, Queries, Services)          │
└──────────────────┬──────────────────────┘
                   │
┌──────────────────▼──────────────────────┐
│          Domain Layer                    │ ← Business Logic, Entities
│  (Entities, Value Objects, Enums)       │
└──────────────────┬──────────────────────┘
                   │
┌──────────────────▼──────────────────────┐
│       Infrastructure Layer               │ ← Data Access, External APIs
│  (DbContext, Repositories, Services)    │
└──────────────────────────────────────────┘
```

**Key Principle:** Dependencies point inward. Domain layer has no dependencies.

## Best Practices Checklist

Before submitting your feature:

- ✅ Use strongly-typed IDs (no `string` or `Guid` for entity IDs)
- ✅ Implement factory methods for entity creation
- ✅ Keep business logic in domain entities
- ✅ Use repository pattern for data access
- ✅ Write tests for all business logic
- ✅ Add XML documentation comments
- ✅ Use proper null checking (`is null`, `ThrowIfNull`)
- ✅ Include cancellation tokens in async methods
- ✅ Return appropriate HTTP status codes
- ✅ Validate input at API boundaries

## Common Patterns

### Strongly-Typed IDs
```csharp
public readonly record struct ProjectId
{
    public Ulid Value { get; }
    public static ProjectId NewId() => new(Ulid.NewUlid());
}
```

### Factory Methods
```csharp
public static Project Create(string name, TeamId teamId)
{
    ArgumentException.ThrowIfNullOrEmpty(name);
    return new Project { Id = ProjectId.NewId(), Name = name, TeamId = teamId };
}
```

### Repository Interface
```csharp
public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(ProjectId id, CancellationToken ct = default);
    Task AddAsync(Project project, CancellationToken ct = default);
}
```

### API Controller
```csharp
[HttpGet("{id}")]
public async Task<ActionResult<ProjectDto>> GetProject(string id)
{
    if (!ProjectId.TryParse(id, out var projectId))
        return BadRequest("Invalid ID");
    
    var project = await _repository.GetByIdAsync(projectId);
    return project is null ? NotFound() : Ok(ToDto(project));
}
```

## Development Workflow

1. **Plan** - Identify entities, relationships, and use cases
2. **Domain** - Create entities with business logic
3. **Database** - Configure EF Core, create migration
4. **Repository** - Implement data access layer
5. **API** - Create controllers and DTOs
6. **Test** - Write comprehensive tests
7. **UI** - Build Blazor components (if needed)
8. **Review** - Run tests, check code quality
9. **Deploy** - Merge PR, deploy changes

## Getting Help

- **Detailed Guides** - Click the links above for step-by-step instructions
- **Code Examples** - Check existing features: `TodoEntity`, `TeamEntity`
- **GitHub Discussions** - Ask questions at [saas-factory-labs/Saas-Factory](https://github.com/saas-factory-labs/Saas-Factory/discussions)

## Next Steps

Start with the domain layer:
- [Adding Domain Entities](./01-adding-domain-entities.md)

Or jump to a specific guide:
- [Adding Repositories](./02-adding-repositories.md)
- [Creating API Controllers](./03-creating-api-controllers.md)
- [Creating Blazor Components](./04-creating-blazor-components.md)
- [Writing Tests](./testing-guide.md)
