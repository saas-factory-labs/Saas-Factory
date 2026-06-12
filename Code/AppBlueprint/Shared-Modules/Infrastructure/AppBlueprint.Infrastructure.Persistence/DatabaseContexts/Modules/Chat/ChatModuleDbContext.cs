using AppBlueprint.Infrastructure.Persistence.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.Persistence.DatabaseContexts.Modules.Chat;

public partial class ChatModuleDbContext : ApplicationDbContext
{
    public ChatModuleDbContext(
        DbContextOptions<ChatModuleDbContext> options,
        IConfiguration configuration,
        ILogger<ChatModuleDbContext> logger,
        ITenantContextAccessor? tenantContextAccessor = null) :
        base((DbContextOptions)options, configuration, logger, tenantContextAccessor)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        OnModelCreating_Chat(modelBuilder);
    }

    partial void OnModelCreating_Chat(ModelBuilder modelBuilder);
}
