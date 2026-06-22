# Dependabot Configuration Guide

This guide explains the Dependabot setup for automated dependency updates across the SaaS Factory project.

---

## 🎯 Overview

Dependabot is configured to automatically check for dependency updates across all project directories and ecosystems:

- **33 monitored directories** across 4 ecosystems
- **Automated PR creation** for dependency updates
- **Smart scheduling** to avoid overwhelming the team
- **Grouped updates** for related dependencies
- **Security-first** approach with immediate vulnerability patches

---

## 📦 Monitored Ecosystems

### 1. .NET / NuGet (5 directories)

**Why multiple directories?**
- SaaS Factory uses **Central Package Management** with `Directory.Packages.props`
- Dependabot checks each major project area for package references
- Centralized version management ensures consistency

**Monitored Directories:**
```
/ (root)                            # Central Package Management
/Code/AppBlueprint                  # Main application area
/Code/DeploymentManager             # Deployment tools
/Code/Landingpage                   # Marketing site
/docs/RazorPress/RazorPress         # Documentation site
```

**Schedule:** Daily at 04:00 UTC
**Pull Request Limit:** 5-10 per directory
**Labels:** `dependencies`, `dotnet`, `nuget`, project-specific

**Grouping:**
- Minor and patch updates grouped together
- Major updates create separate PRs
- Allows batch review of non-breaking changes

**Example PR:**
```
chore(deps): Update .NET dependencies (minor/patch)

- Microsoft.EntityFrameworkCore: 10.0.0 → 10.0.1
- MudBlazor: 8.14.0 → 8.14.1
- Serilog.AspNetCore: 12.0.0 → 12.1.0
```

### 2. npm / Node.js (5 directories)

**Monitored Directories:**
```
/ (root)                            # Root package.json
/Code                               # Code directory packages
/Code/Cloudflare-Workers            # Serverless functions
/Code/Landingpage                   # Landing page frontend
/docs/RazorPress/RazorPress         # Docs site frontend
```

**Schedule:** Weekly on Tuesday at 04:00 UTC
**Pull Request Limit:** 3-5 per directory
**Labels:** `dependencies`, `npm`, `javascript`, component-specific

**Grouping:**
- Development dependencies grouped separately
- Production dependencies (patch only) grouped
- Major updates create individual PRs

**Special Rules:**
- Cloudflare packages grouped together (`@cloudflare/*`, `wrangler`)
- Playwright packages in bin directories ignored (build artifacts)

**Example PR:**
```
chore(deps): Update npm development dependencies

- @types/node: 20.10.0 → 20.11.0
- eslint: 8.55.0 → 8.56.0
- prettier: 3.1.0 → 3.1.1
```

### 3. Docker (10 directories)

**Why so many directories?**
- Each microservice has its own Dockerfile
- Ensures base image updates are tracked per service
- Prevents breaking changes from affecting all services

**Monitored Directories:**
```
AppBlueprint Services:
  /Code/AppBlueprint/AppBlueprint.Web
  /Code/AppBlueprint/AppBlueprint.ApiService
  /Code/AppBlueprint/AppBlueprint.AppGateway
  /Code/AppBlueprint/AppBlueprint.AppHost
  /Code/AppBlueprint/AppBlueprint.Tests

DeploymentManager Services:
  /Code/DeploymentManager/DeploymentManager.Web
  /Code/DeploymentManager/DeploymentManager.ApiService

Development & Docs:
  /.devcontainer
  /docs/search-server/typesense-server
```

**Schedule:** Weekly on Wednesday at 04:00 UTC
**Pull Request Limit:** 2-3 per directory
**Labels:** `dependencies`, `docker`, service-specific

**What Gets Updated:**
- .NET base images (`mcr.microsoft.com/dotnet/aspnet`, `sdk`, etc.)
- Node.js images
- Alpine Linux images
- Custom base images

**Integration with Hash Pinning:**
- Dependabot suggests tag updates
- Manual verification of SHA256 digests required
- Use `scripts/update-docker-digests.ps1` after merging

**Example PR:**
```
chore(deps): Update Docker base image in AppBlueprint.Web

FROM mcr.microsoft.com/dotnet/aspnet:10.0
→ FROM mcr.microsoft.com/dotnet/aspnet:10.0.1
```

