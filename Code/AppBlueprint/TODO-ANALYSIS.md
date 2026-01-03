# TODO Items Analysis

Analysis performed: January 2, 2026

## Overview
This document catalogs all TODO comments found in the AppBlueprint codebase with categorization and priority recommendations.

## TODO Items by Category

### ✅ Completed Items

1. **Password Validation** - `MainMenu.cs:217` - ✅ **COMPLETED**
   - Location: `AppBlueprint.DeveloperCli/Menus/MainMenu.cs`
   - Context: PostgreSQL password validation in CLI menu
   - Implementation: Added `ValidatePostgreSqlPassword()` method with:
     - Connection string validation using existing `ConnectionStringValidator`
     - Interactive prompts for database credentials
     - Password masking for security
     - Option to save connection string to environment variable
     - User-friendly status indicators and error messages
   - Completed: January 2, 2026

### 🔴 High Priority - Quick Wins (Ready for Implementation)

2. **Tenant ID Validation** - `TenantProvider.cs:21`
   - Location: `Infrastructure/Services/TenantProvider.cs`
   - Context: Need to validate tenant ID exists in catalog database
   - Effort: Medium
   - Impact: Data integrity
   - Note: "TODO: also remember to add check to check if the tenant id actually exist in the tenant catalog database"

3. **Database Test Fixture** - `B2BDbContextTests.cs:14-15`
   - Location: `Infrastructure/DatabaseContexts/B2B/Tests/B2BDbContextTests.cs`
   - Context: Add shared database fixture class and fake data generation
   - Effort: Medium
   - Impact: Test infrastructure
   - References: Task [SFVDM-348]
   - Notes:
     - "TODO: [SFVDM-348] Add database fixture class to add common test methods and properties across all db contexts"
     - "TODO: [SFVDM-348] Setup shared common fake data generation for all db contexts to use in unit tests (e.g. Faker, Bogus, etc.)"

4. **Entity Modeling** - `DataSeeder.cs:148, 152`
   - Location: `SeedTest/DataSeeder.cs`
   - Context: Missing entities in ApplicationDbContext
   - Effort: Small-Medium
   - Impact: Data completeness
   - Notes:
     - Line 148: "TODO: Add these when available in ApplicationDbContext"
     - Line 152: "TODO: Add these when available in ApplicationDbContext"

5. **Download URL Generation** - `DataExportService.cs:108`
   - Location: `Infrastructure/Services/DataExport/DataExportService.cs`
   - Context: Generate actual download URL based on business rules
   - Effort: Medium
   - Impact: Functionality
   - Note: "TODO: Generate actual download URL based on business rules"
   - Current: Using placeholder `new Uri("about:blank")`

6. **Entity Configuration Enhancement** - Multiple locations
   - Location: `Infrastructure/DatabaseContexts/Baseline/Entities/`
   - Context: Missing optional properties in entities
   - Effort: Small per item
   - Impact: Data model completeness
   - Items:
     - `GlobalRegionEntityConfiguration.cs:27` - "TODO: Add Code property to GlobalRegionEntity if region codes are needed (e.g., "NA", "EU", "AS")"
     - `EmailVerificationEntityConfiguration.cs:41` - "TODO: Add UserId foreign key and User navigation property to EmailVerificationEntity if user relationship is needed"

### 🔵 Low Priority - Future Features (Authentication Providers)

7-10. **Authentication Provider Implementations** - `AuthenticationProviderFactory.cs:93, 102, 111, 120`
   - Location: `Infrastructure/Authorization/AuthenticationProviderFactory.cs`
   - Context: Multiple authentication provider implementations needed
   - Effort: Large per provider
   - Impact: Feature expansion
   - Providers:
     - Azure AD B2C (line 93) - "TODO: Implement Azure AD B2C provider"
     - AWS Cognito (line 102) - "TODO: Implement AWS Cognito provider"
     - Firebase Authentication (line 111) - "TODO: Implement Firebase Authentication provider"
     - Simple JWT (line 120) - "TODO: Implement simple JWT provider for development/testing"

### 🟢 Development Infrastructure

11. **Command/Query Handlers** - `ServiceCollectionExtensions.cs:28-29`
   - Location: `Application/Extensions/ServiceCollectionExtensions.cs`
   - Context: Register command and query handlers when implemented
   - Effort: Depends on implementation
   - Impact: Architecture completion
   - Notes:
     - Line 28: "TODO: Register command handlers when implemented"
     - Line 29: "TODO: Register query handlers when implemented"

### 🔶 Testing TODOs

12-13. **Missing Blazor Components** - `PasswordResetTests.cs:20, 32`
   - Location: `AppBlueprint.Tests/Blazor/PasswordResetTests.cs`
   - Context: Tests exist but components don't
   - Effort: Medium-Large
   - Impact: Feature completeness
   - Notes:
     - Line 20: "TODO: ForgotPassword component doesn't exist yet"
     - Line 32: "TODO: ResetPassword component doesn't exist yet"

### 🟣 Subscription Service

14. **Stripe Customer Creation** - `StripeSubscriptionService.cs:33`
   - Location: `Infrastructure/Services/StripeSubscriptionService.cs`
   - Context: Implement actual customer creation logic
   - Effort: Medium
   - Impact: Payment integration
   - Note: "TODO: Implement actual customer creation logic"

## Priority Recommendations

### Immediate Actions (This Sprint)
1. ~~Password validation in CLI (Quick win)~~ ✅ **COMPLETED**
2. Tenant ID validation (Data integrity)
3. Download URL generation (Functionality gap)

### Next Sprint
1. Database test fixture setup
2. Missing entity additions
3. Entity configuration enhancements

### Backlog
1. Authentication provider implementations (as business need arises)
2. Password reset components
3. Command/Query handler registration
4. Stripe customer creation logic

## Statistics
- Total TODO items: 20
- Completed: 1
- Remaining: 19
  - High Priority: 0
  - Medium Priority: 6
  - Low Priority: 4
  - Testing: 4
  - Infrastructure: 2
  - Payment: 1
  - Authentication: 4

## Notes
- Most TODOs are well-documented with context
- Several TODOs reference specific task IDs (e.g., SFVDM-348)
- Authentication provider TODOs are lower priority as Logto is currently implemented
- Some TODOs are placeholders for future business requirements

