# Signup Flow Troubleshooting Guide

## Overview
This document details the fixes implemented for the signup flow, database stored procedure issues, and multi-tenancy architecture.

**Date Created:** 2026-01-03  
**Status:** ✅ Resolved  
**Related Systems:** Logto Authentication, PostgreSQL Row-Level Security, Blazor Server

---

## Problem Summary

Users attempting to sign up encountered multiple progressive errors:

1. **Function doesn't exist:** `create_tenant_and_user` stored procedure not installed ✅ FIXED
2. **NULL constraint violation:** `SignupAuditLog.Id` was NULL ✅ FIXED
3. **Invalid ID format:** Validation function expected ULID, received custom timestamp format ✅ FIXED (relaxed validation)
4. **Table not found:** Stored procedure referenced `Profiles` instead of `ProfileEntity` ✅ FIXED
5. **Schema mismatch:** Stored procedure assumed `Users.ProfileId` FK, actual schema uses `ProfileEntity.UserId` FK ✅ FIXED
6. **403 on dashboard:** Missing tenant_id claim after successful signup ⚠️ WORKAROUND (requires Logto API Resource configuration)
7. **TenantType always 0:** Both personal and business signups create Personal (0) tenants instead of Organization (1) ✅ FIXED

---

## Architecture Context

### Why Signup Bypasses API Service

**Design Decision:** The signup flow intentionally bypasses the API service and uses a direct database connection with a `SECURITY DEFINER` stored procedure.

**Rationale:**
- **Row-Level Security (RLS):** Database enforces tenant isolation via RLS policies requiring `current_setting('app.current_tenant_id')`
- **Chicken-and-Egg Problem:** Cannot set tenant context before tenant exists
- **Security:** Using `SECURITY DEFINER` on stored procedure allows bypassing RLS only for initial account creation
- **Atomicity:** Entire signup operation (Tenant → User → Profile → Audit) happens in single database transaction

**Trade-offs:**
- ✅ **Pro:** Secure, atomic operation without disabling RLS globally
- ✅ **Pro:** Prevents race conditions during tenant/user creation
- ❌ **Con:** API service doesn't show traces during signup (expected behavior)
- ❌ **Con:** Stored procedure logic must be maintained separately from EF Core migrations

**After Signup:** Normal operations use API service with JWT Bearer tokens containing tenant_id claim, API service sets RLS session variable, all queries are properly tenant-isolated.

---

## Fixes Implemented

### Fix 1: Install Stored Procedure

**Problem:** Function `create_tenant_and_user` did not exist in database.

**Root Cause:** ~~Stored procedure not part of EF Core migrations, must be installed manually.~~ ✅ **FIXED: Now part of migrations**

**Solution (Updated - January 2026):**

The stored procedure is now included in EF Core migration `20260107120000_AddSignupStoredProcedure.cs`.

**Automatic deployment via migrations:**
```bash
cd /Code/AppBlueprint/Shared-Modules/AppBlueprint.Infrastructure
dotnet ef database update --context ApplicationDbContext
```

**Manual deployment (legacy - if needed):**
```bash
cd /Code/AppBlueprint
docker run --rm -i \
  -e PGPASSWORD="<password>" \
  postgres:17 psql \
  -h switchyard.proxy.rlwy.net \
  -p 58225 \
  -U postgres \
  -d appblueprintdb \
  < CreateSignupStoredProcedure.sql
```

**File:** `CreateSignupStoredProcedure.sql`  
**Key Components:**
- `validate_email_format()` - Email validation
- `validate_id_format()` - ID format validation
- `validate_not_empty()` - String validation
- `validate_tenant_unique()` - Duplicate check
- `create_tenant_and_user()` - Main SECURITY DEFINER function
- `SignupAuditLog` table creation with audit trail

**Verification:**
```sql
SELECT routine_name, routine_type, security_type 
FROM information_schema.routines 
WHERE routine_schema = 'public' 
AND routine_name LIKE '%tenant%';
```

---

### Fix 2: NULL Audit ID (gen_ulid Issue)

**Problem:** `NULL value in column "Id" of relation "SignupAuditLog" violates not-null constraint`

