# Moved Files - Detailed Explanation

This document provides a detailed explanation of what each file that was moved during the repository reorganization does and why it's important.

---

## 📚 Documentation Files → `docs/content/guides/`

### 1. `LOGOUT_FIX_TESTING_GUIDE.md`

**What it does:**
- Documents a critical bug fix in Blazor Server authentication where users appeared logged in even after clicking logout
- Explains the root cause: Blazor Server's persistent SignalR circuit maintains old authentication state in memory

**The Problem:**
```
User clicks logout → Server clears auth cookies ✅
User redirected to home page → Blazor circuit still has old auth state ❌
Result: User appears logged in with stale data
```

**The Solution Documented:**
- Created a `/logout-complete` page that forces a full page reload
- Uses `Navigation.NavigateTo("/", forceLoad: true)` to terminate the old circuit
- Ensures fresh authentication state is loaded from cookies

**Testing Steps Included:**
1. Log in as a user
2. Click logout button
3. Verify user is shown as not authenticated
4. Verify cannot access protected pages
5. Verify can log back in successfully

**Why it matters:** This guide helps developers understand and test a non-obvious Blazor Server issue that could cause security concerns if users think they're logged out but their session persists.

---

### 2. `LOGTO_SDK_TESTING_CHECKLIST.md`

**What it does:**
- Provides a comprehensive checklist for testing Logto authentication integration
- Ensures proper configuration in both the application and Logto Console

**Configuration Requirements:**
```
Redirect URIs (Sign-in callbacks):
  - https://localhost:443/Callback
  - https://your-domain.com/Callback

Post sign-out redirect URIs:
  - https://localhost:443/logout-complete
  - https://your-domain.com/logout-complete
```

**Testing Scenarios Covered:**
1. **Login Flow:**
   - Click "Log in" → Redirect to Logto
   - Enter credentials → Redirect back to app
   - Verify authentication state

2. **Logout Flow:**
   - Click "Log out" → Redirect to Logto
   - Logto clears session → Redirect to logout-complete
   - Force reload → Verify logged out state

3. **Protected Routes:**
   - Verify unauthenticated users can't access protected pages
   - Verify authenticated users have proper access

**Common Issues Documented:**
- Redirect URI mismatches (case-sensitive!)
- Missing trailing slashes
- HTTPS vs HTTP configuration
- Cookie domain issues

**Why it matters:** Logto setup has several gotchas that can cause silent failures. This checklist ensures nothing is missed during authentication implementation.

---

### 3. `QUICK_REFERENCE_LOGTO.md`

**What it does:**
- Quick lookup reference for Logto configuration settings
- Saves time by providing current endpoints and settings in one place

**Current Configuration (as of document):**
```
Endpoint: https://32nkyp.logto.app/
Application ID: uovd1gg5ef7i1c4w46mt6
SDK Version: 0.2.0
```

**Key Settings:**
- Redirect URIs for all environments (local, staging, production)
- Post sign-out URIs
- Resource indicators for API access
- Scopes required for user data

**Authentication Flow Summary:**
```
1. User → [Log in button] → Logto
2. Logto → [Authentication] → Redirect to /Callback
3. App → [Process callback] → User authenticated
4. User → [Log out button] → Logto sign-out
5. Logto → [Clear session] → /logout-complete
6. App → [Force reload] → User logged out
```

**Why it matters:** During development, you frequently need to look up Logto settings. Having them in one place prevents constant back-and-forth to the Logto Console.

---

## 🔧 Scripts → `scripts/powershell/`

### 4. `commit-and-push.ps1`

**What it does:**
- Automates the git staging and commit process specifically for Railway deployment changes
- Ensures all related files are committed together atomically

**Files It Stages:**
```powershell
# GitHub Actions
.github/workflows/deploy-to-railway.yml

# Railway Configuration
railway.json, railway-project.json
docker-compose.railway.yml, .railwayignore

# Documentation (13 files)
RAILWAY_*.md, DOCKER_BUILD_INSTRUCTIONS.md

# Scripts
setup-railway.ps1, test-docker-build.ps1

# Critical: Dockerfile fixes
AppBlueprint.ApiService/Dockerfile
AppBlueprint.Web/Dockerfile

# Commit message templates
RAILWAY_COMMIT_MESSAGE.md, etc.
```

**Workflow:**
1. Navigates to repository root
2. Shows current git status (preview changes)
3. Stages all Railway-related files
4. Shows what will be committed
5. Prompts for commit message
6. Creates commit with consistent format
7. Optionally pushes to remote

**Why it matters:** 
- Railway deployments involve many interconnected files
- Missing even one file (like a Dockerfile) can break deployment
- Ensures consistent commit messages for infrastructure changes
- Saves time - no need to manually `git add` 20+ files

**Example Use Case:**
```powershell
# After fixing Railway Dockerfile issues
.\scripts\powershell\commit-and-push.ps1

# Result: All 25+ Railway files staged in one command
```

---

## 🗄️ Database Scripts → `scripts/`

### 5. `test_ulid.sql`

**What it does:**
- Tests ULID (Universally Unique Lexicographically Sortable Identifier) implementation
- Validates database schema supports the multi-tenant hierarchy
- Creates sample data for development/testing

**Data Structure Tested:**
```
Customer (customer_01JGQ1GQ8G9NRQH3X7WVFV2K3X)
  └─ Tenant (tenant_01JGQ1GQ8G9NRQH3X7WVFV2K3Y)
       └─ User (user_01JGQ1GQ8G9NRQH3X7WVFV2K3Z)
```

