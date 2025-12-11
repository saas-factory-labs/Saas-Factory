# NuGet Publishing Guide for AppBlueprint Packages

## Current Status

✅ **What's Already Set Up:**
1. **GitHub Actions Workflow** exists at `.github/workflows/publish-nuget-packages.yml`
2. **Package Metadata** configured in all `.csproj` files with:
   - PackageId (e.g., `SaaS-Factory.AppBlueprint.Infrastructure`)
   - Authors, Description, Tags
   - License (MIT)
   - Repository URL
   - Package Icons
   - README files
3. **GitVersion** configuration for semantic versioning
4. **All 8 packages** listed in workflow matrix:
   - AppBlueprint.SharedKernel
   - AppBlueprint.Contracts
   - AppBlueprint.Domain
   - AppBlueprint.Application
   - AppBlueprint.Infrastructure
   - AppBlueprint.Presentation.ApiModule
   - AppBlueprint.UiKit
   - AppBlueprint.Api.Client.Sdk

## ❌ What's Missing to Publish

### 1. **NuGet.org API Key (CRITICAL)**
**Status:** Not configured

**Required Actions:**
1. **Create NuGet.org Account** (if you don't have one):
   - Go to https://www.nuget.org/
   - Sign up or sign in

2. **Generate API Key**:
   - Navigate to: https://www.nuget.org/account/apikeys
   - Click "Create"
   - Key Name: `SaaS-Factory-AppBlueprint-Packages`
   - Glob Pattern: `SaaS-Factory.AppBlueprint.*`
   - Select Package Owner: (your account)
   - Select Scopes: `Push new packages and package versions`
   - Expiration: Choose appropriate duration (e.g., 365 days)
   - Click "Create"

3. **Add to GitHub Secrets**:
   - Go to: https://github.com/saas-factory-labs/Saas-Factory/settings/secrets/actions
   - Click "New repository secret"
   - Name: `NUGET_API_KEY`
   - Value: (paste your API key)
   - Click "Add secret"

### 2. **Missing README.md Files**
**Status:** ✅ ALL README FILES NOW CREATED

**README Files:**
- ✅ `AppBlueprint.Contracts/README.md` (NEWLY CREATED)
- ✅ `AppBlueprint.Api.Client.Sdk/README.md` (NEWLY CREATED)
- ✅ All other packages already had READMEs

### 3. **Package Icon Validation**
**Status:** ✅ ALL ICONS VERIFIED AND EXIST

**Icon Files:**
- ✅ `assets/icons/infrastructure-icon.png`
- ✅ `assets/icons/domain-icon.png`
- ✅ `assets/icons/application-icon.png`
- ✅ `assets/icons/presentation-icon.png`
- ✅ `assets/icons/sharedkernel-icon.png`
- ✅ `assets/icons/uikit-icon.png`
- ✅ `assets/icons/contracts-icon.png`
- ✅ `assets/icons/api-client-sdk-icon.png`

### 4. **Git Tag for Version**
**Status:** Workflow triggered by version tags

**Required Actions:**
To trigger the first publish:
```bash
# Create and push a version tag
git tag v0.1.0
git push origin v0.1.0
```

Or use **manual workflow dispatch** from GitHub Actions UI.

### 5. **Pre-Release Testing**
**Status:** Packages not yet published

**Recommended Actions:**
1. **Test Local Pack**:
   ```powershell
   # Test packing each package locally
   cd Code/AppBlueprint/Shared-Modules/AppBlueprint.Infrastructure
   dotnet pack --configuration Release --output ./bin/Release
   ```

2. **Validate Package Contents**:
   ```powershell
   # Install NuGet package explorer (optional)
   dotnet tool install -g NuGetPackageExplorer
   
   # Or extract and inspect .nupkg file (it's a zip)
   ```

3. **Test in Sample Project**:
   - Create a test project
   - Add local package source
   - Install and test packages

## 📋 Publishing Checklist

### Pre-Publishing
- [ ] Create NuGet.org account
- [ ] Generate NuGet API key
- [ ] Add `NUGET_API_KEY` to GitHub Secrets
- [ ] Create missing README files (Contracts, Api.Client.Sdk)
- [ ] Verify all package icons exist
- [ ] Review and update package descriptions/tags in `.csproj` files
- [ ] Test local package generation (`dotnet pack`)
- [ ] Review GitVersion.yml configuration
- [ ] Verify all packages build without errors
- [ ] Check that no testing dependencies remain in production packages ✅ (completed)

### First Publish
- [ ] Create git tag `v0.1.0`
- [ ] Push tag to trigger workflow: `git push origin v0.1.0`
- [ ] Monitor GitHub Actions workflow execution
- [ ] Verify packages appear on NuGet.org
- [ ] Test installing packages from NuGet.org
- [ ] Update README badges with actual NuGet version links

### Post-Publishing
- [ ] Document version history in CHANGELOG.md
- [ ] Update main README with installation instructions
- [ ] Announce packages on relevant channels
- [ ] Set up NuGet package vulnerability scanning
- [ ] Configure package retention policies

## 🔄 Publishing Workflow

### Automated Publishing (Recommended)
**Trigger:** Push a version tag (e.g., `v1.0.0`, `v1.2.3-beta`)

```bash
# 1. Ensure all changes are committed
git add .
git commit -m "Release v0.1.0"

# 2. Create and push tag
git tag v0.1.0
git push origin main
git push origin v0.1.0

# 3. GitHub Actions will automatically:
#    - Build all packages
#    - Generate version from GitVersion
#    - Pack packages
#    - Push to NuGet.org
```

### Manual Publishing
**Trigger:** Via GitHub Actions UI

1. Go to: https://github.com/saas-factory-labs/Saas-Factory/actions/workflows/publish-nuget-packages.yml
2. Click "Run workflow"
3. Select branch (usually `main`)
4. Click "Run workflow"

## 📦 Package Naming Convention

Current naming: `SaaS-Factory.AppBlueprint.*`

**Published Package IDs:**
- `SaaS-Factory.AppBlueprint.SharedKernel`
- `SaaS-Factory.AppBlueprint.Contracts`
- `SaaS-Factory.AppBlueprint.Domain`
- `SaaS-Factory.AppBlueprint.Application`
- `SaaS-Factory.AppBlueprint.Infrastructure`
- `SaaS-Factory.AppBlueprint.Presentation.ApiModule`
- `SaaS-Factory.AppBlueprint.UiKit`
- `SaaS-Factory.AppBlueprint.Api.Client.Sdk`

## 🔐 Security Considerations

### API Key Management
- ✅ Use GitHub Secrets (never commit API keys)
- ✅ Set appropriate expiration dates
- ✅ Use scoped API keys (only push permissions)
- ✅ Rotate keys periodically
- ✅ Monitor key usage

### Package Security
- ✅ Enable package vulnerability scanning
- ✅ Sign packages (optional but recommended)
- ✅ Use HTTPS for all package sources
- ✅ Review dependencies for vulnerabilities
- ✅ Keep dependencies up to date

## 📊 Version Management

**Current Configuration:**
- Base version: `0.1.0` (from GitVersion.yml)
- Mode: `ContinuousDeployment`
- Increment: `Patch` on main branch

**Version Examples:**
- `0.1.0` - Initial release
- `0.1.1` - Patch release
- `0.2.0` - Minor version (new features)
- `1.0.0` - Major version (breaking changes)
- `1.0.0-beta.1` - Pre-release

## 🐛 Troubleshooting

### Common Issues

#### 1. **401 Unauthorized**
**Cause:** Invalid or missing API key

**Solution:**
- Verify `NUGET_API_KEY` secret exists in GitHub
- Check API key hasn't expired on NuGet.org
- Regenerate API key if needed

#### 2. **409 Conflict (Package version already exists)**
**Cause:** Version already published

**Solution:**
- NuGet.org doesn't allow overwriting versions
- Increment version number
- Push new tag

#### 3. **GitVersion fails**
**Cause:** Shallow git clone or missing history

**Solution:**
- Workflow uses `fetch-depth: 0` to get full history
- Verify GitVersion.yml is valid
- Check git tags exist

#### 4. **Package validation errors**
**Cause:** Missing required metadata

**Solution:**
- Ensure all `.csproj` files have required properties
- Verify README and icon files exist
- Check license information

## 📖 Additional Resources

- [NuGet Package Creation Best Practices](https://docs.microsoft.com/en-us/nuget/create-packages/package-authoring-best-practices)
- [Semantic Versioning](https://semver.org/)
- [GitVersion Documentation](https://gitversion.net/docs/)
- [GitHub Actions for .NET](https://docs.microsoft.com/en-us/dotnet/devops/github-actions-overview)

## 🎯 Quick Start Commands

```powershell
# 1. Add NuGet API key to GitHub Secrets
# (Do this via GitHub UI)

# 2. Create missing READMEs
New-Item -Path "Code/AppBlueprint/Shared-Modules/AppBlueprint.Contracts/README.md" -ItemType File
New-Item -Path "Code/AppBlueprint/Shared-Modules/AppBlueprint.Api.Client.Sdk/README.md" -ItemType File

# 3. Verify all packages build
cd Code/AppBlueprint/Shared-Modules
Get-ChildItem -Directory | ForEach-Object {
    Write-Host "Building $_..." -ForegroundColor Cyan
    Set-Location $_.FullName
    dotnet build --configuration Release
    Set-Location ..
}

# 4. Test pack packages
Get-ChildItem -Directory | ForEach-Object {
    Write-Host "Packing $_..." -ForegroundColor Cyan
    Set-Location $_.FullName
    dotnet pack --configuration Release --output ./bin/Release
    Set-Location ..
}

# 5. Publish (after verifying everything above)
git tag v0.1.0
git push origin v0.1.0
```

## ✅ Success Criteria

Packages are successfully published when:
- ✅ All 8 packages appear on NuGet.org
- ✅ Package pages show correct metadata, README, and icons
- ✅ Packages can be installed via `dotnet add package`
- ✅ Package dependencies resolve correctly
- ✅ No security vulnerabilities reported
- ✅ Package search works on NuGet.org

## 🔄 Ongoing Maintenance

### Regular Tasks
1. **Update dependencies** monthly
2. **Review security advisories** weekly
3. **Rotate API keys** annually
4. **Monitor package downloads** and usage
5. **Respond to issues** and feedback
6. **Update documentation** as needed

### Release Cadence
- **Patch releases** (0.1.x): Bug fixes, as needed
- **Minor releases** (0.x.0): New features, monthly
- **Major releases** (x.0.0): Breaking changes, quarterly

---

**Next Steps:**
1. ⚠️ **Generate and add NuGet API key** (CRITICAL)
2. 📝 Create missing README files
3. ✅ Verify package icons
4. 🧪 Test local pack
5. 🚀 Push first version tag
