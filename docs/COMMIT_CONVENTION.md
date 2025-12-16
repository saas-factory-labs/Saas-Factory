# Commit Convention Guide

This project uses [Conventional Commits](https://www.conventionalcommits.org/) for automated semantic versioning and changelog generation.

## Commit Message Format

```
<type>[optional scope]: <description>

[optional body]

[optional footer(s)]
```

## Types and Version Bumps

### Major Version Bump (Breaking Changes)
- `feat!: breaking change description`
- `fix!: breaking fix description`
- Or include `BREAKING CHANGE:` in the commit body/footer

**Examples:**
```bash
git commit -m "feat!: remove deprecated authentication methods"

git commit -m "refactor: update API endpoints

BREAKING CHANGE: The /api/v1/users endpoint has been removed.
Use /api/v2/users instead."
```

### Minor Version Bump (New Features)
- `feat: feature description`

**Examples:**
```bash
git commit -m "feat: add user profile photo upload"
git commit -m "feat(auth): implement OAuth2 support"
git commit -m "feat(api): add pagination to team members endpoint"
```

### Patch Version Bump (Bug Fixes & Improvements)
- `fix: bug fix description`
- `perf: performance improvement`
- `docs: documentation changes`
- `style: code style changes`
- `refactor: code refactoring`
- `test: test changes`
- `build: build system changes`
- `ci: CI/CD changes`

**Examples:**
```bash
git commit -m "fix: resolve null reference in profile service"
git commit -m "fix(auth): prevent token refresh race condition"
git commit -m "perf: optimize database query for large datasets"
git commit -m "docs: update README with deployment instructions"
git commit -m "refactor: extract email validation logic to helper"
git commit -m "test: add unit tests for payment processing"
git commit -m "ci: update NuGet publishing workflow"
```

### No Version Bump
- `chore: maintenance task description`

**Examples:**
```bash
git commit -m "chore: update dependencies"
git commit -m "chore: clean up unused imports"
```

## Scopes (Optional but Recommended)

Use scopes to indicate which part of the codebase is affected:

**Common scopes:**
- `auth` - Authentication & authorization
- `api` - API endpoints
- `ui` - User interface
- `db` - Database
- `payment` - Payment processing
- `email` - Email functionality
- `nuget` - NuGet package changes
- `infra` - Infrastructure code
- `domain` - Domain layer
- `app` - Application layer

**Examples:**
```bash
git commit -m "feat(auth): add two-factor authentication"
git commit -m "fix(payment): handle Stripe webhook timeout"
git commit -m "docs(api): document rate limiting behavior"
```

## Multi-line Commits

For more complex changes, use the body and footer:

```bash
git commit -m "feat(api): add bulk user import endpoint

This new endpoint allows administrators to import multiple users
from a CSV file. Includes validation, error handling, and progress
reporting.

Closes #123
Refs #456"
```

## Real-World Examples

### Adding NuGet Package Features
```bash
# Minor version bump (new feature)
git commit -m "feat(nuget): add README files to Contracts and SDK packages"

# Patch version bump (improvement)
git commit -m "docs(nuget): improve API client SDK documentation"

# Patch version bump (fix)
git commit -m "fix(nuget): correct package icon path in .csproj files"
```

### Breaking Changes
```bash
# Major version bump
git commit -m "feat(api)!: change authentication to OAuth2 only

BREAKING CHANGE: JWT authentication has been removed.
All clients must migrate to OAuth2 authentication."
```

### Bug Fixes
```bash
# Patch version bump
git commit -m "fix(auth): prevent duplicate user registration"

# With more detail
git commit -m "fix(payment): handle Stripe API timeout gracefully

Previously, payment processing would fail silently when Stripe API
timed out. Now we retry up to 3 times and provide clear error messages
to users.

Fixes #789"
```

## Version Bump Summary

| Commit Type | Example | Version Bump | New Version |
|-------------|---------|--------------|-------------|
| `feat!:` or `BREAKING CHANGE:` | `feat!: remove old API` | **Major** | 1.0.0 → 2.0.0 |
| `feat:` | `feat: add new feature` | **Minor** | 1.0.0 → 1.1.0 |
| `fix:`, `perf:`, `docs:`, etc. | `fix: bug fix` | **Patch** | 1.0.0 → 1.0.1 |
| `chore:` | `chore: update deps` | **None** | 1.0.0 → 1.0.0 |

## How It Works

1. **Commit with conventional format** to `main` or `develop` branch
2. **Semantic Release runs** automatically via GitHub Actions
3. **Version calculated** based on commit types since last release
4. **Tag created** (e.g., `v1.2.3`)
5. **CHANGELOG.md updated** with release notes
6. **GitHub Release created** with notes
7. **NuGet publish triggered** automatically by the new tag

## Tips

### ✅ Good Commits
```bash
feat(auth): add password reset functionality
fix(api): resolve 500 error on user creation
docs: update installation instructions
perf(db): optimize user query with index
```

### ❌ Bad Commits
```bash
updated stuff
fixed things
changes
wip
.
```

## Checking Your Commits

Before pushing, verify your commit messages:

```bash
# View recent commits
git log --oneline -5

# If you need to amend the last commit message
git commit --amend -m "feat: correct commit message"
```

## Related Documentation

- [Conventional Commits Specification](https://www.conventionalcommits.org/)
- [Semantic Versioning](https://semver.org/)
- [Keep a Changelog](https://keepachangelog.com/)

## Questions?

If unsure which type to use:
- Adding functionality users will notice? → `feat:`
- Fixing something that was broken? → `fix:`
- Everything else (refactoring, docs, tests, etc.)? → Use the appropriate type (`refactor:`, `docs:`, `test:`, etc.)
