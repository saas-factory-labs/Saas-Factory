# Database Context Flexibility Guide

## Overview

AppBlueprint supports flexible DbContext configuration, allowing you to choose the appropriate database context based on your application type:

- **Baseline**: Core entities only (authentication, notifications, integrations, file management)
- **B2C**: Consumer-focused applications (Baseline + family relationships, personal preferences)
- **B2B**: Organization-focused applications (Baseline + organizations, teams, API keys)
- **Hybrid**: Applications needing both B2C and B2B features (marketplaces, platforms, **demo apps**)

## Quick Reference

| Configuration | Use Case | Entities | When to Use |
|--------------|----------|----------|-------------|
| **B2C** | Consumer apps | Baseline + consumer features | Individual user apps (fitness, finance) |
| **B2B** | Enterprise apps | Baseline + organizational features | Team/org apps (CRM, project mgmt) |
| **Hybrid** | Platform apps | **ALL** (Baseline + B2C + B2B) | Marketplaces, demo apps with dynamic UIs |
| **Baseline** | Microservices | Core only | Auth services, minimal apps |


### Environment Variables (Quick Setup)

```bash
# For Hybrid Mode (Demo Apps / Marketplaces)
DatabaseContext__ContextType="B2C"          # or "B2B" - your primary context
DatabaseContext__EnableHybridMode="true"    # Registers ALL contexts

# For Single Mode
DatabaseContext__ContextType="B2C"          # Only B2C context
DatabaseContext__EnableHybridMode="false"   # (default)
```

## Architecture

```
BaselineDbContext (Core entities - ALWAYS included)
├── Users, Roles, Permissions
├── Notifications, Integrations
├── Files, Webhooks, Languages
├── Audit Logs, Data Exports
└── Payment/Subscription tracking

B2CdbContext : BaselineDbContext
├── All Baseline entities
└── Family relationships
└── Personal preferences (future)

B2BDbContext : BaselineDbContext
├── All Baseline entities
├── Organizations
├── Teams
└── API Keys

ApplicationDbContext : B2CdbContext
├── All B2C entities (includes Baseline)
├── GDPR data classification
├── Soft delete filters
└── Automatic timestamp management
```

## Configuration

### 1. Basic Configuration (appsettings.json)

```json
{
  "DatabaseContext": {
    "ContextType": "B2C",
    "EnableHybridMode": false,
    "BaselineOnly": false,
    "ConnectionStringName": "DefaultConnection",
    "CommandTimeout": 60,
    "MaxRetryCount": 5,
    "MaxRetryDelaySeconds": 10
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=myapp;Username=postgres;Password=postgres"
  }
}
```

### 2. Environment Variables (Recommended for Production)

```bash
# Connection string (overrides appsettings.json)
DATABASE_CONNECTION_STRING="Host=prod-db;Database=myapp;Username=app;Password=secret"

# Optional: Configure via environment
DatabaseContext__ContextType="B2B"
DatabaseContext__EnableHybridMode="false"
DatabaseContext__CommandTimeout="90"
```

### 3. Azure Key Vault Configuration

```json
{
  "DatabaseContext": {
    "ContextType": "B2B",
    "ConnectionStringName": "PostgreSQL"
  },
  "ConnectionStrings": {
    "PostgreSQL": "@Microsoft.KeyVault(SecretUri=https://myvault.vault.azure.net/secrets/DbConnectionString/)"
  }
}
```

## Configuration Options

### `ContextType` (enum)

Determines which DbContext to use:

| Value      | Use Case                              | Entities Included                |
|------------|---------------------------------------|----------------------------------|
| `Baseline` | Minimal apps, custom contexts         | Core entities only               |
| `B2C`      | Consumer SaaS (default)               | Baseline + consumer features     |
| `B2B`      | Organization SaaS                     | Baseline + organizational features|


**Example:**
```json
{
  "DatabaseContext": {
    "ContextType": "B2B"
  }
}
```

### `EnableHybridMode` (bool)

When `true`, registers **all** DbContext types (Baseline, B2C, B2B). Use for applications that need both consumer and organizational features.

