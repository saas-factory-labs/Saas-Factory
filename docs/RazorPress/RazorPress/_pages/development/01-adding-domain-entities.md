---
title: Adding Domain Entities
---

Learn how to create domain entities with strongly-typed IDs following Domain-Driven Design principles.

## Overview

Domain entities represent core business concepts in your application. In AppBlueprint, all entities follow these patterns:
- **Strongly-typed IDs** using ULIDs
- **Factory methods** for creation
- **Business logic** encapsulated in entity methods
- **Proper encapsulation** with private setters

## Step 1: Define the Domain Entity

Create your aggregate root in the Domain layer.

**Location:** `Shared-Modules/AppBlueprint.Domain/Entities/Project.cs`

```csharp
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Domain.Entities;

/// <summary>
/// Represents a project within a team
/// </summary>
public sealed class Project
{
    // Strongly-typed ID
    public ProjectId Id { get; private set; } = null!;
    
    // Required properties
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required TeamId TeamId { get; set; }
    
    // Enum for type-safe status
    public ProjectStatus Status { get; private set; }
    
    // Timestamps
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation properties (for EF Core)
    public Team Team { get; set; } = null!;

    // Factory method - preferred way to create entities
    public static Project Create(string name, string description, TeamId teamId)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentException.ThrowIfNullOrEmpty(description);
        ArgumentNullException.ThrowIfNull(teamId);

        return new Project
        {
            Id = ProjectId.NewId(),
            Name = name,
            Description = description,
            TeamId = teamId,
            Status = ProjectStatus.Active,
            CreatedAt = DateTime.UtcNow
        };
    }

    // Business methods - encapsulate domain logic
    public void UpdateDetails(string name, string description)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentException.ThrowIfNullOrEmpty(description);

        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Archive()
    {
        Status = ProjectStatus.Archived;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        Status = ProjectStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum ProjectStatus
{
    Active = 1,
    Archived = 2,
    Completed = 3
}
```

## Step 2: Create Strongly-Typed ID

Every entity must have its own strongly-typed ID type.

**Location:** `Shared-Modules/AppBlueprint.SharedKernel/StronglyTypedIds/ProjectId.cs`

```csharp
using System.ComponentModel;
using System.Globalization;
using System.Text.Json.Serialization;

namespace AppBlueprint.SharedKernel;

/// <summary>
/// Strongly-typed identifier for Project entity
/// </summary>
[TypeConverter(typeof(ProjectIdTypeConverter))]
[JsonConverter(typeof(ProjectIdJsonConverter))]
public readonly record struct ProjectId
{
    public Ulid Value { get; }

    public ProjectId(Ulid value) => Value = value;

    public static ProjectId NewId() => new(Ulid.NewUlid());

    public static ProjectId Parse(string value) => new(Ulid.Parse(value));

    public static bool TryParse(string? value, out ProjectId result)
    {
        if (Ulid.TryParse(value, out var ulid))
        {
            result = new ProjectId(ulid);
            return true;
        }
        result = default;
        return false;
    }

    public override string ToString() => Value.ToString();

    // Type converter for model binding in controllers
    private class ProjectIdTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
            => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
            => value is string str ? Parse(str) : base.ConvertFrom(context, culture, value);

        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
            => destinationType == typeof(string) && value is ProjectId id ? id.ToString() : base.ConvertTo(context, culture, value, destinationType);
    }

    // JSON converter for serialization
    private class ProjectIdJsonConverter : JsonConverter<ProjectId>
    {
        public override ProjectId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => Parse(reader.GetString()!);

        public override void Write(Utf8JsonWriter writer, ProjectId value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString());
    }
}
```

## Step 3: Configure Entity in DbContext

Configure the entity mapping and relationships in Entity Framework Core.

**Location:** `Shared-Modules/AppBlueprint.Infrastructure/DatabaseContexts/ApplicationDbContext.cs`

```csharp
// Add DbSet property
public DbSet<Project> Projects => Set<Project>();

// Configure entity in OnModelCreating
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Project>(entity =>
    {
        // Primary key
        entity.HasKey(e => e.Id);
        
        // Strongly-typed ID conversion
        entity.Property(e => e.Id)
            .HasConversion(
                id => id.Value.ToString(),
                value => new ProjectId(Ulid.Parse(value)))
            .IsRequired();

        // String properties with constraints
        entity.Property(e => e.Name)
            .HasMaxLength(200)
            .IsRequired();

        entity.Property(e => e.Description)
            .HasMaxLength(2000);

        // Enum stored as integer
        entity.Property(e => e.Status)
            .HasConversion<int>()
            .IsRequired();

        // Timestamp properties
        entity.Property(e => e.CreatedAt)
            .IsRequired();

        entity.Property(e => e.UpdatedAt);

        // Relationships
        entity.HasOne(e => e.Team)
            .WithMany()
            .HasForeignKey(e => e.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for performance
        entity.HasIndex(e => e.TeamId);
        entity.HasIndex(e => new { e.TeamId, e.Status });
    });
}
```

## Step 4: Create and Apply Migration

```bash
cd Shared-Modules/AppBlueprint.Infrastructure

# Create migration
dotnet ef migrations add AddProjectEntity --context ApplicationDbContext

# Review the generated migration file
# Then apply it to the database
dotnet ef database update --context ApplicationDbContext
```

## Best Practices

### ✅ DO
- Use factory methods (`Create()`) for entity creation
- Keep business logic in domain entities
- Use strongly-typed IDs for all entities
- Use `required` keyword for mandatory properties
- Use private setters to enforce encapsulation
- Validate inputs in business methods
- Use UTC for all timestamps

### ❌ DON'T
- Don't use `string` or `Guid` directly for IDs
- Don't put data access logic in entities
- Don't expose public setters for business rules
- Don't create entities with invalid state
- Don't use nullable properties when not needed

## Common Patterns

### Value Objects

For complex types that don't have identity:

```csharp
public readonly record struct Money(decimal Amount, string Currency);
```

### Aggregate Relationships

```csharp
// One-to-Many
entity.HasMany(e => e.Tasks)
    .WithOne(t => t.Project)
    .HasForeignKey(t => t.ProjectId);

// Many-to-Many
entity.HasMany(e => e.Tags)
    .WithMany(t => t.Projects)
    .UsingEntity(j => j.ToTable("ProjectTags"));
```

## Next Steps

- [Adding Repositories](./02-adding-repositories.md)
- [Creating API Controllers](./03-creating-api-controllers.md)
- [Writing Tests](./testing-guide.md)

## Examples in Codebase

See existing implementations:
- `TodoEntity` - Simple entity with basic CRUD
- `TeamEntity` - Entity with relationships
- `OrganizationEntity` - Complex entity with multiple relationships
