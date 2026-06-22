# Authentication and File Storage Troubleshooting Guide

This document captures critical learnings from debugging authentication (Logto), file storage (Cloudflare R2), and database issues in the AppBlueprint application.

---

## Table of Contents

1. [Logto Authentication Issues](#logto-authentication-issues)
   - [Opaque Tokens vs JWT Tokens](#opaque-tokens-vs-jwt-tokens)
   - [Wrong Logto NuGet Package](#wrong-logto-nuget-package)
   - [API Resource Scopes Configuration](#api-resource-scopes-configuration)
   - [RBAC Roles](#rbac-roles)
2. [File Storage Issues](#file-storage-issues)
   - [401 Unauthorized on API Endpoints](#401-unauthorized-on-api-endpoints)
   - [Tenant Resolution Failures](#tenant-resolution-failures)
   - [FileMetadata Table Missing (42P01)](#filemetadata-table-missing-42p01)
   - [JSONB Serialization Error](#jsonb-serialization-error)
   - [Query Filter Bug](#query-filter-bug)
   - [UI Buttons Not Working](#ui-buttons-not-working)
3. [Database Migration Issues](#database-migration-issues)
   - [Table Case Sensitivity in PostgreSQL](#table-case-sensitivity-in-postgresql)
   - [Npgsql 8.0+ Dynamic JSON Requirement](#npgsql-80-dynamic-json-requirement)

---

## Logto Authentication Issues

### Opaque Tokens vs JWT Tokens

**Problem:** Logto returns **opaque tokens** by default when no `resource` parameter is specified. Opaque tokens are short random strings that cannot be validated locally - they require calling Logto's introspection endpoint.

**Symptoms:**
- 401 Unauthorized errors on API endpoints
- Token appears as a short random string instead of a long JWT (xxx.yyy.zzz format)
- `JwtBearerHandler` fails with "invalid token" errors
- Token validation fails because there's no signature to verify

**Root Cause:**
When authenticating with Logto OIDC, if you don't request a specific API resource, Logto issues an opaque access token for user identity only. These tokens:
- Cannot be validated using standard JWT validation
- Are meant for Logto's own endpoints only
- Don't contain audience, scope, or custom claims

**Solution:**
Always specify the `resource` parameter when configuring OIDC to request a JWT access token:

```csharp
// In OpenIdConnectOptions configuration
options.Events.OnRedirectToIdentityProvider = context =>
{
    // CRITICAL: Request JWT token for API access, not opaque token
    context.ProtocolMessage.Resource = "https://api.appblueprint.dev";
    return Task.CompletedTask;
};
```

**Verification:**
- JWT tokens have three parts separated by dots: `header.payload.signature`
- Opaque tokens are short random strings like `oat_abc123xyz`
- Use [jwt.io](https://jwt.io) to decode and verify JWT structure

---

### Wrong Logto NuGet Package

**Problem:** Using `Logto.AspNetCore.Authentication` NuGet package instead of standard OIDC.

**Symptoms:**
- Complex authentication setup with Logto-specific abstractions
- Difficulty integrating with standard ASP.NET Core authentication patterns
- Token refresh issues
- SignalR authentication problems

**Root Cause:**
The `Logto.AspNetCore.Authentication` package:
- Wraps standard OIDC in Logto-specific handlers
- May not expose all OIDC configuration options
- Can conflict with other authentication schemes
- Is unnecessary since Logto is OIDC-compliant

**Solution:**
Use standard `Microsoft.AspNetCore.Authentication.OpenIdConnect` package:

```csharp
// ❌ Wrong - Logto-specific package
services.AddLogtoAuthentication(options => { ... });

// ✅ Correct - Standard OIDC
services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie()
.AddOpenIdConnect(options =>
{
    options.Authority = "https://your-tenant.logto.app/oidc";
    options.ClientId = "your-client-id";
    options.ClientSecret = "your-client-secret";
    options.ResponseType = OpenIdConnectResponseType.Code;
    options.SaveTokens = true;
    // ... standard OIDC config
});
```

**Benefits:**
- Full control over authentication flow
- Compatible with all ASP.NET Core authentication features
- Easier debugging with standard middleware
- Works seamlessly with SignalR, minimal APIs, etc.

---

### API Resource Scopes Configuration

**Problem:** API endpoints return 401 even with valid authentication.

**Symptoms:**
- User is logged in (cookies work)
- API calls fail with 401
- Token doesn't contain expected scopes

**Root Cause:**
Logto requires explicit API Resource configuration:
1. API Resource must be created in Logto Console
2. Scopes must be defined on the API Resource
3. Application must request the resource and scopes
4. RBAC permissions must grant access

**Solution:**

**Step 1: Create API Resource in Logto Console**
```
Settings → API Resources → Create API Resource
- Name: AppBlueprint API
- Identifier: https://api.appblueprint.dev
```

**Step 2: Define Scopes on API Resource**
```
API Resource → Permissions (Scopes):
- filestorage:read - Read files
- filestorage:write - Upload/delete files
- todos:read - Read todos
- todos:write - Create/update todos
```

**Step 3: Configure OIDC to Request Resource**
```csharp
options.Scope.Clear();
options.Scope.Add("openid");
options.Scope.Add("profile");
options.Scope.Add("email");
options.Scope.Add("offline_access"); // For refresh tokens
// Note: API scopes are requested via resource parameter

options.Events.OnRedirectToIdentityProvider = context =>
{
    context.ProtocolMessage.Resource = "https://api.appblueprint.dev";
    return Task.CompletedTask;
};
```

**Step 4: Validate Audience in JWT Bearer**
```csharp
services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.Authority = "https://your-tenant.logto.app/oidc";
        options.Audience = "https://api.appblueprint.dev";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = "https://api.appblueprint.dev"
        };
    });
```

---

### RBAC Roles

**Problem:** Users don't have expected permissions even with correct scopes.

**Root Cause:**
Logto RBAC requires:
1. Roles defined with permissions
2. Roles assigned to users
3. Permissions linked to API Resource scopes

**Solution:**

**Step 1: Create Roles in Logto Console**
```
Settings → Roles → Create Role
- Name: Admin
- Permissions: Select all scopes from API Resources
```

**Step 2: Assign Roles to Users**
```
User Management → Select User → Roles → Assign Role
```

**Step 3: Access Role Claims in Application**
```csharp
// Roles appear in JWT claims
var roles = User.FindAll("roles").Select(c => c.Value);

// Or use policy-based authorization
services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", policy =>
        policy.RequireClaim("roles", "admin"));
});
```

---

## File Storage Issues

### Public File Access Without Authentication

**Problem:** Public files uploaded to Cloudflare R2 require authentication when accessed directly via R2 endpoint URL.

**Symptoms:**
- Accessing public file URL shows XML error: `<Error><Code>InvalidArgument</Code><Message>Authorization</Message></Error>`
- Files marked as `IsPublic=true` cannot be accessed without authentication
- R2 endpoint URL (`https://xxx.r2.cloudflarestorage.com/bucket/file`) requires AWS signature

**Root Cause:**
The R2 endpoint URL (`https://account-id.r2.cloudflarestorage.com`) is an **API endpoint that requires AWS signature authentication**, not a public CDN URL. Even files in a "public" bucket cannot be accessed without credentials through this endpoint.

**Solution Options:**

**Option 1: Enable R2 Public Access (Recommended)**
1. Go to Cloudflare Dashboard → R2 → Your Bucket
2. Click "Settings" tab
3. Enable "Public Access" 
4. Copy the public URL (e.g., `https://pub-xxx.r2.dev`)
5. Add to Doppler secrets:
   ```bash
   APPBLUEPRINT_CLOUDFLARE_R2_PUBLICDOMAIN=https://pub-aa55877c270843248ef5a0c14e5def58.r2.dev
   ```

**Option 2: Use Anonymous API Endpoint**
Files can be accessed via the anonymous API endpoint without R2 public access:
```
GET /api/v1/filestorage/public/{fileKey}
```

This endpoint:
- Decorated with `[AllowAnonymous]` - no authentication required
- Verifies file has `IsPublic=true` before serving
- Downloads file from R2 and streams through API

**Implementation Details:**
- Added `DownloadPublicAsync` method to `IFileStorageService`
- Added anonymous endpoint in `FileStorageController`
- Updated `GetPublicUrl` to support API-based URLs when R2 public domain not configured

**Verification:**
```bash
# Test anonymous access (no auth token required)
curl https://your-api/api/v1/filestorage/public/tenant_xxx/file.png
```

---

### 401 Unauthorized on API Endpoints

**Problem:** File storage API returns 401 Unauthorized.

**Symptoms:**
- Upload/list/download all return 401
- Browser shows "Failed to fetch" in console
- Network tab shows 401 response

**Root Cause:**
Multiple possible causes:
1. Missing `resource` parameter (opaque token)
2. Token not forwarded in API requests
3. JWT Bearer not configured correctly

**Solution:**

**Ensure token is forwarded in HttpClient:**
```csharp
public sealed class AuthenticationDelegatingHandler(
    IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is not null)
        {
            string? token = await httpContext.GetTokenAsync("access_token");
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = 
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }
        return await base.SendAsync(request, cancellationToken);
    }
}
```

**Register the handler:**
```csharp
services.AddHttpClient<FileStorageService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7001");
})
.AddHttpMessageHandler<AuthenticationDelegatingHandler>();
```

---

### Tenant Resolution Failures

**Problem:** API returns 400 Bad Request with "TenantId is required".

**Symptoms:**
- Authentication succeeds
- Tenant lookup returns null
- `HttpContext.Items["TenantId"]` is empty

**Root Cause:**
Tenant resolution was looking up by email instead of external auth ID.

**Wrong approach:**
```csharp
// ❌ Wrong - Email might not match
var email = User.FindFirst(ClaimTypes.Email)?.Value;
var tenant = await db.Tenants.FirstOrDefaultAsync(t => t.Email == email);
```

**Correct approach:**
```csharp
// ✅ Correct - Use Logto's subject claim (external auth ID)
var externalAuthId = User.FindFirst("sub")?.Value;
var user = await db.Users.FirstOrDefaultAsync(u => u.ExternalAuthId == externalAuthId);
var tenant = user?.Tenant;
```

**Also ensure TenantContextAccessor reads from HttpContext.Items:**
```csharp
public sealed class TenantContextAccessor(IHttpContextAccessor httpContextAccessor) 
    : ITenantContextAccessor
{
    public string? TenantId
    {
        get
        {
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext?.Items.TryGetValue("TenantId", out var tenantId) == true)
            {
                return tenantId?.ToString();
            }
            return null;
        }
    }
}
```

---

### FileMetadata Table Missing (42P01)

**Problem:** `42P01: relation "FileMetadata" does not exist`

**Symptoms:**
- File upload fails with PostgreSQL error
- Error mentions `"FileMetadata"` with quotes

**Root Cause:**
PostgreSQL table naming case sensitivity:
- EF Core generates quoted identifiers: `"FileMetadata"`
- If table was created as `filemetadata` (lowercase), queries fail
- PostgreSQL treats quoted identifiers as case-sensitive

**Verification:**
```sql
-- Check what tables exist
SELECT table_name FROM information_schema.tables 
WHERE table_schema = 'public';

-- You might see: filemetadata (wrong)
-- Should be: FileMetadata (correct, if using quoted identifiers)
```

**Solution:**

**Option 1: Recreate table with correct case**
```sql
DROP TABLE IF EXISTS filemetadata;

CREATE TABLE "FileMetadata" (
    "Id" TEXT PRIMARY KEY,
    "TenantId" TEXT NOT NULL,
    "FileKey" TEXT NOT NULL,
    -- ... other columns
);
```

**Option 2: Configure EF Core to use lowercase**
```csharp
// In OnModelCreating
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Use snake_case for PostgreSQL convention
    modelBuilder.Entity<FileMetadataEntity>()
        .ToTable("file_metadata");
}
```

**Option 3: Run migrations properly**
```bash
dotnet ef database update --project AppBlueprint.Infrastructure
```

---

### JSONB Serialization Error

**Problem:** `Type 'Dictionary`2' required dynamic JSON serialization, which requires an explicit opt-in`

**Symptoms:**
- File upload fails after table exists
- Error occurs when saving `CustomMetadata` column
- Stack trace mentions `NpgsqlParameter` and JSON serialization

**Root Cause:**
Npgsql 8.0+ requires explicit opt-in for dynamic JSONB serialization of complex types like `Dictionary<string, string>`.

**Wrong approach:**
```csharp
// ❌ Missing EnableDynamicJson
services.AddDbContext<BaselineDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});
```

**Solution:**
Use `NpgsqlDataSourceBuilder` with `EnableDynamicJson()`:

```csharp
// ✅ Correct - Enable dynamic JSON for JSONB columns
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.EnableDynamicJson(); // Required for Dictionary<string, string> JSONB
var dataSource = dataSourceBuilder.Build();

services.AddDbContext<BaselineDbContext>(options =>
{
    options.UseNpgsql(dataSource, npgsqlOptions =>
    {
        npgsqlOptions.MigrationsAssembly("AppBlueprint.Infrastructure");
    });
});
```

**Files that needed changes:**
- `ServiceCollectionExtensions.cs` - For legacy DbContext registration
- `DbContextConfigurator.cs` - For `ConfigureNpgsqlOptions` method

---

### Query Filter Bug

**Problem:** ListFiles returns empty even after successful upload.

**Symptoms:**
- Upload succeeds (file appears in R2)
- Database row exists (verified via psql)
- API returns empty array

**Root Cause:**
Incorrect query filter in `FileMetadataConfiguration.cs`:

```csharp
// ❌ WRONG - This filters for TenantId IS NULL (excludes all real data)
builder.HasQueryFilter(f => EF.Property<string>(f, "TenantId") == null);
```

**Solution:**
```csharp
// ✅ CORRECT - Only filter out soft-deleted records
builder.HasQueryFilter(f => !f.IsSoftDeleted);
```

**Explanation:**
- The original filter was checking if `TenantId` equals `null`
- Since all uploaded files have a valid TenantId, they were ALL filtered out
- Changed to filter by `IsSoftDeleted` which is the intended behavior

---

### UI Buttons Not Working

**Problem:** Copy URL, Download, and Delete buttons don't work.

**Symptoms:**
- Buttons are visible
- Clicking does nothing
- No errors in console

**Root Cause:**
Missing JavaScript Interop implementation:
1. `CopyToClipboard` only logged, didn't use `navigator.clipboard`
2. `DownloadFile` downloaded to memory, didn't trigger browser download
3. `DeleteFile` worked but had no user feedback

**Solution:**

**Add IJSRuntime injection:**
```razor
@using Microsoft.JSInterop
@inject IJSRuntime JSRuntime
```

**Implement CopyToClipboard:**
```csharp
private async Task CopyToClipboard(string text)
{
    try
    {
        await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
        _actionMessage = "✓ URL copied to clipboard!";
        _actionSuccess = true;
        StateHasChanged();
        
        await Task.Delay(3000);
        _actionMessage = null;
        StateHasChanged();
    }
    catch (Exception ex)
    {
        Logger.LogError(ex, "Failed to copy to clipboard");
        _actionMessage = "✗ Failed to copy to clipboard";
        _actionSuccess = false;
        StateHasChanged();
    }
}
```

**Implement DownloadFile with browser download:**
```csharp
private async Task DownloadFile(string fileKey, string fileName)
{
    Stream? stream = await FileStorage.DownloadFileAsync(fileKey);
    if (stream is not null)
    {
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        byte[] bytes = memoryStream.ToArray();
        string base64 = Convert.ToBase64String(bytes);
        string mimeType = GetMimeType(fileName);
        
        await JSRuntime.InvokeVoidAsync("eval", $@"
            (function() {{
                const link = document.createElement('a');
                link.href = 'data:{mimeType};base64,{base64}';
                link.download = '{fileName}';
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
            }})();
        ");
    }
}
```

**Add user feedback toast:**
```razor
@if (!string.IsNullOrEmpty(_actionMessage))
{
    <div class="fixed bottom-4 right-4 z-50 p-4 rounded-lg shadow-lg 
                @(_actionSuccess ? "bg-green-100 text-green-800" : "bg-red-100 text-red-800")">
        @_actionMessage
    </div>
}
```

---

## Database Migration Issues

### Table Case Sensitivity in PostgreSQL

**Key Learning:**
PostgreSQL handles identifiers differently based on quoting:

| SQL | Interpreted As | Notes |
|-----|---------------|-------|
| `SELECT * FROM FileMetadata` | `filemetadata` | Unquoted = lowercase |
| `SELECT * FROM "FileMetadata"` | `FileMetadata` | Quoted = exact case |
| `SELECT * FROM FILEMETADATA` | `filemetadata` | Unquoted = lowercase |


**EF Core Default Behavior:**
EF Core quotes all identifiers, so it generates:
```sql
SELECT "f"."Id", "f"."TenantId" FROM "FileMetadata" AS "f"
```

**Implications:**
- If table was created manually as `filemetadata`, EF Core queries fail
- Always use migrations or ensure manual SQL matches EF Core's expectations
- Consider using `ToTable("file_metadata")` for PostgreSQL conventions

---

### Npgsql 8.0+ Dynamic JSON Requirement

**Breaking Change in Npgsql 8.0:**
Dynamic JSON serialization (for types like `Dictionary<string, string>` in JSONB columns) requires explicit opt-in.

**Before Npgsql 8.0:**
```csharp
// This just worked
options.UseNpgsql(connectionString);
```

**Npgsql 8.0+:**
```csharp
// Must use NpgsqlDataSourceBuilder
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.EnableDynamicJson();
var dataSource = dataSourceBuilder.Build();

options.UseNpgsql(dataSource);
```

**Why This Change:**
- Performance: Avoids reflection-based serialization overhead
- Security: Explicit opt-in for potentially dangerous dynamic types
- Clarity: Makes it clear which types need JSON handling

**Affected Entity Properties:**
```csharp
public sealed class FileMetadataEntity
{
    // This property requires EnableDynamicJson()
    public Dictionary<string, string>? CustomMetadata { get; set; }
}
```

---

## Quick Debugging Checklist

### Authentication Issues
- [ ] Check if token is JWT or opaque (three dots vs short string)
- [ ] Verify `resource` parameter is set in OIDC config
- [ ] Confirm API Resource exists in Logto Console
- [ ] Check token has correct `aud` (audience) claim
- [ ] Verify user has roles/permissions assigned

### File Storage Issues
- [ ] Check 401 → Token issue (see authentication)
- [ ] Check 400 → Tenant resolution issue
- [ ] Check 42P01 → Table doesn't exist or wrong case
- [ ] Check JSONB error → Need `EnableDynamicJson()`
- [ ] Check empty results → Query filter issue

### Database Issues
- [ ] Run `dotnet ef database update` for migrations
- [ ] Check table exists with correct case in PostgreSQL
- [ ] Verify connection string is correct
- [ ] Check for query filters that might exclude data

---

## Environment Variables Reference

```bash
# Logto Configuration
LOGTO_ENDPOINT=https://your-tenant.logto.app
LOGTO_APPID=your-app-id
LOGTO_APPSECRET=your-app-secret
LOGTO_RESOURCE=https://api.appblueprint.dev

# Database
APPBLUEPRINT_RAILWAY_CONNECTIONSTRING=Host=...;Database=...;Username=...;Password=...

# Cloudflare R2
CLOUDFLARE_R2_ACCOUNTID=your-account-id
CLOUDFLARE_R2_ACCESSKEYID=your-access-key
CLOUDFLARE_R2_SECRETACCESSKEY=your-secret-key
CLOUDFLARE_R2_PRIVATEBUCKETNAME=your-private-bucket
CLOUDFLARE_R2_PUBLICBUCKETNAME=your-public-bucket
```

---

## Related Documentation

- [Logto OIDC Documentation](https://docs.logto.io/docs/references/openid-connect/)
- [Logto API Resources](https://docs.logto.io/docs/references/resources/)
- [Npgsql 8.0 Breaking Changes](https://www.npgsql.org/doc/release-notes/8.0.html)
- [EF Core PostgreSQL](https://www.npgsql.org/efcore/)
- [PostgreSQL Identifier Case Sensitivity](https://www.postgresql.org/docs/current/sql-syntax-lexical.html#SQL-SYNTAX-IDENTIFIERS)

---

*Last Updated: February 2, 2026*
