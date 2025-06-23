using AppBlueprint.TodoAppKernel.Domain;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.TodoAppKernel.Infrastructure;

/// <summary>
/// Extension methods for configuring TodoAppKernel entities in Entity Framework DbContext.
/// </summary>
public static class TodoAppKernelDbContextExtensions
{
    /// <summary>
    /// Configures the TodoAppKernel entities in the provided ModelBuilder.
    /// </summary>
    /// <param name="modelBuilder">The ModelBuilder to configure.</param>
    public static void ConfigureTodoAppKernel(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        // Apply TodoEntity configuration
        modelBuilder.ApplyConfiguration(new TodoEntityConfiguration());
    }

    /// <summary>
    /// Adds TodoAppKernel DbSets to the provided DbContext.
    /// Call this method from your DbContext's OnModelCreating method.
    /// </summary>
    /// <param name="modelBuilder">The ModelBuilder to configure.</param>
    /// <param name="dbContext">The DbContext to add DbSets to.</param>
    public static void AddTodoAppKernelDbSets(this ModelBuilder modelBuilder, DbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        ArgumentNullException.ThrowIfNull(dbContext);

        // DbSets are typically added as properties on the DbContext itself
        // This method is here for consistency and future extension
        modelBuilder.ConfigureTodoAppKernel();
    }
}
