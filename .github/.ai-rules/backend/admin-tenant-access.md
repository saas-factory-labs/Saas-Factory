# Admin Access to Tenant Data

This document describes secure patterns for administrators to access tenant data for support, debugging, and operational purposes without compromising the multi-tenant security architecture.

---

## Overview

**Problem:** Admins need to view/modify tenant data for support tickets, debugging, and operational tasks, but our defense-in-depth architecture (Named Query Filters + RLS) enforces strict tenant isolation.

**Solution:** Implement audited, logged, role-based admin access that explicitly bypasses isolation in a controlled manner.

---

## Security Principles

1. **Read-Only Access** - ⚠️ **CRITICAL:** Admins can ONLY VIEW tenant data, never modify or delete
2. **Explicit Admin Intent** - Admin must explicitly specify which tenant they're accessing
3. **Audit Logging** - All admin access to tenant data must be logged with timestamps and reasons
4. **Role-Based Access Control (RBAC)** - Only users with `SuperAdmin` or `TenantAdmin` roles can access other tenants
5. **Temporary Context Switching** - Admin context should be scoped to a single operation, not session-wide
6. **Bypass RLS Safely** - Use PostgreSQL RLS policies that allow admins to switch tenants, not bypass completely

---

## Implementation Patterns

### Pattern 1: Admin Service with Explicit Tenant Context

Create a dedicated admin service that explicitly sets tenant context for administrative operations.

```csharp
// AppBlueprint.Application/Services/AdminTenantAccessService.cs
using AppBlueprint.Infrastructure.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Application.Services;

/// <summary>
/// Service for administrators to access tenant data with audit logging.
/// ⚠️ READ-ONLY: Admins can only VIEW tenant data, never modify or delete.
/// </summary>
public sealed class AdminTenantAccessService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<AdminTenantAccessService> _logger;
    private readonly ICurrentUserService _currentUserService;

    public AdminTenantAccessService(
        ApplicationDbContext dbContext,
        ILogger<AdminTenantAccessService> logger,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _logger = logger;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Executes a READ-ONLY query in the context of a specific tenant with admin privileges.
    /// ⚠️ IMPORTANT: 
    /// - This bypasses Named Query Filters but RLS still enforces based on session variable
    /// - Admin access is READ-ONLY - use AsNoTracking() to prevent accidental modifications
    /// - Never use this for SaveChanges, Update, or Delete operations
    /// </summary>
    /// <param name="tenantId">The tenant to access</param>
    /// <param name="reason">Business justification (logged for audit)</param>
    /// <param name="queryAction">The READ-ONLY query to execute</param>
    public async Task<TResult> ExecuteReadOnlyAsAdminAsync<TResult>(
        string tenantId,
        string reason,
        Func<ApplicationDbContext, Task<TResult>> queryAction)
    {
        // Verify current user has admin role
        if (!_currentUserService.IsInRole("SuperAdmin"))
        {
            throw new UnauthorizedAccessException("Only SuperAdmins can access other tenants' data");
        }

        string adminUserId = _currentUserService.UserId 
            ?? throw new InvalidOperationException("Admin user ID not found");

        // Log admin access attempt
        _logger.LogWarning(
            "ADMIN ACCESS: User {AdminUserId} accessing tenant {TenantId}. Reason: {Reason}",
            adminUserId,
            tenantId,
            reason);

        try
        {
            // Set PostgreSQL session variable for RLS to allow this tenant's data
            await _dbContext.Database.ExecuteSqlRawAsync(
                "SELECT set_config('app.current_tenant_id', {0}, FALSE);",
                tenantId);

            // Execute the query with Named Query Filters bypassed
            // RLS still enforces based on session variable we just set
            TResult result = await queryAction(_dbContext);

            // Log successful access
            _logger.LogWarning(
                "ADMIN ACCESS SUCCESS: User {AdminUserId} accessed tenant {TenantId}",
                adminUserId,
                tenantId);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "ADMIN ACCESS FAILED: User {AdminUserId} failed to access tenant {TenantId}",
                adminUserId,
                tenantId);
            throw;
        }
        finally
        {
            // Clear tenant context after operation
            await _dbContext.Database.ExecuteSqlRawAsync(
                "SELECT set_config('app.current_tenant_id', NULL, FALSE);");
        }
    }
}
ReadOnlyAsAdminAsync(
        tenantId,
        reason: "Support ticket #12345 - user cannot login",
        async (dbContext) =>
        {
            // ⚠️ READ-ONLY: Use AsNoTracking() to prevent accidental modifications
            // Use IgnoreQueryFilters to bypass Named Query Filters (Layer 1)
            // RLS (Layer 2) still enforces based on session variable
            return await dbContext.Users
                .AsNoTracking() // ✅ Read-only - prevents SaveChanges from modifying data
                .IgnoreQueryFilters()
                .Where(u => u.Id == userId)
                .Select(u => new UserProfile { /* ... */ })
                .FirstOrDefaultAsync();
        });
}

// ❌ FORBIDDEN: Never implement write operations for admin access
// public async Task UpdateUserAsAdmin(...) { } // NO!
// public async Task DeleteUserAsAdmin(...) { } // NO!               .Where(u => u.Id == userId)
                .Select(u => new UserProfile { /* ... */ })
                .FirstOrDefaultAsync();
        }); (Read-Only)

Modify the RLS setup to allow admin users to switch tenants for **read-only access**.

```sql
-- SetupRowLevelSecurity.sql (enhanced version)

