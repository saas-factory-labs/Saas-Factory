# Multi-Tenancy Configuration Guide

AppBlueprint implements **shared database multi-tenancy** with PostgreSQL Row-Level Security (RLS) by default. This provides strong tenant isolation while maintaining operational simplicity and cost-effectiveness.

## Multi-Tenancy Strategy

### Default: Shared Database with Row-Level Security (RLS)

**Architecture:**
- ✅ Single PostgreSQL database for all tenants
- ✅ Row-Level Security (RLS) policies enforce tenant isolation at database level
- ✅ `tenant_id` column on all tenant-scoped tables
- ✅ Automatic filtering via RLS policies
- ✅ Cost-effective and operationally simple

**Why RLS?**
- **Strong Security**: Database-level enforcement (can't bypass in application code)
- **Performance**: PostgreSQL optimizes RLS policies efficiently
- **Simplicity**: No complex database routing or connection management
- **Compliance**: Clear audit trail and data isolation for regulatory requirements

**Not Supported** (by design):
- ❌ Database-per-tenant (operational overhead)
- ❌ Schema-per-tenant (migration complexity)

## Tenant Isolation Implementation

### 1. Tenant-Scoped Entities

All entities that need tenant isolation implement `ITenantScoped`:

```csharp
namespace AppBlueprint.SharedKernel;

public interface ITenantScoped
{
    string TenantId { get; set; }
}
```

**Example Implementation:**

```csharp
using AppBlueprint.SharedKernel;

public sealed class UserEntity : BaseEntity, ITenantScoped
{
    public string TenantId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    // ... other properties
}
```

### 2. Database Configuration with RLS

#### Enable Row-Level Security

Create a migration to enable RLS on tenant-scoped tables:

```sql
-- Enable RLS on Users table
ALTER TABLE "Users" ENABLE ROW LEVEL SECURITY;

-- Create RLS policy for tenant isolation
CREATE POLICY tenant_isolation_policy ON "Users"
    USING ("TenantId" = current_setting('app.current_tenant_id', TRUE)::TEXT);

-- Allow tenant admins to see all data in their tenant
CREATE POLICY tenant_isolation_policy ON "Users"
    USING ("TenantId" = current_setting('app.current_tenant_id', TRUE)::TEXT);
```

#### Automated RLS Setup Script

Create `SetupRowLevelSecurity.sql` in your project root:

```sql
-- ========================================
-- PostgreSQL Row-Level Security Setup
-- for Multi-Tenant AppBlueprint Applications
-- ========================================

-- Function to set current tenant context
CREATE OR REPLACE FUNCTION set_current_tenant(tenant_id TEXT)
RETURNS VOID AS $$
BEGIN
    PERFORM set_config('app.current_tenant_id', tenant_id, FALSE);
END;
$$ LANGUAGE plpgsql;

-- Enable RLS on all tenant-scoped tables
ALTER TABLE "Users" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "Todos" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "Teams" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "Organizations" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "ContactPersons" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "EmailAddresses" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "PhoneNumbers" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "Addresses" ENABLE ROW LEVEL SECURITY;

-- Create RLS policies for tenant isolation
CREATE POLICY tenant_isolation ON "Users"
    USING ("TenantId" = current_setting('app.current_tenant_id', TRUE)::TEXT);

CREATE POLICY tenant_isolation ON "Todos"
    USING ("TenantId" = current_setting('app.current_tenant_id', TRUE)::TEXT);

CREATE POLICY tenant_isolation ON "Teams"
    USING ("TenantId" = current_setting('app.current_tenant_id', TRUE)::TEXT);

CREATE POLICY tenant_isolation ON "Organizations"
    USING ("TenantId" = current_setting('app.current_tenant_id', TRUE)::TEXT);

CREATE POLICY tenant_isolation ON "ContactPersons"
    USING ("TenantId" = current_setting('app.current_tenant_id', TRUE)::TEXT);

CREATE POLICY tenant_isolation ON "EmailAddresses"
    USING ("TenantId" = current_setting('app.current_tenant_id', TRUE)::TEXT);

CREATE POLICY tenant_isolation ON "PhoneNumbers"
    USING ("TenantId" = current_setting('app.current_tenant_id', TRUE)::TEXT);

CREATE POLICY tenant_isolation ON "Addresses"
    USING ("TenantId" = current_setting('app.current_tenant_id', TRUE)::TEXT);

-- Verify RLS is enabled
SELECT schemaname, tablename, rowsecurity 
FROM pg_tables 
WHERE schemaname = 'public' AND rowsecurity = true;
```

**Apply the script:**
```bash
psql -h localhost -U postgres -d your_database -f SetupRowLevelSecurity.sql
```

### 3. DbContext Configuration

Configure your DbContext to set tenant context on every query:

```csharp
using AppBlueprint.SharedKernel;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    private readonly ITenantProvider? _tenantProvider;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ITenantProvider? tenantProvider = null)
        : base(options)
    {
        _tenantProvider = tenantProvider;
    }

    public DbSet<UserEntity> Users { get; set; }
    public DbSet<TodoEntity> Todos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply ITenantScoped global query filter (defense in depth)
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ITenantScoped).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(ITenantScoped.TenantId));
                var tenantId = Expression.Constant(_tenantProvider?.GetCurrentTenantId());
                var equals = Expression.Equal(property, tenantId);
                var lambda = Expression.Lambda(equals, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        // Set tenant context for RLS before executing queries
        if (_tenantProvider is not null)
        {
            var tenantId = _tenantProvider.GetCurrentTenantId();
            if (!string.IsNullOrEmpty(tenantId))
            {
                await Database.ExecuteSqlRawAsync(
                    $"SELECT set_config('app.current_tenant_id', '{tenantId}', FALSE)",
                    cancellationToken
                );
            }
        }

        // Automatically set TenantId for new entities
        var entries = ChangeTracker.Entries<ITenantScoped>()
            .Where(e => e.State == EntityState.Added);

        foreach (var entry in entries)
        {
            if (string.IsNullOrEmpty(entry.Entity.TenantId) && _tenantProvider is not null)
            {
                entry.Entity.TenantId = _tenantProvider.GetCurrentTenantId();
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
```

### 4. Tenant Provider Implementation

```csharp
namespace AppBlueprint.Infrastructure.MultiTenancy;

public interface ITenantProvider
{
    string GetCurrentTenantId();
    void SetTenantId(string tenantId);
}

public class TenantProvider : ITenantProvider
{
    private string? _currentTenantId;

    public string GetCurrentTenantId()
    {
        return _currentTenantId ?? throw new InvalidOperationException(
            "Tenant context not set. Ensure TenantResolutionMiddleware is registered.");
    }

    public void SetTenantId(string tenantId)
    {
        ArgumentNullException.ThrowIfNull(tenantId);
        _currentTenantId = tenantId;
    }
}
```

### 5. Tenant Resolution Middleware

```csharp
namespace AppBlueprint.Infrastructure.MultiTenancy;

public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantProvider tenantProvider)
    {
        // Option 1: Resolve from subdomain
        var host = context.Request.Host.Host;
        var subdomain = host.Split('.')[0];
        
        // Option 2: Resolve from custom header
        var tenantId = context.Request.Headers["X-Tenant-ID"].FirstOrDefault();
        
        // Option 3: Resolve from JWT claim
        var user = context.User;
        if (user.Identity?.IsAuthenticated == true)
        {
            tenantId = user.FindFirst("tenant_id")?.Value;
        }
        
        // Option 4: Resolve from path segment
        // /tenant/{tenantId}/...
        var pathSegments = context.Request.Path.Value?.Split('/');
        if (pathSegments?.Length > 2 && pathSegments[1] == "tenant")
        {
            tenantId = pathSegments[2];
        }

        if (!string.IsNullOrEmpty(tenantId))
        {
            tenantProvider.SetTenantId(tenantId);
        }

        await _next(context);
    }
}
```

### 6. Service Registration

```csharp
// Program.cs
using AppBlueprint.Infrastructure.MultiTenancy;

var builder = WebApplication.CreateBuilder(args);

// Register tenant provider as scoped (per-request)
builder.Services.AddScoped<ITenantProvider, TenantProvider>();

// Register DbContext with tenant provider
builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connectionString);
});

var app = builder.Build();

// Add tenant resolution middleware BEFORE authentication
app.UseMiddleware<TenantResolutionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.Run();
```

## Tenant Resolution Strategies

### 1. Subdomain-based (Recommended for B2B SaaS)

**Example:** `acme.yourapp.com`, `contoso.yourapp.com`

```csharp
var host = context.Request.Host.Host;
var tenantId = host.Split('.')[0]; // Extract subdomain
tenantProvider.SetTenantId(tenantId);
```

**Pros:**
- Clear tenant identification
- Professional appearance
- Easy to configure (wildcard DNS: `*.yourapp.com`)

**Cons:**
- Requires wildcard SSL certificate
- Cookie sharing complications between subdomains

### 2. JWT Claim-based (Recommended for B2C SaaS)

**Example:** User JWT contains `"tenant_id": "acme"`

```csharp
if (context.User.Identity?.IsAuthenticated == true)
{
    var tenantId = context.User.FindFirst("tenant_id")?.Value;
    if (tenantId is not null)
    {
        tenantProvider.SetTenantId(tenantId);
    }
}
```

**Pros:**
- Works for any domain
- No DNS configuration needed
- Tied to user authentication

**Cons:**
- Requires authenticated requests
- Tenant ID embedded in every token

### 3. Custom Header (API-only)

**Example:** `X-Tenant-ID: acme`

```csharp
var tenantId = context.Request.Headers["X-Tenant-ID"].FirstOrDefault();
if (tenantId is not null)
{
    tenantProvider.SetTenantId(tenantId);
}
```

**Pros:**
- Simple for API consumers
- Easy to implement

**Cons:**
- Clients must set header on every request
- Easily forgotten or misconfigured

### 4. Path Segment

**Example:** `/tenant/acme/api/todos`

```csharp
var pathSegments = context.Request.Path.Value?.Split('/');
if (pathSegments?.Length > 2 && pathSegments[1] == "tenant")
{
    tenantProvider.SetTenantId(pathSegments[2]);
}
```

**Pros:**
- Explicit in URL
- No DNS or header configuration

**Cons:**
- Verbose URLs
- Must strip tenant segment in routing

## Security Best Practices

### Defense in Depth

Use both RLS (database-level) AND query filters (application-level):

```csharp
// Database-level: RLS policies (primary defense)
ALTER TABLE "Users" ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation ON "Users"
    USING ("TenantId" = current_setting('app.current_tenant_id')::TEXT);

// Application-level: Query filters (defense in depth)
modelBuilder.Entity<UserEntity>()
    .HasQueryFilter(u => u.TenantId == _tenantProvider.GetCurrentTenantId());
```

### Audit Tenant Access

Log all tenant context changes:

```csharp
public void SetTenantId(string tenantId)
{
    _logger.LogInformation(
        "Tenant context set to {TenantId} for request {RequestId}",
        tenantId,
        _httpContextAccessor.HttpContext?.TraceIdentifier
    );
    _currentTenantId = tenantId;
}
```

### Validate Tenant Exists

```csharp
public async Task<bool> ValidateTenantAsync(string tenantId)
{
    return await _dbContext.Tenants
        .AnyAsync(t => t.Id == tenantId && t.IsActive);
}
```

## Testing Tenant Isolation

### Unit Test Example

```csharp
[Test]
public async Task User_CanOnly_Access_OwnTenant_Data()
{
    // Arrange
    var tenantA = "tenant-a";
    var tenantB = "tenant-b";
    
    var userInTenantA = new UserEntity { TenantId = tenantA, Email = "user@tenant-a.com" };
    var userInTenantB = new UserEntity { TenantId = tenantB, Email = "user@tenant-b.com" };
    
    await _dbContext.Users.AddRangeAsync(userInTenantA, userInTenantB);
    await _dbContext.SaveChangesAsync();
    
    // Act - Query as tenant A
    _tenantProvider.SetTenantId(tenantA);
    var tenantAUsers = await _dbContext.Users.ToListAsync();
    
    // Assert
    tenantAUsers.Should().ContainSingle();
    tenantAUsers.Single().TenantId.Should().Be(tenantA);
}
```

## Migration Strategy

If you're adding multi-tenancy to an existing application:

### 1. Add TenantId Column

```csharp
public partial class AddTenantIdColumn : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "TenantId",
            table: "Users",
            maxLength: 40,
            nullable: false,
            defaultValue: "default-tenant");
            
        migrationBuilder.CreateIndex(
            name: "IX_Users_TenantId",
            table: "Users",
            column: "TenantId");
    }
}
```

### 2. Backfill Existing Data

```sql
-- Assign all existing users to a default tenant
UPDATE "Users" SET "TenantId" = 'default-tenant' WHERE "TenantId" = '';
```

### 3. Enable RLS

```sql
ALTER TABLE "Users" ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation ON "Users"
    USING ("TenantId" = current_setting('app.current_tenant_id', TRUE)::TEXT);
```

## Performance Considerations

### Indexing

Always index `TenantId` columns:

```csharp
builder.HasIndex(e => e.TenantId)
    .HasDatabaseName("IX_Users_TenantId");
```

### Composite Indexes

For common queries involving tenant + other filters:

```csharp
builder.HasIndex(e => new { e.TenantId, e.IsActive })
    .HasDatabaseName("IX_Users_TenantId_IsActive");
```

### Connection Pooling

RLS works seamlessly with connection pooling - the `app.current_tenant_id` setting is transaction-scoped.

## Troubleshooting

### Issue: RLS Not Filtering Data

**Cause:** Tenant context not set before query execution

**Solution:** Ensure `SaveChangesAsync` sets tenant context:

```csharp
await Database.ExecuteSqlRawAsync(
    $"SELECT set_config('app.current_tenant_id', '{tenantId}', FALSE)"
);
```

### Issue: Cross-Tenant Data Leakage

**Cause:** Raw SQL queries bypass RLS

**Solution:** Always use parameterized queries and set tenant context:

```csharp
await Database.ExecuteSqlRawAsync(
    "SELECT set_config('app.current_tenant_id', @p0, FALSE)",
    tenantId
);
```

### Issue: Performance Degradation

**Cause:** Missing indexes on TenantId

**Solution:** Add indexes to all tenant-scoped tables.

## Further Reading

- [PostgreSQL Row-Level Security Documentation](https://www.postgresql.org/docs/current/ddl-rowsecurity.html)
- [Multi-Tenancy Patterns](https://docs.microsoft.com/en-us/azure/architecture/guide/multitenant/overview)
- [AUTHENTICATION_GUIDE.md](./AUTHENTICATION_GUIDE.md)
- [SECURITY_BEST_PRACTICES.md](./SECURITY_BEST_PRACTICES.md)
