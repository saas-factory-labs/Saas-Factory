# AppBlueprint Service Registration

This guide shows how to integrate AppBlueprint NuGet packages into your existing or new SaaS application.

## Quick Start

### Option 1: Add All Services (Recommended for New Projects)

```csharp
using AppBlueprint.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add all AppBlueprint services in one call
builder.Services.AddAppBlueprint(builder.Configuration);

var app = builder.Build();

// Configure middleware pipeline
app.ConfigureAppBlueprintMiddleware();

app.Run();
```

### Option 2: Add Services Individually (For Existing Projects)

```csharp
using AppBlueprint.Infrastructure.Extensions;
using AppBlueprint.Application.Extensions;
using AppBlueprint.Presentation.ApiModule.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services layer by layer
builder.Services.AddAppBlueprintInfrastructure(builder.Configuration);
builder.Services.AddAppBlueprintApplication();
builder.Services.AddAppBlueprintPresentation();

var app = builder.Build();

// Configure middleware
app.ConfigureAppBlueprintMiddleware();

app.Run();
```

## Configuration

### Environment Variables (Recommended for Production)

Set these environment variables for cloud deployments:

```bash
# Required
DATABASE_CONNECTION_STRING="Host=your-db-host;Port=5432;Database=your-db;Username=user;Password=pass;SSL Mode=Require"

# Optional (if using Redis)
REDIS_CONNECTION_STRING="your-redis-connection-string"

# Authentication (Logto)
LOGTO_ENDPOINT="https://your-tenant.logto.app"
LOGTO_APP_ID="your-app-id"
LOGTO_APP_SECRET="your-app-secret"

# Stripe
STRIPE_SECRET_KEY="sk_live_..."
STRIPE_WEBHOOK_SECRET="whsec_..."

# AWS S3
AWS_ACCESS_KEY_ID="your-access-key"
AWS_SECRET_ACCESS_KEY="your-secret-key"
AWS_REGION="us-east-1"
AWS_S3_BUCKET_NAME="your-bucket"

# Email (Resend)
RESEND_API_KEY="re_..."
```

### Configuration Fallback

If environment variables are not set, AppBlueprint falls back to `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "appblueprintdb": "Host=localhost;Port=5432;Database=appblueprint;Username=postgres;Password=postgres"
  }
}
```

## Integration Examples

### Example 1: Dating App with Existing Database

```csharp
// Program.cs
using AppBlueprint.Infrastructure.Extensions;
using AppBlueprint.Application.Extensions;
using DatingApp.Infrastructure.DatabaseContexts;

var builder = WebApplication.CreateBuilder(args);

// 1. Add AppBlueprint Infrastructure and Application
builder.Services.AddAppBlueprintInfrastructure(builder.Configuration);
builder.Services.AddAppBlueprintApplication();

// 2. Add your own DbContext (inherits ApplicationDbContext)
builder.Services.AddDbContext<DatingDbContext>((serviceProvider, options) =>
{
    var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING") ??
                          builder.Configuration.GetConnectionString("appblueprintdb");
    
    options.UseNpgsql(connectionString);
});

// 3. Add your own repositories
builder.Services.AddScoped<IMatchRepository, MatchRepository>();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();

// 4. Add Presentation layer (if you want AppBlueprint controllers)
builder.Services.AddAppBlueprintPresentation();

// 5. Add your own controllers
builder.Services.AddControllers();

var app = builder.Build();

// Configure middleware
app.ConfigureAppBlueprintMiddleware();

app.Run();
```

```csharp
// DatingDbContext.cs
using AppBlueprint.Infrastructure.DatabaseContexts;
using DatingApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Infrastructure.DatabaseContexts;

public class DatingDbContext : ApplicationDbContext
{
    public DatingDbContext(DbContextOptions<DatingDbContext> options)
        : base(options)
    {
    }

    // Dating app specific DbSets
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<Message> Messages => Set<Message>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure dating app entities
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatingDbContext).Assembly);
    }
}
```

### Example 2: E-commerce Platform

```csharp
// Program.cs
using AppBlueprint.Infrastructure.Extensions;
using AppBlueprint.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add AppBlueprint services
builder.Services.AddAppBlueprintInfrastructure(builder.Configuration);
builder.Services.AddAppBlueprintApplication();

// Add your e-commerce services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICartService, CartService>();

// Add your repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// Add your DbContext
builder.Services.AddDbContext<EcommerceDbContext>((serviceProvider, options) =>
{
    var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING") ??
                          builder.Configuration.GetConnectionString("appblueprintdb");
    
    options.UseNpgsql(connectionString);
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

### Example 3: Multi-Tenant SaaS Application

```csharp
// Program.cs
using AppBlueprint.Infrastructure.Extensions;
using AppBlueprint.Application.Extensions;
using MultiTenantApp.Infrastructure.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add AppBlueprint services
builder.Services.AddAppBlueprint(builder.Configuration);

