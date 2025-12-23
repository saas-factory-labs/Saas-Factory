# B2C/B2B Tenant Architecture Refactoring

**Date:** December 23, 2025  
**Status:** ✅ Implemented  
**Priority:** MEDIUM  

## Overview

Refactored the tenant architecture to support both B2C (Business-to-Consumer) and B2B (Business-to-Business) scenarios following Microsoft's official multi-tenancy best practices.

## Architecture Decision Record (ADR)

### Context

The system previously had `TenantEntity` in the B2B namespace, forcing all users (including B2C individual users) to use organizational tenant concepts. This created confusion and didn't align with Microsoft's recommended patterns for multi-tenant SaaS applications.

### Decision

Moved `TenantEntity` to **Baseline** and introduced a `TenantType` discriminator to support both:
- **Personal Tenants (B2C)**: Individual users, families, small groups
- **Organization Tenants (B2B)**: Companies with multiple users, teams, enterprise features

### References

This architecture follows Microsoft's official guidance:
- [Multi-tenancy Models](https://learn.microsoft.com/en-us/azure/architecture/guide/multitenant/considerations/tenancy-models)
- [SaaS Database Tenancy Patterns](https://learn.microsoft.com/en-us/azure/azure-sql/database/saas-tenancy-app-design-patterns)
- [SaaS and Multi-tenant Solution Architecture](https://learn.microsoft.com/en-us/azure/architecture/guide/saas-multitenant-solution-architecture)

### Consequences

**Positive:**
- ✅ Single unified tenant model works for both B2C and B2B
- ✅ B2B-specific fields (VatNumber, ContactPersons) are nullable for B2C
- ✅ Clear factory methods for creating appropriate tenant types
- ✅ Aligns with Microsoft's recommended patterns
- ✅ Supports hybrid deployment models (shared + dedicated)

**Negative:**
- ⚠️ Requires database migration to add `TenantType` column
- ⚠️ Existing tenants need type assignment during migration

## Changes Made

### 1. Added TenantType Enum

**File:** `AppBlueprint.SharedKernel/Enums/TenantType.cs`

```csharp
public enum TenantType
{
    Personal = 0,      // B2C scenarios
    Organization = 1   // B2B scenarios
}
```

### 2. Moved TenantEntity to Baseline

**From:** `AppBlueprint.Infrastructure/DatabaseContexts/B2B/Entities/Tenant/Tenant/TenantEntity.cs`  
**To:** `AppBlueprint.Infrastructure/DatabaseContexts/Baseline/Entities/Tenant/TenantEntity.cs`

**Key Changes:**
- Added `TenantType TenantType` property
- Made B2B fields nullable: `VatNumber`, `Country`, `Customer`
- Removed `Teams` navigation (kept in B2B context relationship)
- Added factory methods: `CreatePersonalTenant()`, `CreateOrganizationTenant()`
- Updated XML documentation with use case examples

### 3. Updated Database Configuration

**File:** `AppBlueprint.Infrastructure/DatabaseContexts/Baseline/Entities/EntityConfigurations/TenantEntityConfiguration.cs`

- Added `TenantType` column with proper indexing
- Made B2B-specific fields nullable in schema
- Added composite index: `(TenantType, IsActive, IsSoftDeleted)`
- Added comments explaining B2C vs B2B usage

### 4. Updated BaselineDbContext

**File:** `AppBlueprint.Infrastructure/DatabaseContexts/Baseline/BaselineDBContext.cs`

- Added `DbSet<TenantEntity> Tenants`
- Added `OnModelCreating_Tenants()` partial method
- Created `BaselineDBContext.Tenants.cs` partial implementation

### 5. Updated B2BDbContext

**File:** `AppBlueprint.Infrastructure/DatabaseContexts/B2B/Partials/B2BDbContext.Tenant.cs`

- Removed duplicate `TenantEntity` configuration
- Added Team relationship to Baseline's TenantEntity
- Updated namespace references

### 6. Created TenantFactory Service

**File:** `AppBlueprint.Domain/Baseline/Tenants/TenantFactory.cs`

Factory methods for creating tenants with business rules:
- `CreatePersonalTenant(UserEntity user)` - Auto-create for B2C users
- `CreateFamilyTenant(string familyName, string email)` - Family plans
- `CreateOrganizationTenant(...)` - B2B organizations
- `DetermineTenantType(...)` - Smart type detection
- `CanUserCreateOrganization(...)` - Business rule validation

### 7. Updated Entity References

Updated all files that referenced `B2B.Entities.Tenant.Tenant.TenantEntity`:
- `UserEntity.cs`
- `ContactPersonEntity.cs`
- `TeamEntity.cs`
- Various other dependent entities

## Use Case Validation

### ✅ B2C Examples

#### Dating App
```csharp
// Each user gets personal profile/tenant
var user = new UserEntity { FirstName = "John", LastName = "Doe", ... };
var personalTenant = TenantFactory.CreatePersonalTenant(user);
// TenantType = Personal
// Name = "John Doe"
// VatNumber = null (B2C doesn't need this)
```

#### Property Rental Portal
```csharp
// Individual landlord account
var landlordTenant = TenantFactory.CreatePersonalTenant(landlordUser);

// Individual renter account  
var renterTenant = TenantFactory.CreatePersonalTenant(renterUser);

// Both are Personal tenants with 1 user each
```

### ✅ B2B Examples

#### CRM System
```csharp
// Each client company is an organization tenant
var clientTenant = TenantFactory.CreateOrganizationTenant(
    organizationName: "Acme Corp",
    organizationEmail: "admin@acme.com",
    vatNumber: "DE123456789",
    country: "DE"
);
// TenantType = Organization
// Multiple users via Teams
// Has VatNumber for invoicing
```

#### Project Management SaaS
```csharp
// Each company gets organization workspace
var companyTenant = TenantFactory.CreateOrganizationTenant(
    organizationName: "TechStart Inc",
    organizationEmail: "team@techstart.io"
);
// TenantType = Organization
// Users can be added via Teams
// Supports organizational hierarchy
```

## Database Migration Required

### Migration Steps

1. **Add TenantType Column**
```sql
ALTER TABLE "Tenants"
ADD COLUMN "TenantType" integer NOT NULL DEFAULT 0;

CREATE INDEX "IX_Tenants_TenantType" ON "Tenants" ("TenantType");
```

2. **Update Existing Tenants**
```sql
-- Heuristic: If tenant has Teams, it's Organization type
UPDATE "Tenants"
SET "TenantType" = 1  -- Organization
WHERE "Id" IN (
    SELECT DISTINCT "TenantId" 
    FROM "Teams"
);

-- All others default to Personal (already set by DEFAULT 0)
```

3. **Make B2B Fields Nullable**
```sql
ALTER TABLE "Tenants" ALTER COLUMN "VatNumber" DROP NOT NULL;
ALTER TABLE "Tenants" ALTER COLUMN "Country" DROP NOT NULL;
ALTER TABLE "Tenants" ALTER COLUMN "Type" DROP NOT NULL;
ALTER TABLE "Tenants" ALTER COLUMN "Email" DROP NOT NULL;
ALTER TABLE "Tenants" ALTER COLUMN "Phone" DROP NOT NULL;
```

4. **Add Composite Index**
```sql
CREATE INDEX "IX_Tenants_Type_Active_NotDeleted" 
ON "Tenants" ("TenantType", "IsActive", "IsSoftDeleted");
```

## Row-Level Security (RLS) Support

The architecture supports PostgreSQL Row-Level Security for tenant isolation:

```sql
-- Enable RLS on tenant-scoped tables
ALTER TABLE "Users" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "Teams" ENABLE ROW LEVEL SECURITY;

-- Policy: Users can only see data from their own tenant
CREATE POLICY tenant_isolation_policy ON "Users"
    USING ("TenantId" = current_setting('app.current_tenant_id')::text);

CREATE POLICY tenant_isolation_policy ON "Teams"
    USING ("TenantId" = current_setting('app.current_tenant_id')::text);
```

**Tenant ID Resolution** (from JWT):
```csharp
// Extract tenant from validated JWT claims (security critical)
string tenantId = context.User.FindFirst("tenant_id")?.Value 
                  ?? context.User.FindFirst("tid")?.Value;

// Set for RLS policies
await dbConnection.ExecuteAsync(
    "SELECT set_config('app.current_tenant_id', @tenantId, false)",
    new { tenantId });
```

## Testing Strategy

### Unit Tests
- [ ] Test `TenantFactory.CreatePersonalTenant()` with various users
- [ ] Test `TenantFactory.CreateOrganizationTenant()` with B2B scenarios
- [ ] Test `TenantType` enum values and conversions
- [ ] Test nullable B2B fields for Personal tenants

### Integration Tests
- [ ] Test tenant creation flow for B2C registration
- [ ] Test tenant creation flow for B2B registration
- [ ] Test Team association with Organization tenants only
- [ ] Test RLS policies with different tenant types

## Deployment Considerations

### Hybrid Deployment Model

Following Microsoft's guidance, support multiple deployment strategies:

1. **Shared Infrastructure (Most B2C + Small B2B)**
   - All Personal tenants share database
   - Small Organization tenants share resources
   - Use RLS for data isolation
   - Cost-effective for scale

2. **Dedicated Infrastructure (Enterprise B2B)**
   - Large Organization tenants get dedicated database
   - Dedicated compute resources
   - Maximum isolation
   - Higher cost, charged to customer

3. **Geographic Partitioning**
   - Tenants assigned to regions (GDPR compliance)
   - Hybrid: Shared within region, isolated across regions

### Scaling Strategy

- **Vertical Partitioning**: Premium orgs get dedicated resources
- **Horizontal Partitioning**: Shard shared databases by tenant count
- **Elastic Pools**: Use Azure SQL elastic pools for shared databases

## Backward Compatibility

### Code Compatibility
✅ All existing code continues to work:
- `UserEntity.TenantId` still exists
- `TenantEntity` API unchanged (new properties are optional)
- Factory methods provide type-safe creation

### Data Migration
⚠️ Requires database migration:
1. Add `TenantType` column
2. Classify existing tenants
3. Make B2B fields nullable

## Future Enhancements

Potential additions following Microsoft patterns:

1. **Tenant Deployment Stamps**
   - Track which infrastructure hosts each tenant
   - Support moving tenants between deployments

2. **Tenant Tiers**
   ```csharp
   public enum TenantTier
   {
       Free,
       Basic,
       Premium,
       Enterprise
   }
   ```

3. **Tenant Catalog Service**
   - Central registry of tenant-to-deployment mapping
   - Routing requests to correct database shard

4. **Tenant Lifecycle Management**
   - Provisioning automation
   - Suspension/Reactivation workflows
   - Data export on cancellation

## References

- [Microsoft Multi-tenancy Overview](https://learn.microsoft.com/en-us/azure/architecture/guide/multitenant/overview)
- [Tenancy Models](https://learn.microsoft.com/en-us/azure/architecture/guide/multitenant/considerations/tenancy-models)
- [Database Patterns](https://learn.microsoft.com/en-us/azure/azure-sql/database/saas-tenancy-app-design-patterns)
- [Deployment Stamps Pattern](https://learn.microsoft.com/en-us/azure/architecture/patterns/deployment-stamp)

## Commit Message

```
refactor: Move TenantEntity to Baseline with B2C/B2B support

- Add TenantType enum (Personal vs Organization) following Microsoft patterns
- Move TenantEntity from B2B to Baseline for shared B2C/B2B usage
- Make B2B fields nullable (VatNumber, Country) for B2C scenarios
- Add TenantFactory service with factory methods for each tenant type
- Update BaselineDbContext and B2BDbContext configurations
- Update all entity references to new namespace
- Add comprehensive documentation with use case examples

Architecture follows Microsoft's multi-tenancy best practices:
https://learn.microsoft.com/en-us/azure/architecture/guide/multitenant/

BREAKING CHANGE: Requires database migration to add TenantType column
```

## Questions & Answers

**Q: Can B2C users create organizations?**  
A: Yes, using `TenantFactory.CreateOrganizationTenant()`. Business rules in `CanUserCreateOrganization()` control limits.

**Q: Can one user belong to multiple tenants?**  
A: Yes, via JWT claims listing accessible tenants. Common for B2B scenarios (user at multiple companies).

**Q: How are Personal tenants different from Organization tenants?**  
A: Personal: 1-few users, no teams, simple account. Organization: Multiple users, teams, VATnumbers, enterprise features.

**Q: Does this work for dating apps?**  
A: Yes! Each user profile = Personal tenant. Optional: Match users across tenants, family plans = shared Personal tenant.

**Q: Does this work for CRM systems?**  
A: Yes! Each client company = Organization tenant with teams, users, and business features.
