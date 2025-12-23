# Admin Tenant Access in DeploymentManager

## Overview

DeploymentManager can securely view tenant data from the main AppBlueprint application using `IAdminTenantAccessService`. This enables support operations like troubleshooting user issues, investigating bugs, and verifying data integrity.

## Security Architecture

### Defense-in-Depth Approach

1. **Application Layer (EF Core Named Query Filters)**
   - Automatically filters queries by tenant ID
   - Prevents developer mistakes
   - Can be bypassed with `.IgnoreQueryFilters()` (intentionally, for admin access)

2. **Database Layer (PostgreSQL Row-Level Security)**
   - Enforces tenant isolation at database level
   - **Separate policies for read vs write**:
     - `FOR SELECT`: Allows admin when `app.is_admin = 'true'`
     - `FOR ALL` (INSERT/UPDATE/DELETE): **Blocks admin** regardless of flag
   - Cannot be bypassed without `BYPASSRLS` privilege

3. **Read-Only Enforcement**
   - `.AsNoTracking()` prevents EF Core change tracking
   - RLS write policies block INSERT/UPDATE/DELETE at database level
   - Even if admin calls `SaveChanges()`, nothing happens

4. **Audit Logging**
   - All admin access logged with:
     - Admin user ID
     - Target tenant ID
     - Reason for access (required)
     - Timestamp
     - Operation result (success/failure)

## Integration Guide

### 1. Add Package Reference

DeploymentManager needs reference to AppBlueprint infrastructure:

```xml
<!-- DeploymentManager.Web.csproj or DeploymentManager.ApiService.csproj -->
<ItemGroup>
  <ProjectReference Include="..\..\AppBlueprint\Shared-Modules\AppBlueprint.Application\AppBlueprint.Application.csproj" />
  <ProjectReference Include="..\..\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure\AppBlueprint.Infrastructure.csproj" />
</ItemGroup>
```

### 2. Register Services

In `Program.cs`:

```csharp
using AppBlueprint.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register AppBlueprint database contexts and services
builder.Services.AddInfrastructureServices(
    builder.Configuration,
    builder.Configuration["ConnectionStrings:DefaultConnection"]!);

// This includes:
// - ApplicationDbContext
// - ITenantContextAccessor
// - ICurrentUserService
// - IAdminTenantAccessService

var app = builder.Build();
```

### 3. Inject and Use IAdminTenantAccessService

#### In Blazor Component (Recommended)

See `TenantDataViewer.razor` for full example:

```razor
@inject IAdminTenantAccessService AdminService

@code {
    private async Task LoadTenantData()
    {
        var users = await AdminService.ExecuteReadOnlyAsAdminAsync(
            tenantId: "tenant-abc-123",
            reason: "Support ticket #456 - user login issue",
            async () => await _dbContext.Users
                .AsNoTracking() // ✅ REQUIRED - Read-only
                .IgnoreQueryFilters()
                .Where(u => u.TenantId == tenantId)
                .ToListAsync());
    }
}
```

#### In API Controller

```csharp
[ApiController]
[Route("api/admin/tenants")]
[Authorize(Roles = "SuperAdmin")]
public class AdminTenantController : ControllerBase
{
    private readonly IAdminTenantAccessService _adminService;

    public AdminTenantController(IAdminTenantAccessService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("{tenantId}/users")]
    public async Task<IActionResult> GetTenantUsers(
        string tenantId,
        [FromQuery] string reason)
    {
        var users = await _adminService.ExecuteReadOnlyAsAdminAsync(
            tenantId,
            reason,
            async () => await _dbContext.Users
                .AsNoTracking() // ✅ Read-only
                .IgnoreQueryFilters()
                .Where(u => u.TenantId == tenantId)
                .Select(u => new UserDto 
                { 
                    Id = u.Id, 
                    Email = u.Email, 
                    Role = u.Role 
                })
                .ToListAsync());

        return Ok(users);
    }
}
```

## Important Rules

### ✅ DO

1. **Always use `.AsNoTracking()`** on queries to prevent change tracking
2. **Always provide a reason** for audit logging (e.g., support ticket number)
3. **Use `.IgnoreQueryFilters()`** to bypass Named Query Filters
4. **Show visual indicator** (banner, badge) when viewing tenant data as admin
5. **Log access locally** in addition to service logging
6. **Require `SuperAdmin` role** - add `[Authorize(Roles = "SuperAdmin")]`

### ❌ DON'T

1. **Never call `SaveChanges()`** after admin queries (will fail silently anyway)
2. **Never remove `.AsNoTracking()`** - this is your safety net
3. **Never use for bulk data extraction** - rate limit admin operations
4. **Never cache tenant data** without clearing it immediately after
5. **Never mix admin and regular user interfaces** - separate pages/controllers
6. **Never allow anonymous reasons** - always require specific justification

## Example: View Messages for Dating App

For sensitive data like messages in a dating app:

```csharp
public async Task<List<MessageEntity>> ViewUserMessages(
    string tenantId,
    string userId,
    string reason)
{
    return await _adminService.ExecuteReadOnlyAsAdminAsync(
        tenantId,
        $"{reason} | Viewing messages for user {userId}",
        async () => await _dbContext.Messages
            .AsNoTracking() // ✅ Read-only
            .IgnoreQueryFilters()
            .Where(m => m.TenantId == tenantId && 
                       (m.SenderId == userId || m.ReceiverId == userId))
            .OrderByDescending(m => m.CreatedAt)
            .Take(50) // Limit to recent messages
            .ToListAsync());
}
```

## Audit Log Monitoring

Admin access logs can be queried:

```sql
-- View all admin access in last 24 hours
SELECT * FROM "AdminAuditLog"
WHERE "Timestamp" >= NOW() - INTERVAL '24 hours'
ORDER BY "Timestamp" DESC;

-- Find suspicious patterns (same admin accessing many tenants)
SELECT "AdminUserId", COUNT(DISTINCT "TenantId") as tenant_count
FROM "AdminAuditLog"
WHERE "Timestamp" >= NOW() - INTERVAL '1 hour'
GROUP BY "AdminUserId"
HAVING COUNT(DISTINCT "TenantId") > 10;
```

## Testing

Integration tests are available at:
- `AppBlueprint.Tests/Integration/AdminTenantAccessTests.cs`

Run tests:
```bash
dotnet test --filter "Category=AdminAccess"
```

## References

- Implementation: `AppBlueprint.Infrastructure/Services/AdminTenantAccessService.cs`
- Interface: `AppBlueprint.Application/Services/IAdminTenantAccessService.cs`
- Documentation: `.github/.ai-rules/backend/admin-tenant-access.md`
- RLS Setup: `Code/AppBlueprint/SetupRowLevelSecurity.sql`
- Tests: `AppBlueprint.Tests/Integration/AdminTenantAccessTests.cs`

## Summary

DeploymentManager can securely view any tenant's data with:
- ✅ **Read-only** enforcement (`.AsNoTracking()` + RLS policies)
- ✅ **Role-based access** (SuperAdmin only)
- ✅ **Audit logging** (all access tracked)
- ✅ **Defense-in-depth** (multiple security layers)
- ✅ **Zero modifications** (cannot INSERT/UPDATE/DELETE)

This enables legitimate support operations while maintaining security and compliance.
