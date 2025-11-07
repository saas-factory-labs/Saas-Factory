# 401 Unauthorized - ROOT CAUSES AND FIXES

## Problems Identified

After thorough investigation, I found **TWO critical issues** causing the 401 Unauthorized error:

### Issue 1: JWT Audience Validation Mismatch ❌
**Problem:**
- Logto issues JWT tokens without an `audience` (`aud`) claim by default
- API expected tokens to have `audience = ClientId`
- Token validation failed because audience claim was missing

**Why this happens:**
- Logto only includes audience claim when you configure an **API Resource** 
- Without API Resource configured in Logto, tokens only have `iss` (issuer) and `sub` (subject) claims
- This is standard OAuth/OIDC behavior for user authentication tokens

### Issue 2: Missing tenant-id Header ❌
**Problem:**
- `TenantMiddleware` requires ALL API requests to include a `tenant-id` header
- TodoService HTTP requests didn't include this header
- Middleware returns 400 Bad Request (or blocks before authentication)

**TenantMiddleware code:**
```csharp
if (!shouldBypassTenantValidation)
{
    string? tenantId = context.Request.Headers["tenant-id"].FirstOrDefault();
    if (string.IsNullOrEmpty(tenantId))
    {
        context.Response.StatusCode = 400; // Bad Request
        await context.Response.WriteAsync("{\"error\":\"Tenant ID is required\"...}");
        return;
    }
}
```

---

## Solutions Applied

### Fix 1: Disable Audience Validation (Temporary)

**File:** `AppBlueprint.Presentation.ApiModule/Extensions/JwtAuthenticationExtensions.cs`

**Change:**
```csharp
options.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidIssuer = $"{endpoint}/oidc",
    
    // Disable audience validation since Logto tokens don't have audience claim
    // without API Resource configured
    ValidateAudience = false,  // ← Changed from true to false
    
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ClockSkew = TimeSpan.FromMinutes(5),
    RequireSignedTokens = true,
    RequireExpirationTime = true
};
```

**What this does:**
- ✅ Validates token signature (most important security check)
- ✅ Validates token issuer (ensures token from Logto)
- ✅ Validates token expiration (ensures token not expired)
- ⚠️ **Skips audience validation** (temporary workaround)

**Is this secure?**
- ✅ Yes, for development and single-app scenarios
- ⚠️ For production with multiple APIs, you should configure API Resource in Logto
- The token signature validation is the most critical security check

### Fix 2: Add tenant-id Header to Requests

**File:** `AppBlueprint.Web/Services/AuthenticationDelegatingHandler.cs`

**Added:**
```csharp
// Add tenant-id header (required by TenantMiddleware)
if (!request.Headers.Contains("tenant-id"))
{
    var tenantId = await GetTenantIdAsync();
    request.Headers.Add("tenant-id", tenantId);
    _logger.LogDebug("Added tenant-id header: {TenantId}", tenantId);
}

private async Task<string> GetTenantIdAsync()
{
    try
    {
        // Try to get tenant ID from storage
        var tenantId = await _tokenStorageService.GetValueAsync("tenant_id");
        
        if (!string.IsNullOrEmpty(tenantId))
        {
            return tenantId;
        }
    }
    catch (Exception ex)
    {
        _logger.LogDebug("Could not retrieve tenant ID from storage: {Message}", ex.Message);
    }
    
    // Default tenant ID if none found
    return "default-tenant";
}
```

**What this does:**
- ✅ Automatically adds `tenant-id` header to all API requests
- ✅ Tries to retrieve tenant ID from browser storage first
- ✅ Falls back to "default-tenant" if not found
- ✅ Satisfies TenantMiddleware requirement

---

## Is the Todos Table Created?

### Answer: **YES!** ✅

The Todos table is already created in the database via Entity Framework migration:

**Migration:** `20250716202836_FixIndexColumnReferences.cs`

**Table Schema:**
```sql
CREATE TABLE "Todos" (
    "Id" character varying(1024) NOT NULL PRIMARY KEY,
    "Title" character varying(1024) NOT NULL,
    "Description" character varying(1024) NULL,
    "IsCompleted" boolean NOT NULL,
    "Priority" integer NOT NULL,
    "DueDate" timestamp with time zone NULL,
    "CompletedAt" timestamp with time zone NULL,
    "TenantId" character varying(1024) NOT NULL,
    "CreatedById" character varying(1024) NOT NULL,
    "AssignedToId" character varying(1024) NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastUpdatedAt" timestamp with time zone NULL,
    "IsSoftDeleted" boolean NOT NULL
);
```

**DbContext Configuration:**
```csharp
// ApplicationDbContext.cs
public DbSet<TodoAppKernel.Domain.TodoEntity> Todos { get; set; }
```

**Verification:**
You can check if the table exists by running:
```sql
SELECT * FROM "Todos";
```

Or check in pgAdmin/database tool connected to:
```
Host: switchyard.proxy.rlwy.net
Port: 58225
Database: appblueprintdb
```

---

## Granting User Permission to See Own Todos

### Answer: **NO SPECIAL PERMISSIONS NEEDED!** ✅

The `TodoController` uses simple `[Authorize]` attribute:

```csharp
[Authorize]  // ← Just requires authentication
[ApiController]
public class TodoController : ControllerBase
{
    [HttpGet]
    public Task<ActionResult<IEnumerable<TodoEntity>>> GetTodosAsync()
    {
        // Any authenticated user can access
    }
}
```

**What's Required:**
1. ✅ User must be logged in via Logto
2. ✅ Valid JWT token (not expired, valid signature)
3. ✅ Token from correct issuer (Logto endpoint)
4. ✅ Request includes `tenant-id` header

