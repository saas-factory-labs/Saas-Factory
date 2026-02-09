# Database Context Configuration Examples

This directory contains example configurations for different types of applications using AppBlueprint.

## Quick Start

1. **Copy the appropriate example** to your project's `appsettings.json`
2. **Update the connection string** (use environment variables or secret managers in production)
3. **Configure authentication** according to your provider
4. **Run migrations** for your chosen context type

## Available Examples

### 1. appsettings.b2c.example.json
**For: Consumer SaaS Applications**

**Use When:**
- Building apps for individual users
- Personal productivity tools
- Fitness trackers, finance apps
- Consumer-focused services

**Includes:**
- Baseline entities (Users, Notifications, Files, etc.)
- B2C consumer features
- Family relationships (optional)

**Configuration:**
```json
{
  "DatabaseContext": {
    "ContextType": "B2C"
  }
}
```

### 2. appsettings.b2b.example.json
**For: Organization SaaS Applications**

**Use When:**
- Building enterprise software
- Team collaboration tools
- CRM, project management
- B2B services

**Includes:**
- Baseline entities
- Organizations
- Teams
- API Keys

**Configuration:**
```json
{
  "DatabaseContext": {
    "ContextType": "B2B"
  }
}
```

### 3. appsettings.hybrid.example.json
**For: Marketplace/Platform Applications**

**Use When:**
- Building marketplaces with buyers and sellers
- Platforms serving both individuals and organizations
- Apps needing both B2C and B2B features

**Includes:**
- ALL entities (Baseline + B2C + B2B)

**Configuration:**
```json
{
  "DatabaseContext": {
    "ContextType": "B2B",
    "EnableHybridMode": true
  }
}
```

### 4. appsettings.baseline.example.json
**For: Microservices / Minimal Applications**

**Use When:**
- Building authentication microservices
- Notification services
- Services needing only core entities
- Custom contexts extending Baseline

**Includes:**
- ONLY Baseline entities (minimal footprint)

**Configuration:**
```json
{
  "DatabaseContext": {
    "BaselineOnly": true
  }
}
```

## Configuration Steps

### Step 1: Choose Your Configuration

Copy the appropriate example file to your `appsettings.json`:

```bash
# For B2C apps
cp appsettings.b2c.example.json ../../../MyApp/appsettings.json

# For B2B apps
cp appsettings.b2b.example.json ../../../MyApp/appsettings.json

# For hybrid apps
cp appsettings.hybrid.example.json ../../../MyApp/appsettings.json

# For microservices
cp appsettings.baseline.example.json ../../../MyApp/appsettings.json
```

### Step 2: Secure Your Connection String

**❌ NEVER commit connection strings to source control!**

**Local Development:**
```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=myapp;Username=postgres;Password=postgres"
```

**Production (Environment Variable):**
```bash
export DATABASE_CONNECTION_STRING="Host=prod-db;Database=myapp;Username=app;Password=secret"
```

**Azure (Key Vault):**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "@Microsoft.KeyVault(SecretUri=https://myvault.vault.azure.net/secrets/DbConnectionString/)"
  }
}
```

**AWS (Secrets Manager):**
```bash
aws secretsmanager create-secret --name db-connection-string --secret-string "Host=prod;Database=app;..."
```

### Step 3: Configure Authentication

Set authentication provider secrets as environment variables:

**For Auth0:**
```bash
export AUTH0_DOMAIN="your-tenant.auth0.com"
export AUTH0_CLIENT_ID="your-client-id"
export AUTH0_CLIENT_SECRET="your-client-secret"
export AUTH0_AUDIENCE="https://api.yourapp.com"
```

**For Azure AD:**
```bash
export AZUREAD_TENANT_ID="your-tenant-id"
export AZUREAD_CLIENT_ID="your-client-id"
export AZUREAD_CLIENT_SECRET="your-client-secret"
```

**For Logto:**
```bash
export LOGTO_ENDPOINT="https://your-tenant.logto.app"
export LOGTO_APP_ID="your-app-id"
export LOGTO_APP_SECRET="your-app-secret"
```

### Step 4: Run Migrations

**For B2C (ApplicationDbContext):**
```bash
cd Code/AppBlueprint/Shared-Modules/AppBlueprint.Infrastructure
dotnet ef migrations add InitialCreate --context ApplicationDbContext --startup-project ../../AppBlueprint.Web
dotnet ef database update --context ApplicationDbContext --startup-project ../../AppBlueprint.Web
```

**For B2B (B2BDbContext):**
```bash
cd Code/AppBlueprint/Shared-Modules/AppBlueprint.Infrastructure
dotnet ef migrations add InitialCreate --context B2BDbContext --startup-project ../../AppBlueprint.Web
dotnet ef database update --context B2BDbContext --startup-project ../../AppBlueprint.Web
```

**For Baseline (BaselineDbContext):**
```bash
cd Code/AppBlueprint/Shared-Modules/AppBlueprint.Infrastructure
dotnet ef migrations add InitialCreate --context BaselineDbContext --startup-project ../../AppBlueprint.Web
dotnet ef database update --context BaselineDbContext --startup-project ../../AppBlueprint.Web
```

### Step 5: Enable Row-Level Security (Optional but Recommended)

If using multi-tenancy with RLS:

```bash
# Connect to your PostgreSQL database
psql -h localhost -U postgres -d myapp

# Run the RLS setup script
\i SetupRowLevelSecurity.sql
```

### Step 6: Creating Feature-Specific Contexts (Optional)

If you need domain-specific entities (e.g., dating app, e-commerce), create a custom context:

**Example: Dating App Context**

```csharp
// DatingApp.Infrastructure/DatabaseContexts/DatingAppDbContext.cs
using AppBlueprint.Infrastructure.DatabaseContexts.B2B;

