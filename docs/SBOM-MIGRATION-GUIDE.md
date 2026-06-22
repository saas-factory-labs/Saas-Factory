# SBOM Workflow Migration Guide

**Migration from:** Basic SBOM (`generate-sbom.yml`)
**Migration to:** Enhanced SBOM (`generate-sbom-enhanced.yml`)
**Date:** 2026-06-22

---

## 🚀 Why Migrate?

The enhanced SBOM workflow provides significant improvements over the basic version:

| Feature | Basic (Old) | Enhanced (New) |
|---------|-------------|----------------|
| **SBOM Formats** | CycloneDX only | SPDX 2.3 + CycloneDX 1.6 |
| **Ecosystems** | Docker only | .NET + npm + Docker |
| **Vulnerability Scanning** | ❌ None | ✅ Grype + Trivy + OSV |
| **License Compliance** | ❌ None | ✅ Automated checking |
| **SBOM Signing** | ❌ None | ✅ Cosign attestation |
| **PR Integration** | ❌ None | ✅ Automatic comments |
| **Security Tab** | ❌ No integration | ✅ Uploads to GitHub Security |
| **Dependency Tracking** | ❌ None | ✅ Drift detection |
| **Validation** | ❌ None | ✅ Format validation |
| **Retention** | 5 days | 90 days |

---

## 📋 Migration Steps

### Step 1: Review Configuration

1. Open `.github/sbom-config.yml`
2. Review and customize settings:
   ```yaml
   ecosystems:
     dotnet:
       enabled: true
     npm:
       enabled: true
     docker:
       enabled: true
   ```

3. Configure license compliance:
   ```yaml
   license_compliance:
     approved_licenses:
       - MIT
       - Apache-2.0
     fail_on_restricted: false  # Set true to block builds
   ```

### Step 2: Update Workflow Triggers

The enhanced workflow runs automatically on:
- Push to `main` or `development`
- Pull requests
- Releases

**No action needed** - it's already configured!

### Step 3: Enable Required Permissions

Ensure your repository has these permissions enabled:

**Settings → Actions → General → Workflow permissions:**
- ✅ Read and write permissions
- ✅ Allow GitHub Actions to create and approve pull requests

**Settings → Security → Code security and analysis:**
- ✅ Dependency graph (enabled)
- ✅ Dependabot alerts (enabled)

### Step 4: Configure Secrets (Optional)

For enhanced features, add these secrets:

**Settings → Secrets → Actions:**

```bash
# For Docker Scout (optional)
DOCKER_USER=your-docker-username
DOCKER_PAT=your-docker-personal-access-token

# For Slack notifications (optional)
SLACK_WEBHOOK_URL=https://hooks.slack.com/services/...

# For email notifications (optional)
EMAIL_RECIPIENTS=security@yourcompany.com
```

### Step 5: Test Enhanced Workflow

1. Create a test branch:
   ```bash
   git checkout -b test/enhanced-sbom
   ```

2. Make a small change (e.g., update README)

3. Push and create PR:
   ```bash
   git add .
   git commit -m "test: trigger enhanced SBOM workflow"
   git push origin test/enhanced-sbom
   ```

4. Verify workflow runs:
   - Check GitHub Actions tab
   - Review PR comment
   - Download artifacts

### Step 6: Deprecate Old Workflow

The old workflow is already marked as deprecated. After successful testing:

1. Verify all teams are using enhanced workflow
2. Archive old SBOM artifacts (if needed)
3. Remove old workflow file (optional):
   ```bash
   git rm .github/workflows/generate-sbom.yml
   ```

---

## 🔄 What Changed?

### SBOM Generation

**Before (Basic):**
```yaml
- name: Generate SBOM
  uses: anchore/sbom-action@v0.22.2
  with:
    image: your-docker-image:latest
    output: sbom.json
    path: .
```

