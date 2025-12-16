# Versioning Strategy

AppBlueprint NuGet packages follow [Semantic Versioning 2.0.0](https://semver.org/).

## Version Format: MAJOR.MINOR.PATCH

```
Example: 1.2.3
         │ │ │
         │ │ └─ Patch: Bug fixes (backward compatible)
         │ └─── Minor: New features (backward compatible)
         └───── Major: Breaking changes
```

## Version Types

### Major Versions (X.0.0) - Breaking Changes

Major version increments indicate **breaking changes** that may require code modifications in consuming projects.

**Examples of breaking changes:**
- Removed public APIs or classes
- Changed method signatures (parameters, return types)
- Renamed namespaces or types
- Changed service registration requirements
- Database schema breaking changes (Infrastructure package)
- Removed or renamed configuration options
- Changed default behavior that breaks existing usage

**When we release:**
- v1.0.0 → v2.0.0: Major architectural change
- Minimum once per year (planned breaking changes batch)

**Consumer impact:**
- ⚠️ Review MIGRATION.md before upgrading
- ⚠️ Expect compilation errors or runtime issues
- ⚠️ May require code changes in your project

---

### Minor Versions (0.X.0) - New Features

Minor version increments add **new functionality** while maintaining backward compatibility.

**Examples of new features:**
- New components or pages (UiKit)
- New extension methods
- New configuration options (with defaults)
- New optional parameters (with defaults)
- New themes or theme presets
- Performance improvements
- Deprecated APIs (still work, but marked obsolete)

**When we release:**
- v0.1.0 → v0.2.0: New dashboard component added
- v0.2.0 → v0.3.0: ThemeBuilder added with new methods
- Typically every 2-4 weeks with feature batches

**Consumer impact:**
- ✅ Safe to upgrade
- ✅ No code changes required
- ℹ️ Review release notes for new capabilities
- ℹ️ Review deprecation warnings

---

### Patch Versions (0.0.X) - Bug Fixes

Patch version increments contain **bug fixes and minor improvements** with no API changes.

**Examples of patches:**
- Bug fixes
- Security patches
- Performance optimizations
- Documentation updates
- Internal refactoring (no public API impact)
- Dependency version updates (compatible)

**When we release:**
- v0.1.0 → v0.1.1: Fixed navigation menu bug
- v0.1.1 → v0.1.2: Performance optimization in ThemeBuilder
- As needed when critical bugs are discovered

**Consumer impact:**
- ✅ Safe to upgrade immediately
- ✅ No code changes required
- ✅ Recommended for bug fixes and security

---

## Pre-release Versions

Pre-release versions are used for testing before stable releases.

### Alpha (X.Y.Z-alpha.N)
- **Purpose:** Early testing, API may change
- **Example:** `0.2.0-alpha.1`
- **Use when:** Major new features in development
- **Stability:** ⚠️ Not recommended for production

### Beta (X.Y.Z-beta.N)
- **Purpose:** Feature complete, testing for bugs
- **Example:** `0.2.0-beta.1`
- **Use when:** APIs are stable, need user feedback
- **Stability:** ⚠️ Use with caution in production

### Release Candidate (X.Y.Z-rc.N)
- **Purpose:** Final testing before stable release
- **Example:** `1.0.0-rc.1`
- **Use when:** No known issues, awaiting final validation
- **Stability:** ✅ Generally safe for production

---

## Compatibility Matrix

### Package Version Compatibility

| Package Version | .NET Version | MudBlazor | EF Core | PostgreSQL | Status |
|----------------|--------------|-----------|---------|------------|--------|
| **0.1.x** | .NET 10.0 | 8.14.0 | 10.0.0-rc.1 | Npgsql 10.0.0-rc.1 | ✅ Current |
| **0.2.x** | .NET 10.0 | 8.15.0+ | 10.0.0 | Npgsql 10.0.0 | 🚧 Planned |
| **1.0.x** | .NET 10.0+ | 9.0.0+ | 10.0.0+ | Npgsql 10.0.0+ | 📅 Future |

### Cross-Package Compatibility

All AppBlueprint packages with the **same MAJOR.MINOR version** are compatible:

✅ **Compatible:**
- `AppBlueprint.SharedKernel 0.1.0` + `AppBlueprint.UiKit 0.1.5` = ✅ Works
- `AppBlueprint.Infrastructure 0.2.0` + `AppBlueprint.Domain 0.2.3` = ✅ Works

⚠️ **May have issues:**
- `AppBlueprint.SharedKernel 0.1.x` + `AppBlueprint.UiKit 0.2.x` = ⚠️ Test first
- Minor version mismatches may work but aren't guaranteed

❌ **Not compatible:**
- `AppBlueprint.SharedKernel 1.x.x` + `AppBlueprint.UiKit 2.x.x` = ❌ Breaking changes

**Recommendation:** Keep all AppBlueprint packages on the same MAJOR.MINOR version.

---

## Release Cadence

### Planned Schedule

- **Major releases:** Once per year (January)
- **Minor releases:** Every 2-4 weeks
- **Patch releases:** As needed (bug fixes, security)
- **Pre-releases:** Continuous (alpha/beta/rc)

### Version Lifecycle

| Version | Release Date | Support Status | End of Life |
|---------|-------------|----------------|-------------|
| 0.1.x | Current | ✅ Active Support | Until 0.3.0 |
| 0.2.x | Planned Q2 2025 | 📅 Planned | TBD |
| 1.0.x | Planned Q1 2026 | 📅 Planned | TBD |

**Support policy:**
- Current MAJOR version: Full support
- Previous MAJOR version: Security fixes only (6 months)
- Older versions: No support

---

## How We Determine Version Numbers

### Automated with GitVersion

We use [GitVersion](https://gitversion.net/) to automatically calculate version numbers based on:

1. **Git tags:** `v1.0.0`, `v1.1.0-beta.1`
2. **Branch names:** `main`, `develop`, `feature/*`
3. **Commit messages:** `feat:`, `fix:`, `BREAKING CHANGE:`

**Examples:**

```bash
# Minor version bump (new feature)
git commit -m "feat: add new DashboardCard component"
# Results in: 0.1.0 → 0.2.0

# Patch version bump (bug fix)
git commit -m "fix: navigation menu icon alignment"
# Results in: 0.1.0 → 0.1.1

# Major version bump (breaking change)
git commit -m "feat!: remove deprecated ThemeManager

BREAKING CHANGE: ThemeManager has been removed. Use ThemeBuilder instead."
# Results in: 0.2.0 → 1.0.0
```

---

## Upgrading Between Versions

### Patch Upgrades (0.1.0 → 0.1.1)

```bash
# Upgrade all packages to latest patch
dotnet add package SaaS-Factory.AppBlueprint.UiKit
dotnet build
# No code changes needed ✅
```

### Minor Upgrades (0.1.x → 0.2.0)

```bash
# Update Directory.Packages.props
<PackageVersion Include="SaaS-Factory.AppBlueprint.UiKit" Version="0.2.*" />

dotnet restore
dotnet build
# Review deprecation warnings ⚠️
# Optional: Adopt new features ℹ️
```

### Major Upgrades (1.x.x → 2.0.0)

```bash
# Read MIGRATION.md first! ⚠️
# Update one package at a time
dotnet add package SaaS-Factory.AppBlueprint.UiKit --version 2.0.0

dotnet build
# Fix compilation errors 🔧
# Update code per migration guide 📖
# Test thoroughly ✅
```

---

## Version Decision Tree

```
Is this a bug fix only?
├─ YES → Patch version (0.1.0 → 0.1.1)
└─ NO ↓

Does it add new features?
├─ YES ↓
│   └─ Are all changes backward compatible?
│       ├─ YES → Minor version (0.1.0 → 0.2.0)
│       └─ NO → Major version (0.1.0 → 1.0.0)
└─ NO ↓

Does it change existing behavior or APIs?
├─ YES → Major version (0.1.0 → 1.0.0)
└─ NO → Patch version (0.1.0 → 0.1.1)
```

---

## For Package Maintainers

### Creating a Release

**1. Determine version bump:**
```bash
# Review commits since last release
git log v0.1.0..HEAD --oneline

# Check for breaking changes
git log v0.1.0..HEAD --grep="BREAKING CHANGE"
```

**2. Create and push tag:**
```bash
# Patch release
git tag v0.1.1
git push origin v0.1.1

# Minor release
git tag v0.2.0
git push origin v0.2.0

# Major release (requires approval)
git tag v1.0.0
git push origin v1.0.0
```

**3. GitHub Actions automatically:**
- Builds packages
- Runs tests
- Publishes to NuGet.org
- Creates GitHub release notes

### Pre-release Testing

```bash
# Create alpha tag
git tag v0.2.0-alpha.1
git push origin v0.2.0-alpha.1

# Test locally before publishing
dotnet pack --configuration Release
dotnet add package SaaS-Factory.AppBlueprint.UiKit --source ./bin/Release --version 0.2.0-alpha.1
```

---

## Questions?

- **Issue Tracker:** https://github.com/saas-factory-labs/Saas-Factory/issues
- **Discussions:** https://github.com/saas-factory-labs/Saas-Factory/discussions
- **Migration Guides:** See MIGRATION.md
- **Changelog:** See CHANGELOG.md