### 4. GitHub Actions (1 directory)

**Monitored Directory:**
```
/ (root)                            # All workflow files in .github/workflows/
```

**Schedule:** Weekly on Thursday at 04:00 UTC
**Pull Request Limit:** 5
**Labels:** `dependencies`, `github-actions`, `ci-cd`

**Grouping:**
- Minor and patch updates grouped
- Major version updates separate (potential breaking changes)

**What Gets Updated:**
```
actions/checkout@v4 → actions/checkout@v5
docker/build-push-action@v5 → docker/build-push-action@v6
```

**Example PR:**
```
ci(deps): Update GitHub Actions (minor/patch)

- actions/checkout: v4.1.0 → v4.1.1
- actions/upload-artifact: v4.3.0 → v4.3.1
- github/codeql-action: v3.24.0 → v3.25.0
```

---

## ⏰ Update Schedule

### Daily Updates (04:00 UTC)
- ✅ .NET / NuGet packages (all directories)

**Why daily?**
- High development activity
- Central package management makes updates safer
- Security patches applied quickly

### Weekly Updates

| Day | Ecosystem | Time (UTC) | Reason |
|-----|-----------|------------|--------|
| **Tuesday** | npm | 04:00 | JavaScript ecosystem stability |
| **Wednesday** | Docker | 04:00 | Base image updates |
| **Thursday** | GitHub Actions | 04:00 | CI/CD workflow updates |

**Why staggered?**
- Prevents PR overload
- Allows focused review per ecosystem
- Different team members can specialize

---

## 🏷️ Labels & Organization

### Standard Labels (all PRs)
- `dependencies` - All dependency updates

### Ecosystem Labels
- `dotnet` - .NET / NuGet updates
- `npm` - Node.js / npm updates
- `docker` - Docker image updates
- `github-actions` - Workflow updates

### Project Labels
- `app-blueprint` - AppBlueprint project
- `deployment-manager` - DeploymentManager project
- `landingpage` - Landing page
- `docs` - Documentation site

### Component Labels
- `web` - Web service
- `api` - API service
- `gateway` - Gateway service
- `tests` - Test projects
- `ci-cd` - CI/CD workflows

### Example Label Combination
```
dependencies, dotnet, app-blueprint, web
↓
.NET dependency update for AppBlueprint.Web
```

---

## 🔐 Security Updates

### Automatic Security PRs

**Behavior:**
- Created **immediately** (not subject to schedule)
- Highest priority
- Labeled with `security` (automatic)
- Pull request limit does not apply

**Example:**
```
chore(deps): [SECURITY] Update Microsoft.Data.SqlClient

CVE-2024-XXXXX: SQL Injection vulnerability
Severity: HIGH
Patch: 5.1.5 → 5.1.6
```

**Response:**
1. Review security advisory
2. Test in development environment
3. Merge immediately if tests pass
4. Deploy to production ASAP

---

## 📊 Pull Request Limits

### Strategy
- Prevent overwhelming the team with too many PRs
- Focus on high-value updates first
- Maintain development velocity

### Limits by Priority

| Priority | Directories | Limit | Rationale |
|----------|-------------|-------|-----------|
| **Critical** | Root NuGet | 10 | Central package management |
| **High** | AppBlueprint, npm root | 5 | Active development |
| **Medium** | Individual services | 3 | Isolated changes |
| **Low** | Docs, supporting projects | 2-3 | Lower impact |

**What happens when limit reached?**
- Oldest PRs remain open
- New updates queued for next cycle
- Security updates bypass limit

---

## 🔄 Grouping Strategy

### .NET Dependencies
```yaml
groups:
  dotnet-minor-patch:
    patterns: ["*"]
    update-types: ["minor", "patch"]
```

**Result:** Single PR with multiple minor/patch updates

**Example:**
```
chore(deps): Update .NET dependencies (minor/patch)

Updates 12 packages:
- Serilog: 3.1.0 → 3.1.1
- MudBlazor: 8.14.0 → 8.14.1
- ...
```

