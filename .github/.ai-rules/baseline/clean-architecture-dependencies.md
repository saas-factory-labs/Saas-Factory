# Clean Architecture - Dependency Flow

This document defines the strict dependency rules for clean architecture layers in this codebase. **Violations of these rules must be prevented at all costs.**

## Core Principle

**The Dependency Rule:** Source code dependencies must only point inward toward higher-level policies. Inner layers must not know anything about outer layers.

```
┌─────────────────────────────────────┐
│         Presentation/Web            │ ← User Interface, Controllers, Pages, Components
│  (AppBlueprint.Web, AppBlueprint.   │
│   ApiService, AppBlueprint.Tests)   │
└──────────────┬──────────────────────┘
               │ depends on ↓
┌──────────────▼──────────────────────┐
│         Infrastructure              │ ← External Concerns, Database, Auth, Email
│  (AppBlueprint.Infrastructure)      │
└──────────────┬──────────────────────┘
               │ depends on ↓
┌──────────────▼──────────────────────┐
│          Application                │ ← Use Cases, Business Logic, Orchestration
│  (AppBlueprint.Application)         │
└──────────────┬──────────────────────┘
               │ depends on ↓
┌──────────────▼──────────────────────┐
│            Domain                   │ ← Entities, Value Objects, Enums, Interfaces
│  (AppBlueprint.Domain)              │
│  (AppBlueprint.SharedKernel)        │
│  (AppBlueprint.Contracts)           │
└─────────────────────────────────────┘
          Core - No Dependencies
```

---

## Layer Definitions

### 1. Domain Layer (Core)

**Projects:**
- `AppBlueprint.Domain` - Domain logic, interfaces
- `AppBlueprint.SharedKernel` - Shared types (enums, base classes, exceptions)
- `AppBlueprint.Contracts` - DTOs, requests, responses

**Purpose:** Contains enterprise business rules and domain models.

**Can Depend On:**
- ❌ **NOTHING** - This is the innermost layer with zero external dependencies
- ✅ Only standard .NET libraries (System.*, Microsoft.Extensions.DependencyInjection.Abstractions)

**Responsibilities:**
- Domain entities (aggregates, entities, value objects)
- Domain events
- Enums and constants
- Domain exceptions
- Repository interfaces (contracts only, not implementations)
- Domain service interfaces
- Data Transfer Objects (DTOs)
- API contracts (requests, responses)

**Examples:**
```csharp
// ✅ Correct - Domain entity with no dependencies
namespace AppBlueprint.Domain.TodoApp;

public sealed class TodoItem : Entity<TodoItemId>
{
    public string Title { get; private set; }
    public bool IsCompleted { get; private set; }
    
    public void Complete() => IsCompleted = true;
}

// ✅ Correct - Repository interface in Domain
namespace AppBlueprint.Domain.TodoApp;

public interface ITodoRepository
{
    Task<TodoItem?> GetByIdAsync(TodoItemId id);
    Task AddAsync(TodoItem item);
}

// ❌ WRONG - Domain depending on Infrastructure
using AppBlueprint.Infrastructure.DatabaseContexts; // NO!

public sealed class TodoItem
{
    private readonly BaselineDbContext _db; // NO! Infrastructure dependency
}

// ❌ WRONG - Domain depending on Application
using AppBlueprint.Application.Services; // NO!

public sealed class TodoItem
{
    private readonly IEmailService _emailService; // NO! Application dependency
}
```

---

### 2. Application Layer

**Projects:**
- `AppBlueprint.Application`

**Purpose:** Contains application business rules, use cases, and orchestration logic.

**Can Depend On:**
- ✅ Domain layer (`AppBlueprint.Domain`, `AppBlueprint.SharedKernel`, `AppBlueprint.Contracts`)
- ❌ Infrastructure layer (`AppBlueprint.Infrastructure`) - **NEVER**
- ❌ Presentation layer (Web, ApiService, Tests) - **NEVER**

**Responsibilities:**
- Application services (orchestration)
- CQRS commands and queries (handlers)
- Application-level validation
- Service interfaces (IEmailService, IFileStorageService, etc.)
- Mappers (Domain ↔ DTOs)
- Application exceptions
- Background jobs
- Event handlers

