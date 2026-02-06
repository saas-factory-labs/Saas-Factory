using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Modules.Chat;

public partial class ChatModuleDbContext : ApplicationDbContext
{
    public ChatModuleDbContext(
        DbContextOptions<Partials.ChatModuleDbContext> options,
        IConfiguration configuration,
        ILogger<Partials.ChatModuleDbContext> logger,
        ITenantContextAccessor? tenantContextAccessor = null) :
        base((DbContextOptions)(DbContextOptions<Partials.ChatModuleDbContext>)options, configuration, logger, tenantContextAccessor)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        OnModelCreating_Chat(modelBuilder);
    }

    partial void OnModelCreating_Chat(ModelBuilder modelBuilder);
}
