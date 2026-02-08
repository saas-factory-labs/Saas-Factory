# GitHub Actions SHA Reference

**Last Updated**: 2026-02-08  
**Status**: ✅ **ALL ACTIONS SECURED** - All GitHub Actions now use immutable commit SHA references

This file maintains the commit SHA hashes for GitHub Actions used in workflows to ensure supply chain security.

## ✅ Completed SHA Implementations

The following actions have been successfully updated with commit SHAs across all workflows:

### actions/checkout
- ✅ `v6.0.2` → `de0fac2e4500dabe0009e67214ff5f5447ce83dd` (RazorPress)
- ✅ `v4.3.1` → `34e114876b0b11c390a56381ad16ebd13914f8d5` (All main workflows)
- ✅ `v4.2.2` → `11bd71901bbe5b1630ceea73d27597364c9af683` (scorecard.yml)
- ✅ All v3 references upgraded to v4.3.1

### actions/setup-dotnet
- ✅ `v5.1.0` → `baa11fbfe1d6520db94683bd5c7a3818018e4309` (All workflows)
- ✅ All v3 and v4 references upgraded to v5.1.0

### actions/upload-artifact
- ✅ `v4.6.1` → `4cec3d8aa04e39d1a68397de0c4cd6fb9dce8ec1` (All workflows)
- ✅ All v3 references upgraded to v4.6.1

### actions/setup-node
- ✅ `v6.2.0` → `6044e13b5dc448c55e2357c09f80417699197238` (deploy-combined-github-pages.yml)

###🎯 Supply Chain Security Achievement

All GitHub Actions across **20 unique actions** in **10 workflow files** now use immutable commit SHA references:

### Files Updated
- ✅ `.github/workflows/deploy-combined-github-pages.yml` (4 actions)
- ✅ `.github/workflows/docker-scout-vulnerability-scan.yml` (5 actions)
- ✅ `.github/workflows/generate-sbom.yml` (4 actions)
- ✅ `.github/workflows/git-guardian.yml` (1 action)
- ✅ `.github/workflows/publish-container-image-github-registry.yml` (5 actions)
- ✅ `.github/workflows/auto-release.yml` (1 action)
- ✅ `.github/workflows/scorecard.yml` (1 action)
- ✅ `.github/workflows/open-telemetry-tracing.yml` (1 action)
- ✅ `.github/workflows/bytebase-sql-review.yml` (1 action)
- ✅ `docs/RazorPress/.github/workflows/build.yml` (1 action)
- ✅ Previous sessions: CI workflows and other core actions

### Security Impact
- **Zero mutable references** remaining (all tags and branches replaced with 40-character SHAs)
- **Tag retargeting attacks prevented** across entire CI/CD pipeline
- **Version upgrades applied** where beneficial (v1→v4, v3→v4 for several actions)
- **Audit trail established** with version comments alongside each SHA
   - Get SHA from: https://github.com/actions/deploy-pages/releases

### Docker Actions (Upgrade & SHA Needed)
7. **docker/setup-buildx-action** (upgrade `@v2` → `@v3`)
   - Files: `generate-sbom.yml`, `docker-scout-vulnerability-scan.yml`
   - Get v3 SHA from: https://github.com/docker/setup-buildx-action/releases

8. **docker/login-action** (upgrade `@v2` → `@v3`)
   - File: `docker-scout-vulnerability-scan.yml`
   - Get v3 SHA from: https://github.com/docker/login-action/releases

9. **docker/metadata-action** (upgrade `@v4` → `@v5`)
   - File: `docker-scout-vulnerability-scan.yml`
   - Get v5 SHA from: https://github.com/docker/metadata-action/releases

10. **docker/build-push-action** (upgrade `@v4` → `@v5`)
    - File: `docker-scout-vulnerability-scan.yml`
    - Get v5 SHA from: https://github.com/docker/build-push-action/releases