**Examples:**
```csharp
// ✅ Correct - Application service depending on Domain interfaces
namespace AppBlueprint.Application.TodoApp;

using AppBlueprint.Domain.TodoApp; // ✅ Domain dependency OK

public sealed class TodoService(ITodoRepository repository)
{
    public async Task<TodoItem> CreateTodoAsync(CreateTodoRequest request)
    {
        var todo = TodoItem.Create(request.Title);
        await repository.AddAsync(todo);
        return todo;
    }
}

// ✅ Correct - Application interface (contract)
namespace AppBlueprint.Application.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
}

// ❌ WRONG - Application depending on Infrastructure
using AppBlueprint.Infrastructure.Email; // NO!

public sealed class TodoService
{
    private readonly SmtpEmailSender _sender; // NO! Concrete infrastructure class
}

// ❌ WRONG - Application depending on Presentation
using AppBlueprint.Web.Components; // NO!

public sealed class TodoService
{
    private void RenderComponent() { } // NO! UI logic in Application
}
```

---

### 3. Infrastructure Layer

**Projects:**
- `AppBlueprint.Infrastructure`

**Purpose:** Contains implementations of external concerns (database, file system, web services, etc.).

**Can Depend On:**
- ✅ Domain layer (`AppBlueprint.Domain`, `AppBlueprint.SharedKernel`, `AppBlueprint.Contracts`)
- ✅ Application layer (`AppBlueprint.Application`) - for implementing interfaces
- ❌ Presentation layer (Web, ApiService, Tests) - **NEVER**

**Responsibilities:**
- Database contexts (EF Core DbContext)
- Entity configurations (IEntityTypeConfiguration)
- Repository implementations (implementing Domain interfaces)
- Service implementations (implementing Application interfaces)
- External API clients
- Authentication providers
- Email services
- File storage services
- Caching implementations
- Logging implementations

**Examples:**
```csharp
// ✅ Correct - Infrastructure implementing Domain interface
namespace AppBlueprint.Infrastructure.Repositories;

using AppBlueprint.Domain.TodoApp; // ✅ Domain dependency OK
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline; // ✅ Own project OK

public sealed class TodoRepository(BaselineDbContext context) : ITodoRepository
{
    public async Task<TodoItem?> GetByIdAsync(TodoItemId id)
    {
        return await context.TodoItems.FindAsync(id);
    }
    
    public async Task AddAsync(TodoItem item)
    {
        await context.TodoItems.AddAsync(item);
        await context.SaveChangesAsync();
    }
}

// ✅ Correct - Infrastructure implementing Application interface
namespace AppBlueprint.Infrastructure.Email;

using AppBlueprint.Application.Interfaces; // ✅ Application dependency OK

public sealed class SmtpEmailSender : IEmailService
{
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        // SMTP implementation
    }
}

// ❌ WRONG - Infrastructure depending on Presentation
using AppBlueprint.Web.Models; // NO!

public sealed class TodoRepository
{
    public async Task<TodoViewModel> GetTodoViewModelAsync() // NO! UI model
    {
        // ...
    }
}
```

---

### 4. Presentation Layer

**Projects:**
- `AppBlueprint.Web` - Blazor UI
- `AppBlueprint.ApiService` - REST API
- `AppBlueprint.Tests` - Test projects

**Purpose:** Contains user interface and API endpoint logic.

**Can Depend On:**
- ✅ Application layer (`AppBlueprint.Application`)
- ✅ Domain layer (`AppBlueprint.Domain`, `AppBlueprint.SharedKernel`, `AppBlueprint.Contracts`) - for DTOs
- ✅ Infrastructure layer (`AppBlueprint.Infrastructure`) - **ONLY for dependency injection registration**
- ⚠️ **WARNING:** Never use Infrastructure types directly in controllers/pages - only via Application interfaces

**Responsibilities:**
- API controllers (minimal APIs, MVC controllers)
- Blazor pages and components
- View models
- API request/response mapping
- Authentication/authorization attributes
- Dependency injection container configuration
- Startup configuration