**After (Enhanced):**
```yaml
# .NET SBOM (CycloneDX)
- dotnet CycloneDX . -o sbom-outputs -f sbom-dotnet.json

# .NET SBOM (SPDX)
- sbom-tool generate -b ./sbom-outputs -bc ./Code/AppBlueprint

# npm SBOM
- npx @cyclonedx/cyclonedx-npm --output-file sbom-npm.json

# Docker SBOM (with Syft)
- syft image:latest -o spdx-json=sbom-docker-spdx.json
- syft image:latest -o cyclonedx-json=sbom-docker-cyclonedx.json
```

### Vulnerability Scanning (NEW!)

```yaml
# Grype (Anchore)
- grype sbom:sbom-dotnet.json -o json --fail-on critical

# Trivy (Aqua Security)
- trivy fs sbom-outputs/ --format sarif --output trivy-results.sarif

# OSV Scanner (Google)
- osv-scanner --lockfile package-lock.json --format json
```

### License Compliance (NEW!)

```yaml
- license-checker --json --out npm-licenses.json
- # Parse and validate against approved list
```

### SBOM Attestation (NEW!)

```yaml
# Sign with Cosign (keyless)
- cosign sign-blob sbom.json --bundle sbom.json.bundle --yes

# GitHub Attestation
- uses: actions/attest-sbom@v2
  with:
    subject-path: sbom.json
    sbom-path: sbom.json
```

---

## 📦 Artifact Structure

### Before (Basic)
```
Artifacts/
└── sbom/
    └── sbom.json  (Single file, CycloneDX)
```

### After (Enhanced)
```
Artifacts/
├── sbom-all-formats-{sha}/
│   ├── sbom-dotnet.json              # .NET (CycloneDX)
│   ├── _manifest/spdx_2.2/           # .NET (SPDX)
│   ├── sbom-npm-*.json               # npm dependencies
│   ├── sbom-docker-web-spdx.json     # Docker Web (SPDX)
│   ├── sbom-docker-web-cyclonedx.json # Docker Web (CycloneDX)
│   ├── sbom-docker-api-spdx.json     # Docker API (SPDX)
│   ├── sbom-docker-api-cyclonedx.json # Docker API (CycloneDX)
│   └── SBOM-MANIFEST.md              # Summary
│
├── vulnerability-reports-{sha}/
│   ├── sbom-dotnet-vulnerabilities.json
│   ├── trivy-results.sarif
│   ├── osv-results.json
│   └── VULNERABILITY-SUMMARY.txt
│
├── license-reports-{sha}/
│   ├── npm-licenses-root.json
│   ├── npm-licenses-appblueprint.json
│   └── LICENSE-COMPLIANCE.md
│
└── dependency-reports-{sha}/
    └── DEPENDENCY-DRIFT.md
```

---

## 🔍 Finding Your SBOMs

### GitHub Actions Artifacts

1. Go to: `Actions → Workflow runs`
2. Click on a workflow run
3. Scroll to **Artifacts** section
4. Download:
   - `sbom-all-formats-{sha}` - All SBOM files
   - `vulnerability-reports-{sha}` - Security scans
   - `license-reports-{sha}` - License analysis

### GitHub Releases

For tagged releases, SBOMs are automatically attached:

1. Go to: `Releases`
2. Click on a release
3. Scroll to **Assets**
4. Download SBOM files

---

## 🎯 Use Cases

### Development Team

**Old Workflow:**
- Manually triggered
- Limited visibility
- No security feedback

**New Workflow:**
- Automatic on every PR
- Security summary in PR comments
- Block merges on critical vulnerabilities

### Security Team

**Old Workflow:**
- Download SBOM manually
- Run separate vulnerability scans
- Manual license review

**New Workflow:**
- Automated vulnerability scanning
- Centralized in GitHub Security tab
- License compliance reports

### Compliance Team

**Old Workflow:**
- Single SBOM format
- Manual archival
- No attestation

