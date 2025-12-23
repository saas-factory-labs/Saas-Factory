# Custom Feature-Specific DbContext Guide

## Overview

This guide shows how to create custom DbContext classes that inherit from AppBlueprint's base contexts (Baseline, B2C, or B2B) to add domain-specific entities for your application.

**Key Concept:** By inheriting from B2B or B2C contexts, you get **ALL** Baseline entities PLUS organizational or consumer features, and can add your own domain-specific entities.

## Why Create Custom Contexts?

Use custom contexts when building domain-specific applications such as:
- 🎯 Dating apps (profiles, matches, swipes)
- 🛒 E-commerce (products, orders, carts)
- 📚 Learning platforms (courses, lessons, enrollments)
- 🏥 Healthcare systems (patients, appointments, prescriptions)
- 🏠 Real estate (properties, listings, viewings)
- 🚗 Ride-sharing (trips, drivers, ratings)

## Architecture

```
BaselineDbContext (Core)
├── Users, Notifications, Files, Integrations
├── Audit Logs, Languages, Webhooks
└── Payment/Subscription entities

    ↓ Inherited by

B2BDbContext (Organizational)
├── ALL Baseline entities
├── Organizations, Teams
└── API Keys

    ↓ Inherited by YOUR CUSTOM CONTEXT

DatingAppDbContext (Your Domain)
├── ALL Baseline entities (via B2B → Baseline)
├── ALL B2B entities (via B2B)
└── YOUR entities (Profiles, Matches, Messages, etc.)
```

## Complete Example: Dating App

### Step 1: Create Domain Entities

Create your domain entities following DDD patterns:

```csharp
// DatingApp.Core/Domain/ProfileEntity.cs
using AppBlueprint.SharedKernel;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities;

namespace DatingApp.Core.Domain;

public sealed class ProfileEntity : Entity<ProfileId>, ITenantScopedEntity
{
    private ProfileEntity() { } // EF Core constructor
    
    private ProfileEntity(
        ProfileId id,
        TenantId tenantId,
        string userId,
        string bio,
        int age,
        string gender)
    {
        Id = id;
        TenantId = tenantId;
        UserId = userId;
        Bio = bio;
        Age = age;
        Gender = gender;
    }
    
    public string UserId { get; private set; } = string.Empty;
    public string Bio { get; private set; } = string.Empty;
    public int Age { get; private set; }
    public string Gender { get; private set; } = string.Empty;
    public string[] Interests { get; private set; } = [];
    public string[] Photos { get; private set; } = [];
    public Location? Location { get; private set; }
    public TenantId TenantId { get; private set; }
    
    public static ProfileEntity Create(
        TenantId tenantId,
        string userId,
        string bio,
        int age,
        string gender)
    {
        if (age < 18) throw new ArgumentException("Must be 18 or older", nameof(age));
        if (string.IsNullOrWhiteSpace(bio)) throw new ArgumentException("Bio is required", nameof(bio));
        
        return new ProfileEntity(
            ProfileId.NewId(),
            tenantId,
            userId,
            bio,
            age,
            gender);
    }
    
    public void UpdateBio(string bio)
    {
        if (string.IsNullOrWhiteSpace(bio)) throw new ArgumentException("Bio cannot be empty", nameof(bio));
        Bio = bio;
    }
    
    public void AddPhoto(string photoUrl)
    {
        if (Photos.Length >= 6) throw new InvalidOperationException("Maximum 6 photos allowed");
        Photos = [..Photos, photoUrl];
    }
    
    public void SetLocation(double latitude, double longitude)
    {
        Location = new Location(latitude, longitude);
    }
}

// Value Object
public sealed record Location(double Latitude, double Longitude);

// Strongly Typed ID
[PublicAPI]
[IdPrefix("prof")]
[JsonConverter(typeof(StronglyTypedIdJsonConverter<string, ProfileId>))]
public sealed record ProfileId(string Value) : StronglyTypedUlid<ProfileId>(Value);
```

