using AppBlueprint.Infrastructure.DatabaseContexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Modules.Chat;

public partial class ChatModuleDbContext : ApplicationDbContext
{
    public ChatModuleDbContext(
        DbContextOptions<ChatModuleDbContext> options,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor,
        ILogger<ChatModuleDbContext> logger) :
        base((DbContextOptions)(DbContextOptions<ChatModuleDbContext>)options, configuration, httpContextAccessor, logger)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        OnModelCreating_Chat(modelBuilder);
    }

    partial void OnModelCreating_Chat(ModelBuilder modelBuilder);
}