**Root Cause:** Stored procedure called `gen_ulid()` function which doesn't exist in PostgreSQL (requires ULID extension).

**Original Code (Line 114):**
```sql
v_audit_id := 'audit_' || gen_ulid(); -- ❌ Function doesn't exist
```

**Fix (Line 114):**
```sql
v_audit_id := 'audit_' || REPLACE(gen_random_uuid()::text, '-', '');
```

**Rationale:**
- PostgreSQL's built-in `gen_random_uuid()` generates RFC 4122 UUIDs
- Remove hyphens to match prefixed ID format: `audit_<32_hex_chars>`
- Same fix applied to `v_profile_id` (Line 204)

**Trade-off:** Not a true ULID (no timestamp component), but sufficient for audit log uniqueness.

---

### Fix 3: Invalid tenant_id Format

**Problem:** `Invalid tenant_id format. Expected: tenant_XXXXXXXXXXXXXXXXXXXX`

**Root Cause:** Custom `PrefixedUlid.Generate()` produces format `tenant_18D5F3A8A5C_1234567890` (timestamp in hex + random), not standard ULID format `tenant_01ABCDEFGHIJKLMNOPQRSTUV` (Base32 encoded).

**Original Validation (Lines 49-62):**
```sql
IF NOT p_id ~ ('^' || p_prefix || '_[0-9A-Z]{26}$') THEN
    RAISE EXCEPTION 'Invalid % format. Expected: %_XXXXXXXXXXXXXXXXXXXX', p_prefix, p_prefix;
END IF;
```

**Fix (Lines 49-62):**
```sql
-- Relaxed validation to accept custom timestamp-based format
-- Format: prefix_TIMESTAMP_RANDOM (e.g., tenant_18D5F3A8A5C_1234567890)
IF NOT p_id ~ ('^' || p_prefix || '_[0-9A-Za-z_]+$') THEN
    RAISE EXCEPTION 'Invalid % format. Expected: %_<alphanumeric>', p_prefix, p_prefix;
END IF;

-- Minimum length check (prefix + underscore + at least 10 chars)
IF LENGTH(p_id) < LENGTH(p_prefix) + 11 THEN
    RAISE EXCEPTION '% is too short', p_prefix;
END IF;
```

**PrefixedUlid Implementation:**
```csharp
// AppBlueprint.SharedKernel/PrefixedUlid.cs
public static string Generate(string prefix)
{
    long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    string random = Guid.NewGuid().ToString("N")[..10]; // First 10 chars of GUID
    return $"{prefix}_{timestamp:X}_{random}"; // Hex timestamp
}
```

**Example Output:** `tenant_18D5F3A8A5C_1234567890`

---

### Fix 4: Table Name - Profiles vs ProfileEntity

**Problem:** `relation "Profiles" does not exist`

**Root Cause:** EF Core entity is named `ProfileEntity`, not `Profiles`.

**Database Schema Verification:**
```sql
\dt "ProfileEntity"
-- Confirms table exists as ProfileEntity, not Profiles
```

**Fix:** Changed all references from `"Profiles"` to `"ProfileEntity"` in stored procedure (Lines 222-255).

---

### Fix 5: User/Profile Relationship Correction

**Problem:** Stored procedure attempted to insert `ProfileId` into `Users` table, but column doesn't exist.

**Root Cause:** Database schema defines **one-to-one** relationship:
- `Users.Id` (PK, varchar(40))
- `ProfileEntity.UserId` (FK → Users.Id, unique, varchar(1024))

**Incorrect Assumption:**
```
❌ User → ProfileId (FK) → ProfileEntity
```

**Actual Schema:**
```
✅ ProfileEntity → UserId (FK) → User
```

**Database Verification:**
```sql
\d "Users"
-- Columns: Id, FirstName, LastName, UserName, Email, TenantId, ExternalAuthId, IsActive, IsSoftDeleted, LastLogin, CreatedAt, LastUpdatedAt
-- NO ProfileId column!

\d "ProfileEntity"  
-- Columns: Id, UserId (FK to Users), PhoneNumber, Bio, AvatarUrl, ...
-- UserId is UNIQUE constraint (one-to-one)
```