```csharp
// DatingApp.Core/Domain/MatchEntity.cs
public sealed class MatchEntity : Entity<MatchId>, ITenantScopedEntity
{
    private MatchEntity() { }
    
    private MatchEntity(
        MatchId id,
        TenantId tenantId,
        ProfileId profile1Id,
        ProfileId profile2Id)
    {
        Id = id;
        TenantId = tenantId;
        Profile1Id = profile1Id;
        Profile2Id = profile2Id;
        MatchedAt = DateTime.UtcNow;
        IsActive = true;
    }
    
    public ProfileId Profile1Id { get; private set; }
    public ProfileId Profile2Id { get; private set; }
    public DateTime MatchedAt { get; private set; }
    public DateTime? UnmatchedAt { get; private set; }
    public bool IsActive { get; private set; }
    public TenantId TenantId { get; private set; }
    
    public static MatchEntity Create(
        TenantId tenantId,
        ProfileId profile1Id,
        ProfileId profile2Id)
    {
        return new MatchEntity(
            MatchId.NewId(),
            tenantId,
            profile1Id,
            profile2Id);
    }
    
    public void Unmatch()
    {
        if (!IsActive) throw new InvalidOperationException("Match is already inactive");
        
        IsActive = false;
        UnmatchedAt = DateTime.UtcNow;
    }
}

[PublicAPI]
[IdPrefix("mtch")]
[JsonConverter(typeof(StronglyTypedIdJsonConverter<string, MatchId>))]
public sealed record MatchId(string Value) : StronglyTypedUlid<MatchId>(Value);
```

### Step 2: Create Entity Configurations

```csharp
// DatingApp.Infrastructure/Configurations/ProfileEntityConfiguration.cs
using DatingApp.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DatingApp.Infrastructure.Configurations;

public sealed class ProfileEntityConfiguration : IEntityTypeConfiguration<ProfileEntity>
{
    public void Configure(EntityTypeBuilder<ProfileEntity> builder)
    {
        builder.ToTable("Profiles");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Id)
            .HasConversion(
                id => id.Value,
                value => new ProfileId(value))
            .HasMaxLength(40)
            .IsRequired();
        
        builder.Property(p => p.UserId)
            .HasMaxLength(255)
            .IsRequired();
        
        builder.Property(p => p.Bio)
            .HasMaxLength(1000)
            .IsRequired();
        
        builder.Property(p => p.Age)
            .IsRequired();
        
        builder.Property(p => p.Gender)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(p => p.Interests)
            .HasColumnType("jsonb");
        
        builder.Property(p => p.Photos)
            .HasColumnType("jsonb");
        
        builder.OwnsOne(p => p.Location, loc =>
        {
            loc.Property(l => l.Latitude).HasColumnName("Latitude");
            loc.Property(l => l.Longitude).HasColumnName("Longitude");
        });
        
        builder.Property(p => p.TenantId)
            .HasConversion(
                id => id.Value,
                value => new TenantId(value))
            .HasMaxLength(40)
            .IsRequired();
        
        builder.HasIndex(p => p.TenantId);
        builder.HasIndex(p => p.UserId);
        builder.HasIndex(p => p.Age);
    }
}
```

### Step 3: Create Custom DbContext

```csharp
// DatingApp.Infrastructure/DatabaseContexts/DatingAppDbContext.cs
using AppBlueprint.Infrastructure.DatabaseContexts.B2B;
using DatingApp.Core.Domain;
using DatingApp.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DatingApp.Infrastructure.DatabaseContexts;

/// <summary>
/// Dating app DbContext that includes all Baseline + B2B entities plus dating-specific entities.
/// Inherits from B2BDbContext to get Organizations, Teams, and all Baseline features.
/// </summary>
public sealed class DatingAppDbContext : B2BDbContext
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
    public DbSet<LikeEntity> Likes { get; set; }
    public DbSet<ReportEntity> Reports { get; set; }
    public DbSet<SubscriptionTierEntity> SubscriptionTiers { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        
        // CRITICAL: Call base to include ALL Baseline + B2B entities
        base.OnModelCreating(modelBuilder);
        
        // Apply dating app entity configurations
        modelBuilder.ApplyConfiguration(new ProfileEntityConfiguration());
        modelBuilder.ApplyConfiguration(new MatchEntityConfiguration());
        modelBuilder.ApplyConfiguration(new MessageEntityConfiguration());
        modelBuilder.ApplyConfiguration(new SwipeEntityConfiguration());
        modelBuilder.ApplyConfiguration(new LikeEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ReportEntityConfiguration());
        modelBuilder.ApplyConfiguration(new SubscriptionTierEntityConfiguration());
        
        // Add custom indexes for performance
        ConfigurePerformanceIndexes(modelBuilder);
    }
    
    private static void ConfigurePerformanceIndexes(ModelBuilder modelBuilder)
    {
        // Composite indexes for common queries
        modelBuilder.Entity<MatchEntity>()
            .HasIndex(m => new { m.Profile1Id, m.Profile2Id, m.IsActive });
        
        modelBuilder.Entity<SwipeEntity>()
            .HasIndex(s => new { s.SwiperId, s.SwipedProfileId, s.Direction });
        
        modelBuilder.Entity<MessageEntity>()
            .HasIndex(m => new { m.MatchId, m.SentAt });
    }
}
```

### Step 4: Register Custom Context