**Example - Marketplace App:**
```json
{
  "DatabaseContext": {
    "ContextType": "B2B",
    "EnableHybridMode": true
  }
}
```

**Usage in code:**
```csharp
// Inject the context you need
public class UserService
{
    private readonly B2BDbContext _b2bContext;
    private readonly ApplicationDbContext _b2cContext;
    
    public UserService(B2BDbContext b2bContext, ApplicationDbContext b2cContext)
    {
        _b2bContext = b2bContext;
        _b2cContext = b2cContext;
    }
    
    public async Task<List<Organization>> GetOrganizationsAsync()
    {
        // Use B2B context for organizations
        return await _b2bContext.Organizations.ToListAsync();
    }
    
    public async Task<List<NotificationEntity>> GetNotificationsAsync()
    {
        // Use B2C context for common entities
        return await _b2cContext.Notifications.ToListAsync();
    }
}
```

### `BaselineOnly` (bool)

When `true`, only registers `BaselineDbContext`. Use for:
- Microservices needing only core entities
- Custom contexts that extend Baseline differently
- Minimal footprint applications

**Cannot be combined with `EnableHybridMode`.**

**Example:**
```json
{
  "DatabaseContext": {
    "BaselineOnly": true
  }
}
```

### `ConnectionStringName` (string)

Name of the connection string in `ConnectionStrings` section. Default: `"DefaultConnection"`.

**Fallback order:**
1. `DATABASE_CONNECTION_STRING` environment variable
2. `ConnectionStrings:{ConnectionStringName}`
3. `ConnectionStrings:appblueprintdb`
4. `ConnectionStrings:postgres-server`
5. `ConnectionStrings:DefaultConnection`

### Performance Settings

```json
{
  "DatabaseContext": {
    "CommandTimeout": 60,
    "MaxRetryCount": 5,
    "MaxRetryDelaySeconds": 10
  }
}
```

- **CommandTimeout**: Command timeout in seconds (default: 60)
- **MaxRetryCount**: Retry attempts on transient failures (default: 5)
- **MaxRetryDelaySeconds**: Max delay between retries (default: 10)

## Usage Examples

### Example 1: B2C Consumer SaaS (Default)

**Scenario:** Personal finance app for individual users

**appsettings.json:**
```json
{
  "DatabaseContext": {
    "ContextType": "B2C"
  }
}
```

**Program.cs:**
```csharp
builder.Services.AddAppBlueprintInfrastructure(
    builder.Configuration,
    builder.Environment);
```

**Service:**
```csharp
public class FinanceService
{
    private readonly ApplicationDbContext _context;
    
    public FinanceService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<NotificationEntity>> GetUserNotificationsAsync(string userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId)
            .ToListAsync();
    }
}
```

### Example 2: B2B Organization SaaS

**Scenario:** Project management tool for companies

**appsettings.json:**
```json
{
  "DatabaseContext": {
    "ContextType": "B2B"
  }
}
```

**Service:**
```csharp
public class OrganizationService
{
    private readonly B2BDbContext _context;
    
    public OrganizationService(B2BDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<OrganizationEntity>> GetOrganizationsAsync()
    {
        return await _context.Organizations
            .Include(o => o.Teams)
            .ToListAsync();
    }
    
    public async Task<List<NotificationEntity>> GetOrgNotificationsAsync()
    {
        // Baseline entities are available in B2BDbContext
        return await _context.Notifications.ToListAsync();
    }
}
```

### Example 3: Minimal Microservice (Baseline Only)

**Scenario:** Authentication microservice needing only users and roles

**appsettings.json:**
```json
{
  "DatabaseContext": {
    "BaselineOnly": true
  }
}
```

**Service:**
```csharp
public class AuthService
{
    private readonly BaselineDbContext _context;
    
    public AuthService(BaselineDbContext context)
    {
        _context = context;
    }
    
    // Only core entities available
}
```

### Example 4: Marketplace Platform (Hybrid Mode)

**Scenario:** Platform with both individual sellers and corporate buyers

**appsettings.json:**
```json
{
  "DatabaseContext": {
    "ContextType": "B2B",
    "EnableHybridMode": true
  }
}
```