**Original Code (Lines 196-255):**
```sql
-- STEP 5: Create Profile (WRONG ORDER)
INSERT INTO "Profiles" (...) VALUES (v_profile_id, ...);

-- STEP 6: Create User (WITH NON-EXISTENT COLUMN)
INSERT INTO "Users" (..., "ProfileId", ...) 
VALUES (..., v_profile_id, ...); -- ❌ ProfileId doesn't exist in Users
```

**Fix (Lines 196-255):**
```sql
-- STEP 5: Create User FIRST (without ProfileId reference)
INSERT INTO "Users" (
    "Id",
    "FirstName",
    "LastName",
    "UserName",
    "Email",
    "TenantId",
    "ExternalAuthId",
    "IsActive",
    "IsSoftDeleted",
    "CreatedAt",
    "LastUpdatedAt"
) VALUES (
    p_user_id,
    p_user_first_name,
    p_user_last_name,
    p_user_email, -- UserName = Email for simplicity
    p_user_email,
    p_tenant_id,
    p_external_auth_id,
    true,  -- IsActive
    false, -- IsSoftDeleted
    NOW(),
    NOW()
);

-- STEP 6: Create ProfileEntity SECOND (with UserId FK)
INSERT INTO "ProfileEntity" (
    "Id",
    "UserId", -- ✅ FK to Users.Id
    "PhoneNumber",
    "Bio",
    "AvatarUrl",
    "WebsiteUrl",
    "TimeZone",
    "Language",
    "Country",
    "IsSoftDeleted",
    "CreatedAt",
    "LastUpdatedAt"
) VALUES (
    v_profile_id,
    p_user_id, -- ✅ References User.Id
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    false,
    NOW(),
    NOW()
);
```

**Key Changes:**
1. User created first (no ProfileId column)
2. ProfileEntity created second with `UserId` FK referencing User
3. Added `IsSoftDeleted` column (exists in actual schema)

---

### Fix 6: TenantType Always 0 (Personal) - ✅ FIXED

**Problem:** Both personal and business signups create tenants with `TenantType = 0` (Personal).

**Status:** ✅ **RESOLVED** - All components correctly implemented

**Solution Implemented:**

**1. CreateSignupStoredProcedure.sql:**
```sql
-- Line 99: Parameter accepts tenant type
CREATE OR REPLACE FUNCTION create_tenant_and_user(
    p_tenant_type INTEGER,  -- 0 = Personal (B2C), 1 = Organization (B2B)
    ...
)

-- Line 185: Uses parameter value
v_tenant_type := p_tenant_type;

-- Line 199: Stores in Tenants table
INSERT INTO "Tenants" (..., "TenantType", ...)
VALUES (..., v_tenant_type, ...);
```

**2. SignupService.cs:**
```csharp
// Line 71: Passes TenantType to stored procedure
FormattableString sql = $@"
    SELECT create_tenant_and_user(
        {request.TenantId},
        {request.TenantName},
        {request.TenantType},  -- ✅ Included
        ...
    )";

// SignupRequest includes TenantType
public sealed record SignupRequest
{
    public required int TenantType { get; init; }  // ✅ Present
    ...
}
```

**3. Onboarding.razor:**
```csharp
// Line 389: Personal signup
SignupResult result = await SignupService.CreateTenantAndUserAsync(new SignupRequest
{
    TenantType = 0 // ✅ Personal (B2C)
    ...
});

// Line 450: Business signup
SignupResult result = await SignupService.CreateTenantAndUserAsync(new SignupRequest
{
    TenantType = 1 // ✅ Organization (B2B)
    ...
});
```

**Verification:**
- Personal signups create `TenantType = 0` ✅
- Business signups create `TenantType = 1` ✅

**Test Query:**
```sql
SELECT "Id", "Name", "TenantType", "CreatedAt" 
FROM "Tenants" 
ORDER BY "CreatedAt" DESC;
```

---

## Database Schema Reference