```csharp
// DatingApp.Web/Program.cs
using DatingApp.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// 1. Register AppBlueprint Infrastructure (Baseline + B2B via configuration)
builder.Services.AddAppBlueprintInfrastructure(
    builder.Configuration,
    builder.Environment);

// 2. Register YOUR custom DatingApp context
builder.Services.AddDbContext<DatingAppDbContext>((serviceProvider, options) =>
{
    var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING")
                          ?? builder.Configuration.GetConnectionString("DefaultConnection");
    
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.CommandTimeout(60);
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: null);
        npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    });
    
    options.ConfigureWarnings(warnings =>
    {
        warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId
            .PendingModelChangesWarning);
    });
});

// 3. Register your services
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<IMatchingService, MatchingService>();

var app = builder.Build();
app.Run();
```

### Step 5: Create and Apply Migrations

```bash
# Navigate to your infrastructure project
cd DatingApp.Infrastructure

# Create initial migration
# This includes ALL tables: Baseline + B2B + Dating
dotnet ef migrations add InitialDatingAppSchema \
  --context DatingAppDbContext \
  --startup-project ../DatingApp.Web \
  --output-dir Migrations

# The migration will create:
# ✅ ALL Baseline tables (Users, Notifications, Files, Integrations, etc.)
# ✅ ALL B2B tables (Organizations, Teams, ApiKeys)
# ✅ YOUR Dating tables (Profiles, Matches, Messages, Swipes, Likes, Reports, SubscriptionTiers)

# Apply migration to database
dotnet ef database update \
  --context DatingAppDbContext \
  --startup-project ../DatingApp.Web
```

### Step 6: Use Your Custom Context

```csharp
// DatingApp.Application/Services/MatchingService.cs
using DatingApp.Core.Domain;
using DatingApp.Infrastructure.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Application.Services;

public sealed class MatchingService
{
    private readonly DatingAppDbContext _context;
    
    public MatchingService(DatingAppDbContext context)
    {
        _context = context;
    }
    
    // Use dating-specific entities
    public async Task<List<ProfileEntity>> GetPotentialMatchesAsync(
        ProfileId profileId,
        int maxDistance,
        CancellationToken cancellationToken = default)
    {
        var profile = await _context.Profiles
            .FirstOrDefaultAsync(p => p.Id == profileId, cancellationToken);
        
        if (profile?.Location is null) return [];
        
        // Get profiles they haven't swiped on yet
        var swipedProfileIds = await _context.Swipes
            .Where(s => s.SwiperId == profileId)
            .Select(s => s.SwipedProfileId)
            .ToListAsync(cancellationToken);
        
        return await _context.Profiles
            .Where(p => p.Id != profileId)
            .Where(p => !swipedProfileIds.Contains(p.Id))
            .Where(p => p.Age >= profile.Age - 5 && p.Age <= profile.Age + 5)
            .Where(p => p.TenantId == profile.TenantId) // Tenant isolation
            .Take(20)
            .ToListAsync(cancellationToken);
    }
    
    // Access B2B entities (inherited from B2BDbContext)
    public async Task<bool> HasActiveSubscriptionAsync(
        string organizationId,
        CancellationToken cancellationToken = default)
    {
        var org = await _context.Organizations
            .FirstOrDefaultAsync(o => o.Id == organizationId, cancellationToken);
        
        return org?.SubscriptionStatus == "active";
    }
    
    // Access Baseline entities (inherited via B2B → Baseline)
    public async Task SendMatchNotificationAsync(
        MatchId matchId,
        CancellationToken cancellationToken = default)
    {
        var match = await _context.Matches
            .FirstOrDefaultAsync(m => m.Id == matchId, cancellationToken);
        
        if (match is null) return;
        
        // Use Baseline NotificationEntity
        var notification = new NotificationEntity
        {
            UserId = match.Profile1Id.Value,
            Title = "New Match!",
            Message = "You have a new match. Start chatting now!",
            Type = "match",
            IsRead = false
        };
        
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
```

## Configuration

### appsettings.json

```json
{
  "DatabaseContext": {
    "ContextType": "B2B",
    "EnableHybridMode": false,
    "CommandTimeout": 90,
    "MaxRetryCount": 5
  },
  "ConnectionStrings": {
    "_SECURITY_NOTE": "Use environment variables in production",
    "DefaultConnection": "Host=localhost;Database=dating_app;Username=postgres;Password=postgres"
  },
  "MultiTenancy": {
    "Strategy": "SharedDatabase",
    "TenantResolutionStrategy": "JwtClaim",
    "EnableRowLevelSecurity": true,
    "EnableQueryFilters": true
  }
}
```

### Environment Variables (Production)

```bash
export DATABASE_CONNECTION_STRING="Host=prod-db.example.com;Database=dating_app;Username=app_user;Password=secure_password"
export DatabaseContext__ContextType="B2B"
```