**Service:**
```csharp
public class MarketplaceService
{
    private readonly B2BDbContext _b2bContext;
    private readonly ApplicationDbContext _b2cContext;
    
    public MarketplaceService(B2BDbContext b2bContext, ApplicationDbContext b2cContext)
    {
        _b2bContext = b2bContext;
        _b2cContext = b2cContext;
    }
    
    public async Task<List<OrganizationEntity>> GetCorporateBuyersAsync()
    {
        return await _b2bContext.Organizations
            .Where(o => o.Type == "Corporate")
            .ToListAsync();
    }
    
    public async Task<List<UserEntity>> GetIndividualSellersAsync()
    {
        // Use B2C context for individual users
        return await _b2cContext.Users
            .Where(u => u.Role == UserRole.Seller)
            .ToListAsync();
    }
}
```

### Example 5: Demo App with Dynamic B2C/B2B Dashboards (Hybrid Mode)

**Scenario:** AppBlueprint demo app where users choose B2C or B2B signup flow, and the UI adapts dynamically

**Why Hybrid Mode?**
- Users can sign up as **individual consumers** (B2C) or **organization members** (B2B)
- Dashboard displays different features based on user type
- Same codebase, same database, different entity sets accessed per user context
- Repositories need access to both B2C entities (personal features) and B2B entities (teams, organizations)

**launchSettings.json (Both Web and ApiService):**
```json
{
  "environmentVariables": {
    "ASPNETCORE_ENVIRONMENT": "Development",
    "DatabaseContext__ContextType": "B2C",
    "DatabaseContext__EnableHybridMode": "true"
  }
}
```

**Program.cs:**
```csharp
// AppBlueprint.Web/Program.cs
builder.Services.AddAppBlueprintInfrastructure(builder.Configuration, builder.Environment);

// AppBlueprint.ApiService/Program.cs
builder.Services.AddAppBlueprintInfrastructure(builder.Configuration, builder.Environment);
```

**Dynamic Dashboard Service:**
```csharp
public class DashboardService
{
    private readonly B2BDbContext _b2bContext;
    private readonly ApplicationDbContext _b2cContext;
    private readonly ICurrentUserService _currentUser;
    
    public DashboardService(
        B2BDbContext b2bContext,
        ApplicationDbContext b2cContext,
        ICurrentUserService currentUser)
    {
        _b2bContext = b2bContext;
        _b2cContext = b2cContext;
        _currentUser = currentUser;
    }
    
    public async Task<DashboardViewModel> GetDashboardAsync()
    {
        var userId = _currentUser.GetCurrentUserId();
        var user = await _b2cContext.Users.FindAsync(userId);
        
        if (user.AccountType == AccountType.Organization)
        {
            // Load B2B dashboard with Teams, Organizations, API Keys
            var org = await _b2bContext.Organizations
                .Include(o => o.Teams)
                .FirstOrDefaultAsync(o => o.Id == user.OrganizationId);
                
            var apiKeys = await _b2bContext.ApiKeys
                .Where(k => k.OrganizationId == user.OrganizationId)
                .ToListAsync();
            
            return new DashboardViewModel
            {
                UserType = "B2B",
                Organization = org,
                Teams = org?.Teams,
                ApiKeys = apiKeys,
                ShowOrganizationFeatures = true
            };
        }
        else
        {
            // Load B2C dashboard with personal features, notifications
            var notifications = await _b2cContext.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(10)
                .ToListAsync();
            
            return new DashboardViewModel
            {
                UserType = "B2C",
                Notifications = notifications,
                ShowPersonalFeatures = true
            };
        }
    }
}
```

