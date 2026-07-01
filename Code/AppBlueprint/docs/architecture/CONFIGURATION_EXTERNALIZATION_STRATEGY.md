# Configuration Externalization Strategy

## Priority: ⚠️ HIGH PRIORITY

## Executive Summary

Standardize configuration management across AppBlueprint to eliminate inconsistencies, improve security, enable hot-reload, and provide a unified interface for all configuration sources (appsettings.json, environment variables, Azure Key Vault, AWS Secrets Manager, etc.).

## Current State Analysis

### Problems Identified

**1. Inconsistent Configuration Access Patterns**
```csharp
// Pattern 1: Direct Environment.GetEnvironmentVariable
string? apiKey = Environment.GetEnvironmentVariable("STRIPE_API_KEY");

// Pattern 2: IConfiguration indexer
string? baseUrl = _configuration["ObjectStorage:EndpointUrl"];

// Pattern 3: GetConnectionString
string? connectionString = configuration.GetConnectionString("appblueprintdb");

// Pattern 4: Mix of env var with fallback
string? key = Environment.GetEnvironmentVariable("KEY") ?? configuration["Setting:Key"];
```

**Issues:**
- ❌ No centralized configuration interface
- ❌ Hard to test (direct environment variable access)
- ❌ No validation
- ❌ No hot-reload support
- ❌ Scattered configuration logic
- ❌ Duplicate fallback patterns everywhere

**2. No IOptions Pattern Usage**
```csharp
// Current: Configuration is read directly in services
public class StripeSubscriptionService
{
    private readonly string _stripeApiKey;
    
    public StripeSubscriptionService(IConfiguration configuration)
    {
        _stripeApiKey = configuration.GetConnectionString("StripeApiKey") 
            ?? throw new InvalidOperationException("StripeApiKey not configured");
    }
}
```

**Issues:**
- ❌ No validation at startup
- ❌ Configuration errors discovered at runtime
- ❌ Can't leverage IOptionsSnapshot for hot-reload
- ❌ Difficult to unit test

**3. Mixed Secret Management**
```csharp
// Some secrets from environment variables
string? secret1 = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");

// Others from configuration (might be Key Vault)
string? secret2 = configuration["Resend:ApiKey"];

// Others from ConnectionStrings
string? secret3 = configuration.GetConnectionString("StripeApiKey");
```

**Issues:**
- ❌ Inconsistent secret handling
- ❌ No clear separation of secrets vs configuration
- ❌ Hard to audit secret access

## Proposed Strategy

### 1. Options Pattern for All Configuration

**Principle:** Every configuration section becomes a strongly-typed Options class.

#### Benefits:
- ✅ Type safety
- ✅ Validation at startup
- ✅ IntelliSense support
- ✅ Hot-reload capable (with IOptionsSnapshot)
- ✅ Testable (inject IOptions<T> with test data)
- ✅ Self-documenting

#### Implementation Pattern:

```csharp
// 1. Define Options class
public sealed class StripeOptions
{
    public const string SectionName = "Stripe";
    
    [Required]
    public string ApiKey { get; set; } = string.Empty;
    
    public string WebhookSecret { get; set; } = string.Empty;
    
    public int TimeoutSeconds { get; set; } = 30;
    
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ApiKey))
            throw new InvalidOperationException("Stripe:ApiKey is required");
            
        if (TimeoutSeconds <= 0)
            throw new InvalidOperationException("Stripe:TimeoutSeconds must be positive");
    }
}

// 2. Register with validation
services.AddOptions<StripeOptions>()
    .BindConfiguration(StripeOptions.SectionName)
    .ValidateOnStart()
    .Validate(options =>
    {
        options.Validate();
        return true;
    });

// 3. Inject in services
public class StripeSubscriptionService
{
    private readonly StripeOptions _options;
    
    public StripeSubscriptionService(IOptions<StripeOptions> options)
    {
        _options = options.Value;
    }
    
    public async Task CreateSubscriptionAsync()
    {
        // Use _options.ApiKey
        // Validated at startup - guaranteed to be non-null here
    }
}
```

### 2. Configuration Hierarchy

**Priority Order (Highest to Lowest):**

1. **Command-line arguments** (for deployment scripts)
2. **Environment variables** (container/cloud secrets)
3. **Azure Key Vault** (production secrets)
4. **User Secrets** (local development)
5. **appsettings.{Environment}.json** (environment-specific)
6. **appsettings.json** (defaults)

