# AppBlueprint Demo App - Hybrid Mode Configuration

## Overview

The AppBlueprint demo application uses **Hybrid Mode** to support dynamic B2C and B2B user flows within the same application. Users can sign up as either:

- **Individual consumers (B2C)** - Personal accounts with notifications, preferences
- **Organization members (B2B)** - Company accounts with Teams, Organizations, API Keys

The dashboard and available features dynamically adapt based on the user's account type.

## Why Hybrid Mode?

```
┌─────────────────────────────────────────────────────────┐
│           AppBlueprint Demo Application                 │
│                                                          │
│  User Signup Choice:                                    │
│  ┌───────────┐              ┌────────────────┐         │
│  │ B2C User  │              │ B2B User       │         │
│  │ Personal  │              │ Organization   │         │
│  └─────┬─────┘              └────────┬───────┘         │
│        │                             │                  │
│        v                             v                  │
│  ┌──────────────┐          ┌─────────────────┐        │
│  │  B2C Context │          │  B2B Context    │        │
│  │ - Baseline   │          │  - Baseline     │        │
│  │ - Personal   │          │  - Teams        │        │
│  │ - Family     │          │  - Orgs         │        │
│  └──────────────┘          │  - API Keys     │        │
│                            └─────────────────┘        │
│                                                          │
│  Both contexts registered via Hybrid Mode ✓             │
│  Single database, same connection string                │
└─────────────────────────────────────────────────────────┘
```

## Configuration

### Current Setup (Already Applied)

**Location 1: AppBlueprint.ApiService/Properties/launchSettings.json**
```json
{
  "environmentVariables": {
    "ASPNETCORE_ENVIRONMENT": "Development",
    "DatabaseContext__ContextType": "B2C",
    "DatabaseContext__EnableHybridMode": "true"
  }
}
```

**Location 2: AppBlueprint.Web/Properties/launchSettings.json**
```json
{
  "environmentVariables": {
    "ASPNETCORE_ENVIRONMENT": "Development",
    "DatabaseContext__ContextType": "B2C",
    "DatabaseContext__EnableHybridMode": "true"
  }
}
```

**Location 3: AppBlueprint.Web/Program.cs**
```csharp
// Migrated from legacy to new flexible configuration
builder.Services.AddAppBlueprintInfrastructure(builder.Configuration, builder.Environment);
```

### What Gets Registered

When the application starts, you'll see this in the logs:

```
[DbContextConfigurator] Database Connection Source: Environment Variable
[DbContextConfigurator] Context Type: B2C
[DbContextConfigurator] Baseline Only: False
[DbContextConfigurator] Hybrid Mode: True
[DbContextConfigurator] Hybrid Mode: All contexts registered
```

**All 4 DbContext types are now available:**
1. ✅ `BaselineDbContext` - Core entities (Users, Notifications, Files, etc.)
2. ✅ `B2CdbContext` - Baseline + consumer features
3. ✅ `ApplicationDbContext` - B2C + GDPR, soft deletes
4. ✅ `B2BDbContext` - Baseline + Organizations, Teams, API Keys

## How It Works in Code

### Services Can Inject Both Contexts

```csharp
// Example: TeamRepository (B2B entity)
public class TeamRepository : ITeamRepository
{
    private readonly B2BDbContext _context;  // ✅ Available via Hybrid Mode
    
    public TeamRepository(B2BDbContext context)
    {
        _context = context;
    }
}

// Example: UserRepository (B2C entity)
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;  // ✅ Available via Hybrid Mode
    
    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }
}

// Example: UnitOfWork (needs BOTH)
public class UnitOfWorkImplementation : IUnitOfWork
{
    private readonly ApplicationDbContext _applicationDbContext;  // ✅
    private readonly B2BDbContext _b2bDbContext;                   // ✅
    
    public UnitOfWorkImplementation(
        ApplicationDbContext context,
        B2BDbContext b2bDbContext)
    {
        _applicationDbContext = context;
        _b2bDbContext = b2bDbContext;
    }
}
```

### Custom B2B Module Integration

```csharp
// FeatureModuleDbContext inherits from B2BDbContext
public class FeatureModuleDbContext : B2BDbContext  // ✅ Works because B2BDbContext is registered
{
    public FeatureModuleDbContext(
        DbContextOptions<B2BDbContext> options,
        IConfiguration configuration,
        ILogger<FeatureModuleDbContext> logger)
        : base(options, configuration, logger)
    {
    }
    
    public DbSet<FeatureEntity> Features => Set<FeatureEntity>();
}
```

## Production Configuration

### Option 1: Environment Variables (Recommended)

```bash
# Railway, Azure, AWS, etc.
DATABASE_CONNECTION_STRING="Host=prod-db;Database=app;Username=user;Password=secret"
DatabaseContext__ContextType="B2C"
DatabaseContext__EnableHybridMode="true"
```

### Option 2: appsettings.Production.json

```json
{
  "DatabaseContext": {
    "ContextType": "B2C",
    "EnableHybridMode": true,
    "ConnectionStringName": "DefaultConnection"
  },
  "ConnectionStrings": {
    "DefaultConnection": "@Microsoft.KeyVault(SecretUri=https://vault.azure.net/secrets/DbConnectionString/)"
  }
}
```

### Option 3: Docker Compose

```yaml
services:
  web:
    environment:
      - DatabaseContext__ContextType=B2C
      - DatabaseContext__EnableHybridMode=true
      - DATABASE_CONNECTION_STRING=Host=postgres;Database=app;Username=postgres;Password=postgres
```

