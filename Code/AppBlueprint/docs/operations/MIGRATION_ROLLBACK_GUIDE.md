# Entity Framework Migration Rollback Guide

This guide covers how to safely rollback Entity Framework Core migrations in AppBlueprint projects.

## Table of Contents

1. [Understanding Migrations](#understanding-migrations)
2. [Rollback Scenarios](#rollback-scenarios)
3. [Development Environment Rollback](#development-environment-rollback)
4. [Production Environment Rollback](#production-environment-rollback)
5. [Safety Considerations](#safety-considerations)
6. [Troubleshooting](#troubleshooting)

## Understanding Migrations

Entity Framework Core migrations create database schema changes. Each migration consists of:

- **Up()** method: Applies schema changes
- **Down()** method: Reverts schema changes
- **Migration file**: Contains both Up and Down methods
- **Snapshot file**: Represents current model state

### Migration History Table

EF Core tracks applied migrations in `__EFMigrationsHistory` table with columns:
- `MigrationId`: Unique identifier (timestamp_MigrationName)
- `ProductVersion`: EF Core version that created the migration

## Rollback Scenarios

### Scenario 1: Remove Unapplied Migration (Development Only)

**When:** You created a migration but haven't applied it yet.

**Action:** Simply delete the migration file.

```powershell
# Remove the last migration file
dotnet ef migrations remove --project .\Code\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure
```

**What it does:**
- Deletes the migration file
- Reverts the model snapshot to previous state
- Does NOT modify the database (safe operation)

**⚠️ Warning:** Only use if migration hasn't been applied to ANY database (including teammates' databases).

### Scenario 2: Rollback to Previous Migration (Development)

**When:** You applied a migration to your local database and need to undo it.

**Action:** Update database to a previous migration.

```powershell
# Rollback to a specific migration (by name)
dotnet ef database update PreviousMigrationName --project .\Code\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure

# Rollback all migrations (reset database)
dotnet ef database update 0 --project .\Code\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure
```

**What it does:**
- Executes Down() method of migrations newer than target
- Removes entries from `__EFMigrationsHistory`
- Reverts database schema changes
- Does NOT delete migration files

### Scenario 3: Production Rollback (Controlled)

**When:** You deployed a migration to production and need to rollback.

**Action:** Generate SQL script and review before executing.

```powershell
# Generate rollback SQL script
dotnet ef migrations script CurrentMigration PreviousMigration --output rollback.sql --project .\Code\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure

# Review the rollback.sql file before executing
# Apply manually to production database
```

**⚠️ Critical:** Always review the generated SQL script before applying to production.

## Development Environment Rollback

### Quick Rollback (Local Database Only)

```powershell
# 1. Check current migrations
dotnet ef migrations list --project .\Code\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure

# 2. Rollback to specific migration
dotnet ef database update PreviousMigrationName --project .\Code\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure

# 3. Remove the migration file (if needed)
dotnet ef migrations remove --project .\Code\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure
```

### Complete Database Reset

```powershell
# Option 1: Rollback all migrations
dotnet ef database update 0 --project .\Code\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure

# Option 2: Drop and recreate database
dotnet ef database drop --project .\Code\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure
dotnet ef database update --project .\Code\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure
```

### Working with Multiple DbContexts

AppBlueprint uses multiple DbContexts (ApplicationDbContext, B2BDbContext). Specify the context:

```powershell
# Rollback ApplicationDbContext
dotnet ef database update PreviousMigrationName --context ApplicationDbContext --project .\Code\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure

# Rollback B2BDbContext
dotnet ef database update PreviousMigrationName --context B2BDbContext --project .\Code\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure
```

## Production Environment Rollback

### Pre-Rollback Checklist

- [ ] **Backup database** before any rollback operation
- [ ] **Review rollback script** thoroughly
- [ ] **Test rollback** on staging environment first
- [ ] **Check data loss** - ensure Down() method doesn't drop data columns
- [ ] **Coordinate with team** - ensure no concurrent deployments
- [ ] **Plan rollback window** - schedule during low-traffic period

### Safe Production Rollback Process

#### Step 1: Backup Database

```sql
-- PostgreSQL backup command
pg_dump -h your-host -U your-user -d your-database -F c -f backup_$(date +%Y%m%d_%H%M%S).dump
```

#### Step 2: Generate Rollback Script

```powershell
# Generate SQL script from current to target migration
dotnet ef migrations script 20251114142057_InitialB2BDbContext 20251114034552_UpdatePaymentProviderToBaseEntity --output rollback_production.sql --project .\Code\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure
```

#### Step 3: Review Rollback Script

**Critical checks:**
- ✅ Verify no data-bearing columns are dropped
- ✅ Check for foreign key constraint issues
- ✅ Ensure proper transaction handling
- ✅ Look for potential data loss operations

**Example dangerous operations to watch for:**

```sql
-- ⚠️ DANGEROUS: Dropping columns with data
ALTER TABLE "Users" DROP COLUMN "Email";

-- ⚠️ DANGEROUS: Dropping tables with data
DROP TABLE "Orders";

-- ✅ SAFE: Adding back dropped columns (but data is lost)
ALTER TABLE "Users" ADD "Email" text NULL;

-- ✅ SAFE: Recreating dropped indexes
CREATE INDEX "IX_Users_Email" ON "Users" ("Email");
```

#### Step 4: Execute Rollback

```sql
-- Connect to production database
psql -h your-host -U your-user -d your-database

-- Start transaction
BEGIN;

-- Execute rollback script
\i rollback_production.sql

-- Verify changes
SELECT * FROM "__EFMigrationsHistory" ORDER BY "MigrationId" DESC LIMIT 5;

-- Commit or rollback transaction
COMMIT; -- or ROLLBACK;
```

#### Step 5: Update Application Code

After rolling back database:

1. Revert application code to match database schema
2. Redeploy application with previous version
3. Monitor for errors and data integrity issues

### Emergency Rollback (Data Loss Acceptable)

If you need to rollback quickly and data loss is acceptable:

```powershell
# Generate and apply rollback immediately (LOCAL ONLY)
dotnet ef database update PreviousMigrationName --project .\Code\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure

# ⚠️ WARNING: DO NOT use on production without review
```

## Safety Considerations

### Migrations That Cannot Be Safely Rolled Back

Some migrations cannot be safely rolled back without data loss:

#### 1. Column Drops

```csharp
// Migration Up
migrationBuilder.DropColumn(name: "LegacyField", table: "Users");

// Migration Down - data is permanently lost
migrationBuilder.AddColumn<string>(name: "LegacyField", table: "Users");
```

**Solution:** Implement multi-phase migration:
1. Phase 1: Stop writing to column (deploy code)
2. Phase 2: Archive data elsewhere
3. Phase 3: Drop column (create migration)

#### 2. Table Drops

```csharp
// Migration Up
migrationBuilder.DropTable(name: "ArchivedOrders");

// Migration Down - data is permanently lost
migrationBuilder.CreateTable(name: "ArchivedOrders", ...);
```

**Solution:** Export table data before dropping.

#### 3. Data Transformations

```csharp
// Migration Up - combines FirstName + LastName
migrationBuilder.Sql("UPDATE Users SET FullName = CONCAT(FirstName, ' ', LastName)");
migrationBuilder.DropColumn(name: "FirstName", table: "Users");
migrationBuilder.DropColumn(name: "LastName", table: "Users");

// Migration Down - cannot recover original FirstName/LastName split
```

**Solution:** Keep original data until transformation is verified.

### Rollback-Safe Migration Patterns

#### Pattern 1: Additive Changes Only

```csharp
// ✅ SAFE: Adding new columns (nullable or with defaults)
migrationBuilder.AddColumn<string>(
    name: "NewField",
    table: "Users",
    nullable: true);

// ✅ SAFE: Adding new tables
migrationBuilder.CreateTable(name: "NewFeature", ...);

// ✅ SAFE: Adding indexes
migrationBuilder.CreateIndex(
    name: "IX_Users_Email",
    table: "Users",
    column: "Email");
```

#### Pattern 2: Two-Phase Column Removal

```csharp
// Phase 1 Migration: Mark column as obsolete (keep data)
// [Obsolete] attribute on model property
// Deploy application code

// Phase 2 Migration: Remove column (after verification)
migrationBuilder.DropColumn(name: "ObsoleteField", table: "Users");
```

#### Pattern 3: Rename with Backward Compatibility

```csharp
// Instead of renaming, add new column and copy data
migrationBuilder.AddColumn<string>(name: "NewColumnName", table: "Users");
migrationBuilder.Sql("UPDATE Users SET NewColumnName = OldColumnName");

// Keep old column temporarily for rollback safety
// Remove old column in future migration after verification
```

## Troubleshooting

### Problem: "No migration was removed" when running `dotnet ef migrations remove`

**Cause:** Migration was already applied to database.

**Solution:**
```powershell
# Rollback database first
dotnet ef database update PreviousMigrationName --project .\Code\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure

# Then remove migration
dotnet ef migrations remove --project .\Code\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure
```

### Problem: "The migration 'X' has already been applied to the database"

**Cause:** Trying to rollback beyond current database state.

**Solution:**
```powershell
# Check current database state
dotnet ef migrations list --project .\Code\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure

# Ensure target migration is before current state
dotnet ef database update TargetMigrationName --project .\Code\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure
```

### Problem: "Could not execute because the specified command was not found" (dotnet-ef)

**Cause:** EF Core tools not installed.

**Solution:**
```powershell
# Install EF Core tools globally
dotnet tool install --global dotnet-ef

# Or update existing tools
dotnet tool update --global dotnet-ef
```

### Problem: Rollback fails with foreign key constraint errors

**Cause:** Down() method tries to drop table/column referenced by foreign keys.

**Solution:** Modify Down() method to:
1. Drop foreign key constraints first
2. Then drop table/column
3. Recreate foreign keys if needed

```csharp
protected override void Down(MigrationBuilder migrationBuilder)
{
    // Drop foreign keys first
    migrationBuilder.DropForeignKey(
        name: "FK_Orders_Users_UserId",
        table: "Orders");

    // Then drop the column/table
    migrationBuilder.DropColumn(
        name: "UserId",
        table: "Orders");
}
```

### Problem: "A network-related or instance-specific error occurred"

**Cause:** Cannot connect to database.

**Solution:**
```powershell
# Verify connection string
echo $env:DATABASE_CONNECTION_STRING

# Test database connection
dotnet ef database update --project .\Code\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure --verbose
```

### Problem: Rollback succeeded but application throws errors

**Cause:** Application code is not in sync with database schema.

**Solution:**
1. Ensure application code matches target migration
2. Revert code changes along with database rollback
3. Redeploy application

## Best Practices

### 1. Always Test Migrations in Staging

```powershell
# Apply migration to staging
dotnet ef database update --project .\Code\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure

# Test application functionality

# Test rollback
dotnet ef database update PreviousMigration --project .\Code\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure
```

### 2. Use Transactions for Rollback Scripts

```sql
BEGIN TRANSACTION;

-- Rollback operations
-- ...

-- Verify changes
SELECT COUNT(*) FROM "__EFMigrationsHistory";

-- Commit if successful, otherwise rollback
COMMIT; -- or ROLLBACK;
```

### 3. Document Breaking Changes

Add comments to migrations that cannot be safely rolled back:

```csharp
/// <summary>
/// ⚠️ WARNING: This migration drops the LegacyOrders table.
/// Data will be permanently lost on rollback.
/// Backup exported to Azure Blob Storage before deployment.
/// </summary>
public partial class DropLegacyOrders : Migration
{
    // ...
}
```

### 4. Keep Migration History

```powershell
# Generate migration with descriptive name
dotnet ef migrations add AddUserEmailVerification --project .\Code\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure

# Document in CHANGELOG.md
# - 2024-12-11: Added email verification columns to Users table
```

### 5. Use Idempotent Scripts for Production

```powershell
# Generate idempotent SQL script (can be run multiple times)
dotnet ef migrations script --idempotent --output migration.sql --project .\Code\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure
```

## Quick Reference

### Common Commands

```powershell
# List all migrations
dotnet ef migrations list --project .\Code\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure

# Check database current migration
dotnet ef migrations list --project .\Code\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure

# Rollback to specific migration
dotnet ef database update MigrationName --project .\Code\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure

# Rollback all migrations
dotnet ef database update 0 --project .\Code\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure

# Remove last migration (if not applied)
dotnet ef migrations remove --project .\Code\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure

# Generate rollback script
dotnet ef migrations script ToMigration FromMigration --output rollback.sql --project .\Code\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure

# Drop database
dotnet ef database drop --project .\Code\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure
```

### Rollback Decision Tree

```
Is migration applied to ANY database?
├─ NO → Use `dotnet ef migrations remove`
└─ YES → Is this production?
    ├─ NO (Development) → Use `dotnet ef database update PreviousMigration`
    └─ YES (Production) → Follow production rollback process:
        1. Backup database
        2. Generate SQL script
        3. Review script for data loss
        4. Test on staging
        5. Execute during maintenance window
        6. Update application code
```

## Related Documentation

- [Entity Framework Core Migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [Migration Deployment Strategies](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying)
- [AppBlueprint Integration Guide](./INTEGRATION_GUIDE.md)
- [Database Configuration Guide](../../AppBlueprint/CLOUD_DATABASE_SETUP.md)
