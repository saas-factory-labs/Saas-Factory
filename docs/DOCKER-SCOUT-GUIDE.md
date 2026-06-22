# Docker Scout Enhanced Guide

This guide explains the enhanced Docker Scout vulnerability scanning workflow for comprehensive container security analysis.

---

## 🎯 Overview

The Enhanced Docker Scout workflow provides enterprise-grade container security analysis:

- **Multi-Service Scanning**: Web, API, and Gateway services
- **CVE Analysis**: Comprehensive vulnerability detection with SARIF export
- **Security Recommendations**: Actionable security best practices
- **SBOM Generation**: Software Bill of Materials in multiple formats
- **Policy Evaluation**: Compliance checking against security policies
- **Image Comparison**: Vulnerability drift analysis for pull requests
- **GitHub Integration**: Automated Security tab updates and PR comments

---

## 🚀 Quick Start

### Trigger Vulnerability Scan

The workflow runs automatically on:
- Push to `main` or `development` branches (if Dockerfile changes)
- Pull requests to `main` or `development` (if Dockerfile changes)
- Weekly schedule (Sundays at 2 AM UTC)
- Manual trigger via GitHub Actions UI

### Manual Trigger

```bash
# Via GitHub CLI
gh workflow run docker-scout-enhanced.yml

# With custom scan depth
gh workflow run docker-scout-enhanced.yml -f scan_depth=deep

# Via GitHub UI
Actions → "Docker Scout Enhanced Scan" → Run workflow
```

---

## 📦 What Gets Scanned

### Services

1. **Web Service** (`AppBlueprint.Web`)
   - Blazor Server frontend
   - Node.js dependencies
   - .NET runtime

2. **API Service** (`AppBlueprint.ApiService`)
   - REST API backend
   - .NET runtime
   - Database drivers

3. **Gateway Service** (`AppBlueprint.AppGateway`)
   - YARP reverse proxy
   - .NET runtime
   - Routing configuration

### Scan Types

Each service undergoes:
- **CVE Scanning**: All known vulnerabilities
- **Recommendations**: Security improvements
- **QuickView**: Executive summary
- **SBOM**: Complete dependency list
- **Policy**: Compliance evaluation
- **Comparison**: Changes from base (PRs only)

---

## 🔍 Understanding Docker Scout

### What is Docker Scout?

Docker Scout is Docker's official security analysis platform that provides:
- Real-time vulnerability intelligence
- Base image recommendations
- Security best practices
- Compliance policy evaluation
- Supply chain insights

### How It Works

```
1. Build Docker image
   ↓
2. Docker Scout analyzes layers
   ↓
3. Identifies all packages/dependencies
   ↓
4. Cross-references with CVE databases
   ↓
5. Generates reports and recommendations
   ↓
6. Uploads to GitHub Security tab
```

---

## 📊 Generated Reports

### 1. CVE Reports

**Location**: `scout-cve-reports-{service}-{sha}`

**Files**:
- `{service}-cves-all.json` - All vulnerabilities
- `{service}-cves-critical-high.json` - High-priority issues
- `{service}-cves.sarif` - GitHub Security format

**Content**:
```json
{
  "vulnerabilities": [
    {
      "id": "CVE-2024-12345",
      "severity": "HIGH",
      "package": "openssl",
      "version": "1.1.1",
      "fixedVersion": "1.1.1w",
      "description": "...",
      "cvss": 7.5
    }
  ]
}
```

### 2. Recommendations

**Location**: `scout-recommendations-{service}-{sha}`

**Content**:
- Base image updates (newer, more secure versions)
- Security configuration improvements
- Package updates available
- Best practice violations

**Example**:
```json
{
  "recommendations": [
    {
      "type": "base-image-update",
      "current": "mcr.microsoft.com/dotnet/aspnet:10.0",
      "recommended": "mcr.microsoft.com/dotnet/aspnet:10.0.1",
      "reason": "Contains security patches for CVE-2024-XXXXX"
    }
  ]
}
```

### 3. QuickView

**Location**: `scout-quickview-{service}-{sha}`

**Content**: Executive summary with:
- Total vulnerabilities by severity
- Critical issues requiring immediate attention
- Compliance status
- Risk score

### 4. SBOM (Software Bill of Materials)

**Location**: `scout-sbom-{service}-{sha}`

**Files**:
- `{service}-sbom-spdx.json` - SPDX 2.3 format
- `{service}-sbom-cyclonedx.json` - CycloneDX format

**Use Cases**:
- License compliance verification
- Dependency tracking
- Supply chain analysis
- Audit requirements

### 5. Policy Reports

**Location**: `scout-policy-{service}-{sha}`

**Content**: Compliance against security policies:
- No critical CVEs
- Base image up-to-date
- No high-risk packages
- Custom organizational policies

### 6. Comparison Reports (PRs only)

**Location**: `scout-compare-{service}-{sha}`

**Content**: Vulnerability drift analysis:
- New vulnerabilities introduced
- Fixed vulnerabilities
- Overall risk change
- Recommendation: approve/reject PR

---

## 🔐 Severity Levels

### Understanding Severity

