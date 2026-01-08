# EF Core Migrations

This directory contains Entity Framework Core database migrations for the ApplicationDbContext.

## Important Migrations

### 20260107120000_AddSignupStoredProcedure.cs

**Added:** January 7, 2026

This migration includes the secure signup stored procedure (`create_tenant_and_user`) and related database objects:

- ✅ `SignupAuditLog` table for tracking signup attempts
- ✅ `create_tenant_and_user()` function (SECURITY DEFINER)
- ✅ `validate_id_format()` validation function
- ✅ `validate_email_format()` validation function
- ✅ `email_exists()` helper function
- ✅ Rate limiting indexes

**Why this matters:**
- Stored procedure is now deployed automatically with migrations
- No manual SQL script execution required
- Version controlled alongside schema changes

## Applying Migrations

**Production:**
```bash
cd /path/to/AppBlueprint.Infrastructure
dotnet ef database update --context ApplicationDbContext
```

**Development (with AppHost running):**
Migrations are applied automatically on startup when using .NET Aspire AppHost.

## Rolling Back

To rollback the signup stored procedure migration:
```bash
dotnet ef database update 20260103100029_AddExternalAuthIdToUser --context ApplicationDbContext
```

This will:
- Drop `create_tenant_and_user` function
- Drop validation functions
- Drop `SignupAuditLog` table

## Creating New Migrations

```bash
dotnet ef migrations add YourMigrationName --context ApplicationDbContext --output-dir Migrations
```

## Verifying Migration Status

```bash
dotnet ef migrations list --context ApplicationDbContext
```