// Add multi-tenancy
builder.Services.AddScoped<ITenantProvider, TenantProvider>();
builder.Services.AddScoped<ITenantResolver, TenantResolver>();

// Add your DbContext with tenant support
builder.Services.AddDbContext<TenantDbContext>((serviceProvider, options) =>
{
    var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING") ??
                          builder.Configuration.GetConnectionString("appblueprintdb");
    
    options.UseNpgsql(connectionString);
});

var app = builder.Build();

// Add tenant resolution middleware
app.UseMiddleware<TenantResolutionMiddleware>();

app.ConfigureAppBlueprintMiddleware();

app.Run();
```

## What Gets Registered

### Infrastructure Layer (`AddAppBlueprintInfrastructure`)
- ✅ `ApplicationDbContext` (PostgreSQL with EF Core)
- ✅ `B2BDbContext` (for B2B features)
- ✅ Repository implementations (`ITodoRepository`, `ITeamRepository`, `IDataExportRepository`)
- ✅ Unit of Work pattern (`IUnitOfWork`)
- ✅ Health checks (Database, Redis)
- ✅ Authentication providers (Logto)

### Application Layer (`AddAppBlueprintApplication`)
- ✅ FluentValidation validators
- ✅ Application services (`IDataExportService`)
- 🚧 Command handlers (TODO)
- 🚧 Query handlers (TODO)

### Presentation Layer (`AddAppBlueprintPresentation`)
- ✅ Controllers
- ✅ API versioning (URL segments, query strings, headers)
- ✅ CORS policy
- ✅ Problem details
- ✅ Antiforgery tokens
- ✅ HttpContext accessor

### Middleware Pipeline (`ConfigureAppBlueprintMiddleware`)
- ✅ HTTPS redirection
- ✅ Routing
- ✅ CORS
- ✅ Authentication
- ✅ Authorization
- ✅ Controller mapping

## Database Context Strategies

### Strategy 1: Inherit ApplicationDbContext (Your Approach)

```csharp
public class DatingDbContext : ApplicationDbContext
{
    public DatingDbContext(DbContextOptions<DatingDbContext> options)
        : base(options)
    {
    }

    public DbSet<Match> Matches => Set<Match>();
    public DbSet<Profile> Profiles => Set<Profile>();
}
```

**Pros:**
- Access to AppBlueprint entities (Todos, Teams, etc.)
- Single database connection
- Shared transactions

**Cons:**
- Tight coupling to AppBlueprint schema

### Strategy 2: Separate DbContext (Loose Coupling)

```csharp
public class DatingDbContext : DbContext
{
    public DatingDbContext(DbContextOptions<DatingDbContext> options)
        : base(options)
    {
    }

    public DbSet<Match> Matches => Set<Match>();
    public DbSet<Profile> Profiles => Set<Profile>();
}
```

**Pros:**
- Complete isolation from AppBlueprint
- Can point to different database if needed
- Easier to remove AppBlueprint later

**Cons:**
- Cannot access AppBlueprint entities directly
- Need distributed transactions for cross-context operations

## Database Migration Strategy

AppBlueprint uses **separate migration histories within the same database schema**. This allows you to:
- ✅ Use AppBlueprint's pre-built tables (Todos, Teams, DataExports, etc.)
- ✅ Add your own app-specific tables (Profiles, Matches, Messages, etc.)
- ✅ Keep migrations organized by context
- ✅ Avoid schema conflicts

### How It Works

1. **ApplicationDbContext** (from AppBlueprint.Infrastructure NuGet package)
   - Manages AppBlueprint tables: `TodoItems`, `Teams`, `DataExports`, `Roles`, `Permissions`, etc.
   - Migrations are pre-applied in the NuGet package
   - Migration history tracked in `__EFMigrationsHistory` with `ContextType = 'ApplicationDbContext'`

2. **DatingDbContext** (your app-specific context)
   - Inherits from `ApplicationDbContext` → access to AppBlueprint tables
   - Adds your tables: `Profiles`, `Matches`, `Messages`, etc.
   - Your migrations tracked in `__EFMigrationsHistory` with `ContextType = 'DatingDbContext'`
   - **Same database, same schema, separate migration tracking**

### Initial Setup (New Database)

```bash
# 1. Set connection string
$env:DATABASE_CONNECTION_STRING = "Host=localhost;Port=5432;Database=datingapp;Username=postgres;Password=yourpassword"