## Adding New Features

When you need to add new entities later:

```bash
# 1. Create new entity classes
# 2. Add DbSet to DatingAppDbContext
# 3. Create entity configuration
# 4. Create migration

dotnet ef migrations add AddVideoCallFeature \
  --context DatingAppDbContext \
  --startup-project ../DatingApp.Web

# 5. Apply migration
dotnet ef database update --context DatingAppDbContext
```

## Testing

### Unit Tests

```csharp
public class MatchingServiceTests
{
    [Fact]
    public async Task GetPotentialMatches_ShouldExcludeAlreadySwipedProfiles()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DatingAppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
        
        var config = new ConfigurationBuilder().Build();
        var logger = new NullLogger<DatingAppDbContext>();
        
        await using var context = new DatingAppDbContext(
            options,
            config,
            logger);
        
        var tenantId = TenantId.NewId();
        var profile1 = ProfileEntity.Create(tenantId, "user1", "Bio 1", 25, "male");
        var profile2 = ProfileEntity.Create(tenantId, "user2", "Bio 2", 26, "female");
        
        context.Profiles.AddRange(profile1, profile2);
        await context.SaveChangesAsync();
        
        // Act
        var service = new MatchingService(context);
        var matches = await service.GetPotentialMatchesAsync(profile1.Id, 50);
        
        // Assert
        matches.Should().Contain(p => p.Id == profile2.Id);
    }
}
```

## Best Practices

### 1. Always Call base.OnModelCreating()

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // CRITICAL - includes all inherited entities
    base.OnModelCreating(modelBuilder);
    
    // Then add your configurations
    modelBuilder.ApplyConfiguration(new YourEntityConfiguration());
}
```

### 2. Use ITenantScopedEntity

```csharp
public sealed class ProfileEntity : Entity<ProfileId>, ITenantScopedEntity
{
    public TenantId TenantId { get; private set; }
    // ...
}
```

This enables:
- Automatic tenant filtering
- Row-Level Security support
- Multi-tenancy compliance

### 3. Follow DDD Patterns

- ✅ Private setters for properties
- ✅ Factory methods for creation
- ✅ Methods for state changes
- ✅ Strongly typed IDs
- ✅ Value objects for concepts

### 4. Create Indexes for Performance

```csharp
private static void ConfigurePerformanceIndexes(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<MatchEntity>()
        .HasIndex(m => new { m.Profile1Id, m.Profile2Id, m.IsActive });
}
```

### 5. Use Migrations Properly

```bash
# Always specify your context
dotnet ef migrations add MigrationName --context YourAppDbContext

# Review migration before applying
dotnet ef migrations script --context YourAppDbContext

# Apply to production carefully
dotnet ef database update --context YourAppDbContext
```

## Troubleshooting

### Issue: "Table already exists"

**Cause:** Migration trying to create Baseline/B2B tables that already exist.

**Solution:** Don't run Baseline or B2B migrations separately. Your custom context migration includes them.

### Issue: "Context not found"

**Cause:** Context not registered in DI.

**Solution:** Ensure `AddDbContext<YourAppDbContext>()` is called in `Program.cs`.

### Issue: Inherited entities not available

**Cause:** Forgot to call `base.OnModelCreating()`.

**Solution:**
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder); // Add this!
    // ...
}
```

### Issue: "No migration configuration type was found"

**Cause:** Migration output directory or namespace issues.

**Solution:** Use `--output-dir` parameter:
```bash
dotnet ef migrations add InitialCreate \
  --context YourAppDbContext \
  --output-dir Migrations
```

## Summary

✅ **Inherit from B2B or B2C** - Gets you all base features  
✅ **Call base.OnModelCreating()** - Includes inherited entities  
✅ **Single migration creates all tables** - Baseline + B2B/B2C + yours  
✅ **Access all inherited entities** - Organizations, Users, Notifications available  
✅ **Follow DDD patterns** - Private setters, factory methods, strongly typed IDs  
✅ **Use ITenantScopedEntity** - Enable multi-tenancy support  
✅ **Create performance indexes** - Optimize common queries  
✅ **Test with real database** - Use TestContainers for integration tests  

## Related Documentation

- [Database Context Flexibility Guide](./DATABASE_CONTEXT_FLEXIBILITY_GUIDE.md)
- [Multi-Tenancy Guide](./MULTI_TENANCY_GUIDE.md)
- [Entity Modeling](./.github/.ai-rules/baseline/entity-modeling.md)
- [Authentication Guide](./AUTHENTICATION_GUIDE.md)

## Examples in Repository

See these real examples in the codebase:
- `AppBlueprint.TodoAppKernel/Infrastructure/TodoDbContext.cs` - Todo app extending B2B
- More examples coming soon...