11. **docker/** actions in `publish-container-image-github-registry.yml**
    - `docker/setup-buildx-action@v3`
    - `docker/login-action@v3`
    - `docker/metadata-action@v5`
    - `docker/build-push-action@v5`
    - Get SHAs from respective release pages

### Security & Scanning
12. **GitGuardian/ggshield-action** (`@v1`)
    - File: `git-guardian.yml`
    - Get SHA from: https://github.com/GitGuardian/ggshield-action/releases

13. **anchore/sbom-action** (`@v0.1.3`)
    - File: `generate-sbom.yml`
    - Get SHA from: https://github.com/anchore/sbom-action/releases

14. **sigstore/cosign-installer** (`@v3.5.0`)
    - File: `publish-container-image-github-registry.yml`
    - Get SHA from: https://github.com/sigstore/cosign-installer/releases

### Third-Party
15. **peaceiris/actions-gh-pages** (`@v3`)
    - File: `docs/RazorPress/.github/workflows/build.yml`
    - Get SHA from: https://github.com/peaceiris/actions-gh-pages/releases

16. **corentinmusard/otel-cicd-action** (`@v1`)
    - File: `open-telemetry-tracing.yml`
    - Get SHA from: https://github.com/corentinmusard/otel-cicd-action/releases

### corentinmusard/otel-cicd-action
- ✅ `v4.0.0` → `0f9f16fceb53fd8c996042e28c642ec61f844876` (open-telemetry-tracing.yml)
- ✅ Upgraded from v1 to v4

### bytebase/sql-review-action
- ✅ Latest from main branch (Jan 22, 2026) → `8ccd33832304934154de305b91cd9d09370460cf` (bytebase-sql-review.yml)
- Note: Action is in `/action` subdirectory of main bytebase/bytebase repository

## Why Use Commit SHAs?

- **Immutable**: Tags can be force-pushed, SHAs cannot be changed
- **Security**: Prevents malicious code injection via tag retargeting attack
- **Auditable**: Exact version of code can be verified
- **Best Practice**: Recommended by OSSF Scorecard and security guidelines

## Official GitHub Actions

### actions/checkout
- `v6.0.2` → `de0fac2e4500dabe0009e67214ff5f5447ce83dd`
- `v4.3.1` → `34e114876b0b11c390a56381ad16ebd13914f8d5` ✅
- `v4.2.2` → `11bd71901bbe5b1630ceea73d27597364c9af683` ✅ (from scorecard.yml)
- `v3.x` → **UPGRADE TO v4.x** (v3 is outdated)

### actions/setup-dotnet
- `v5.1.0` → `baa11fbfe1d6520db94683bd5c7a3818018e4309`
- `v4.3.1` → `67a3573c9a986a3f9c594539f4ab511d57bb3ce9`
- `v3.4.2` → `55ec9447dda3d1cf6bd587150f3262f30ee10815`

### actions/setup-node
- `v6.2.0` → `6044e13b5dc448c55e2357c09f80417699197238` ✅

### actions/upload-artifact
- `v4.6.1` → `4cec3d8aa04e39d1a68397de0c4cd6fb9dce8ec1` ✅ (from scorecard.yml)
- `v3.x` → **UPGRADE TO v4.x**

### actions/upload-pages-artifact
- `v4.0.0` → `7b1f4a764d45c48632c6b24a0339c27f5614fb0b` ✅

### actions/configure-pages
- `v5.0.0` → `983d7736d9b0ae728b81ab479565c72886d7745b` ✅

### actions/deploy-pages
- `v4.0.5` → `d6db90164ac5ed86f2b6aed7e0febac5b3c0c03e` ✅

## Docker Actions

### docker/setup-buildx-action
- `v3.12.0` → `8d2750c68a42422c14e847fe6c8ac0403b4cbd6f` ✅

### docker/login-action
- `v3.7.0` → `c94ce9fb468520275223c153574b00df6fe4bcc9` ✅

### docker/metadata-action
- `v5.10.0` → `c299e40c65443455700f0fdfc63efafe5b349051` ✅

### docker/build-push-action
- `v6.18.0` → `263435318d21b8e681c14492fe198d362a7d2c83` ✅

### docker/scout-action
- `v1.18.2` → `f8c776824083494ab0d56b8105ba2ca85c86e4de` ✅

## Security Scanning Actions

### ossf/scorecard-action
- `v2.4.1` → `f49aabe0b5af0936a0987cfb85d86b75731b0186` ✅ (from scorecard.yml)

### github/codeql-action/upload-sarif
- `v3.x` → Visit https://github.com/github/codeql-action/releases
4.32.2` → `45cbd0c69e560cd9e7cd7f8c32362050c9b7ded2` ✅

### GitGuardian/ggshield-action
- `v1.47.0` → `247ad4911092eee12989d379779c115632de6f84` ✅

### anchore/sbom-action
- `v0.22.2` → `28d71544de8eaf1b958d335707167c5f783590ad` ✅
## Signing & Publishing Actions

### sigstore/cosign-installer
- `v4.0.0` → `faadad0cce49287aee09b3a48701e75088a2c6ad` ✅

### peaceiris/actions-gh-pages
- `v4.0.0` → `4f9cc6602d3f66b9c108549d475ec49e8ef4d45e` ✅

## Third-Party Actions

### bytebase/sql-review-action
- `main` → Visit https://github.com/Bytebase/sql-review-action/commits/main (get latest commit SHA)

### corentinmusard/otel-cicd-action  
- `v1.x` → Visit https://github.com/corentinmusard/otel-cicd-action/releases

## How to Update This File

1. Visit the action's GitHub releases page
2. Find the latest release or specific version you want
3. Click on the commit hash (short SHA)
4. Copy the full 40-character SHA from the URL or commit page
5. Update this file with format: `vX.Y.Z` → `<full-sha>`
6. Add ✅ emoji when implemented in workflows

## How to Update Workflows

Replace mutable references:
```yaml
# ❌ BAD - mutable tag
uses: actions/checkout@v4

# ✅ GOOD - immutable SHA with version comment  
uses: actions/checkout@34e114876b0b11c390a56381ad16ebd13914f8d5  # v4.3.1
```

## Security Notes

- **Never use `@main` or `@master`** - these are constantly changing
- **Never use major version tags alone** (e.g., `@v4`) without SHA - they can be retargeted
- **Always add version comment** after SHA for human readability
- **Update quarterly** or when security advisories are published
- **Verify SHA** matches the official release before updating workflows