**Examples:**
```csharp
// ✅ Correct - Controller depending on Application service
namespace AppBlueprint.ApiService.Controllers;

using AppBlueprint.Application.TodoApp; // ✅ Application dependency OK
using AppBlueprint.Contracts.TodoApp; // ✅ Contracts (DTOs) OK

[ApiController]
[Route("api/todos")]
public sealed class TodoController(TodoService todoService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateTodo([FromBody] CreateTodoRequest request)
    {
        TodoItem todo = await todoService.CreateTodoAsync(request);
        return CreatedAtAction(nameof(GetTodo), new { id = todo.Id }, todo);
    }
}

// ✅ Correct - DI registration in Program.cs
using AppBlueprint.Application.Interfaces;
using AppBlueprint.Infrastructure.Email; // ✅ OK for DI registration only

builder.Services.AddScoped<IEmailService, SmtpEmailSender>();

// ❌ WRONG - Controller using Infrastructure directly
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline; // NO!
using AppBlueprint.Infrastructure.Repositories; // NO!

public sealed class TodoController(BaselineDbContext context) // NO! Direct DB access
{
    [HttpGet]
    public async Task<IActionResult> GetTodos()
    {
        var todos = await context.TodoItems.ToListAsync(); // NO! Direct EF Core query
        return Ok(todos);
    }
}

// ❌ WRONG - Controller using concrete Infrastructure class
using AppBlueprint.Infrastructure.Email; // NO! (except in DI registration)

public sealed class TodoController(SmtpEmailSender emailSender) // NO! Use IEmailService
{
    // ...
}
```

---

## Dependency Injection Pattern

**Correct Pattern:**
```
1. Domain defines interface       (ITodoRepository)
2. Infrastructure implements     (TodoRepository : ITodoRepository)
3. Application uses interface    (TodoService(ITodoRepository repo))
4. Presentation registers in DI  (services.AddScoped<ITodoRepository, TodoRepository>())
5. Presentation calls Application (TodoController(TodoService service))
```

**Example:**
```csharp
// 1. Domain - Interface
namespace AppBlueprint.Domain.TodoApp;
public interface ITodoRepository { }

// 2. Infrastructure - Implementation
namespace AppBlueprint.Infrastructure.Repositories;
using AppBlueprint.Domain.TodoApp;
public sealed class TodoRepository : ITodoRepository { }

// 3. Application - Uses Interface
namespace AppBlueprint.Application.TodoApp;
using AppBlueprint.Domain.TodoApp;
public sealed class TodoService(ITodoRepository repository) { }

// 4. Presentation - DI Registration (Program.cs)
using AppBlueprint.Application.TodoApp;
using AppBlueprint.Domain.TodoApp;
using AppBlueprint.Infrastructure.Repositories;

builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddScoped<TodoService>();

// 5. Presentation - Uses Application
namespace AppBlueprint.ApiService.Controllers;
using AppBlueprint.Application.TodoApp;

[ApiController]
public sealed class TodoController(TodoService service) : ControllerBase { }
```

---

## Common Violations to Avoid

### ❌ Violation 1: Domain depending on Application/Infrastructure
```csharp
// Domain/TodoApp/TodoItem.cs
using AppBlueprint.Infrastructure.DatabaseContexts; // NO!

public sealed class TodoItem
{
    private readonly BaselineDbContext _db; // NO! Breaks dependency rule
}
```

**Fix:** Move logic to Application or Infrastructure layer.

---

### ❌ Violation 2: Application depending on Infrastructure
```csharp
// Application/TodoApp/TodoService.cs
using AppBlueprint.Infrastructure.Email; // NO!

public sealed class TodoService
{
    private readonly SmtpEmailSender _emailSender; // NO! Use interface
}
```

**Fix:** Depend on `IEmailService` interface from Application layer.

```csharp
// Application/Interfaces/IEmailService.cs
public interface IEmailService { }

// Application/TodoApp/TodoService.cs
using AppBlueprint.Application.Interfaces; // ✅

public sealed class TodoService(IEmailService emailService) { }
```

---

### ❌ Violation 3: Presentation bypassing Application layer
```csharp
// Web/Controllers/TodoController.cs
using AppBlueprint.Infrastructure.Repositories; // NO!

public sealed class TodoController(TodoRepository repository) // NO! Use Application service
{
    // Direct repository access from controller
}
```

**Fix:** Create Application service and use that instead.

