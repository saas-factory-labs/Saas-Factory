# AppBlueprint.TodoAppKernel

A self-contained Todo application module for the AppBlueprint SaaS framework. This module provides a complete todo management system with multi-tenant support, following Clean Architecture principles.

## Overview

The TodoAppKernel module is designed as a standalone, pluggable component that can be easily integrated into the AppBlueprint ecosystem. It includes all necessary components for todo management:

- **Domain Layer**: Core business entities and logic
- **Infrastructure Layer**: Entity Framework configurations and database setup
- **Controllers**: RESTful API endpoints for todo management

## Features

- ✅ **Multi-tenant Support**: Full isolation between tenants using `ITenantScoped`
- ✅ **User Assignment**: Todos can be created and assigned to specific users
- ✅ **Priority System**: Support for Low, Medium, High, and Urgent priorities
- ✅ **Due Dates**: Optional due date tracking for todos
- ✅ **Completion Tracking**: Mark todos as complete/incomplete with timestamps
- ✅ **Clean Architecture**: No dependencies on infrastructure from domain layer
- ✅ **Entity Framework Integration**: Optimized database configuration with indexes

## Architecture

### Domain Layer (`Domain/`)
- `TodoEntity`: Core domain entity with business logic
- `TodoPriority`: Enumeration for priority levels
- Domain methods: `MarkAsCompleted()`, `MarkAsIncomplete()`, `UpdateDetails()`, `AssignTo()`

### Infrastructure Layer (`Infrastructure/`)
- `TodoEntityConfiguration`: Entity Framework configuration
- `TodoAppKernelDbContextExtensions`: DbContext integration helpers

### Controllers (`Controllers/`)
- `TodoController`: RESTful API endpoints
- Request/Response models for API interactions

## Integration

### 1. Add Project Reference

Add the TodoAppKernel reference to your project:

```xml
<ProjectReference Include="..\AppBlueprint.TodoAppKernel\AppBlueprint.TodoAppKernel.csproj"/>
```

### 2. Configure Database Context

In your `DbContext`, add the TodoAppKernel configuration:

```csharp
using AppBlueprint.TodoAppKernel.Infrastructure;
using AppBlueprint.TodoAppKernel.Domain;

public partial class YourDbContext : DbContext
{
    public DbSet<TodoEntity> Todos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure TodoAppKernel
        modelBuilder.ConfigureTodoAppKernel();
        
        // ... other configurations
        base.OnModelCreating(modelBuilder);
    }
}
```

### 3. Controller Registration

The TodoController will be automatically discovered by ASP.NET Core when the project is referenced.

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/todos` | Get all todos for current tenant |
| POST | `/api/v1/todos` | Create a new todo |
| GET | `/api/v1/todos/{id}` | Get specific todo by ID |
| PUT | `/api/v1/todos/{id}` | Update existing todo |
| DELETE | `/api/v1/todos/{id}` | Delete todo |
| PATCH | `/api/v1/todos/{id}/complete` | Mark todo as complete |

## Request Models

### Create Todo Request
```json
{
  "title": "Complete project documentation",
  "description": "Write comprehensive README and API docs",
  "priority": "High",
  "dueDate": "2025-07-01T12:00:00Z"
}
```

### Update Todo Request
```json
{
  "title": "Updated title",
  "description": "Updated description",
  "priority": "Medium",
  "dueDate": "2025-07-15T12:00:00Z",
  "assignedToId": "user_456"
}
```

## Database Schema

The TodoEntity creates a `Todos` table with the following structure:

| Column | Type | Description |
|--------|------|-------------|
| Id | string(40) | Prefixed ULID primary key |
| Title | string(200) | Todo title (required) |
| Description | string(1000) | Optional description |
| IsCompleted | boolean | Completion status |
| Priority | int | Priority level (0-3) |
| DueDate | datetime | Optional due date |
| CompletedAt | datetime | Completion timestamp |
| TenantId | string(40) | Tenant identifier |
| CreatedById | string(40) | Creator user ID |
| AssignedToId | string(40) | Assigned user ID |
| CreatedAt | datetime | Creation timestamp |
| LastUpdatedAt | datetime | Last update timestamp |
| IsSoftDeleted | boolean | Soft delete flag |

### Indexes

The module includes optimized indexes for common query patterns:

- Single column indexes on TenantId, CreatedById, AssignedToId, IsCompleted, Priority, DueDate
- Composite indexes for tenant-scoped queries
- Soft delete filtering index

## Development Status

This module implements **Step 1** of the TodoApp development plan:

- ✅ Domain entity with multi-tenancy
- ✅ Entity Framework configuration  
- ✅ API controller structure
- 🚧 Repository and service layer (TODO: Step 2)
- 🚧 Business logic and validation (TODO: Step 2)
- 🚧 UI components (TODO: Step 3)

## Dependencies

- AppBlueprint.SharedKernel (BaseEntity, ITenantScoped)
- AppBlueprint.Infrastructure (DbContext integration)
- Entity Framework Core
- ASP.NET Core MVC

## Future Enhancements

- Todo categories and tags
- Recurring todos
- File attachments
- Comments and collaboration
- Notifications and reminders
- Advanced filtering and search
- Bulk operations
