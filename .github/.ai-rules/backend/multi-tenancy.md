# Multi-Tenancy Rules

## Tenant Resolution - SECURITY CRITICAL

### JWT Claims: The ONLY Secure Source

**For ALL authenticated requests (B2C and B2B), tenant ID MUST be extracted from JWT claims.**

**Why JWT is the only secure source:**
- JWT tokens are cryptographically signed by the authentication provider
- Backend validates signature before trusting any claims
- Claims cannot be tampered with or forged by malicious clients

**Why other methods are insecure:**
- ❌ HTTP Headers - Can be forged by malicious clients
- ❌ Subdomains - Client controls the URL
- ❌ Path segments - Client controls the URL
- ❌ Query parameters - Client controls the URL

### Implementation Requirements

#### 1. TenantMiddleware MUST Extract from JWT

```csharp
// ✅ CORRECT - Always extract from validated JWT claims
if (context.User.Identity?.IsAuthenticated == true)
{
    string? tenantId = context.User.FindFirst("tenant_id")?.Value 
                       ?? context.User.FindFirst("tid")?.Value;
    
    if (string.IsNullOrEmpty(tenantId))
    {
        // Return 401 - tenant claim is REQUIRED
        return Results.Unauthorized();
    }
    
    context.Items["TenantId"] = tenantId;
}
else
{
    // Unauthenticated requests to tenant-scoped endpoints not allowed
    return Results.Unauthorized();
}
```

```csharp
// ❌ WRONG - Never trust headers for authenticated requests
string? tenantId = context.Request.Headers["tenant-id"].FirstOrDefault();
// This can be forged by malicious clients!
```

#### 2. JWT Claim Names

**Primary claim name:** `tenant_id`  
**Fallback claim name:** `tid`

Always check both in order. These must be configured in your authentication provider (Logto, Auth0, etc.) to include the tenant ID in issued JWT tokens.

#### 3. Fail-Fast Security

If tenant cannot be resolved from JWT claims:
- Return `401 Unauthorized` immediately
- Do NOT proceed with the request
- Do NOT use fallback methods (headers, subdomains, etc.)
- Log the security violation for audit

#### 4. Defense-in-Depth

Even with JWT-based tenant resolution, maintain PostgreSQL Row-Level Security (RLS):
- RLS policies enforce tenant isolation at database level
- Provides protection even if application code has bugs
- Based on `app.current_tenant_id()` session variable
#### RLS Health Check - CRITICAL STARTUP REQUIREMENT

**The application MUST NOT start if Row-Level Security is not properly configured.**

This is enforced by `RowLevelSecurityHealthCheck` which validates:
1. ✅ RLS functions exist (`set_current_tenant`, `get_current_tenant`)
2. ✅ RLS enabled on all tenant-scoped tables
3. ✅ `tenant_isolation_policy` exists on all tables
4. ❌ Application startup fails if any check fails

```csharp
// Registered in ServiceCollectionExtensions.AddHealthChecksServices()
healthChecksBuilder.AddCheck(
    "row-level-security",
    new RowLevelSecurityHealthCheck(connectionString, logger),
    failureStatus: HealthStatus.Unhealthy,
    tags: new[] { "db", "security", "rls", "critical" });
```

**Tables that MUST have RLS enabled:**
- Users
- Teams
- Organizations
- ContactPersons
- EmailAddresses
- PhoneNumbers
- Addresses
- Todos

**Setup Instructions:**
1. Apply all Entity Framework migrations
2. Run `SetupRowLevelSecurity.sql` script
3. Verify with health check: `GET /health` should return status 200
4. Application will refuse to start if RLS not configured

**Health Check Responses:**
- `Healthy` - RLS properly configured on all tables
- `Degraded` - Tables don't exist yet (pre-migration)
- `Unhealthy` - RLS missing or policies not configured (APPLICATION WILL NOT START)
### Pre-Authentication Scenarios (Exceptions)

**ONLY** before user authentication, other resolution methods may be used:

#### Login Page Routing
Use subdomain to determine which authentication provider configuration to use:

```csharp
// For unauthenticated /login requests only
if (!context.User.Identity?.IsAuthenticated == true && 
    requestPath.StartsWith("/login", StringComparison.OrdinalIgnoreCase))
{
    // Extract tenant from subdomain to determine auth provider
    var subdomain = GetSubdomainFromHost(context.Request.Host);
    // Use subdomain to select Logto/Auth0 configuration
}
```

#### Public Landing Pages
Use path segment or subdomain for branding before authentication:

```csharp
// For unauthenticated public pages only
if (!context.User.Identity?.IsAuthenticated == true && 
    IsPublicLandingPage(requestPath))
{
    // Extract tenant for branding purposes only
    // NOT for data access
}
```

**Critical:** These methods are ONLY for UI routing/branding. Never use them to authorize access to tenant-scoped data.

### Admin Tenant Switching (B2B)

For admin users who can access multiple tenants:

```csharp
// 1. JWT must contain list of accessible tenants
var accessibleTenants = context.User
    .FindAll("accessible_tenants")
    .Select(c => c.Value)
    .ToList();

// 2. Extract current tenant from JWT (not from header!)
string requestedTenant = context.User.FindFirst("current_tenant")?.Value 
                         ?? context.User.FindFirst("tenant_id")?.Value;

// 3. Validate user has access
if (!accessibleTenants.Contains(requestedTenant))
{
    return Results.Forbidden(); // User can't access this tenant
}

// 4. Store validated tenant
context.Items["TenantId"] = requestedTenant;
```

**Never allow client to specify tenant via header** - this would allow privilege escalation.

### MultiTenancyOptions Configuration

The `ResolutionStrategy` enum exists for pre-authentication scenarios only:

```jsonc
{
  "MultiTenancy": {
    "ResolutionStrategy": "JwtClaim",  // For authenticated requests (enforced)
    "TenantClaimName": "tenant_id",    // Primary JWT claim name
    "EnableRowLevelSecurity": true,     // Always enabled
    "ValidateTenantExists": true        // Validate tenant in database
  }
}
```

**Important:** Regardless of `ResolutionStrategy` setting, authenticated requests ALWAYS use JWT claims. The setting only affects pre-authentication scenarios.

### Excluded Paths

These paths bypass tenant validation (no authentication required):

```csharp
private static readonly string[] ExcludedPaths = [
    "/swagger",        // API documentation
    "/openapi",        // OpenAPI spec
    "/health",         // Health checks
    "/metrics",        // Observability
    "/_framework",     // Blazor framework files
    "/favicon.ico",    // Static files
    "/api/system"      // System endpoints
];
```

### Testing Requirements

#### Unit Tests

Test that TenantMiddleware enforces JWT-based tenant resolution:

```csharp
[Test]
public async Task Invoke_AuthenticatedUser_ExtractsTenantFromJWT()
{
    // Arrange
    var context = CreateHttpContextWithJwtClaim("tenant_id", "acme-corp");
    
    // Act
    await _middleware.Invoke(context);
    
    // Assert
    await Assert.That(context.Items["TenantId"]).IsEqualTo("acme-corp");
}

[Test]
public async Task Invoke_AuthenticatedUserWithoutTenantClaim_Returns401()
{
    // Arrange
    var context = CreateHttpContextWithJwtClaim("sub", "user-123"); // No tenant_id
    
    // Act
    await _middleware.Invoke(context);
    
    // Assert
    await Assert.That(context.Response.StatusCode).IsEqualTo(401);
}

[Test]
public async Task Invoke_HeaderProvidedButNotInJWT_Returns401()
{
    // Arrange - malicious client sends forged header
    var context = CreateHttpContextWithJwtClaim("sub", "user-123");
    context.Request.Headers["tenant-id"] = "malicious-tenant";
    
    // Act
    await _middleware.Invoke(context);
    
    // Assert - header must be ignored, only JWT claims trusted
    await Assert.That(context.Response.StatusCode).IsEqualTo(401);
}
```

#### Integration Tests

Test end-to-end tenant isolation with real JWT tokens:

```csharp
[Test]
public async Task GetTodos_UserFromTenantA_OnlySeesTheirData()
{
    // Arrange
    var jwtToken = GenerateJwtToken(userId: "user-1", tenantId: "tenant-a");
    _httpClient.DefaultRequestHeaders.Authorization = 
        new AuthenticationHeaderValue("Bearer", jwtToken);
    
    // Act
    var response = await _httpClient.GetAsync("/api/todos");
    var todos = await response.Content.ReadFromJsonAsync<List<Todo>>();
    
    // Assert
    await Assert.That(todos).IsNotNull();
    await Assert.That(todos!.All(t => t.TenantId == "tenant-a")).IsTrue();
}
```

### Common Mistakes to Avoid

❌ **Never extract tenant from HTTP headers for authenticated requests**
```csharp
// WRONG - security vulnerability!
var tenantId = context.Request.Headers["tenant-id"];
```

❌ **Never allow client to choose tenant**
```csharp
// WRONG - allows privilege escalation!
var tenantId = context.Request.Query["tenant"];
```

❌ **Never trust subdomain for data access**
```csharp
// WRONG - client controls URL!
var tenantId = GetSubdomainFromHost(context.Request.Host);
```

✅ **Always extract from validated JWT claims**
```csharp
// CORRECT - cryptographically verified
var tenantId = context.User.FindFirst("tenant_id")?.Value;
```

### Summary

**Golden Rule:** For authenticated requests, tenant ID MUST come from JWT claims. No exceptions.

**Security principle:** Never trust client-provided data (headers, URLs, query params) for tenant resolution. Only trust cryptographically signed JWT tokens.

**Defense-in-depth:** Even with JWT validation, maintain PostgreSQL Row-Level Security for additional protection.