### Tenants Table
```sql
CREATE TABLE "Tenants" (
    "Id" varchar(40) PRIMARY KEY,
    "Name" varchar(100) NOT NULL,
    "TenantType" integer NOT NULL DEFAULT 0, -- 0 = Personal, 1 = Organization
    "IsActive" boolean NOT NULL DEFAULT true,
    "IsPrimary" boolean NOT NULL DEFAULT false,
    "Email" varchar(255),
    "VatNumber" varchar(50),    -- B2B only
    "Country" varchar(100),     -- B2B only
    "Description" varchar(500), -- B2B only
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT NOW(),
    "LastUpdatedAt" timestamp with time zone NOT NULL DEFAULT NOW()
);
```

### Users Table
```sql
CREATE TABLE "Users" (
    "Id" varchar(40) PRIMARY KEY,
    "FirstName" varchar(100) NOT NULL,
    "LastName" varchar(100) NOT NULL,
    "UserName" varchar(256) NOT NULL UNIQUE,
    "Email" varchar(256) NOT NULL UNIQUE,
    "TenantId" varchar(40) NOT NULL REFERENCES "Tenants"("Id"),
    "ExternalAuthId" varchar(255), -- Logto 'sub' claim
    "IsActive" boolean NOT NULL DEFAULT true,
    "IsSoftDeleted" boolean NOT NULL DEFAULT false,
    "LastLogin" timestamp with time zone,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT NOW(),
    "LastUpdatedAt" timestamp with time zone NOT NULL DEFAULT NOW()
);
```

### ProfileEntity Table
```sql
CREATE TABLE "ProfileEntity" (
    "Id" varchar(1024) PRIMARY KEY,
    "UserId" varchar(1024) NOT NULL UNIQUE REFERENCES "Users"("Id"),
    "PhoneNumber" varchar(20),
    "Bio" text,
    "AvatarUrl" varchar(500),
    "WebsiteUrl" varchar(500),
    "TimeZone" varchar(100),
    "Language" varchar(10),
    "Country" varchar(100),
    "IsSoftDeleted" boolean NOT NULL DEFAULT false,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT NOW(),
    "LastUpdatedAt" timestamp with time zone NOT NULL DEFAULT NOW()
);
```

### SignupAuditLog Table
```sql
CREATE TABLE "SignupAuditLog" (
    "Id" text PRIMARY KEY,
    "TenantId" text,
    "UserId" text,
    "Email" text,
    "IpAddress" text,
    "UserAgent" text,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT NOW(),
    "Success" boolean NOT NULL,
    "ErrorMessage" text
);
```

---

## Testing & Verification

### Verify Stored Procedure Installation
```bash
docker run --rm -i -e PGPASSWORD="<password>" postgres:17 psql \
  -h switchyard.proxy.rlwy.net -p 58225 -U postgres -d appblueprintdb \
  -c "SELECT routine_name, security_type FROM information_schema.routines WHERE routine_name = 'create_tenant_and_user';"
```

**Expected Output:**
```
    routine_name      | security_type 
----------------------+---------------
 create_tenant_and_user | DEFINER
```

### Check Recent Signups
```sql
SELECT 
    "Id", 
    "Name", 
    "TenantType", 
    "Email", 
    "CreatedAt" 
FROM "Tenants" 
ORDER BY "CreatedAt" DESC 
LIMIT 5;
```

### Verify User Creation
```sql
SELECT 
    u."Id", 
    u."Email", 
    u."FirstName", 
    u."LastName", 
    u."TenantId",
    u."ExternalAuthId",
    u."CreatedAt"
FROM "Users" u
WHERE u."Email" = 'test@example.com';
```

### Check Profile Created
```sql
SELECT 
    p."Id",
    p."UserId",
    u."Email",
    p."CreatedAt"
FROM "ProfileEntity" p
JOIN "Users" u ON p."UserId" = u."Id"
WHERE u."Email" = 'test@example.com';
```

### Audit Log Verification
```sql
SELECT 
    "Id",
    "TenantId",
    "UserId",
    "Email",
    "Success",
    "ErrorMessage",
    "CreatedAt"
FROM "SignupAuditLog"
WHERE "Success" = true
ORDER BY "CreatedAt" DESC
LIMIT 5;
```

---

## Outstanding Issues

### 1. TenantType Not Set for Business Accounts