#### Setup in Program.cs:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Configure configuration sources in priority order
builder.Configuration.Sources.Clear();

// 1. Base configuration
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", 
        optional: true, reloadOnChange: true);

// 2. User Secrets (Development only)
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

// 3. Environment Variables (always)
builder.Configuration.AddEnvironmentVariables(prefix: "APPBLUEPRINT_");

// 4. Azure Key Vault (if configured)
var keyVaultUrl = builder.Configuration["KeyVault:Url"];
if (!string.IsNullOrEmpty(keyVaultUrl))
{
    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultUrl),
        new DefaultAzureCredential());
}

// 5. Command-line arguments (highest priority)
builder.Configuration.AddCommandLine(args);
```

### 3. Configuration Categories

#### A. Public Configuration (appsettings.json)
Non-sensitive settings that can be in source control:

```json
{
  "DatabaseContext": {
    "ContextType": "B2B",
    "CommandTimeout": 60,
    "MaxRetryCount": 5
  },
  "MultiTenancy": {
    "Strategy": "SharedDatabase",
    "TenantResolutionStrategy": "JwtClaim",
    "EnableRowLevelSecurity": true
  },
  "Features": {
    "EnableEmailNotifications": true,
    "EnableWebhooks": true,
    "EnableAnalytics": false
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

#### B. Secrets (Environment Variables / Secret Managers)

**Never in source control:**

```bash
# Connection strings
APPBLUEPRINT_ConnectionStrings__DefaultConnection="Host=..."
APPBLUEPRINT_Database__ConnectionString="Host=..."

# API Keys
APPBLUEPRINT_Stripe__ApiKey="sk_live_..."
APPBLUEPRINT_Resend__ApiKey="re_..."
APPBLUEPRINT_Cloudflare__R2__AccessKeyId="..."
APPBLUEPRINT_Cloudflare__R2__SecretAccessKey="..."

# Authentication secrets
APPBLUEPRINT_Auth0__ClientSecret="..."
APPBLUEPRINT_AzureAD__ClientSecret="..."
APPBLUEPRINT_JWT__SecretKey="..."
```

**Azure Key Vault:**
```
DbConnectionString → ConnectionStrings:DefaultConnection
StripeApiKey → Stripe:ApiKey
Auth0ClientSecret → Authentication:Auth0:ClientSecret
```

### 4. Standard Options Classes

Create Options classes for all configuration sections:

```
AppBlueprint.Application/Options/
├── DatabaseContextOptions.cs          ✅ Exists
├── MultiTenancyOptions.cs            ✅ Exists
├── AuthenticationOptions.cs          ✅ Exists
├── StripeOptions.cs                  ⚠️ Need to create
├── CloudflareR2Options.cs            ⚠️ Need to create
├── ResendEmailOptions.cs             ⚠️ Need to create
├── FeatureFlagsOptions.cs            ⚠️ Need to create
├── LoggingOptions.cs                 ⚠️ Need to create
├── CorsOptions.cs                    ⚠️ Need to create
└── TelemetryOptions.cs               ⚠️ Need to create
```

#### Example: CloudflareR2Options

```csharp
namespace AppBlueprint.Application.Options;

public sealed class CloudflareR2Options
{
    public const string SectionName = "Cloudflare:R2";
    
    /// <summary>
    /// Cloudflare R2 Access Key ID.
    /// Environment variable: APPBLUEPRINT_Cloudflare__R2__AccessKeyId
    /// </summary>
    [Required]
    public string AccessKeyId { get; set; } = string.Empty;
    
    /// <summary>
    /// Cloudflare R2 Secret Access Key.
    /// Environment variable: APPBLUEPRINT_Cloudflare__R2__SecretAccessKey
    /// </summary>
    [Required]
    [SensitiveData]
    public string SecretAccessKey { get; set; } = string.Empty;
    
    /// <summary>
    /// R2 endpoint URL (e.g., https://[account-id].r2.cloudflarestorage.com).
    /// Environment variable: APPBLUEPRINT_Cloudflare__R2__EndpointUrl
    /// </summary>
    [Required]
    [Url]
    public string EndpointUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Default bucket name.
    /// Environment variable: APPBLUEPRINT_Cloudflare__R2__BucketName
    /// </summary>
    [Required]
    public string BucketName { get; set; } = string.Empty;
    
    /// <summary>
    /// Request timeout in seconds.
    /// </summary>
    [Range(1, 300)]
    public int TimeoutSeconds { get; set; } = 30;
    
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(AccessKeyId))
            throw new InvalidOperationException($"{SectionName}:AccessKeyId is required");
            
        if (string.IsNullOrWhiteSpace(SecretAccessKey))
            throw new InvalidOperationException($"{SectionName}:SecretAccessKey is required");
            
        if (string.IsNullOrWhiteSpace(EndpointUrl))
            throw new InvalidOperationException($"{SectionName}:EndpointUrl is required");
            
        if (!Uri.IsWellFormedUriString(EndpointUrl, UriKind.Absolute))
            throw new InvalidOperationException($"{SectionName}:EndpointUrl must be a valid URL");
            
        if (string.IsNullOrWhiteSpace(BucketName))
            throw new InvalidOperationException($"{SectionName}:BucketName is required");
    }
}
```

### 5. Configuration Service Interface

Create abstraction for configuration access:

```csharp
// AppBlueprint.Application/Interfaces/Configuration/IConfigurationService.cs
namespace AppBlueprint.Application.Interfaces.Configuration;

/// <summary>
/// Provides strongly-typed access to application configuration.
/// Abstracts the underlying configuration source (appsettings, env vars, Key Vault, etc.).
/// </summary>
public interface IConfigurationService
{
    /// <summary>
    /// Gets configuration options of type T.
    /// Throws if configuration is invalid or missing.
    /// </summary>
    T GetRequired<T>() where T : class, new();
    
    /// <summary>
    /// Gets configuration options of type T.
    /// Returns null if configuration section doesn't exist.
    /// </summary>
    T? GetOptional<T>() where T : class, new();
    
    /// <summary>
    /// Gets a connection string by name.
    /// </summary>
    string? GetConnectionString(string name);
    
    /// <summary>
    /// Gets a secret value (always from secure source - env var or secret manager).
    /// </summary>
    string? GetSecret(string key);
    
    /// <summary>
    /// Validates all registered options at startup.
    /// </summary>
    void ValidateAllOptions();
}

// Implementation
public sealed class ConfigurationService : IConfigurationService
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    
    public ConfigurationService(
        IConfiguration configuration,
        IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }
    
    public T GetRequired<T>() where T : class, new()
    {
        var options = _serviceProvider.GetService<IOptions<T>>();
        if (options?.Value is null)
        {
            throw new InvalidOperationException(
                $"Configuration for {typeof(T).Name} is not registered");
        }
        return options.Value;
    }
    
    public T? GetOptional<T>() where T : class, new()
    {
        var options = _serviceProvider.GetService<IOptions<T>>();
        return options?.Value;
    }
    
    public string? GetConnectionString(string name)
    {
        return _configuration.GetConnectionString(name);
    }
    
    public string? GetSecret(string key)
    {
        // Always prefer environment variable for secrets
        return Environment.GetEnvironmentVariable($"APPBLUEPRINT_{key.Replace(":", "__")}")
               ?? _configuration[key];
    }
    
    public void ValidateAllOptions()
    {
        // Validate is handled by ValidateOnStart() in registration
        // This method can be used for custom cross-option validation
    }
}
```

### 6. Registration Pattern

Create extension method for registering all options:

```csharp
// AppBlueprint.Infrastructure/Extensions/ConfigurationServiceCollectionExtensions.cs
public static class ConfigurationServiceCollectionExtensions
{
    public static IServiceCollection AddAppBlueprintConfiguration(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        // Register configuration service
        services.AddSingleton<IConfigurationService, ConfigurationService>();
        
        // Register all options with validation
        services.AddDatabaseContextOptions(configuration);
        services.AddMultiTenancyOptions(configuration);
        services.AddAuthenticationOptions(configuration);
        services.AddExternalServiceOptions(configuration);
        services.AddFeatureFlagsOptions(configuration);
        
        return services;
    }
    
    private static IServiceCollection AddDatabaseContextOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<DatabaseContextOptions>()
            .BindConfiguration(DatabaseContextOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart()
            .Validate(options =>
            {
                options.Validate();
                return true;
            });
            
        return services;
    }
    
    private static IServiceCollection AddExternalServiceOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Stripe
        services.AddOptions<StripeOptions>()
            .BindConfiguration(StripeOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart()
            .Validate(options =>
            {
                options.Validate();
                return true;
            });
        
        // Cloudflare R2
        services.AddOptions<CloudflareR2Options>()
            .BindConfiguration(CloudflareR2Options.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart()
            .Validate(options =>
            {
                options.Validate();
                return true;
            });
        
        // Resend Email
        services.AddOptions<ResendEmailOptions>()
            .BindConfiguration(ResendEmailOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart()
            .Validate(options =>
            {
                options.Validate();
                return true;
            });
            
        return services;
    }
}
```

### 7. Usage in Services

**Before (Current):**
```csharp
public class StripeSubscriptionService
{
    private readonly string _stripeApiKey;
    
    public StripeSubscriptionService(IConfiguration configuration)
    {
        _stripeApiKey = configuration.GetConnectionString("StripeApiKey") 
            ?? throw new InvalidOperationException("StripeApiKey not configured");
    }
}
```

**After (Standardized):**
```csharp
public class StripeSubscriptionService
{
    private readonly StripeOptions _options;
    
    public StripeSubscriptionService(IOptions<StripeOptions> options)
    {
        _options = options.Value; // Guaranteed valid due to ValidateOnStart
    }
    
    public async Task CreateSubscriptionAsync()
    {
        // Use _options.ApiKey - no null checks needed!
        StripeConfiguration.ApiKey = _options.ApiKey;
        // ...
    }
}
```

### 8. Testing Benefits

**Before:**
```csharp
[Fact]
public void Test_Service()
{
    // Hard to mock Environment.GetEnvironmentVariable
    Environment.SetEnvironmentVariable("STRIPE_API_KEY", "test_key");
    
    var config = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string>
        {
            ["ConnectionStrings:StripeApiKey"] = "test_key"
        })
        .Build();
        
    var service = new StripeSubscriptionService(config);
    // Test...
}
```

**After:**
```csharp
[Fact]
public void Test_Service()
{
    // Clean, easy to set up
    var options = Options.Create(new StripeOptions
    {
        ApiKey = "test_key",
        TimeoutSeconds = 30
    });
    
    var service = new StripeSubscriptionService(options);
    // Test...
}
```

### 9. Hot Reload Support

For configuration that can change at runtime:

```csharp
// Use IOptionsSnapshot instead of IOptions
public class DynamicFeatureService
{
    private readonly IOptionsSnapshot<FeatureFlagsOptions> _options;
    
    public DynamicFeatureService(IOptionsSnapshot<FeatureFlagsOptions> options)
    {
        _options = options; // Gets fresh value on each request
    }
    
    public bool IsFeatureEnabled(string featureName)
    {
        // _options.Value is evaluated per-request
        return _options.Value.EnabledFeatures.Contains(featureName);
    }
}
```

### 10. Configuration Validation

Fail fast at startup if configuration is invalid:

```csharp
var app = builder.Build();

// Validate all options before starting
app.Services.GetRequiredService<IConfigurationService>().ValidateAllOptions();

// Or validate specific options
var dbOptions = app.Services.GetRequiredService<IOptions<DatabaseContextOptions>>();
dbOptions.Value.Validate(); // Throws if invalid

app.Run();
```

## Implementation Phases

### Phase 1: Core Options Classes (Week 1)
- ✅ DatabaseContextOptions (done)
- ✅ MultiTenancyOptions (done)
- ✅ AuthenticationOptions (done)
- ⚠️ Create StripeOptions
- ⚠️ Create CloudflareR2Options
- ⚠️ Create ResendEmailOptions

### Phase 2: Configuration Service (Week 1)
- ⚠️ Create IConfigurationService interface
- ⚠️ Implement ConfigurationService
- ⚠️ Create registration extensions

### Phase 3: Migrate Existing Code (Week 2)
- ⚠️ Update ServiceCollectionExtensions to use Options
- ⚠️ Update services to inject IOptions<T>
- ⚠️ Remove direct Environment.GetEnvironmentVariable calls
- ⚠️ Remove direct IConfiguration indexer access

### Phase 4: Documentation & Testing (Week 2)
- ⚠️ Update all documentation
- ⚠️ Create configuration examples
- ⚠️ Write migration guide
- ⚠️ Update unit tests

### Phase 5: Secret Managers Integration (Week 3)
- ⚠️ Azure Key Vault setup guide
- ⚠️ Docker Secrets support
- ⚠️ Kubernetes Secrets support

## Benefits Summary

### For Developers
✅ **Type Safety** - Compile-time checking of configuration access  
✅ **IntelliSense** - Auto-completion for configuration properties  
✅ **Validation** - Fail fast at startup with clear error messages  
✅ **Testing** - Easy to mock and test services  
✅ **Documentation** - Options classes are self-documenting  

### For Operations
✅ **Consistency** - Same pattern everywhere  
✅ **Security** - Clear separation of secrets vs configuration  
✅ **Flexibility** - Easy to switch between config sources  
✅ **Troubleshooting** - Validation errors show exact problem  

### For Architecture
✅ **Maintainability** - Centralized configuration management  
✅ **Scalability** - Easy to add new configuration sections  
✅ **Portability** - Works in any environment (local, container, cloud)  
✅ **Hot Reload** - Update configuration without restart (where needed)  

## Migration Path

### Step 1: No Breaking Changes
- Add new Options classes alongside existing code
- Services can use either IConfiguration or IOptions<T>
- Both patterns work during migration

### Step 2: Gradual Migration
- Migrate one service at a time
- Update tests as you go
- Document the new pattern

### Step 3: Deprecate Old Pattern
- Mark direct IConfiguration usage as obsolete
- Provide migration guide
- Set deadline for full migration

### Step 4: Remove Old Pattern
- Remove Environment.GetEnvironmentVariable calls
- Remove direct IConfiguration usage in services
- Keep IConfiguration for advanced scenarios only

## Configuration File Structure

### appsettings.json (Development)
```json
{
  "DatabaseContext": {
    "ContextType": "B2C",
    "ConnectionStringName": "DefaultConnection",
    "CommandTimeout": 60,
    "MaxRetryCount": 5
  },
  "MultiTenancy": {
    "Strategy": "SharedDatabase",
    "TenantResolutionStrategy": "JwtClaim",
    "EnableRowLevelSecurity": true
  },
  "Authentication": {
    "Provider": "Auth0",
    "Auth0": {
      "Domain": "dev-tenant.auth0.com",
      "ClientId": "dev-client-id"
    }
  },
  "Stripe": {
    "TimeoutSeconds": 30,
    "WebhookEndpoint": "/api/webhooks/stripe"
  },
  "Cloudflare": {
    "R2": {
      "BucketName": "app-uploads-dev",
      "TimeoutSeconds": 30
    }
  },
  "Features": {
    "EnableEmailNotifications": true,
    "EnableWebhooks": true,
    "EnableAnalytics": false
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

### Environment Variables (Production)
```bash
# Database
APPBLUEPRINT_ConnectionStrings__DefaultConnection="Host=..."

# Authentication Secrets
APPBLUEPRINT_Authentication__Auth0__ClientSecret="secret..."
APPBLUEPRINT_Authentication__AzureAD__ClientSecret="secret..."

# External Service Secrets
APPBLUEPRINT_Stripe__ApiKey="sk_live_..."
APPBLUEPRINT_Stripe__WebhookSecret="whsec_..."
APPBLUEPRINT_Cloudflare__R2__AccessKeyId="..."
APPBLUEPRINT_Cloudflare__R2__SecretAccessKey="..."
APPBLUEPRINT_Resend__ApiKey="re_..."
```

## Next Steps

1. **Review and approve** this strategy
2. **Create remaining Options classes** (Phase 1)
3. **Implement IConfigurationService** (Phase 2)
4. **Migrate ServiceCollectionExtensions** (Phase 3)
5. **Update documentation** (Phase 4)
6. **Add Azure Key Vault integration** (Phase 5)

## Summary

This strategy provides:
- ✅ **Unified configuration interface** via IOptions<T> pattern
- ✅ **Type safety and validation** with strongly-typed Options classes
- ✅ **Clear separation** of public config vs secrets
- ✅ **Flexible sources** supporting all environments (dev, container, cloud)
- ✅ **Testability** with easy mocking
- ✅ **Backward compatibility** during migration
- ✅ **Hot reload** support where needed
- ✅ **Self-documenting** configuration via XML comments

**Estimated Time:** 3 weeks for full implementation and migration.

**Risk:** Low - Non-breaking changes, gradual migration path.

**Priority:** HIGH - Improves security, maintainability, and developer experience.