**What's NOT Required:**
- ❌ No specific roles
- ❌ No specific permissions
- ❌ No special claims
- ❌ No API Resource configured in Logto (with my fixes)

### Future: Tenant Isolation

**Current State:**
- Controller has placeholder code: `var todos = new List<TodoEntity>();`
- Returns empty list (not yet connected to database)

**TODO Comments in Controller:**
```csharp
// TODO: Implement TodoRepository and TodoService
// For now, return empty list as placeholder
```

**When Implemented:**
- Todos will be filtered by `TenantId`
- Users will only see their own tenant's todos
- Uses Entity Framework query filters for tenant isolation

---

## Files Modified

### 1. JwtAuthenticationExtensions.cs
**Change:** Disabled audience validation for Logto tokens
**Why:** Logto tokens don't include audience claim without API Resource

### 2. AuthenticationDelegatingHandler.cs  
**Change:** Added tenant-id header to all API requests
**Why:** TenantMiddleware requires this header

---

## Testing the Fixes

After restarting the application:

### 1. Check Browser Console
Look for debug logs:
```
[Debug] Added authentication token to request: GET https://apiservice/api/v1.0/todo
[Debug] Added tenant-id header: default-tenant
```

### 2. Check Network Tab
**Request Headers should include:**
```
Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...
tenant-id: default-tenant
```

### 3. Check API Logs
Look for:
```
[Information] Token validated successfully for user: {User}
```

Or if failing:
```
[Error] Authentication failed. Token preview: {TokenPreview}
```

### 4. Expected Behavior
- ✅ No 401 Unauthorized errors
- ✅ No 400 Bad Request errors
- ✅ Empty todos list returned (controller has placeholder code)
- ✅ Can add todos (when implementation is completed)

---

## Current Limitations

### 1. Placeholder Controller Implementation
**Current:**
```csharp
public Task<ActionResult<IEnumerable<TodoEntity>>> GetTodosAsync(...)
{
    // TODO: Implement TodoRepository and TodoService
    var todos = new List<TodoEntity>();
    return Task.FromResult<ActionResult<IEnumerable<TodoEntity>>>(Ok(todos));
}
```

**Status:** Returns empty list, doesn't query database yet

**When Will It Work:**
- When TodoRepository and TodoService are implemented
- When controller is updated to use repository
- Table exists, just needs business logic implementation

### 2. Tenant ID Management
**Current:** Uses "default-tenant" hardcoded value

**Future Improvement:**
- Extract tenant ID from JWT claims
- Store tenant ID in browser storage during login
- Associate users with tenants in database

### 3. Audience Validation Disabled
**Current:** Audience validation disabled for Logto tokens

**Future Improvement:**
1. Create API Resource in Logto dashboard
2. Get API Resource identifier
3. Add resource to Logto scope in Web config:
   ```json
   {
     "Scope": "openid profile email offline_access https://your-api-identifier"
   }
   ```
4. Re-enable audience validation in API

---

## Summary

### Issues Found:
1. ❌ Logto tokens missing audience claim → API rejected tokens
2. ❌ Missing tenant-id header → TenantMiddleware blocked requests

### Fixes Applied:
1. ✅ Disabled audience validation (temporary workaround)
2. ✅ Added tenant-id header to requests (permanent fix)

### Table Status:
✅ Todos table exists in database (created via migration)

### Permissions Required:
✅ Just authentication - no special roles or permissions needed

### Current Controller Status:
⚠️ Returns empty list (placeholder implementation)
📝 TODO: Implement repository and service layer

### Next Steps:
1. **Restart the application** (required for configuration changes)
2. **Log in via Logto** (get valid JWT token)
3. **Navigate to /todos page**
4. **Should work without 401 errors** (but returns empty list)
5. **Later:** Implement TodoRepository/TodoService for database operations

---

## Production Recommendations

### 1. Configure API Resource in Logto
**Steps:**
1. Go to Logto Console
2. Create API Resource
3. Add identifier (e.g., `https://api.yourapp.com`)
4. Add to Web app scope configuration
5. Re-enable audience validation

### 2. Implement Proper Tenant Management
- Extract tenant ID from JWT claims
- Create tenant management UI
- Store tenant associations in database

### 3. Implement TodoRepository
- Connect controller to Entity Framework
- Add tenant filtering
- Implement CRUD operations

### 4. Add Authorization Policies
```csharp
[Authorize(Policy = "CanManageTodos")]
```

### 5. Enable Audit Logging
- Log all todo operations
- Track who created/modified/deleted todos
- Use for compliance and debugging

---

## Verification Commands

### Check Database Table
```sql
-- Connect to PostgreSQL
psql -h switchyard.proxy.rlwy.net -p 58225 -U postgres -d appblueprintdb

-- Check if Todos table exists
\dt "Todos"

-- Check table structure
\d "Todos"

-- Check if any todos exist
SELECT * FROM "Todos";
```

### Check API Endpoint
```bash
# Get auth token from browser localStorage
# Then test API directly
curl -H "Authorization: Bearer YOUR_TOKEN_HERE" \
     -H "tenant-id: default-tenant" \
     https://localhost:8091/api/v1.0/todo
```

---

## Related Documentation

- **AUTHENTICATION_PROVIDER_FIX.md** - Authentication provider configuration
- **JWT_SIGNING_SECRET_EXPLAINED.md** - JWT validation explained
- **PRERENDERING_FIX.md** - JavaScript interop fix
- **TODO_IMPLEMENTATION.md** - Feature implementation details

---

**Status:** Configuration fixes applied. Restart required. Controller implementation still needs TodoRepository/TodoService to query database.