**Blazor Dashboard Component:**
```razor
@page "/dashboard"
@inject DashboardService DashboardService

<MudContainer>
    @if (_dashboard?.UserType == "B2B")
    {
        <MudText Typo="Typo.h4">Organization Dashboard</MudText>
        
        <MudGrid>
            <MudItem xs="12" md="6">
                <MudCard>
                    <MudCardHeader>
                        <MudText Typo="Typo.h6">Teams</MudText>
                    </MudCardHeader>
                    <MudCardContent>
                        @foreach (var team in _dashboard.Teams ?? [])
                        {
                            <MudText>@team.Name</MudText>
                        }
                    </MudCardContent>
                </MudCard>
            </MudItem>
            
            <MudItem xs="12" md="6">
                <MudCard>
                    <MudCardHeader>
                        <MudText Typo="Typo.h6">API Keys</MudText>
                    </MudCardHeader>
                    <MudCardContent>
                        @foreach (var key in _dashboard.ApiKeys ?? [])
                        {
                            <MudText>@key.Name</MudText>
                        }
                    </MudCardContent>
                </MudCard>
            </MudItem>
        </MudGrid>
    }
    else
    {
        <MudText Typo="Typo.h4">Personal Dashboard</MudText>
        
        <MudCard>
            <MudCardHeader>
                <MudText Typo="Typo.h6">Recent Notifications</MudText>
            </MudCardHeader>
            <MudCardContent>
                @foreach (var notification in _dashboard?.Notifications ?? [])
                {
                    <MudAlert Severity="Severity.Info">@notification.Message</MudAlert>
                }
            </MudCardContent>
        </MudCard>
    }
</MudContainer>

@code {
    private DashboardViewModel? _dashboard;
    
    protected override async Task OnInitializedAsync()
    {
        _dashboard = await DashboardService.GetDashboardAsync();
    }
}
```

**Key Benefits for Demo Apps:**
- ✅ **Single codebase** supports both user types
- ✅ **Dynamic UI** adapts based on signup choice
- ✅ **All features available** without code duplication
- ✅ **Easy to demo** different scenarios
- ✅ **Repositories work seamlessly** with both contexts

### Example 6: Custom Feature-Specific Context (Dating App)

**Scenario:** Dating app with custom entities (Profiles, Matches, Messages) extending B2B for organization features

**DatingAppDbContext.cs:**
```csharp
using AppBlueprint.Infrastructure.DatabaseContexts.B2B;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Infrastructure.DatabaseContexts;

public class DatingAppDbContext : B2BDbContext  // Inherits ALL Baseline + B2B entities
{
    public DatingAppDbContext(
        DbContextOptions<DatingAppDbContext> options,
        IConfiguration configuration,
        ILogger<DatingAppDbContext> logger)
        : base((DbContextOptions)options, configuration, logger)
    {
    }
    
    // Dating app specific entities
    public DbSet<ProfileEntity> Profiles { get; set; }
    public DbSet<MatchEntity> Matches { get; set; }
    public DbSet<MessageEntity> Messages { get; set; }
    public DbSet<SwipeEntity> Swipes { get; set; }
    public DbSet<SubscriptionTierEntity> SubscriptionTiers { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        
        // CRITICAL: Call base to include ALL Baseline + B2B entities
        base.OnModelCreating(modelBuilder);
        
        // Add dating app configurations
        modelBuilder.ApplyConfiguration(new ProfileEntityConfiguration());
        modelBuilder.ApplyConfiguration(new MatchEntityConfiguration());
        modelBuilder.ApplyConfiguration(new MessageEntityConfiguration());
        modelBuilder.ApplyConfiguration(new SwipeEntityConfiguration());
        modelBuilder.ApplyConfiguration(new SubscriptionTierEntityConfiguration());
    }
}
```

**Domain Entities:**
```csharp
public sealed class ProfileEntity : Entity<ProfileId>, ITenantScopedEntity
{
    public string UserId { get; private set; } = string.Empty;
    public string Bio { get; private set; } = string.Empty;
    public int Age { get; private set; }
    public string Gender { get; private set; } = string.Empty;
    public string[] Interests { get; private set; } = [];
    public string[] Photos { get; private set; } = [];
    public TenantId TenantId { get; private set; }

    public static ProfileEntity Create(TenantId tenantId, string userId, string bio, int age)
    {
        return new ProfileEntity 
        { 
            Id = ProfileId.NewId(),
            TenantId = tenantId,
            UserId = userId,
            Bio = bio,
            Age = age
        };
    }
}

public sealed class MatchEntity : Entity<MatchId>, ITenantScopedEntity
{
    public ProfileId Profile1Id { get; private set; }
    public ProfileId Profile2Id { get; private set; }
    public DateTime MatchedAt { get; private set; }
    public bool IsActive { get; private set; }
    public TenantId TenantId { get; private set; }

    public static MatchEntity Create(TenantId tenantId, ProfileId profile1, ProfileId profile2)
    {
        return new MatchEntity
        {
            Id = MatchId.NewId(),
            TenantId = tenantId,
            Profile1Id = profile1,
            Profile2Id = profile2,
            MatchedAt = DateTime.UtcNow,
            IsActive = true
        };
    }
}
```