# 2. Apply AppBlueprint migrations (creates shared tables)
dotnet ef database update --context ApplicationDbContext --project ./AppBlueprint.Infrastructure

# 3. Add your initial migration (for dating app tables)
dotnet ef migrations add InitialDatingSchema --context DatingDbContext --project ./DatingApp.Infrastructure --output-dir Migrations

# 4. Apply your migrations (creates your tables)
dotnet ef database update --context DatingDbContext --project ./DatingApp.Infrastructure
```

### Result: Single Database with Two Migration Histories

```sql
-- Check migration history
SELECT "MigrationId", "ProductVersion", "ContextType"
FROM "__EFMigrationsHistory"
ORDER BY "MigrationId";

-- Output:
-- 20250716183440_InitialULIDSchema     | 10.0.0 | ApplicationDbContext
-- 20251114033416_AddRolePermission     | 10.0.0 | ApplicationDbContext
-- 20251210120000_InitialDatingSchema   | 10.0.0 | DatingDbContext
-- 20251210150000_AddMatchesTable       | 10.0.0 | DatingDbContext
```

### Adding New Tables to Your App

```bash
# 1. Add entities to DatingDbContext
# DatingApp.Infrastructure/DatabaseContexts/DatingDbContext.cs

public class DatingDbContext : ApplicationDbContext
{
    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<Match> Matches => Set<Match>();     // ← New table
    public DbSet<Message> Messages => Set<Message>(); // ← New table
}

# 2. Create migration
dotnet ef migrations add AddMatchesAndMessages --context DatingDbContext --project ./DatingApp.Infrastructure

# 3. Apply migration
dotnet ef database update --context DatingDbContext --project ./DatingApp.Infrastructure
```

### Migrating Existing Database (Already Has Data)

If you have an existing dating app database and want to integrate AppBlueprint:

```bash
# 1. Backup your database
pg_dump -U postgres -d datingapp > backup_before_appblueprint.sql

# 2. Add AppBlueprint migrations WITHOUT applying them yet
#    (EF Core will see they need to run)
dotnet ef database update --context ApplicationDbContext --project ./AppBlueprint.Infrastructure

# 3. Review what tables will be created
dotnet ef migrations script --context ApplicationDbContext --project ./AppBlueprint.Infrastructure

# 4. If safe, apply AppBlueprint migrations
#    (Creates TodoItems, Teams, DataExports, Roles, Permissions, etc.)
dotnet ef database update --context ApplicationDbContext --project ./AppBlueprint.Infrastructure

# 5. Create initial migration for YOUR existing tables
#    (This will be empty if tables already exist - that's OK!)
dotnet ef migrations add InitialDatingSchema --context DatingDbContext --project ./DatingApp.Infrastructure

# 6. Apply your migration (EF Core will record it as applied)
dotnet ef database update --context DatingDbContext --project ./DatingApp.Infrastructure
```

### Production Deployment

```bash
# Option 1: Run migrations on startup (recommended for development)
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var appBlueprintDb = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var datingDb = scope.ServiceProvider.GetRequiredService<DatingDbContext>();
    
    await appBlueprintDb.Database.MigrateAsync(); // Apply AppBlueprint migrations
    await datingDb.Database.MigrateAsync();       // Apply your migrations
}

app.Run();

# Option 2: Generate SQL scripts for manual deployment (recommended for production)
dotnet ef migrations script --context ApplicationDbContext --project ./AppBlueprint.Infrastructure --output appblueprint-migrations.sql
dotnet ef migrations script --context DatingDbContext --project ./DatingApp.Infrastructure --output datingapp-migrations.sql

# Review scripts, then apply to production database
```

### Migration Commands Reference

```bash
# === AppBlueprint (Infrastructure Package) ===
# List AppBlueprint migrations
dotnet ef migrations list --context ApplicationDbContext --project ./AppBlueprint.Infrastructure

# View pending AppBlueprint migrations
dotnet ef migrations script --context ApplicationDbContext --project ./AppBlueprint.Infrastructure

# Apply AppBlueprint migrations
dotnet ef database update --context ApplicationDbContext --project ./AppBlueprint.Infrastructure