```csharp
// Application/TodoApp/TodoService.cs
public sealed class TodoService(ITodoRepository repository) { }

// Web/Controllers/TodoController.cs
public sealed class TodoController(TodoService service) { } // ✅
```

---

### ❌ Violation 4: Infrastructure depending on Presentation
```csharp
// Infrastructure/Repositories/TodoRepository.cs
using AppBlueprint.Web.Models; // NO!

public sealed class TodoRepository
{
    public async Task<TodoViewModel> GetViewModel() { } // NO! UI model in Infrastructure
}
```

**Fix:** Return Domain entities, let Presentation map to view models.

---

## Verification Checklist

Before committing code, verify:

- [ ] **Domain layer** has no dependencies on Application, Infrastructure, or Presentation
- [ ] **Application layer** only depends on Domain (never Infrastructure or Presentation)
- [ ] **Infrastructure layer** implements interfaces from Domain/Application (never depends on Presentation)
- [ ] **Presentation layer** uses Application services (not Infrastructure classes directly, except DI registration)
- [ ] All repository interfaces are in Domain, implementations in Infrastructure
- [ ] All service interfaces are in Application, implementations in Infrastructure
- [ ] Controllers/Pages only inject Application services or Domain DTOs
- [ ] No circular dependencies exist between layers

---

## Project References (for verification)

```xml
<!-- Domain (SharedKernel, Domain, Contracts) -->
<!-- NO PROJECT REFERENCES - Only framework packages -->

<!-- Application -->
<ItemGroup>
  <ProjectReference Include="..\AppBlueprint.Domain\AppBlueprint.Domain.csproj" />
  <ProjectReference Include="..\AppBlueprint.SharedKernel\AppBlueprint.SharedKernel.csproj" />
  <ProjectReference Include="..\AppBlueprint.Contracts\AppBlueprint.Contracts.csproj" />
  <!-- NO Infrastructure reference -->
  <!-- NO Presentation reference -->
</ItemGroup>

<!-- Infrastructure -->
<ItemGroup>
  <ProjectReference Include="..\AppBlueprint.Application\AppBlueprint.Application.csproj" />
  <ProjectReference Include="..\AppBlueprint.Domain\AppBlueprint.Domain.csproj" />
  <ProjectReference Include="..\AppBlueprint.SharedKernel\AppBlueprint.SharedKernel.csproj" />
  <ProjectReference Include="..\AppBlueprint.Contracts\AppBlueprint.Contracts.csproj" />
  <!-- NO Presentation reference -->
</ItemGroup>

<!-- Presentation (Web, ApiService, Tests) -->
<ItemGroup>
  <ProjectReference Include="..\AppBlueprint.Application\AppBlueprint.Application.csproj" />
  <ProjectReference Include="..\AppBlueprint.Infrastructure\AppBlueprint.Infrastructure.csproj" />
  <ProjectReference Include="..\AppBlueprint.Domain\AppBlueprint.Domain.csproj" />
  <ProjectReference Include="..\AppBlueprint.SharedKernel\AppBlueprint.SharedKernel.csproj" />
  <ProjectReference Include="..\AppBlueprint.Contracts\AppBlueprint.Contracts.csproj" />
</ItemGroup>
```

---

## Key Takeaways

1. **Dependencies flow inward**: Presentation → Infrastructure → Application → Domain
2. **Domain is independent**: Has zero dependencies on other layers
3. **Application defines interfaces**: Infrastructure implements them
4. **Use Dependency Injection**: To wire up implementations at runtime
5. **Presentation coordinates**: But doesn't contain business logic
6. **Infrastructure is a plugin**: Can be swapped without changing Domain/Application

---

## When in Doubt

Ask yourself:
- "Could I replace the database (Infrastructure) without changing my business logic (Application/Domain)?"
- "Could I swap the UI (Presentation) without changing my use cases (Application)?"
- "Does my Domain entity know about databases, APIs, or UI frameworks?"

If the answer to the third question is "yes", or the first two are "no", you have a dependency violation.

---

## Further Reading

- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Microsoft Clean Architecture Template](https://github.com/jasontaylordev/CleanArchitecture)
- [.NET Architecture Guides](https://learn.microsoft.com/en-us/dotnet/architecture/)
