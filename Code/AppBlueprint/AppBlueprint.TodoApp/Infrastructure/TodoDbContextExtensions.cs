using AppBlueprint.TodoApp.Domain;
using AppBlueprint.TodoApp.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.TodoApp.Infrastructure;

/// <summary>
/// Extension methods for configuring TodoApp entities in DbContext.
/// Provides a clean way to integrate TodoApp into existing B2BDbContext.
/// </summary>
public static class TodoDbContextExtensions
{
    /// <summary>
    /// Configures the TodoApp entities in the model builder.
    /// </summary>
    /// <param name="modelBuilder">The model builder instance</param>
    public static void ConfigureTodoApp(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        
        modelBuilder.ApplyConfiguration(new TodoEntityConfiguration());
    }

    /// <summary>
    /// Adds the TodoApp DbSets to the context. This should be called from a partial class.
    /// </summary>
    /// <param name="context">The DbContext instance</param>
    /// <returns>DbSet for TodoEntity</returns>
    public static DbSet<TodoEntity> GetTodos(this DbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        
        return context.Set<TodoEntity>();
    }
}
