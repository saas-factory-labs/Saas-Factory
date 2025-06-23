using AppBlueprint.TodoApp.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.TodoApp.Infrastructure;

/// <summary>
/// Entity configuration for TodoEntity defining table structure, relationships, and constraints.
/// Supports multi-tenant todo management with user assignment and priority tracking.
/// </summary>
public sealed class TodoEntityConfiguration : IEntityTypeConfiguration<TodoEntity>
{
    public void Configure(EntityTypeBuilder<TodoEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Table mapping with standardized naming
        builder.ToTable("Todos");

        // Primary key
        builder.HasKey(t => t.Id);

        // Configure ULID ID with proper length for prefixed ULID (prefix + underscore + 26 char ULID)
        builder.Property(t => t.Id)
            .HasMaxLength(40)
            .IsRequired();

        // BaseEntity properties
        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.LastUpdatedAt);

        builder.Property(t => t.IsSoftDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Properties with validation and constraints
        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .HasMaxLength(1000);

        builder.Property(t => t.IsCompleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(t => t.Priority)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(TodoPriority.Medium);

        builder.Property(t => t.DueDate);

        builder.Property(t => t.CompletedAt);

        // ITenantScoped property
        builder.Property(t => t.TenantId)
            .IsRequired()
            .HasMaxLength(40);

        // User relationship properties
        builder.Property(t => t.CreatedById)
            .IsRequired()
            .HasMaxLength(40);

        builder.Property(t => t.AssignedToId)
            .HasMaxLength(40);

        // Foreign key indexes for performance (no navigation properties per Clean Architecture)
        builder.HasIndex(t => t.TenantId)
            .HasDatabaseName("IX_Todos_TenantId");

        builder.HasIndex(t => t.CreatedById)
            .HasDatabaseName("IX_Todos_CreatedById");

        builder.HasIndex(t => t.AssignedToId)
            .HasDatabaseName("IX_Todos_AssignedToId");

        // Performance indexes with standardized naming
        builder.HasIndex(t => t.TenantId)
            .HasDatabaseName("IX_Todos_TenantId");

        builder.HasIndex(t => t.CreatedById)
            .HasDatabaseName("IX_Todos_CreatedById");

        builder.HasIndex(t => t.AssignedToId)
            .HasDatabaseName("IX_Todos_AssignedToId");

        builder.HasIndex(t => t.IsCompleted)
            .HasDatabaseName("IX_Todos_IsCompleted");

        builder.HasIndex(t => t.Priority)
            .HasDatabaseName("IX_Todos_Priority");

        builder.HasIndex(t => t.DueDate)
            .HasDatabaseName("IX_Todos_DueDate");

        builder.HasIndex(t => t.CreatedAt)
            .HasDatabaseName("IX_Todos_CreatedAt");

        // Add index for soft delete filtering
        builder.HasIndex(t => t.IsSoftDeleted)
            .HasDatabaseName("IX_Todos_IsSoftDeleted");

        // Composite indexes for common queries
        builder.HasIndex(t => new { t.TenantId, t.IsCompleted })
            .HasDatabaseName("IX_Todos_TenantId_IsCompleted");

        builder.HasIndex(t => new { t.TenantId, t.AssignedToId, t.IsCompleted })
            .HasDatabaseName("IX_Todos_TenantId_AssignedToId_IsCompleted");

        builder.HasIndex(t => new { t.TenantId, t.Priority, t.DueDate })
            .HasDatabaseName("IX_Todos_TenantId_Priority_DueDate");
    }
}
