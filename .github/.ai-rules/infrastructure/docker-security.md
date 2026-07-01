---
description: Docker security rules including digest pinning and supply chain security
globs:
  - "**/Dockerfile"
  - "**/*.dockerfile"
---

# Docker Security

## Digest Pinning (SECURITY REQUIREMENT)

**ALWAYS** use SHA256 digest-only pinning in Dockerfiles for supply chain security. Include the tag as a comment for human readability and Dependabot compatibility. This prevents tag-mutation attacks where attackers push malicious images with the same tag.

```dockerfile
# ✅ Correct — digest pinning with tag comment (REQUIRED)
# mcr.microsoft.com/dotnet/aspnet:10.0
FROM mcr.microsoft.com/dotnet/aspnet@sha256:ddcf70ad1ab963a4fcd41fbd722a6b660e404e87567cfbd46fd2809c21b02088 AS base

# ❌ Incorrect — tag-only (vulnerable to supply chain attacks)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

# ❌ Incorrect — tag AND digest in FROM statement
FROM mcr.microsoft.com/dotnet/aspnet:10.0@sha256:ddcf... AS base
```

**Security Rationale**: Digest pinning provides cryptographic verification of image integrity per the SLSA framework and NIST SP 800-190. SonarCloud S8431 is suppressed in `.github/workflows/sonarcloud-analysis.yaml` via `/d:sonar.issue.ignore.multicriteria` parameters. **NEVER remove digest pinning** — it is a deliberate security hardening measure.

## SonarCloud Configuration for .NET Projects

SonarScanner for .NET does **NOT** support `sonar-project.properties` files. All SonarCloud configuration MUST be specified via command-line `/d:` parameters in the GitHub workflow (`.github/workflows/sonarcloud-analysis.yaml`).

- A `sonar-project.properties` file will cause the scanner to fail with exit code 1.
- All exclusions, issue suppressions, and settings are configured in the workflow's `dotnet-sonarscanner begin` step.
