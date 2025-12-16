# Migration Guide

This guide helps you upgrade between major versions of AppBlueprint NuGet packages.

## Table of Contents

- [From 0.1.x to 0.2.x](#from-01x-to-02x) (Upcoming)
- [From 0.2.x to 1.0.0](#from-02x-to-100) (Planned)
- [General Migration Tips](#general-migration-tips)

---

## From 0.1.x to 0.2.x

> **Status:** 🚧 Not yet released - This section will be updated when 0.2.0 is released

**Planned changes:**
- Enhanced ThemeBuilder with dark mode support
- New dashboard components
- Performance improvements

**Breaking changes:** None expected (minor version)

### Migration Steps

1. Update package versions:
```bash
dotnet add package SaaS-Factory.AppBlueprint.UiKit --version 0.2.*
dotnet add package SaaS-Factory.AppBlueprint.Infrastructure --version 0.2.*
```

2. Build and run:
```bash
dotnet build
dotnet run
```

3. Review deprecation warnings (if any)
4. Optional: Adopt new features

---

## From 0.2.x to 1.0.0

> **Status:** 📅 Planned for Q1 2026

This will be the first major version with potential breaking changes.

**Expected changes:**
- Stable API surface
- Production-ready defaults
- Possible namespace reorganization

**Migration guide will be published before 1.0.0 release.**

---

## General Migration Tips

### Before You Upgrade

1. **Read the Release Notes**
   - Check [CHANGELOG.md](./CHANGELOG.md) for all changes
   - Review breaking changes section carefully

2. **Check Compatibility Matrix**
   - See [VERSIONING.md](./VERSIONING.md#compatibility-matrix)
   - Ensure your .NET version is compatible

3. **Backup Your Code**
   ```bash
   git commit -am "Before upgrading AppBlueprint packages"
   git push
   ```

4. **Review Dependencies**
   ```bash
   dotnet list package --outdated
   ```

### During Upgrade

1. **Upgrade One Package at a Time**
   ```bash
   # Start with SharedKernel (base dependency)
   dotnet add package SaaS-Factory.AppBlueprint.SharedKernel --version X.0.0

   # Then Contracts
   dotnet add package SaaS-Factory.AppBlueprint.Contracts --version X.0.0

   # Then Domain
   dotnet add package SaaS-Factory.AppBlueprint.Domain --version X.0.0

   # Finally Infrastructure and UiKit
   dotnet add package SaaS-Factory.AppBlueprint.Infrastructure --version X.0.0
   dotnet add package SaaS-Factory.AppBlueprint.UiKit --version X.0.0
   ```

2. **Build After Each Package**
   ```bash
   dotnet build
   ```
   Fix compilation errors before continuing

3. **Use IDE Refactoring Tools**
   - Visual Studio / Rider: Use "Find All References"
   - VS Code: Use "Rename Symbol" (F2)

### After Upgrade

1. **Run All Tests**
   ```bash
   dotnet test
   ```

2. **Manual Testing**
   - Test critical user flows
   - Check UI components render correctly
   - Verify authentication/authorization
   - Test database operations

3. **Monitor Logs**
   - Watch for new warnings or errors
   - Check for deprecation messages

4. **Update Documentation**
   - Update your project's README if needed
   - Document any workarounds used

### Rollback Plan

If you encounter issues:

1. **Revert packages:**
   ```bash
   # Option A: Git revert
   git revert HEAD
   git push

   # Option B: Manual downgrade
   dotnet add package SaaS-Factory.AppBlueprint.UiKit --version 0.1.*
   ```

2. **Report Issue:**
   - Create GitHub issue: https://github.com/saas-factory-labs/Saas-Factory/issues
   - Include error messages, stack traces
   - Mention version numbers (from → to)

---

## Common Migration Scenarios

### Scenario 1: Namespace Changed

**Before:**
```csharp
using AppBlueprint.UiKit.Themes.Old;
```

**After:**
```csharp
using AppBlueprint.UiKit.Themes;
```

**Fix:**
- Use Find/Replace across solution: `AppBlueprint.UiKit.Themes.Old` → `AppBlueprint.UiKit.Themes`

### Scenario 2: Method Signature Changed

**Before:**
```csharp
builder.Services.AddUiKit(theme);
```

**After:**
```csharp
builder.Services.AddUiKit(options =>
{
    options.Theme = theme;
});
```

**Fix:**
- Update all service registration calls
- Use IDE "Change Signature" refactoring if available

### Scenario 3: Class Renamed

**Before:**
```csharp
var service = new OldNavigationService();
```

**After:**
```csharp
var service = new NavigationService();
```

**Fix:**
- Use IDE "Rename" refactoring (Ctrl+R, Ctrl+R in Visual Studio)
- Or Find/Replace: `OldNavigationService` → `NavigationService`

### Scenario 4: Configuration Changed

**Before:**
```json
{
  "UiKit": {
    "ThemeName": "Superhero"
  }
}
```

**After:**
```csharp
builder.Services.AddUiKitWithPreset(ThemePreset.Superhero);
```

**Fix:**
- Remove JSON configuration
- Add code-based configuration in Program.cs

### Scenario 5: Component Properties Changed

**Before:**
```razor
<DashboardCard Value="@totalRevenue" />
```

**After:**
```razor
<DashboardCard ValueText="@totalRevenue" />
```

**Fix:**
- Update all component usages
- Search for `<DashboardCard` in .razor files

---

## Migration Checklist

Use this checklist when upgrading major versions:

### Pre-Migration
- [ ] Read CHANGELOG.md for target version
- [ ] Check VERSIONING.md compatibility matrix
- [ ] Backup code (commit + push)
- [ ] Note current package versions
- [ ] Create test plan for critical features

### Migration
- [ ] Upgrade SharedKernel package
- [ ] Build and fix errors
- [ ] Upgrade Contracts package
- [ ] Build and fix errors
- [ ] Upgrade Domain package
- [ ] Build and fix errors
- [ ] Upgrade Infrastructure package
- [ ] Build and fix errors
- [ ] Upgrade UiKit package
- [ ] Build and fix errors
- [ ] Upgrade Api.Client.Sdk package (if used)
- [ ] Build and fix errors

### Post-Migration
- [ ] Run unit tests (`dotnet test`)
- [ ] Run integration tests
- [ ] Manual testing of critical flows
- [ ] Check application logs for warnings
- [ ] Review deprecation warnings
- [ ] Update project documentation
- [ ] Deploy to staging environment
- [ ] Smoke test staging
- [ ] Deploy to production
- [ ] Monitor production logs

---

## Getting Help

If you encounter issues during migration:

1. **Check Documentation**
   - [VERSIONING.md](./VERSIONING.md) - Compatibility info
   - [README.md](./README.md) - General documentation
   - Package-specific READMEs in each module

2. **Search Existing Issues**
   - GitHub Issues: https://github.com/saas-factory-labs/Saas-Factory/issues
   - Use search: `is:issue label:migration`

3. **Ask for Help**
   - GitHub Discussions: https://github.com/saas-factory-labs/Saas-Factory/discussions
   - Create new issue with "migration" label

4. **Community Resources**
   - Stack Overflow tag: `appblueprint`
   - Discord/Slack (links in README.md)

---

## Contributing Migration Guides

Found an undocumented breaking change? Help others by contributing to this guide:

1. Fork the repository
2. Add your migration scenario to this file
3. Submit a pull request

**Template for new migration scenarios:**

```markdown
### Scenario X: [Brief Description]

**Before:**
```csharp
// Old code
```

**After:**
```csharp
// New code
```

**Fix:**
- Step-by-step instructions
- Alternative approaches if any
- Common pitfalls to avoid
```

---

## Version History

| Version | Release Date | Migration Guide | Breaking Changes |
|---------|-------------|-----------------|------------------|
| 0.1.0 | Current | N/A (initial) | N/A |
| 0.2.0 | TBD | [Link](#from-01x-to-02x) | None (minor) |
| 1.0.0 | Q1 2026 | [Link](#from-02x-to-100) | TBD |

---

_Last updated: [Date will be auto-updated on release]_