# === Your App (DatingApp) ===
# Add new migration for your tables
dotnet ef migrations add AddNewFeature --context DatingDbContext --project ./DatingApp.Infrastructure

# List your migrations
dotnet ef migrations list --context DatingDbContext --project ./DatingApp.Infrastructure

# Apply your migrations
dotnet ef database update --context DatingDbContext --project ./DatingApp.Infrastructure

# Rollback your migration to specific version
dotnet ef database update PreviousMigrationName --context DatingDbContext --project ./DatingApp.Infrastructure

# Remove last migration (if not applied yet)
dotnet ef migrations remove --context DatingDbContext --project ./DatingApp.Infrastructure
```

### Schema Visualization

```
┌─────────────────────────────────────────────────────────────┐
│                     PostgreSQL Database                      │
│                         datingapp                            │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  ┌────────────────────────────────────────────────────┐     │
│  │  AppBlueprint Tables (ApplicationDbContext)        │     │
│  ├────────────────────────────────────────────────────┤     │
│  │  • TodoItems                                        │     │
│  │  • Teams                                            │     │
│  │  • DataExports                                      │     │
│  │  • Roles                                            │     │
│  │  • Permissions                                      │     │
│  │  • UserRoles                                        │     │
│  └────────────────────────────────────────────────────┘     │
│                                                              │
│  ┌────────────────────────────────────────────────────┐     │
│  │  Dating App Tables (DatingDbContext)               │     │
│  ├────────────────────────────────────────────────────┤     │
│  │  • Profiles                                         │     │
│  │  • Matches                                          │     │
│  │  • Messages                                         │     │
│  │  • Likes                                            │     │
│  │  • UserPreferences                                  │     │
│  └────────────────────────────────────────────────────┘     │
│                                                              │
│  ┌────────────────────────────────────────────────────┐     │
│  │  __EFMigrationsHistory                             │     │
│  ├────────────────────────────────────────────────────┤     │
│  │  MigrationId                 | ContextType         │     │
│  │  ──────────────────────────────────────────────────│     │
│  │  InitialULIDSchema           | ApplicationDbContext│     │
│  │  AddRolePermission           | ApplicationDbContext│     │
│  │  InitialDatingSchema         | DatingDbContext     │     │
│  │  AddMatchesTable             | DatingDbContext     │     │
│  └────────────────────────────────────────────────────┘     │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

### Troubleshooting

**Problem:** "Table 'TodoItems' already exists"
```bash
# Solution: EF Core thinks migration wasn't applied
# Manually add migration record:
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion") 
VALUES ('20250716183440_InitialULIDSchema', '10.0.0');
```

**Problem:** "The context type 'DatingDbContext' is not registered"
```bash
# Solution: Make sure you registered the DbContext
builder.Services.AddDbContext<DatingDbContext>(options => 
    options.UseNpgsql(connectionString));
```

**Problem:** "Cannot access AppBlueprint tables from DatingDbContext"
```bash
# Solution: Make sure DatingDbContext inherits ApplicationDbContext
public class DatingDbContext : ApplicationDbContext
{
    // Now you can access TodoItems, Teams, etc.
}
```

## Troubleshooting

### Issue: Missing DATABASE_CONNECTION_STRING

**Error:**
```
ArgumentException: Value cannot be null or empty. (Parameter 'connectionString')
```

**Solution:**
Set the environment variable or add to `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "appblueprintdb": "Host=localhost;Port=5432;Database=mydb;..."
  }
}
```

### Issue: Multiple DbContexts, Same Connection String

**Problem:** Both `ApplicationDbContext` and `DatingDbContext` connect to the same database.

**Solution:** This is intentional! Both contexts point to the same database but manage different entity sets.

### Issue: Cannot Access AppBlueprint Entities

**Error:**
```csharp
var todos = datingDbContext.TodoItems; // Error: TodoItems not found
```

**Solution:** If your DbContext inherits `ApplicationDbContext`, use the base properties:
```csharp
var todos = datingDbContext.Set<TodoItem>();
```

Or make your DbContext inherit `ApplicationDbContext`:
```csharp
public class DatingDbContext : ApplicationDbContext { ... }
```

## Next Steps

1. ✅ Install NuGet packages
2. ✅ Set environment variables
3. ✅ Add service registrations to `Program.cs`
4. ✅ Create your DbContext (inherit or separate)
5. ✅ Run migrations
6. ✅ Test integration

## Support

For issues, questions, or contributions:
- GitHub: https://github.com/saas-factory-labs/Saas-Factory
- License: MIT
