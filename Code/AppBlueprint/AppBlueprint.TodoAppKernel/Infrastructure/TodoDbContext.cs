using AppBlueprint.Infrastructure.DatabaseContexts.B2B;
using AppBlueprint.TodoAppKernel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.TodoAppKernel.Infrastructure;

/// <summary>
/// TodoDbContext - inherits from B2BDbContext to include all baseline and B2B features
/// This follows the same pattern as the Dating app integration example
/// </summary>
public class TodoDbContext : B2BDbContext
{
    public TodoDbContext(
        DbContextOptions<B2BDbContext> options,
        IConfiguration configuration,
        ILogger<TodoDbContext> logger)
        : base(options, configuration, logger)
    {
    }

    // Todo-specific DbSets
    public DbSet<TodoEntity> Todos => Set<TodoEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        
        base.OnModelCreating(modelBuilder);

        // Configure Todo-specific entities
        modelBuilder.ConfigureTodoAppKernel();
    }
}