**Status:** ✅ **RESOLVED** - Implementation complete  
**Priority:** ~~HIGH~~ COMPLETED  
**Impact:** ~~All tenants created as Personal (0)~~ Both Personal and Business signups now create correct tenant types

**Implementation Confirmed:**

#### ✅ Step 1: SignupRequest Model
**File:** `AppBlueprint.Application/Services/SignupService.cs`

```csharp
public sealed record SignupRequest
{
    public required int TenantType { get; init; }  // ✅ Present
    // 0 = Personal, 1 = Organization
    ...
}
```

#### ✅ Step 2: Stored Procedure
**File:** `CreateSignupStoredProcedure.sql`

```sql
-- Line 99: Parameter exists
CREATE OR REPLACE FUNCTION create_tenant_and_user(
    p_tenant_id TEXT,
    p_tenant_name TEXT,
    p_tenant_type INTEGER,  -- ✅ Present
    ...
)

-- Line 185: Uses parameter value
v_tenant_type := p_tenant_type;  -- ✅ Correct

-- Line 199: Stores in database
INSERT INTO "Tenants" (..., "TenantType", ...)
VALUES (..., v_tenant_type, ...);  -- ✅ Correct
```

#### ✅ Step 3: SignupService Passes Parameter
**File:** `AppBlueprint.Application/Services/SignupService.cs` (Line 71)

```csharp
FormattableString sql = $@"
    SELECT create_tenant_and_user(
        {request.TenantId},
        {request.TenantName},
        {request.TenantType},  -- ✅ Included
        {request.UserId},
        ...
    )";
```

#### ✅ Step 4: Onboarding Pages Set Correct Values
**File:** `AppBlueprint.Web/Components/Pages/Auth/Onboarding.razor`

**Personal Account (Line 389):**
```csharp
SignupResult result = await SignupService.CreateTenantAndUserAsync(new SignupRequest
{
    TenantType = 0 // ✅ Personal (B2C)
    ...
});
```

**Business Account (Line 450):**
```csharp
SignupResult result = await SignupService.CreateTenantAndUserAsync(new SignupRequest
{
    TenantType = 1 // ✅ Organization (B2B)
    ...
});
```

**Verification Query:**
```sql
SELECT "Id", "Name", "TenantType", "Email", "CreatedAt" 
FROM "Tenants" 
WHERE "CreatedAt" > NOW() - INTERVAL '1 day'
ORDER BY "CreatedAt" DESC;
-- Expected: TenantType = 0 for personal, 1 for business

**Business Account (Line ~260):**
```csharp
SignupResult result = await SignupService.CreateTenantAndUserAsync(new SignupRequest
{
    TenantId = PrefixedUlid.Generate("tenant"),
    TenantName = sessionData.CompanyName,
    UserId = PrefixedUlid.Generate("user"),
    FirstName = sessionData.FirstName,
    LastName = sessionData.LastName,
    Email = email,
    ExternalAuthId = logtoUserId,
    IpAddress = ipAddress,
    UserAgent = userAgent,
    TenantType = 1 // ✅ Set Organization type for business
});
```

#### Step 5: Redeploy Stored Procedure
```bash
cd /Code/AppBlueprint
docker run --rm -i -e PGPASSWORD="<password>" postgres:17 psql \
```

---

### 2. Missing tenant_id Claim in JWT

**Status:** ⚠️ REQUIRES CONFIGURATION  
**Priority:** MEDIUM  
**Impact:** Dashboard may fail to load tenant-specific data after signup

**Issue:** JWT tokens from Logto don't include `tenant_id` custom claim.

**Solution:** Configure Logto API Resource with custom token claim.

**Steps:**
1. Log into Logto Admin Console
2. Navigate to **API Resources**
3. Create/Edit API Resource for `http://localhost:8091` (API service base URL)
4. Add custom claim script:
```javascript
const getUserTenantId = async (userId) => {
  // Query your database to get tenant_id for user
  const user = await fetch(`http://localhost:8091/api/users/${userId}`);
  return user.tenantId;
};