**Registration:**
```csharp
// DatingApp.Web/Program.cs
var builder = WebApplication.CreateBuilder(args);

// Register AppBlueprint infrastructure (Baseline + B2B)
builder.Services.AddAppBlueprintInfrastructure(
    builder.Configuration,
    builder.Environment);

// Register your custom DatingApp context (adds dating features)
builder.Services.AddDbContext<DatingAppDbContext>((serviceProvider, options) =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.CommandTimeout(60);
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: null);
    });
});
```

**Create and Apply Migrations:**
```bash
# Create migration (includes ALL Baseline + B2B + Dating entities)
cd DatingApp.Infrastructure
dotnet ef migrations add InitialDatingAppSchema \
  --context DatingAppDbContext \
  --startup-project ../DatingApp.Web \
  --output-dir Migrations

# This migration includes:
# ✅ ALL Baseline tables (Users, Notifications, Files, etc.)
# ✅ ALL B2B tables (Organizations, Teams, ApiKeys)
# ✅ YOUR Dating tables (Profiles, Matches, Messages, Swipes, SubscriptionTiers)

# Apply migration
dotnet ef database update --context DatingAppDbContext
```

**Usage:**
```csharp
public class MatchingService
{
    private readonly DatingAppDbContext _context;
    
    public MatchingService(DatingAppDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<ProfileEntity>> GetPotentialMatchesAsync(ProfileId profileId)
    {
        // Access dating-specific entities
        var profile = await _context.Profiles.FindAsync(profileId);
        
        return await _context.Profiles
            .Where(p => p.Age >= profile.Age - 5 && p.Age <= profile.Age + 5)
            .ToListAsync();
    }
    
    public async Task<OrganizationEntity?> GetSubscriptionOrgAsync(string orgId)
    {
        // Also access B2B entities (inherited from B2BDbContext)
        return await _context.Organizations.FindAsync(orgId);
    }
    
    public async Task<List<NotificationEntity>> GetUserNotificationsAsync(string userId)
    {
        // Also access Baseline entities (inherited via B2B → Baseline)
        return await _context.Notifications
            .Where(n => n.UserId == userId)
            .ToListAsync();
    }
}
```

## Migration Strategy

### Migrating from Legacy Registration

**Old way (deprecated):**
```csharp
builder.Services.AddAppBlueprintInfrastructureLegacy(
    builder.Configuration,
    builder.Environment);
```

**New way:**
```csharp
// 1. Add DatabaseContext configuration to appsettings.json
{
  "DatabaseContext": {
    "ContextType": "B2C"
  }
}

// 2. Use new registration method
builder.Services.AddAppBlueprintInfrastructure(
    builder.Configuration,
    builder.Environment);
```

### Backward Compatibility

The legacy registration is maintained for backward compatibility but marked as `[Obsolete]`. It will be removed in a future major version. Migrate to the new configuration-based approach.

## Database Migrations

### Understanding Migration Inheritance

**Critical Concept:** When you create a migration for a derived context (B2B or B2C), EF Core **automatically includes ALL tables from the base context**.

```
BaselineDbContext (Users, Notifications, Files, etc.)
    ↓ inherited by
B2BDbContext (+ Organizations, Teams, ApiKeys)
    ↓ inherited by
YourCustomDbContext (+ Your domain-specific entities)
```

**What this means:**
- ✅ B2B migration includes ALL Baseline tables + B2B tables
- ✅ B2C migration includes ALL Baseline tables + B2C tables  
- ✅ Custom context migration includes ALL base tables + your tables
- ✅ You only run ONE migration for your chosen context type

### Creating Migrations

