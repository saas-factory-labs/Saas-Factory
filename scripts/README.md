# Scripts

Utility scripts for maintaining and validating AppBlueprint packages.

## validate-nuget-packages.sh

Validates that NuGet packages can be consumed correctly before publishing.

### Purpose

This script ensures that:
- Packages build successfully
- Packages can be installed in a consumer project
- Consumer projects build successfully with the packages
- Basic integration tests pass

### Usage

```bash
# From repository root
./Scripts/validate-nuget-packages.sh

# Or from Scripts directory
cd Scripts
./validate-nuget-packages.sh
```

### What It Does

1. **Builds packages** - Creates .nupkg files in `artifacts/packages/`
2. **Creates test project** - Generates temporary Blazor Server app
3. **Installs packages** - Adds packages from local build
4. **Configures project** - Sets up Program.cs, imports, and test pages
5. **Builds consumer project** - Verifies compilation succeeds
6. **Runs integration tests** - Tests service registration and configuration
7. **Cleans up** - Removes temporary test directory

### Requirements

- .NET 10 SDK
- Bash shell (Git Bash on Windows, native on macOS/Linux)
- Packages already built (or script will build them)

### Output

```
==========================================
AppBlueprint NuGet Package Validator
==========================================

Step 1: Building NuGet packages...
-----------------------------------
✓ Found 5 NuGet packages

Step 2: Creating temporary test project...
-------------------------------------------
✓ Created Blazor Server test project

Step 3: Adding package references...
-------------------------------------
✓ Installing AppBlueprint.UiKit version 0.1.0
✓ Packages added successfully

Step 4: Configuring test project...
------------------------------------
✓ Test project configured

Step 5: Building test project...
---------------------------------
✓ Build successful ✓

Step 6: Running integration tests...
-------------------------------------
✓ Integration tests passed ✓

==========================================
Validation Summary
==========================================

✓ Package build: SUCCESS
✓ Package installation: SUCCESS
✓ Project build: SUCCESS
✓ Integration tests: SUCCESS

✓ All validation checks passed!

Packages are ready for publishing.
```

### When to Run

- **Before publishing to NuGet.org** - Ensures packages work correctly
- **After major changes** - Validates breaking changes don't break consumption
- **In CI/CD pipeline** - Automated validation before release
- **When adding new packages** - Verifies new package integrates correctly

### Troubleshooting

**Build fails:**
- Ensure .NET 10 SDK is installed: `dotnet --version`
- Clean and rebuild: `dotnet clean && dotnet build`

**Package not found:**
- Check `artifacts/packages/` directory exists
- Script will build packages if missing

**Integration tests fail:**
- Check test output for specific failures
- Temporary files left in `temp-nuget-validation/` for debugging
- Remove temp directory manually if cleanup fails

### Related Documentation

- [VERSIONING.md](../VERSIONING.md) - Version strategy and compatibility
- [MIGRATION.md](../MIGRATION.md) - Upgrade guides
- [AppBlueprint.UiKit/USAGE.md](../Code/AppBlueprint/Shared-Modules/AppBlueprint.UiKit/USAGE.md) - Package usage guide