### npm Dependencies
```yaml
groups:
  npm-development:
    dependency-type: "development"
    update-types: ["minor", "patch"]
  npm-production:
    dependency-type: "production"
    update-types: ["patch"]
```

**Result:**
- Development dependencies: Minor + patch grouped
- Production dependencies: Only patch updates grouped
- Major updates: Separate PRs

### Cloudflare Workers
```yaml
groups:
  cloudflare-workers:
    patterns:
      - "@cloudflare/*"
      - "wrangler"
```

**Result:** All Cloudflare-related packages in one PR

### GitHub Actions
```yaml
groups:
  github-actions:
    patterns: ["*"]
    update-types: ["minor", "patch"]
```

**Result:** Multiple action updates in single PR

---

## 💬 Commit Message Format

### Conventional Commits

All Dependabot PRs follow [Conventional Commits](https://www.conventionalcommits.org/):

```
<type>(<scope>): <description>

<type>(<scope>): <description>
```

### Types Used

| Type | Use Case | Example |
|------|----------|---------|
| `chore(deps)` | Most dependency updates | `chore(deps): Update .NET packages` |
| `ci(deps)` | GitHub Actions | `ci(deps): Update actions/checkout to v5` |
| `docs(deps)` | Documentation projects | `docs(deps): Update RazorPress dependencies` |
| `test(deps)` | Test projects | `test(deps): Update test framework` |

### Scope

Optional but included for better categorization:
- `chore(deps): Update packages` (no scope)
- `chore(deps-npm): Update npm packages` (with scope)

---

## 🚫 Ignore Rules

### Current Ignore Rules

```yaml
ignore:
  - dependency-name: "@playwright/*"
    update-types: ["version-update:semver-major"]
```

**Why?**
- Playwright packages in `bin/` directories are build artifacts
- Not source code dependencies
- Updated automatically during build

### Adding Ignore Rules

To ignore a specific dependency:

```yaml
ignore:
  - dependency-name: "some-package"
    update-types:
      - "version-update:semver-major"  # Ignore major updates only
      # OR
      - "version-update:all"           # Ignore all updates
```

Common reasons to ignore:
- Known compatibility issues
- Waiting for ecosystem support
- Deprecated but required package
- Build artifacts (like Playwright example)

---

## 👥 Reviewers

### Current Setup

```yaml
reviewers:
  - "saas-factory-labs/maintainers"
```

**Who gets notified?**
- Members of the `saas-factory-labs/maintainers` team
- Currently configured for NuGet updates only

### Customizing Reviewers

Per ecosystem:
```yaml
- package-ecosystem: "npm"
  reviewers:
    - "saas-factory-labs/frontend-team"

- package-ecosystem: "docker"
  reviewers:
    - "saas-factory-labs/devops-team"
```

Per individual:
```yaml
reviewers:
  - "@username"
```

---

## 🛠️ Workflow Integration

### 1. PR Created by Dependabot

```
Title: chore(deps): Update Microsoft.EntityFrameworkCore from 10.0.0 to 10.0.1
Labels: dependencies, dotnet, app-blueprint
Reviewers: @saas-factory-labs/maintainers
```

### 2. Automated Checks Run

- ✅ Build verification
- ✅ Unit tests
- ✅ Integration tests
- ✅ Docker Scout scan (if Docker update)
- ✅ SBOM generation
- ✅ License compliance check

### 3. Manual Review

**Check:**
- Breaking changes in changelog
- Test results
- Security scan results
- Related issues/PRs

### 4. Merge Strategy

**If patch/minor grouped PR:**
- Review once
- Merge if all tests pass
- Deploy to staging first

**If major version PR:**
- Thorough review required
- Test in isolated environment
- Check migration guide
- Update documentation if needed

### 5. Post-Merge

**For Docker updates:**
```bash
# Update SHA256 digests
./scripts/update-docker-digests.ps1

# Commit digest updates
git add Code/**/Dockerfile*
git commit -m "chore: update Docker image digests after Dependabot merge"
```

**For .NET updates:**
- Verify Directory.Packages.props updated
- Check all projects build
- Run full test suite

---

## 📈 Monitoring & Metrics

### Dashboards

**GitHub Insights:**
- Go to: `Insights → Dependency graph → Dependabot`
- View: Open PRs, merged PRs, dismissed PRs
- Filter by: Ecosystem, date range

**Security Alerts:**
- Go to: `Security → Dependabot alerts`
- View: Active vulnerabilities
- Filter by: Severity, ecosystem, status

### Key Metrics to Track

1. **PR Merge Rate**
   - Target: >80% of non-major updates merged within 1 week
   - Indicates: Team responsiveness

2. **Time to Merge**
   - Target: <3 days for patch updates
   - Target: <1 week for minor updates
   - Target: <2 weeks for major updates

3. **Security Update SLA**
   - Target: Critical vulnerabilities patched within 24 hours
   - Target: High severity within 7 days

4. **Stale PRs**
   - Target: <5 PRs older than 2 weeks
   - Action: Review and merge or dismiss

---

## 🔧 Troubleshooting

### Issue: Too Many PRs

**Solution 1: Reduce frequency**
```yaml
schedule:
  interval: "weekly"  # Instead of daily
```

**Solution 2: Reduce limits**
```yaml
open-pull-requests-limit: 3  # Instead of 10
```

**Solution 3: Add grouping**
```yaml
groups:
  all-minor-patch:
    patterns: ["*"]
    update-types: ["minor", "patch"]
```

### Issue: Dependabot Not Creating PRs

**Check:**
1. Repository settings → Security → Dependabot alerts enabled
2. `.github/dependabot.yml` syntax valid
3. No conflicting branch protection rules
4. Not at pull request limit

**Debug:**
```bash
# Validate dependabot.yml locally
gh api /repos/saas-factory-labs/Saas-Factory/dependabot/alerts
```

### Issue: PRs Fail CI Checks

**Common causes:**
1. Breaking changes in dependency
2. Test flakiness
3. Incompatible versions

**Solution:**
```yaml
# Add version constraint
ignore:
  - dependency-name: "problematic-package"
    update-types: ["version-update:semver-major"]
```

Then update manually with proper testing.

### Issue: Conflicts with Manual Updates

**Best practice:**
1. Let Dependabot handle routine updates
2. Manual updates only for:
   - Emergency security patches
   - Major version migrations
   - Coordinated updates across projects

**Conflict resolution:**
1. Close Dependabot PR
2. Make manual update
3. Dependabot will adjust next cycle

---

## 📚 Best Practices

### 1. Review Grouped PRs Carefully

Even though minor/patch updates are grouped, check:
- Changelog highlights
- Any deprecation warnings
- Test coverage for affected code

### 2. Test Docker Updates Thoroughly

After merging Docker base image updates:
```bash
# Rebuild all images
docker-compose build

# Run integration tests
dotnet test

# Verify in staging environment
```

### 3. Keep Dependencies Current

Don't let updates pile up:
- Review weekly
- Merge safe updates promptly
- Tackle major updates in sprints

### 4. Use Dependabot Insights

```
Repository → Insights → Dependency graph → Dependabot
```

Monitor:
- Update frequency
- Merge success rate
- Time to merge

### 5. Document Breaking Changes

When merging major updates:
```markdown
# CHANGELOG.md

## [2.1.0] - 2026-06-22

### Changed
- **BREAKING**: Updated Entity Framework Core to 11.0
  - Migration required for database schema
  - See docs/migrations/efcore-11.md
```

---

## 🔗 Related Documentation

- [GitHub Dependabot Documentation](https://docs.github.com/en/code-security/dependabot)
- [Conventional Commits](https://www.conventionalcommits.org/)
- [Semantic Versioning](https://semver.org/)
- `SECURITY-ENHANCEMENTS-INDEX.md` - Overall security setup
- `docs/DOCKER-SCOUT-GUIDE.md` - Docker security scanning
- `docs/SBOM-GUIDE.md` - Software Bill of Materials

---

## 🆘 Support

**Issues with Dependabot:**
- GitHub Issues: https://github.com/saas-factory-labs/Saas-Factory/issues
- Label: `dependabot`, `dependencies`

**Configuration Questions:**
- Review: `.github/dependabot.yml`
- This guide: `docs/DEPENDABOT-GUIDE.md`

---

_Last Updated: 2026-06-22_
_Maintained by: SaaS Factory Labs Security Team_
