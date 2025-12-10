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

## Migration Guide

### Migrating Existing Database Schema

1. **Backup your database:**
   ```bash
   pg_dump -U postgres -d your-database > backup.sql
   ```

2. **Run AppBlueprint migrations:**
   ```bash
   dotnet ef migrations add InitialAppBlueprint --context ApplicationDbContext
   dotnet ef database update --context ApplicationDbContext
   ```

3. **Run your migrations:**
   ```bash
   dotnet ef migrations add InitialDatingApp --context DatingDbContext
   dotnet ef database update --context DatingDbContext
   ```

4. **Verify both contexts work:**
   ```csharp
   using var scope = app.Services.CreateScope();
   var appBlueprintDb = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
   var datingDb = scope.ServiceProvider.GetRequiredService<DatingDbContext>();
   
   // Test AppBlueprint
   var todos = await appBlueprintDb.TodoItems.ToListAsync();
   
   // Test Dating App
   var profiles = await datingDb.Profiles.ToListAsync();
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