## Verification

### Check Logs on Startup

Look for these lines when the app starts:

```
✅ [DbContextConfigurator] Context Type: B2C
✅ [DbContextConfigurator] Hybrid Mode: True
✅ [DbContextConfigurator] Hybrid Mode: All contexts registered
```

### Test Dependency Injection

Create a test endpoint to verify all contexts are available:

```csharp
// AppBlueprint.ApiService/Program.cs
app.MapGet("/debug/contexts", (
    BaselineDbContext baseline,
    ApplicationDbContext b2c,
    B2BDbContext b2b) =>
{
    return Results.Ok(new
    {
        BaselineAvailable = baseline != null,
        B2CAvailable = b2c != null,
        B2BAvailable = b2b != null,
        Message = "All contexts registered successfully!"
    });
});
```

Expected response:
```json
{
  "baselineAvailable": true,
  "b2cAvailable": true,
  "b2bAvailable": true,
  "message": "All contexts registered successfully!"
}
```

## Benefits for Demo App

| Feature | Without Hybrid Mode | With Hybrid Mode |
|---------|-------------------|-----------------|
| B2C Users | ✅ Works | ✅ Works |
| B2B Users | ❌ Missing B2BDbContext | ✅ Works |
| Teams Feature | ❌ DI Error | ✅ Works |
| Dynamic Dashboard | ❌ Can't switch contexts | ✅ Works |
| TodoApp | ❌ Inherits unavailable B2BDbContext | ✅ Works |
| UnitOfWork | ❌ Needs both contexts | ✅ Works |


## Troubleshooting

### Error: "Unable to resolve service for type 'B2BDbContext'"

**Cause:** Hybrid Mode not enabled or not configured correctly

**Solution:** Check launchSettings.json has:
```json
"DatabaseContext__EnableHybridMode": "true"
```

### Error: "Multiple DbContext types registered"

**Cause:** Mixing old legacy registration with new flexible config

**Solution:** Remove `AddAppBlueprintInfrastructureLegacy()` calls, use only:
```csharp
builder.Services.AddAppBlueprintInfrastructure(builder.Configuration, builder.Environment);
```

### Logs show "Context Type: B2C" but not "Hybrid Mode: True"

**Cause:** Environment variable not being read

**Solution:** Ensure exact format (double underscore):
```bash
DatabaseContext__EnableHybridMode="true"  # ✅ Correct
DatabaseContext:EnableHybridMode="true"   # ❌ Wrong
```

### Error: "HTTP ERROR 403" or "Adgang til localhost blev nægtet"

**Symptoms:**
- 403 Forbidden error when accessing Blazor pages
- "You don't have permission to view this page" in browser
- Occurs after adding `@inject IHttpContextAccessor` to Blazor components

**Cause:** `IHttpContextAccessor.HttpContext` is **only available during initial HTTP request** in Blazor Server, not during SignalR interactive rendering. Attempting to access `HttpContext.GetTokenAsync()` in interactive components causes 403 errors.

**Solution:** Use localStorage to retrieve tokens instead of HttpContext:

```csharp
// ❌ WRONG - Causes 403 in Blazor Server interactive mode
@inject IHttpContextAccessor HttpContextAccessor

private async Task<HttpClient> GetAuthenticatedHttpClientAsync()
{
    var httpContext = HttpContextAccessor.HttpContext;
    string? token = await httpContext.GetTokenAsync("Logto", "access_token");
    // HttpContext is null during interactive rendering = 403 error
}

// ✅ CORRECT - Retrieve token from localStorage
private string? accessToken;

protected override async Task OnInitializedAsync()
{
    // Get token from localStorage where Logto stores it
    accessToken = await JSRuntime.InvokeAsync<string?>("localStorage.getItem", "logto_access_token");
    // ...
}

private HttpClient GetAuthenticatedHttpClient()
{
    var httpClient = HttpClientFactory.CreateClient();
    if (!string.IsNullOrEmpty(accessToken))
    {
        httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", accessToken);
    }
    return httpClient;
}
```

**Key Points:**
- HttpContext in Blazor Server: Available during initial HTTP request only
- SignalR reconnections: No HttpContext available (causes 403)
- Logto tokens: Stored in browser localStorage (accessible anytime)
- Remove `@inject IHttpContextAccessor` from interactive Blazor components
- Retrieve tokens once during `OnInitializedAsync()` and store in component field

**Related Files:**
- `AppBlueprint.Web/Components/Pages/Auth/SignupComplete.razor` - Example implementation

## Related Documentation

- [📖 Complete Database Context Flexibility Guide](./DATABASE_CONTEXT_FLEXIBILITY_GUIDE.md) - Comprehensive guide with all scenarios
- [📋 Configuration Examples](./DatabaseContexts/Examples/README.md) - B2C, B2B, Hybrid, Baseline examples
- [🏗️ Clean Architecture Dependencies](../../../.github/.ai-rules/baseline/clean-architecture-dependencies.md) - Dependency injection patterns
- [🔐 Multi-Tenancy Guide](./MULTI_TENANCY_GUIDE.md) - Tenant isolation with RLS

## Summary

The AppBlueprint demo app now uses **Hybrid Mode** to:

✅ Support both B2C and B2B user signup flows  
✅ Enable dynamic dashboards that adapt to user type  
✅ Allow all repositories and services to access needed contexts  
✅ Maintain single codebase and database  
✅ Demonstrate both consumer and enterprise features  
✅ Work with feature modules that extend B2BDbContext  

**Configuration is complete and active in both Web and ApiService projects.**