**New Workflow:**
- Industry-standard formats (SPDX + CycloneDX)
- 90-day retention
- Cryptographic attestation

---

## ⚠️ Breaking Changes

### None!

The enhanced workflow is **fully backward compatible**. Old SBOMs remain accessible in workflow artifacts.

---

## 🆘 Troubleshooting

### Issue: Workflow Fails on First Run

**Cause:** Missing dependencies or configuration

**Solution:**
```bash
# Check .NET SDK version
dotnet --version  # Should be 10.0.x

# Restore dependencies
cd Code/AppBlueprint
dotnet restore
```

### Issue: Docker SBOM Generation Fails

**Cause:** Docker images not built

**Solution:**
```bash
# Build images before generating SBOM
docker build -f Code/AppBlueprint/AppBlueprint.Web/Dockerfile -t sbom-web .
docker build -f Code/AppBlueprint/AppBlueprint.ApiService/Dockerfile -t sbom-api .
```

### Issue: PR Comment Not Posted

**Cause:** Insufficient permissions

**Solution:**
1. Go to: Settings → Actions → General
2. Enable: "Allow GitHub Actions to create and approve pull requests"

### Issue: Vulnerability Scan Rate Limited

**Cause:** Too many API calls to vulnerability databases

**Solution:**
- Add `GITHUB_TOKEN` to increase rate limits
- Use `scan_depth: quick` input for faster scans

---

## 📊 Metrics & KPIs

Track these metrics with the enhanced workflow:

### Before Migration
- **SBOM Coverage**: 30% (Docker images only)
- **Vulnerability Detection**: Manual
- **License Compliance**: Unknown
- **SBOM Freshness**: On-demand only

### After Migration
- **SBOM Coverage**: 100% (.NET + npm + Docker)
- **Vulnerability Detection**: Automated (3 scanners)
- **License Compliance**: Automated checks
- **SBOM Freshness**: Every commit + PR

---

## 🎓 Training Resources

### Documentation
- **SBOM Guide**: `docs/SBOM-GUIDE.md`
- **Configuration**: `.github/sbom-config.yml`
- **Workflow**: `.github/workflows/generate-sbom-enhanced.yml`

### External Resources
- [CISA SBOM](https://www.cisa.gov/sbom)
- [CycloneDX Docs](https://cyclonedx.org/docs/)
- [SPDX Spec](https://spdx.github.io/spdx-spec/)
- [Grype Docs](https://github.com/anchore/grype)

---

## ✅ Post-Migration Checklist

- [ ] Review `.github/sbom-config.yml` configuration
- [ ] Test enhanced workflow on a PR
- [ ] Verify artifacts are generated correctly
- [ ] Check GitHub Security tab for Trivy results
- [ ] Review PR comment formatting
- [ ] Configure optional secrets (Slack, email)
- [ ] Train team on new workflow features
- [ ] Update internal documentation
- [ ] Archive old SBOM artifacts (if needed)
- [ ] Remove deprecated workflow file (optional)

---

## 🚀 Next Steps

After successful migration:

1. **Enable Automated Workflows**
   - Ensure triggers are active for main/development branches
   - Review PR integration settings

2. **Set Up Notifications**
   - Configure Slack webhook for critical vulnerabilities
   - Set up email alerts for license violations

3. **Integrate with Tools**
   - Import SBOMs into Dependency-Track
   - Connect to GUAC for supply chain analysis

4. **Regular Reviews**
   - Weekly vulnerability report reviews
   - Monthly license compliance audits
   - Quarterly SBOM archival

---

## 📞 Support

**Issues:** https://github.com/saas-factory-labs/Saas-Factory/issues
**Documentation:** `docs/SBOM-GUIDE.md`
**Configuration Help:** Review `.github/sbom-config.yml`

---

_Migration Complete! 🎉_
_You now have enterprise-grade SBOM generation and security analysis._
