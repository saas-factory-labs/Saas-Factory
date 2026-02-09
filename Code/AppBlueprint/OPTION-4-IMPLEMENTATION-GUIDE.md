# Option 4 Implementation Guide: Secure Signup with Stored Procedure

## Overview

This document outlines **Option 4: Stored Procedure with SECURITY DEFINER** for fixing the RLS signup issue. This approach uses a PostgreSQL stored procedure that runs with elevated privileges to safely create tenant and user during signup, without requiring NULL RLS bypass.

---

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    Signup Flow (Option 4)                   │
└─────────────────────────────────────────────────────────────┘

1. User authenticates with Logto
2. SignupComplete.razor gets JWT token
3. C# SignupService.CreateTenantAndUserAsync() called
4. Stored procedure create_tenant_and_user() executes
   ├── Runs with SECURITY DEFINER (bypasses RLS)
   ├── Validates input (prevents SQL injection)
   ├── Creates tenant (INSERT INTO "Tenants")
   ├── Creates profile (INSERT INTO "Profiles")
   ├── Creates user (INSERT INTO "Users")
   ├── Logs to audit table (SignupAuditLog)
   └── Returns JSON result
5. Normal RLS policies remain STRICT (no NULL bypass needed)
6. All future operations use normal tenant context
```

---

## Security Analysis

### ✅ Advantages

1. **RLS Stays Strict**
   - No NULL bypass in RLS policies
   - Policies remain: `WHERE "TenantId" = current_setting('app.current_tenant_id')::TEXT`
   - All normal operations fully protected

2. **Surgical Precision**
   - Only signup operations have elevated privileges
   - Function can only be called from application code (not client)
   - Input validation prevents abuse

3. **Built-in Audit Trail**
   - Every signup attempt logged (successful + failed)
   - Includes IP address, user agent, timestamp
   - Security monitoring built-in

4. **Rate Limiting Ready**
   - Function includes rate limit check (5 signups/hour per email)
   - Prevents signup abuse/spam
   - Easy to adjust thresholds

5. **Atomic Transactions**
   - Tenant + Profile + User created or nothing
   - No orphaned data
   - Automatic rollback on errors

6. **Defense Against Common Attacks**
   - SQL injection: Parameterized queries only
   - Search path attacks: `SET search_path = public`
   - Replay attacks: Rate limiting + audit log
   - Mass signups: Email duplicate check + rate limit

### ⚠️ Risks & Mitigations

| Risk | Likelihood | Mitigation |
|------|-----------|------------|
| **Stored procedure exploited** | LOW | Input validation, parameterized calls, audit logging |
| **Direct database access** | LOW | Requires DB credentials (same risk as NULL bypass) |
| **Rate limit bypass** | LOW | Audit log tracks all attempts, easy to detect |
| **Email enumeration** | MEDIUM | Return generic error "Signup failed" (don't reveal "email exists") |


---

## Files Created

### 1. Database Schema: `CreateSignupStoredProcedure.sql`

**Location:** `/Code/AppBlueprint/CreateSignupStoredProcedure.sql`

**Components:**
- `SignupAuditLog` table - Audit trail for all signup attempts
- `validate_id_format()` - ULID format validation with prefix
- `validate_email_format()` - RFC 5322 email validation
- `email_exists()` - Duplicate email check (case-insensitive)
- `create_tenant_and_user()` - Main signup stored procedure

**Key Features:**
```sql
CREATE OR REPLACE FUNCTION create_tenant_and_user(...)
RETURNS JSON
SECURITY DEFINER  -- Elevated privileges
SET search_path = public  -- Prevent attacks
LANGUAGE plpgsql
AS $$
BEGIN
    -- Step 1: Validate inputs (prevent injection)
    -- Step 2: Rate limiting check
    -- Step 3: Create tenant (Personal type for B2C)
    -- Step 4: Create profile
    -- Step 5: Create user (with TenantId link)
    -- Step 6: Audit log success
    -- Step 7: Return JSON result