**What ULIDs Provide:**
- **Sortable:** Sort by creation time (unlike UUIDs)
- **Unique:** 128-bit random values (no collisions)
- **Readable:** Base32 encoded (no special characters)
- **Prefixed:** Type-safe (`customer_`, `tenant_`, `user_`)

**Schema Validations:**
```sql
-- Customer Table
Id (varchar), CustomerType, OnboardingFlowStep
CreatedAt, IsSoftDeleted

-- Tenant Table  
Id (varchar), Name, Description, IsActive
Email, Phone, Type, VatNumber, Country
CustomerId (FK), CreatedAt, IsSoftDeleted

-- User Table
Id (varchar), FirstName, LastName, UserName
Email, IsActive, LastLogin
TenantId (FK), CreatedAt, IsSoftDeleted
```

**Why it matters:**
- ULIDs are critical for the multi-tenant architecture
- Validates foreign key relationships work with ULID strings
- Provides sample data matching production schema
- Catches schema mismatches early

**Example ULID Breakdown:**
```
customer_01JGQ1GQ8G9NRQH3X7WVFV2K3X
├─ customer_     → Type prefix for safety
└─ 01JGQ1GQ...   → Sortable timestamp + random
```

---

## 📊 Build Artifacts → `build-artifacts/`

### 6. `build-warnings.log` (370 MB!)

**What it does:**
- Captures complete MSBuild diagnostic output during compilation
- Records every assembly load, property evaluation, and build step

**What's Inside:**
```
Environment variables at build start
MSBuild process information
Assembly loading traces (all 500+ assemblies)
NuGet restore diagnostics
Property evaluations and reassignments
Target execution order
Compiler invocations with full arguments
All warnings across all projects
Performance timing
```

**Example Content:**
```
Assembly loaded: System.Runtime.Loader
  Location: C:\Program Files\dotnet\shared\...
  MVID: 862d2667-57a2-4297-b1e4-c84c2bd21fcf
  AssemblyLoadContext: Default

Property reassignment: $(_GenerateRestoreGraphProjectEntryInputProperties)
  Previous: "ExcludeRestorePackageImports=true"
  New: "ExcludeRestorePackageImports=true;_RestoreSolutionFileUsed=true;..."
```

**Use Cases:**
- **Debugging build failures:** See exactly what MSBuild is doing
- **Performance analysis:** Identify slow build steps
- **Dependency issues:** Track where assemblies are loaded from
- **Warning investigation:** Find all warnings in one place
- **NuGet troubleshooting:** See package restore details

**Why it's huge:**
- Contains full paths for every assembly (~500+ assemblies)
- Logs every property evaluation (thousands)
- Records all analyzer executions
- Includes source generator output

**Why it matters:** When builds fail mysteriously or you get obscure warnings, this file contains the answer. But it's too large for git, hence moved to `build-artifacts/`.

---

### 7. `warnings.txt`

**What it does:**
- Contains the raw C# compiler command-line invocations
- Shows exactly how MSBuild calls the C# compiler (`csc.dll`)

**What's Inside:**
```bash
C:\Program Files\dotnet\dotnet.exe exec 
"C:\Program Files\dotnet\sdk\9.0.302\Roslyn\bincore\csc.dll"
  /noconfig
  /unsafe-
  /nowarn:CS8618,CA1062,CA2007,NU1507,1701,1702
  /nullable:enable
  /reference:C:\Users\...\Aspire.Hosting.AppHost.dll
  /reference:C:\Users\...\[500+ more references]
  /target:exe
  /out:obj\Debug\net9.0\DeploymentManager.AppHost.dll
  Program.cs [and all other source files]
```

**Key Information Revealed:**
- **Suppressed Warnings:**
  ```
  CS8618 - Non-nullable field must contain non-null value
  CA1062 - Validate arguments of public methods  
  CA2007 - Do not directly await a Task
  NU1507 - NuGet package vulnerability warnings
  ```

- **Warnings as Errors:**
  ```
  NU1605 - Detected package downgrade
  SYSLIB0011 - Obsolete member usage
  ```

- **All Assembly References:**
  - From NuGet packages (~C:\Users\...\\.nuget\packages\)
  - From .NET SDK (~C:\Program Files\dotnet\packs\)
  - Project references

- **Compiler Flags:**
  ```
  /nullable:enable        → Nullable reference types enabled
  /langversion:latest     → Using latest C# features
  /debug:portable         → Portable debug symbols
  /optimize-             → Debug mode (no optimization)
  ```

**Use Cases:**
- **Understanding warning suppression:** See what's intentionally ignored
- **Reference issues:** Verify correct assembly versions are referenced
- **Analyzer configuration:** See which analyzers are active
- **Compiler behavior:** Understand exact compiler settings

**Why it matters:** Sometimes you need to see exactly how the compiler is being invoked. This is especially useful when:
- Warnings appear mysteriously
- Reference conflicts occur
- Analyzer behavior is unexpected
- You need to replicate build outside MSBuild

---

## Summary

These files were moved because:

1. **Documentation** (`docs/content/guides/`) - Testing guides and references belong together
2. **Scripts** (`scripts/powershell/`) - Utility scripts organized by type
3. **Build Output** (`build-artifacts/`) - Separated from source code, gitignored

Each file serves a specific purpose in the development workflow:
- **Testing guides** ensure features work correctly
- **Reference docs** provide quick lookups
- **Scripts** automate repetitive tasks  
- **SQL tests** validate database schema
- **Build logs** help debug issues

The reorganization makes these files easier to find and maintain while keeping the repository root clean.