| Severity | CVSS Score | Action Required | Example |
|----------|------------|-----------------|---------|
| **CRITICAL** | 9.0-10.0 | Immediate fix | Remote code execution |
| **HIGH** | 7.0-8.9 | Fix within 1 week | Privilege escalation |
| **MEDIUM** | 4.0-6.9 | Fix within 1 month | Information disclosure |
| **LOW** | 0.1-3.9 | Fix when convenient | Minor bugs |

### Response Times (Recommended)

- **CRITICAL**: Fix within 24 hours
- **HIGH**: Fix within 7 days
- **MEDIUM**: Fix within 30 days
- **LOW**: Fix in next maintenance window

---

## 🛠️ GitHub Integration

### Security Tab

All CVE scans are automatically uploaded to:
```
Repository → Security → Code scanning alerts
```

**Features**:
- Historical vulnerability trends
- Filtered views by severity
- Automated Dependabot alerts
- Integration with GitHub Advanced Security

### PR Comments

When a PR is created, the workflow automatically posts:

**Example Comment**:
```markdown
## 🛡️ Docker Scout Security Analysis

**Commit:** `abc1234`
**Branch:** `feature/new-api`

### 📦 Images Scanned
- ✅ Web Service
- ✅ API Service
- ✅ Gateway Service

### 🔍 Vulnerability Summary

| Service | Critical | High | Medium | Low |
|---------|----------|------|--------|-----|
| WEB     | 0        | 2    | 5      | 12  |
| API     | 0        | 1    | 3      | 8   |
| GATEWAY | 0        | 0    | 2      | 5   |

### 📋 Reports Available
- CVE Analysis (All severities)
- Security Recommendations
- Quick Overview
- SBOM (SPDX + CycloneDX)
- Image Comparison

### 🔗 Actions
- [View Workflow Run](...)
- [Download Reports](...)
- [View Security Tab](...)
```

---

## ⚙️ Configuration

### Basic Configuration

Edit `.github/docker-scout-config.yml`:

```yaml
# Enable/disable services
services:
  web:
    enabled: true
  api:
    enabled: true
  gateway:
    enabled: true

# Scanning configuration
scanning:
  fail_on:
    - critical
    # - high  # Uncomment to fail on high severity
```

### Fail Build on Vulnerabilities

To block builds with critical vulnerabilities:

```yaml
scanning:
  exit_code_enabled: true
  fail_on:
    - critical
    - high
```

### Schedule Scans

Default: Weekly on Sundays at 2 AM UTC

To change:
```yaml
scanning:
  schedule:
    enabled: true
    cron: '0 2 * * 1'  # Weekly on Mondays
```

### Ignore Specific CVEs

For known false positives or accepted risks:

```yaml
cve_scanning:
  ignore_cves:
    - CVE-2024-12345  # False positive in test dependency
    - CVE-2024-67890  # Risk accepted, documented in SECURITY.md
```

---

## 🔄 PR Integration

### Automated Security Checks

On every pull request:

1. **Build Images**: All affected services
2. **Scan for CVEs**: Comprehensive analysis
3. **Compare with Base**: Show new vulnerabilities
4. **Generate Recommendations**: Actionable fixes
5. **Post PR Comment**: Summary with links
6. **Update Security Tab**: SARIF upload

### Blocking PRs

To automatically block PRs with critical CVEs:

```yaml
scanning:
  exit_code_enabled: true
  fail_on:
    - critical
```

This will:
- ❌ Fail the workflow
- 🚫 Block merge (if branch protection enabled)
- 📝 Comment on PR with details

---

## 🎯 Best Practices

### 1. **Review Scout Reports Regularly**
- Check Security tab weekly
- Address critical/high vulnerabilities promptly
- Track vulnerability trends

### 2. **Keep Base Images Updated**
- Follow Scout recommendations for base image updates
- Test upgrades in staging first
- Document upgrade decisions

### 3. **Use SHA256 Pinning**
- Already implemented (see SECURITY-HASH-PINNING-IMPLEMENTATION.md)
- Prevents tag poisoning
- Ensures reproducible builds

### 4. **Enable Branch Protection**
```
Settings → Branches → main
✅ Require status checks to pass
✅ Require Docker Scout scan
```

### 5. **Configure Notifications**

Set up alerts for critical issues:

```yaml
notifications:
  critical_vulnerabilities:
    enabled: true
    channels:
      - slack
      - email
```

Add secrets:
```bash
gh secret set SLACK_WEBHOOK_URL
gh secret set EMAIL_RECIPIENTS
```

---

## 🔧 Troubleshooting

### Build Fails Immediately

**Problem**: "DEPRECATED WORKFLOW" error

**Solution**: Using old workflow file
```bash
# Use enhanced workflow instead
gh workflow run docker-scout-enhanced.yml
```

### Docker Login Fails

**Problem**: "authentication required"

**Solution**: Add Docker Hub credentials
```bash
gh secret set DOCKER_USER
gh secret set DOCKER_PAT
```

### Policy Evaluation Fails

**Problem**: "Policy not found"