**For Baseline Only:**
```bash
cd AppBlueprint.Infrastructure
dotnet ef migrations add InitialBaseline \
  --context BaselineDbContext \
  --startup-project ../../AppBlueprint.Web

# Creates: ONLY Baseline tables (Users, Notifications, Files, etc.)
```

**For B2C (ApplicationDbContext):**
```bash
cd AppBlueprint.Infrastructure
dotnet ef migrations add InitialB2C \
  --context ApplicationDbContext \
  --startup-project ../../AppBlueprint.Web

# Creates: Baseline tables + B2C tables (ALL via inheritance)
```

**For B2B:**
```bash
cd AppBlueprint.Infrastructure
dotnet ef migrations add InitialB2B \
  --context B2BDbContext \
  --startup-project ../../AppBlueprint.Web

# Creates: Baseline tables + B2B tables (Organizations, Teams, ApiKeys)
```

**For Custom Feature Context:**
```bash
cd YourApp.Infrastructure
dotnet ef migrations add InitialYourApp \
  --context YourAppDbContext \
  --startup-project ../YourApp.Web \
  --output-dir Migrations

# Creates: Baseline + B2B/B2C + YOUR feature tables (ALL in one migration!)
```

### Applying Migrations

**Important:** You only need to apply ONE migration based on your context type:

```bash
# For B2C apps:
dotnet ef database update --context ApplicationDbContext

# For B2B apps:
dotnet ef database update --context B2BDbContext

# For Baseline only:
dotnet ef database update --context BaselineDbContext

# For custom feature apps:
dotnet ef database update --context YourAppDbContext
```

**You do NOT need to run Baseline migrations separately!** They are included automatically via inheritance.

### Migration History

Each context maintains its own migration history in `__EFMigrationsHistory`:

```sql
-- After running B2B migration
SELECT * FROM "__EFMigrationsHistory";

-- Shows:
-- MigrationId                       | ProductVersion
-- 20251114142057_InitialB2BDbContext | 9.0.0
```

This single migration created:
- ✅ All Baseline tables (Users, Notifications, Files, etc.)
- ✅ All B2B tables (Organizations, Teams, ApiKeys)

### Adding New Features

When you add new entities to your custom context:

```bash
# 1. Add new entity to your DbContext
public DbSet<NewFeatureEntity> NewFeatures { get; set; }

# 2. Create migration
dotnet ef migrations add AddNewFeature --context YourAppDbContext

# 3. Apply migration  
dotnet ef database update --context YourAppDbContext
```

## Testing

### Unit Tests

```csharp
public class ServiceTests
{
    [Fact]
    public async Task Should_Get_Organizations()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<B2BDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
            
        var config = new ConfigurationBuilder().Build();
        var logger = new NullLogger<B2BDbContext>();
        
        await using var context = new B2BDbContext(options, config, logger);
        
        context.Organizations.Add(new OrganizationEntity { Name = "Test Org" });
        await context.SaveChangesAsync();
        
        // Act
        var service = new OrganizationService(context);
        var result = await service.GetOrganizationsAsync();
        
        // Assert
        result.Should().HaveCount(1);
    }
}
```

### Integration Tests with TestContainers

```csharp
public class IntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder().Build();
    
    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
    }
    
    [Fact]
    public async Task Should_Work_With_Real_Database()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["DatabaseContext:ContextType"] = "B2B",
                ["ConnectionStrings:DefaultConnection"] = _postgres.GetConnectionString()
            })
            .Build();
            
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddConfiguredDbContext(configuration);
        
        var provider = services.BuildServiceProvider();
        var context = provider.GetRequiredService<B2BDbContext>();
        
        // Migrate
        await context.Database.MigrateAsync();
        
        // Act & Assert
        context.Organizations.Add(new OrganizationEntity { Name = "Test" });
        await context.SaveChangesAsync();
        
        var orgs = await context.Organizations.ToListAsync();
        orgs.Should().HaveCount(1);
    }
    
    public async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
    }
}
```

## Troubleshooting

### Issue: "No connection string found"

