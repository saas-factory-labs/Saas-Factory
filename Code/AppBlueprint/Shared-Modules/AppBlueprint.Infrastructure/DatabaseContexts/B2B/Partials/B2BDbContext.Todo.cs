using AppBlueprint.TodoAppKernel.Domain;
using AppBlueprint.TodoAppKernel.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2B;

public partial class B2BDbContext
{
    public DbSet<TodoEntity> Todos { get; set; }

    partial void OnModelCreating_Todo(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureTodoAppKernel();
    }
}