END;
$$;
```

**To Apply:**
```bash
psql -h localhost -U postgres -d your_database -f CreateSignupStoredProcedure.sql
```

---

### 2. C# Service: `SignupService.cs`

**Location:** `/Code/AppBlueprint/Shared-Modules/AppBlueprint.Application/Services/SignupService.cs`

**Interface:**
```csharp
public interface ISignupService
{
    Task<SignupResult> CreateTenantAndUserAsync(
        SignupRequest request,
        CancellationToken cancellationToken = default);
}
```

**Key Features:**
- Calls stored procedure via raw SQL (EF Core bypassed for this operation)
- Parameterized queries (prevent SQL injection)
- JSON result parsing
- Comprehensive error logging
- Returns `SignupResult` with tenant/user IDs

**Usage Example:**
```csharp
var result = await _signupService.CreateTenantAndUserAsync(new SignupRequest
{
    TenantId = "tenant_01HX...",
    TenantName = "John Doe",
    UserId = "user_01HX...",
    FirstName = "John",
    LastName = "Doe",
    Email = "john@example.com",
    ExternalAuthId = "logto-sub-12345",
    IpAddress = "192.168.1.1",
    UserAgent = "Mozilla/5.0..."
});
```

---

## Integration Steps

### Step 1: Apply Database Schema

```bash
cd /Code/AppBlueprint
psql -h localhost -U postgres -d appblueprint -f CreateSignupStoredProcedure.sql
```

**Verify:**
```sql
-- Check function was created
SELECT proname, prosecdef FROM pg_proc WHERE proname = 'create_tenant_and_user';

-- Check audit table exists
SELECT table_name FROM information_schema.tables WHERE table_name = 'SignupAuditLog';
```

---

### Step 2: Register SignupService in DI

Add to `Program.cs` or `ServiceCollectionExtensions.cs`:

```csharp
// Register signup service
builder.Services.AddScoped<ISignupService, SignupService>();
```

---

### Step 3: Update SignupComplete.razor

Replace the current two-step approach (CreateTenantAsync → CreateUserAsync) with single stored procedure call:

**Before:**
```csharp
// Step 1: Create tenant
var tenantResponse = await httpClient.PostAsJsonAsync("/api/v1/tenant", createTenantRequest);
var tenant = await tenantResponse.Content.ReadFromJsonAsync<TenantCreationResponse>();

// Step 2: Create user
var userResponse = await httpClient.PostAsJsonAsync("/api/v1/user", createUserRequest);
```

**After:**
```csharp
@inject ISignupService SignupService
@inject IHttpContextAccessor HttpContextAccessor

private async Task CreatePersonalAccountAsync(SignupSessionData sessionData, ClaimsPrincipal user)
{
    // Get user details from claims
    string logtoUserId = user.FindFirst("sub")?.Value ?? throw new Exception("No user ID");
    string email = user.FindFirst("email")?.Value ?? throw new Exception("No email");
    
    // Get IP address for audit
    string? ipAddress = HttpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
    string? userAgent = HttpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString();

    // Call secure stored procedure (single atomic operation)
    SignupResult result = await SignupService.CreateTenantAndUserAsync(new SignupRequest
    {
        TenantId = PrefixedUlid.Generate("tenant"),
        TenantName = $"{sessionData.FirstName} {sessionData.LastName}",
        UserId = PrefixedUlid.Generate("user"),
        FirstName = sessionData.FirstName,
        LastName = sessionData.LastName,
        Email = email,
        ExternalAuthId = logtoUserId,
        IpAddress = ipAddress,
        UserAgent = userAgent
    });

    Logger.LogInformation(
        "Signup successful - TenantId: {TenantId}, UserId: {UserId}",
        result.TenantId,
        result.UserId);
}
```

---

### Step 4: Update Entity Framework Migration (Optional)

Create a new EF Core migration to include the stored procedure:

```bash
cd /Code/AppBlueprint/Shared-Modules/AppBlueprint.Infrastructure
dotnet ef migrations add AddSecureSignupStoredProcedure
```

**In migration Up() method:**
```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    // Read SQL file and execute
    string sql = File.ReadAllText("CreateSignupStoredProcedure.sql");
    migrationBuilder.Sql(sql);
}

protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.Sql("DROP FUNCTION IF EXISTS create_tenant_and_user;");
    migrationBuilder.Sql("DROP TABLE IF EXISTS \"SignupAuditLog\";");
}
```

---

## Testing Plan

### 1. Unit Tests

**Test SignupService:**
```csharp
[Test]
public async Task CreateTenantAndUserAsync_ValidRequest_ReturnsSuccess()
{
    // Arrange
    var request = new SignupRequest
    {
        TenantId = "tenant_01HX...",
        TenantName = "Test User",
        UserId = "user_01HX...",
        FirstName = "Test",
        LastName = "User",
        Email = "test@example.com",
        ExternalAuthId = "logto-12345"
    };

    // Act
    SignupResult result = await _signupService.CreateTenantAndUserAsync(request);

    // Assert
    await Assert.That(result.Success).IsEqualTo(true);
    await Assert.That(result.TenantId).IsEqualTo(request.TenantId);
    await Assert.That(result.UserId).IsEqualTo(request.UserId);
}

[Test]
public async Task CreateTenantAndUserAsync_DuplicateEmail_ThrowsException()
{
    // Arrange - create user with email
    await CreateUserWithEmail("duplicate@example.com");
    
    var request = new SignupRequest { Email = "duplicate@example.com", ... };

    // Act & Assert
    await Assert.ThrowsAsync<InvalidOperationException>(
        async () => await _signupService.CreateTenantAndUserAsync(request));
}
```

### 2. Integration Tests

**Test Stored Procedure Directly:**
```sql
-- Test successful signup
SELECT create_tenant_and_user(
    'tenant_01HX1234567890ABCDEFGHIJ',
    'John Doe',
    'user_01HX1234567890ABCDEFGHIJ',
    'John',
    'Doe',
    'john@example.com',
    'logto-sub-12345',
    '192.168.1.1',
    'Mozilla/5.0'
);

-- Verify tenant created
SELECT * FROM "Tenants" WHERE "Id" = 'tenant_01HX1234567890ABCDEFGHIJ';

-- Verify user created with correct TenantId
SELECT * FROM "Users" WHERE "Id" = 'user_01HX1234567890ABCDEFGHIJ';

-- Verify audit log
SELECT * FROM "SignupAuditLog" WHERE "Email" = 'john@example.com';
```

**Test RLS Still Enforced:**
```sql
-- Set tenant context
SELECT set_config('app.current_tenant_id', 'tenant_other', FALSE);

-- Try to read user from different tenant (should fail)
SELECT * FROM "Users" WHERE "Id" = 'user_01HX1234567890ABCDEFGHIJ';
-- Expected: 0 rows (RLS blocks)
```

### 3. Security Tests

**Test Input Validation:**
```sql
-- Invalid tenant ID format
SELECT create_tenant_and_user('invalid-id', ...);
-- Expected: Exception "Invalid tenant_id format"

-- Invalid email
SELECT create_tenant_and_user(..., 'not-an-email', ...);
-- Expected: Exception "Invalid email format"

-- SQL injection attempt
SELECT create_tenant_and_user('tenant_01HX"; DROP TABLE "Users"; --', ...);
-- Expected: Exception "Invalid tenant_id format" (regex prevents injection)
```

**Test Rate Limiting:**
```bash
# Try 6 signups with same email rapidly
for i in {1..6}; do
  psql -c "SELECT create_tenant_and_user(..., 'test@example.com', ...);"
done
# Expected: First 5 succeed, 6th fails with "Rate limit exceeded"
```

### 4. End-to-End Test

```bash
# 1. Start application
cd /Code/AppBlueprint/AppBlueprint.AppHost
dotnet run

# 2. Navigate to signup page
open http://localhost:5000/signup

# 3. Fill in personal account form
# 4. Authenticate with Logto
# 5. Verify SignupComplete page shows success
# 6. Check logs for stored procedure call
# 7. Query database:

psql -d appblueprint -c "SELECT * FROM \"SignupAuditLog\" ORDER BY \"CreatedAt\" DESC LIMIT 1;"
psql -d appblueprint -c "SELECT * FROM \"Tenants\" ORDER BY \"CreatedAt\" DESC LIMIT 1;"
psql -d appblueprint -c "SELECT * FROM \"Users\" ORDER BY \"CreatedAt\" DESC LIMIT 1;"
```

---

## Monitoring & Operations

### Audit Log Queries

**View recent signups:**
```sql
SELECT 
    "Email",
    "Success",
    "CreatedAt",
    "IpAddress",
    "ErrorMessage"
FROM "SignupAuditLog"
ORDER BY "CreatedAt" DESC
LIMIT 50;
```

**Detect suspicious patterns:**
```sql
-- High failure rate from single IP
SELECT 
    "IpAddress",
    COUNT(*) as attempts,
    COUNT(*) FILTER (WHERE NOT "Success") as failures
FROM "SignupAuditLog"
WHERE "CreatedAt" > NOW() - INTERVAL '1 hour'
GROUP BY "IpAddress"
HAVING COUNT(*) FILTER (WHERE NOT "Success") > 10;

-- Multiple signups from same email
SELECT 
    "Email",
    COUNT(*) as attempts
FROM "SignupAuditLog"
WHERE "CreatedAt" > NOW() - INTERVAL '24 hours'
GROUP BY "Email"
HAVING COUNT(*) > 5;
```

### Performance Monitoring

**Stored procedure execution time:**
```sql
SELECT 
    query,
    mean_exec_time,
    calls
FROM pg_stat_statements
WHERE query LIKE '%create_tenant_and_user%'
ORDER BY mean_exec_time DESC;
```

---

## Rollback Plan

If issues arise, you can rollback to previous implementation:

```sql
-- Drop stored procedure
DROP FUNCTION IF EXISTS create_tenant_and_user;

-- Drop audit table (preserves data)
-- DROP TABLE IF EXISTS "SignupAuditLog";

-- Revert to NULL bypass (temporary)
ALTER POLICY tenant_isolation_write_policy ON "Users"
USING ("TenantId" = current_setting('app.current_tenant_id', TRUE)::TEXT 
       OR current_setting('app.current_tenant_id', TRUE) IS NULL);
```

---

## Comparison: Option 4 vs Option 1

| Factor | Option 1 (NULL Bypass) | Option 4 (Stored Proc) |
|--------|------------------------|------------------------|
| **RLS Policy Change** | Allows NULL context | No change (strict) |
| **Security** | ⚠️ Slight weakness | ✅ Strongest |
| **Code Complexity** | ⭐ Simple | ⭐⭐⭐ Moderate |
| **Audit Trail** | Manual logging needed | ✅ Built-in |

| **Rate Limiting** | Must implement separately | ✅ Built-in |
| **Attack Surface** | Larger (any NULL context) | Smaller (signup only) |
| **Maintenance** | Easy | Moderate (SQL + C#) |
| **Performance** | ⚡ Fast | ⚡⚡ Faster (single DB call) |
| **Supabase Compatible** | ❌ Unsafe | ✅ Safe |


---

## Conclusion

**Option 4 is the most secure approach** for fixing the RLS signup issue. It:
- Maintains strict RLS policies (no NULL bypass)
- Provides surgical precision (only signup operations elevated)
- Includes built-in security features (validation, rate limiting, audit)
- Offers atomic transactions (all-or-nothing semantics)
- Works safely in both backend-controlled and Supabase-like architectures

**Recommended for production use**, especially for applications with:
- Sensitive user data (dating apps, healthcare, finance)
- High security requirements
- Need for compliance audit trails (GDPR, HIPAA, SOC 2)
- Possibility of future Supabase migration

---

## Next Steps

1. ✅ Files created:
   - `CreateSignupStoredProcedure.sql` (database schema)
   - `SignupService.cs` (C# service)
   - This implementation guide

2. ⏭️ Pending actions:
   - Apply SQL schema to database
   - Register SignupService in DI container
   - Update SignupComplete.razor to use new service
   - Add IHttpContextAccessor injection for IP/UserAgent
   - Write unit and integration tests
   - Test end-to-end signup flow
   - Monitor SignupAuditLog for suspicious activity

Would you like me to proceed with updating SignupComplete.razor to use the new SignupService?