const getCustomJwtClaims = async ({ token, context, environmentVariables }) => {
  if (token.aud === 'http://localhost:8091') { // Your API Resource identifier
    const tenantId = await getUserTenantId(context.userId);
    return { tenant_id: tenantId };
  }
  return {};
};
```

5. Update Environment Variables in Web project:
```bash
# .env or launchSettings.json
Logto__Resource=http://localhost:8091
```

6. Test by decoding JWT at jwt.io - should see `tenant_id` claim

---

## Lessons Learned

### 1. Validate Database Schema Before Writing Stored Procedures
- **Issue:** Stored procedure referenced wrong table names and column structures
- **Lesson:** Always run `\d "TableName"` in psql to verify exact schema before implementing stored procedures
- **Tooling:** Consider generating stored procedure templates from EF Core model

### 2. Custom ID Generation Requires Custom Validation
- **Issue:** Validation function expected standard ULID, actual implementation was custom
- **Lesson:** Document ID format clearly, ensure validation matches implementation
- **Alternative:** Consider using pgcrypto extension with true ULID generation

### 3. Stored Procedures vs EF Core Migrations
- **Issue:** ~~Stored procedures not part of migration pipeline, manual deployment required~~ ✅ **FIXED**
- **Solution:** Added `20260107120000_AddSignupStoredProcedure.cs` migration that includes stored procedure SQL
- **Lesson:** Complex SQL objects (functions, triggers) should be added to migrations using `migrationBuilder.Sql()`
- **Trade-off:** Stored procedures provide performance and security benefits, now deployed automatically

### 4. Trust Boundaries Require Runtime Validation
- **Issue:** Assumed column existence without verification
- **Lesson:** Even with nullable reference types, validate external data sources (DB schema) at runtime
- **Best Practice:** Add guard clauses for critical operations

---

## Related Documentation

- **Multi-Tenancy:** `.github/.ai-rules/backend/multi-tenancy.md`
- **Tenant Isolation:** `.github/.ai-rules/backend/tenant-isolation-defense-in-depth.md`
- **Database Setup:** `AppBlueprint/DATABASE_HYBRID_MODE_SETUP.md`
- **Logto Setup:** `AppBlueprint/LOGTO-AUTHENTICATION-SETUP.md`
- **Row-Level Security:** `AppBlueprint/SetupRowLevelSecurity.sql`

---

## Future Improvements

1. ~~**Automate Stored Procedure Deployment:** Add to EF Core migration pipeline~~ ✅ **COMPLETED** - See migration `20260107120000_AddSignupStoredProcedure.cs`
2. **Switch to True ULIDs:** Install PostgreSQL ULID extension for standardized IDs
3. **Add Stored Procedure Tests:** Integration tests validating SQL logic
4. **Improve ID Column Consistency:** Standardize varchar lengths (currently varchar(40) vs varchar(1024))
5. **Rate Limiting Enforcement:** Currently commented out, should be enabled
6. **Add VAT/Country Fields:** Business signup captures but doesn't persist VAT number and country
7. **Tenant_id Claim Automation:** Auto-inject tenant_id into JWT during Logto callback

---

## Quick Reference Commands

### Connect to Database
```bash
docker run --rm -it -e PGPASSWORD="<password>" postgres:17 psql \
  -h switchyard.proxy.rlwy.net -p 58225 -U postgres -d appblueprintdb
```

### List All Tables
```sql
\dt
```

### Describe Table Structure
```sql
\d "TableName"
```

### Check Stored Procedures
```sql
SELECT routine_name, routine_type, security_type 
FROM information_schema.routines 
WHERE routine_schema = 'public';
```

### Drop and Recreate Stored Procedure
```bash
docker run --rm -i -e PGPASSWORD="<password>" postgres:17 psql \
  -h switchyard.proxy.rlwy.net -p 58225 -U postgres -d appblueprintdb \
  < CreateSignupStoredProcedure.sql
```

---

## Contact & Support

For questions or issues with signup flow:
1. Check recent `SignupAuditLog` entries for error messages
2. Review Web service logs for detailed stack traces
3. Verify stored procedure exists and has DEFINER security
4. Confirm database schema matches expected structure
5. Test Logto authentication flow separately

**Last Updated:** 2026-01-03  
**Version:** 1.0  
**Maintainers:** Development Team