**Solution**: Policy requires Docker Scout organization
- Free tier: Policy disabled by default
- Pro tier: Configure at https://scout.docker.com

### SARIF Upload Fails

**Problem**: "permission denied"

**Solution**: Enable Security tab
```
Settings → Security → Code security and analysis
✅ Enable Dependency graph
✅ Enable Dependabot alerts
```

### Scan Takes Too Long

**Problem**: Workflow timeout

**Solution**: Use quick scan mode
```bash
gh workflow run docker-scout-enhanced.yml -f scan_depth=quick
```

Or adjust timeout in config:
```yaml
performance:
  timeout: 60  # minutes
```

---

## 📈 Understanding Results

### CVE Score Interpretation

**CVSS (Common Vulnerability Scoring System)**:
- 0.0: No vulnerability
- 0.1-3.9: Low risk
- 4.0-6.9: Medium risk
- 7.0-8.9: High risk
- 9.0-10.0: Critical risk

### Exploitability Factors

Docker Scout considers:
- **Network Access**: Remote vs. local exploitation
- **Privileges Required**: None vs. admin needed
- **User Interaction**: None vs. user action required
- **Complexity**: Easy vs. difficult to exploit

### False Positives

Common causes:
- Vulnerability in unused code path
- Protected by configuration
- Only affects different OS/architecture
- Already patched in your build

**Handling**:
1. Verify it's truly a false positive
2. Document in SECURITY.md
3. Add to ignore list in config
4. Report to Docker Scout if needed

---

## 📚 Advanced Features

### Custom Policies

Requires Docker Scout organization:

```yaml
policy:
  enabled: true
  enforce: true
  policies:
    - no-critical-cves
    - base-image-up-to-date
    - no-high-risk-packages
    - custom-policy-name
```

Create custom policies at: https://scout.docker.com/policies

### Dependency-Track Integration

Export SBOMs to Dependency-Track:

```yaml
integration:
  dependency_track:
    enabled: true
    url: https://dependency-track.example.com
    api_key: ${{ secrets.DEPENDENCY_TRACK_API_KEY }}
```

### GUAC Integration

Graph for Understanding Artifact Composition:

```yaml
integration:
  guac:
    enabled: true
    endpoint: https://guac.example.com
```

### Multi-Registry Support

Scan images from multiple registries:

```yaml
advanced:
  registry:
    primary:
      url: docker.io
      username: ${{ secrets.DOCKER_USER }}
      password: ${{ secrets.DOCKER_PAT }}

    secondary:
      url: ghcr.io
      username: ${{ secrets.GHCR_USER }}
      password: ${{ secrets.GHCR_TOKEN }}
```

---

## 🔗 Related Tools

### Docker Scout CLI

Install locally for development:
```bash
# Install
curl -sSfL https://raw.githubusercontent.com/docker/scout-cli/main/install.sh | sh

# Scan local image
docker scout cves myimage:latest

# Get recommendations
docker scout recommendations myimage:latest

# Compare images
docker scout compare myimage:latest myimage:v1.0.0
```

### Docker Desktop Integration

Docker Scout is integrated into Docker Desktop:
1. Open Docker Desktop
2. Go to Images tab
3. Click on an image
4. View "Scout" tab for analysis

---

## 📖 Further Reading

### Docker Scout Documentation
- [Official Docs](https://docs.docker.com/scout/)
- [CLI Reference](https://docs.docker.com/engine/reference/commandline/scout/)
- [Policy Guide](https://docs.docker.com/scout/policy/)

### Security Standards
- [CIS Docker Benchmark](https://www.cisecurity.org/benchmark/docker)
- [NIST Container Security](https://csrc.nist.gov/publications/detail/sp/800-190/final)
- [OWASP Container Security](https://owasp.org/www-project-docker-top-10/)

### Vulnerability Databases
- [National Vulnerability Database](https://nvd.nist.gov/)
- [CVE Details](https://www.cvedetails.com/)
- [Docker Security Advisories](https://docs.docker.com/security/)

---

## 🆘 Support

### Issues
Report workflow issues: https://github.com/saas-factory-labs/Saas-Factory/issues

### Configuration Help
- Review: `.github/docker-scout-config.yml`
- See: SECURITY-ENHANCEMENTS-INDEX.md

### Security Concerns
- Security Tab: Check for automated alerts
- Email: security@saas-factory.com (if configured)

---

## 📝 Changelog

### v2.0 - Enhanced Docker Scout (Current)
- ✅ Multi-service scanning (Web, API, Gateway)
- ✅ Comprehensive CVE analysis (all severities)
- ✅ Security recommendations
- ✅ SBOM generation (SPDX + CycloneDX)
- ✅ Policy evaluation
- ✅ Image comparison for PRs
- ✅ GitHub Security integration (SARIF)
- ✅ Automated PR comments
- ✅ Scheduled weekly scans

### v1.0 - Basic Docker Scout (Deprecated)
- Single service scanning
- CVE detection only (critical/high)
- Manual trigger only
- No recommendations
- No SBOM generation

---

_Last Updated: 2026-06-22_
_Maintained by: SaaS Factory Labs Security Team_