**Solution:** Ensure you have either:
1. `DATABASE_CONNECTION_STRING` environment variable, OR
2. A connection string in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=myapp;..."
     }
   }
   ```

### Issue: "Cannot enable both BaselineOnly and EnableHybridMode"

**Solution:** Choose one approach:
- Set `BaselineOnly: true` for minimal context, OR
- Set `EnableHybridMode: true` for all contexts

### Issue: Migrations apply wrong context

**Solution:** Always specify the context explicitly:
```bash
dotnet ef migrations add MyMigration --context B2BDbContext
dotnet ef database update --context B2BDbContext
```

### Issue: Multiple contexts registered unexpectedly

**Solution:** Check your configuration:
- If `EnableHybridMode: false`, only one context is registered based on `ContextType`
- If `EnableHybridMode: true`, all contexts are registered

### Issue: Entity not found in context

**Solution:** Verify the entity belongs to the context you're using:
- **Baseline**: Core entities (Users, Notifications, Files, etc.)
- **B2C**: Baseline + family entities
- **B2B**: Baseline + Organizations, Teams, API Keys

## Best Practices

1. **Choose the Right Context Type:**
   - B2C for consumer apps
   - B2B for organizational apps
   - Hybrid only when you need both

2. **Use Environment Variables for Secrets:**
   ```bash
   DATABASE_CONNECTION_STRING="Host=prod;Database=app;Username=user;Password=secret"
   ```

3. **Configure Connection String Names:**
   ```json
   {
     "DatabaseContext": {
       "ConnectionStringName": "PostgreSQL"
     }
   }
   ```

4. **Adjust Performance Settings:**
   ```json
   {
     "DatabaseContext": {
       "CommandTimeout": 90,
       "MaxRetryCount": 10
     }
   }
   ```

5. **Test with Real Database:**
   Use TestContainers for integration tests with actual PostgreSQL

6. **Custom Contexts:**
   Always call `base.OnModelCreating(modelBuilder)` to include Baseline/B2C/B2B entities

7. **Migrations:**
   Create separate migration folders for each context type

## Advanced Scenarios

### Multiple Databases

**Scenario:** Separate databases for B2C and B2B

**appsettings.json:**
```json
{
  "DatabaseContext": {
    "EnableHybridMode": true
  },
  "ConnectionStrings": {
    "B2C": "Host=b2c-db;Database=consumer;...",
    "B2B": "Host=b2b-db;Database=enterprise;..."
  }
}
```

**Program.cs:**
```csharp
// Register B2C context
builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("B2C"));
});

// Register B2B context with different connection
builder.Services.AddDbContext<B2BDbContext>((sp, options) =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("B2B"));
});
```

### Read Replicas

```csharp
public class OrganizationService
{
    private readonly B2BDbContext _writeContext;
    private readonly B2BDbContext _readContext;
    
    public OrganizationService(
        [FromKeyedServices("write")] B2BDbContext writeContext,
        [FromKeyedServices("read")] B2BDbContext readContext)
    {
        _writeContext = writeContext;
        _readContext = readContext;
    }
    
    public async Task<List<OrganizationEntity>> GetOrganizationsAsync()
    {
        // Use read replica for queries
        return await _readContext.Organizations.ToListAsync();
    }
    
    public async Task CreateOrganizationAsync(OrganizationEntity org)
    {
        // Use primary for writes
        _writeContext.Organizations.Add(org);
        await _writeContext.SaveChangesAsync();
    }
}
```

## Related Documentation

- [Multi-Tenancy Guide](./MULTI_TENANCY_GUIDE.md) - Tenant isolation with RLS
- [Authentication Guide](./AUTHENTICATION_GUIDE.md) - Provider configuration
- [Entity Modeling](./.github/.ai-rules/baseline/entity-modeling.md) - DDD patterns

## Summary

The flexible DbContext configuration allows you to:

✅ Choose the right context for your application type  
✅ Use Baseline for minimal applications  
✅ Use B2C for consumer-focused SaaS  
✅ Use B2B for organization-focused SaaS  
✅ Enable Hybrid mode for marketplace/platform apps  
✅ Configure via appsettings.json or environment variables  
✅ Maintain backward compatibility with legacy registration  
✅ Extend with custom contexts that inherit from any base context  

All contexts inherit from Baseline, ensuring core entities are always available.