-- Create admin check function
CREATE OR REPLACE FUNCTION is_admin_user()
RETURNS BOOLEAN AS $$
BEGIN
    -- Check if current_setting('app.is_admin') is set to 'true'
    -- This is set by admin operations only
    RETURN current_setting('app.is_admin', TRUE) = 'true';
EXCEPTION
    WHEN OTHERS THEN
        RETURN FALSE;
END;
$$ LANGUAGE plpgsql STABLE;

-- ⚠️ READ-ONLY: Admin policy for SELECT only (no INSERT, UPDATE, DELETE)
CREATE POLICY tenant_isolation_with_admin_read ON users
    FOR SELECT  -- ✅ Only SELECT allowed for admins
    USING (
        -- Normal users see only their tenant
        tenant_id = current_setting('app.current_tenant_id', TRUE)::TEXT
        OR
        -- Admins can see any tenant when app.is_admin is set
        is_admin_user()
    );

-- ⚠️ WRITE PROTECTION: Normal tenant isolation for INSERT/UPDATE/DELETE (no admin bypass)
CREATE POLICY tenant_isolation_write ON users
    FOR ALL  -- Covers INSERT, UPDATE, DELETE
    USING (tenant_id = current_setting('app.current_tenant_id', TRUE)::TEXT)
    WITH CHECK (tenant_id = current_setting('app.current (READ-ONLY)
public async Task<TResult> ExecuteReadOnlyAsAdminAsync<TResult>(
    string tenantId,
    string reason,
    Func<ApplicationDbContext, Task<TResult>> queryAction)
{
    // ... authorization checks ...

    try
    {
        // Set BOTH tenant context AND admin flag for RLS
        await _dbContext.Database.ExecuteSqlRawAsync(@"
            SELECT set_config('app.current_tenant_id', {0}, FALSE);
            SELECT set_config('app.is_admin', 'true', FALSE);
        ", tenantId);

        // ⚠️ Execute query in read-only mode
        // PostgreSQL RLS policy only allows SELECT with is_admin_user() = true
        // Any attempt to UPDATE/DELETE will be blocked by RLS write policies    string tenantId,
    string reason,
    Func<ApplicationDbContext, Task<TResult>> queryAction)
{
    // ... authorization checks ...

    try
    {
        // Set BOTH tenant context AND admin flag for RLS
        await _dbContext.Database.ExecuteSqlRawAsync(@"
            SELECT set_config('app.current_tenant_id', {0}, FALSE);
            SELECT set_config('app.is_admin', 'true', FALSE);
        ", tenantId);

        TResult result = await queryAction(_dbContext);
        return result;
    }
    finally
    {
        // Clear admin context
        await _dbContext.Database.ExecuteSqlRawAsync(@"
            SELECT set_config('app.current_tenant_id', NULL, FALSE);
            SELECT set_config('app.is_admin', 'false', FALSE);
        ");
    }
}
```

---

### Pattern 3: DeploymentManager Admin Portal

For the DeploymentManager application, create a dedicated admin portal for tenant management.

```csharp
// DeploymentManager.Web/Components/Pages/TenantDataViewer.razor.cs

public sealed class TenantDataViewerModel : ComponentBase
{
    [Inject] private AdminTenantAccessService AdminService { get; set; } = default!;
    [Inject] private ICurrentUserService CurrentUserService { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    [Parameter] public string TenantId { get; set; } = string.Empty;
    
    private string AccessReason { get; set; } = string.Empty;
    private List<UserProfile>? Users { get; set; }
    private bool IsLoading { get; set; }
    private string? ErrorMessage { get; set; }

    protected override async Task OnInitializedAsync()
    {
        // Verify user is admin
        if (!CurrentUserService.IsInRole("SuperAdmin"))
        {
            Navigation.NavigateTo("/unauthorized");
            return;
        }
    }

    private async Task LoadTenantDataAsync()
    {
        if (string.IsNullOrWhiteSpace(AccessReason))
        {// ⚠️ READ-ONLY access - AsNoTracking() prevents accidental modifications
            Users = await AdminService.ExecuteReadOnlyAsAdminAsync(
                TenantId,
                AccessReason,
                async (dbContext) =>
                {
                    return await dbContext.Users
                        .AsNoTracking() // ✅ Read-only - prevents SaveChange
        ErrorMessage = null;

        try
        {
            Users = await AdminService.ExecuteAsAdminAsync(
                TenantId,
                AccessReason,
                async (dbContext) =>
                {
                    return await dbContext.Users
                        .IgnoreQueryFilters() // Bypass Named Query Filters
                        .Where(u => u.TenantId == TenantId) // Explicit filter for clarity
                        .Select(u => new UserProfile
                        {
                            Id = u.Id,
                            Email = u.Email,
                            Role = u.Role,
                            CreatedAt = u.CreatedAt
                        })
                        .ToListAsync();
                });
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load tenant data: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
```

```razor
<!-- DeploymentManager.Web/Components/Pages/TenantDataViewer.razor -->

@page "/admin/tenant/{TenantId}/data"
@attribute [Authorize(Roles = "SuperAdmin")]

<PageTitle>Admin: View Tena (Read-Only):</strong> You are viewing another tenant's data. 
    You cannot modify or delete this data.
<h1>View Tenant Data - @TenantId</h1>

<div class="alert alert-warning" role="alert">
    <strong>⚠️ Admin Access:</strong> You are viewing another tenant's data. 
    All access is logged for audit purposes.
</div>

<div class="mb-3">
    <label for="accessReason" class="form-label">
        <strong>Reason for Access</strong> (required for audit log)
    </label>
    <input 
        type="text" 
        class="form-control" 
        id="accessReason" 
        @bind="AccessReason" 
        placeholder="e.g., Support ticket #12345 - user cannot login"
        required />
</div>

<button 
    class="btn btn-primary" 
    @onclick="LoadTenantDataAsync" 
    disabled="@IsLoading">
    @if (IsLoading)
    {
        <span class="spinner-border spinner-border-sm" role="status"></span>
        <span> Loading...</span>
    }
    else
    {
        <span>Load Tenant Data</span>
    }
</button>

@if (ErrorMessage is not null)
{
    <div class="alert alert-danger mt-3" role="alert">
        @ErrorMessage
    </div>
}

@if (Users is not null)
{
    <table class="table table-striped mt-3">
        <thead>
            <tr>
                <th>User ID</th>
                <th>Email</th>
                <th>Role</th>
                <th>Created At</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var user in Users)
            {
                <tr>
                    <td>@user.Id</td>
                    <td>@user.Email</td>
                    <td>@user.Role</td>
                    <td>@user.CreatedAt.ToString("yyyy-MM-dd HH:mm")</td>
                </tr>
            }
        </tbody>
    </table>
}
```

---

## Audit Logging Requirements

All admin access to tenant data MUST be logged with:

1. **Admin User ID** - Who accessed the data
2. **Tenant ID** - Which tenant was accessed
3. **Timestamp** - When the access occurred
4. **Reason** - Business justification provided by admin
5. **Operation** - What was done (view, edit, delete)
6. **Result** - Success or failure
7. **IP Address** - Where the request came from

### Recommended Log Format

```csharp
_logger.LogWarning(
    "ADMIN_TENANT_ACCESS | AdminUserId={AdminUserId} | TenantId={TenantId} | " +
    "Operation={Operation} | Reason={Reason} | Result={Result} | IpAddress={IpAddress}",
    adminUserId,
    tenantId,
    "ViewUsers",
    reason,
    "Success",
    httpContext.Connection.RemoteIpAddress);
```

### Store Audit Logs Separately

Create a dedicated `admin_audit_log` table that:
- Is **not tenant-scoped** (no RLS policy)
- Is **append-only** (no UPDATE/DELETE permissions)
- Stores all admin access attempts
- Is monitored for suspicious patterns

```sql
CREATE TABLE admin_audit_log (
    id SERIAL PRIMARY KEY,
    admin_user_id TEXT NOT NULL,
    tenant_id TEXT NOT NULL,
    operation TEXT NOT NULL,
    reason TEXT NOT NULL,
    result TEXT NOT NULL,
    ip_address INET,
    timestamp TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Grant INSERT only (append-only)
GRANT INSERT ON admin_audit_log TO app_user;

-- Create index for querying
CREATE INDEX idx_admin_audit_tenant ON admin_audit_log(tenant_id, timestamp DESC);
CREATE INDEX idx_admin_audit_user ON admin_audit_log(admin_user_id, timestamp DESC);
```

---

## Security Checklist

When implementing admin access, ensure:

- [ ] **READ-ONLY ENFORCEMENT** - Use `.AsNoTracking()` on all queries
- [ ] **NO WRITE METHODS** - Never implement Update/Delete admin methods
- [ ] **RLS WRITE POLICIES** - Separate RLS policies for SELECT vs INSERT/UPDATE/DELETE
- [ ] Admin role verification (RBAC)
- [ ] Explicit tenant ID parameter (no implicit context)
- [ ] Mandatory access reason (business justification)
- [ ] Audit logging (admin user, tenant, timestamp, reason, operation)
- [ ] Temporary context switching (scoped to operation)
- [ ] RLS session variable set correctly
- [ ] Named Query Filters bypassed with `.IgnoreQueryFilters()`
- [ ] Context cleared after operation (`finally` block)
- [ ] Error handling and logging
- [ ] IP address logging
- [ ] Separate audit log table (append-only)
- [ ] UI warning banner ("Read-Only Admin Access")
- [ ] Rate limiting (prevent bulk data extraction)
- [ ] MFA requirement for admin operations

---

## Example: DeploymentManager Integration

### 1. Register AdminTenantAccessService

```csharp
// DeploymentManager.Web/Program.cs

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<AdminTenantAccessService>();

// Add HttpContextAccessor for IP logging
builder.Services.AddHttpContextAccessor();
```

### 2. Create Admin Controller

```csharp
// DeploymentManager.ApiService/Controllers/AdminTenantController.cs

[ApiController]
[Route("api/admin/tenants")]
[Authorize(Roles = "SuperAdmin")]
public sealed class AdminTenantController : ControllerBase
{
    private readonly AdminTenantAccessService _adminService;
    private readonly ILogger<AdminTenantController> _logger;

    public AdminTenantController(
        AdminTenantAccessService adminService,
        ILogger<AdminTenantController> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    [HttpGet("{tenantId}/users")]
    public async Task<IActionResult> GetTenantUsers(
        [FromRoute] string tenantId,
        [FromQuery] string reason)
    {
        // ⚠️ READ-ONLY: AsNoTracking() prevents accidental modifications
        var users = await _adminService.ExecuteReadOnlyAsAdminAsync(
            tenantId,
            reason,
            async (dbContext) =>
            {
                return await dbContext.Users
                    .AsNoTracking() // ✅ Read-only
                    .IgnoreQueryFilters()
                    .Where(u => u.TenantId == tenantId)
                    .Select(u => new UserDto
                    {
                        Id = u.Id,
                        Email = u.Email,
                        Role = u.Role
                    })
                    .ToListAsync();
            });

        return Ok(users);
    }
    
    // ❌ FORBIDDEN: No PUT/PATCH/DELETE endpoints for admin access
    // [HttpPut("{tenantId}/users/{userId}")] // NO!
    // [HttpDelete("{tenantId}/users/{userId}")] // NO!               })
                    .ToListAsync();
            });

        return Ok(users);
    }
}
```

---
Read/Write** | Read + Write | **READ-ONLY** (`.AsNoTracking()`) |
| **Modifications** | Allowed (own tenant) | **FORBIDDEN** (all tenants) |
| **
## Comparison: Admin Access vs Regular Access

| Aspect | Regular User Access | Admin Access |
|--------|-------------------|--------------|
| **Tenant Context** | Implicit (from JWT) | Explicit (parameter) |
| **Named Query Filters** | Active (automatic) | Bypassed (`.IgnoreQueryFilters()`) |
| **RLS Enforcement** | Active (session var from JWT) | Active (session var set manually) |
| **Authorization** | Tenant membership | Admin role + reason |
| **Audit Logging** | Optional | **MANDATORY** |
| **UI Warning** | None | Prominent banner |
| **Rate Limiting** | Standard | Strict (prevent bulk extraction) |

---

## Testing Admin Access

### Test 1: Admin Can Access Other Tenant

```csharp
[Test]
public async Task AdminCanAccessOtherTenant()
{ (read-only)
    var users = await _adminService.ExecuteReadOnlyAsAdminAsync(
        tenantB,
        "Test: Verify admin can access other tenant",
        async (db) => await db.Users
            .AsNoTracking() // ✅ Read-only
            
    await SeedTenantData(tenantA, userCount: 5);
    await SeedTenantData(tenantB, userCount: 3);

    // Act - Admin accesses tenant B's data
    var users = await _adminService.ExecuteAsAdminAsync(
        tenantB,
        "Test: Verify admin can access other tenant",
        async (db) => await db.Users.IgnoreQueryFilters()
            .Where(u => u.TenantId == tenantB)
            .ToListAsync());

    // Assert
    await Assert.That(users).HasCount().EqualTo(3);
    await Assert.That(users).IsAllSatisfy(u => u.TenantId == tenantB);
}
```

### Test 2: Non-Admin Cannot Access Other TenantReadOnlyAsAdminAsync(
        "tenant-bbb",
        "Should fail",
        async (db) => await db.Users.AsNoTracking()
public async Task NonAdminCannotAccessOtherTenant()
{
    // Arrange
    _mockCurrentUserService.Setup(x => x.IsInRole("SuperAdmin")).Returns(false);

    // Act & Assert
    await Assert.That(async () => await _adminService.ExecuteAsAdminAsync(
        "tenant-bbb",
        "Should fail",
        async (db) => await db.Users.ToListAsync()
    )).ThrowsExactly<UnauthorizedAccessException>();
}
```

### Test 3: Audit Log Created
ReadOnlyAsAdminAsync(
        tenantId,
        reason,
        async (db) => await db.Users.AsNoTracking().IgnoreQueryFilters().ToListAsync());

    // Assert - Check logs contain admin access entry
    _testLogger.Verify(
        x => x.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("ADMIN ACCESS") && v.ToString()!.Contains(tenantId)),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
        Times.AtLeastOnce);
}

### Test 4: Admin Cannot Modify Data

```csharp
[Test]
public async Task AdminCannotModifyTenantData()
{
    // Arrange
    string tenantId = "tenant-aaa";
    aREAD-ONLY ONLY** - ⚠️ **NEVER** implement write/delete operations for admin access
2. **Use AsNoTracking()** - Prevent accidental modifications through EF Core change tracking
3. **Separate RLS policies** - Different policies for SELECT vs INSERT/UPDATE/DELETE
4. **Always require a reason** - Never allow silent admin access
5. **Log everything** - Use structured logging for easy querying
6. **Use temporary context** - Never leave admin mode active longer than needed
7. **Rate limit admin operations** - Prevent bulk data extraction
8. **Monitor audit logs** - Alert on suspicious patterns (e.g., accessing 100+ tenants/hour)
9. **Require MFA for admin** - Extra security layer
10. **Use separate admin UI** - Don't mix admin and regular user interfaces
11. **Clear visual indicators** - Always show "Read-Only Admin Access" banner
12. **Time-bound admin sessions** - Expire admin privileges after X minutes
13. **Regular audit reviews** - Review admin access logs weekly/monthly
14. **Integration tests** - Test that admin cannot modify data (Test 4 above)
                .FirstAsync(u => u.TenantId == tenantId);
            
            user.Email = "hacked@example.com"; // ❌ This will fail
     ⚠️ READ-ONLY ACCESS ONLY** - Use `.AsNoTracking()` on all queries, never implement write operations
2. **Create `AdminTenantAccessService`** with explicit tenant context switching
3. **Require admin role** (`SuperAdmin`) and business justification
4. **Set RLS session variable** to the target tenant
5. **Bypass Named Query Filters** with `.IgnoreQueryFilters()`
6. **Separate RLS policies** - Allow SELECT with admin flag, block INSERT/UPDATE/DELETE
7. **Log all access** with admin user, tenant, reason, timestamp, IP, operation type
8. **Store audit logs** in append-only table
9. **Clear context** after operation (use `finally` block)
10. **Show UI warning** "Read-Only Admin Access" banner
11. **Integration tests** - Verify admin cannot modify/delete data

This approach maintains security while allowing legitimate **read-only** administrative operations with full audit trail.

**Security Guarantee:** Even if an admin tries to call `SaveChanges()`, the combination of `.AsNoTracking()` and RLS write policies prevents any modifications to tenant data
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("ADMIN ACCESS") && v.ToString()!.Contains(tenantId)),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
        Times.AtLeastOnce);
}
```

---

## Best Practices

1. **Always require a reason** - Never allow silent admin access
2. **Log everything** - Use structured logging for easy querying
3. **Use temporary context** - Never leave admin mode active longer than needed
4. **Rate limit admin operations** - Prevent bulk data extraction
5. **Monitor audit logs** - Alert on suspicious patterns (e.g., accessing 100+ tenants/hour)
6. **Require MFA for admin** - Extra security layer
7. **Use separate admin UI** - Don't mix admin and regular user interfaces
8. **Clear visual indicators** - Always show when in admin mode
9. **Time-bound admin sessions** - Expire admin privileges after X minutes
10. **Regular audit reviews** - Review admin access logs weekly/monthly

---

## Summary

**For DeploymentManager to securely access tenant data:**

1. **Create `AdminTenantAccessService`** with explicit tenant context switching
2. **Require admin role** (`SuperAdmin`) and business justification
3. **Set RLS session variable** to the target tenant
4. **Bypass Named Query Filters** with `.IgnoreQueryFilters()`
5. **Log all access** with admin user, tenant, reason, timestamp, IP
6. **Store audit logs** in append-only table
7. **Clear context** after operation (use `finally` block)
8. **Show UI warning** when viewing tenant data as admin

This approach maintains security while allowing legitimate administrative operations with full audit trail.