namespace DatingApp.Infrastructure.DatabaseContexts;

public class DatingAppDbContext : B2BDbContext  // Inherits Baseline + B2B
{
    public DatingAppDbContext(
        DbContextOptions<DatingAppDbContext> options,
        IConfiguration configuration,
        ILogger<DatingAppDbContext> logger)
        : base((DbContextOptions)options, configuration, logger)
    {
    }
    
    // Your feature-specific entities
    public DbSet<ProfileEntity> Profiles { get; set; }
    public DbSet<MatchEntity> Matches { get; set; }
    public DbSet<MessageEntity> Messages { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        
        // CRITICAL: Include all base entities
        base.OnModelCreating(modelBuilder);
        
        // Add your configurations
        modelBuilder.ApplyConfiguration(new ProfileEntityConfiguration());
        modelBuilder.ApplyConfiguration(new MatchEntityConfiguration());
    }
}
```

**Register and migrate:**
```bash
# Register in Program.cs
builder.Services.AddDbContext<DatingAppDbContext>(...);

# Create migration (includes Baseline + B2B + your entities)
cd DatingApp.Infrastructure
dotnet ef migrations add InitialDatingApp \
  --context DatingAppDbContext \
  --startup-project ../DatingApp.Web

# Apply migration
dotnet ef database update --context DatingAppDbContext
```

**Key points:**
- ✅ Inherit from B2B or B2C context (gets you Baseline + organizational/consumer features)
- ✅ Call `base.OnModelCreating()` to include all inherited entities
- ✅ Single migration creates ALL tables (Baseline + B2B/B2C + yours)
- ✅ Access inherited entities (Organizations, Users, Notifications) in your services

## Testing Your Configuration

### Verify DbContext Registration

Create a simple endpoint to test:

```csharp
app.MapGet("/test-db", async (ApplicationDbContext context) =>
{
    var userCount = await context.Users.CountAsync();
    return Results.Ok(new { UserCount = userCount, Context = "ApplicationDbContext" });
});

// For B2B
app.MapGet("/test-b2b", async (B2BDbContext context) =>
{
    var orgCount = await context.Organizations.CountAsync();
    return Results.Ok(new { OrganizationCount = orgCount, Context = "B2BDbContext" });
});
```

### Check Logs

Look for DbContext registration logs:

```
[DbContextConfigurator] Database Connection Source: Environment Variable
[DbContextConfigurator] Context Type: B2C
[DbContextConfigurator] Baseline Only: False
[DbContextConfigurator] Hybrid Mode: False
[DbContextConfigurator] Registered: B2CdbContext, ApplicationDbContext
```

## Common Issues

### Issue: "No connection string found"

**Solution:** Set `DATABASE_CONNECTION_STRING` environment variable:
```bash
export DATABASE_CONNECTION_STRING="Host=localhost;Database=myapp;Username=postgres;Password=postgres"
```

### Issue: "Cannot enable both BaselineOnly and EnableHybridMode"

**Solution:** Choose one approach:
```json
{
  "DatabaseContext": {
    "BaselineOnly": true,
    "EnableHybridMode": false
  }
}
```

> **Note:** `EnableHybridMode` can't be true when `BaselineOnly` is true

### Issue: Wrong context registered

**Solution:** Check your `ContextType` setting:
```json
{
  "DatabaseContext": {
    "ContextType": "B2C"
  }
}
```

> **Available Values:** `B2C`, `B2B`, `Baseline`

### Issue: Migrations fail

**Solution:** Ensure you specify the correct context:
```bash
dotnet ef migrations add MyMigration --context ApplicationDbContext
```

## Performance Tuning

Adjust performance settings based on your needs:

```json
{
  "DatabaseContext": {
    "CommandTimeout": 90,
    "MaxRetryCount": 10,
    "MaxRetryDelaySeconds": 15
  }
}
```

- **CommandTimeout**: Increase for long-running queries
- **MaxRetryCount**: Increase for unstable networks
- **MaxRetryDelaySeconds**: Increase max delay between retries

## Environment-Specific Configuration

### Development (appsettings.Development.json)
```json
{
  "DatabaseContext": {
    "ContextType": "B2C",
    "ConnectionStringName": "DefaultConnection"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=myapp_dev;Username=postgres;Password=postgres"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

### Production (appsettings.Production.json)
```json
{
  "DatabaseContext": {
    "ContextType": "B2C",
    "CommandTimeout": 90,
    "MaxRetryCount": 10
  },
  "_NOTE": "Connection string from environment variable DATABASE_CONNECTION_STRING",
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  }
}
```

## Migration from Legacy Configuration

If you're using the old registration method:

**Old way (deprecated):**
```csharp
builder.Services.AddAppBlueprintInfrastructureLegacy(
    builder.Configuration,
    builder.Environment);
```

**New way:**
```csharp
// 1. Add DatabaseContext configuration (choose an example above)
// 2. Update Program.cs
builder.Services.AddAppBlueprintInfrastructure(
    builder.Configuration,
    builder.Environment);
```

## Related Documentation

- [Database Context Flexibility Guide](../DATABASE_CONTEXT_FLEXIBILITY_GUIDE.md) - Comprehensive guide
- [Multi-Tenancy Guide](../MULTI_TENANCY_GUIDE.md) - Tenant isolation with RLS
- [Authentication Guide](../AUTHENTICATION_GUIDE.md) - Provider configuration

## Need Help?

1. Check the [Database Context Flexibility Guide](../DATABASE_CONTEXT_FLEXIBILITY_GUIDE.md)
2. Review the troubleshooting section above
3. Verify your configuration matches the examples
4. Check application logs for detailed error messages
