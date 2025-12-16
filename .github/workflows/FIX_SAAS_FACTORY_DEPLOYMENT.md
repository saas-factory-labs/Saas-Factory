# Fix: SaaS-Factory Instance Not Showing on Deployed Website

## 🐛 Problem

The SaaS-Factory instance (sf) was not showing up on the deployed GitHub Pages website because the workflow was only building the **API-Reference instance (ar)**, not the **SaaS-Factory instance (sf)**.

## ✅ Solution

Changed the workflow to build the **SaaS-Factory instance** instead of the API-Reference instance.

### Changes Made

**File:** `.github/workflows/build-docs.yml`

**Before:**
```yaml
env:
  INSTANCE: 'Writerside/ar'  # ✅ Ensure correct instance path
```

**After:**
```yaml
env:
  INSTANCE: 'Writerside/sf'  # ✅ Build SaaS-Factory instance
```

## 📋 What This Fixes

- ✅ The SaaS-Factory documentation will now be built and deployed to GitHub Pages
- ✅ The deployed site will show the SaaS-Factory content including:
  - README.md (project overview)
  - Architectural Decision Record
  - Use Cases
  - AppBlueprint documentation with all Shared-Modules
  - Code development workflow

## 🔄 Current Workflow Behavior

The workflow now:
1. Builds only the **SaaS-Factory (sf)** instance
2. Creates artifact: `webHelpSF2-all.zip`
3. Deploys to GitHub Pages at: `https://saas-factory-labs.github.io/Saas-Factory/`

## 💡 Why Was This Happening?

The workflow configuration had `INSTANCE: 'Writerside/ar'` which told the Writerside builder to only build the API-Reference documentation. Since we moved all the main content (README, ADR, Use Cases, AppBlueprint docs) to the SaaS-Factory instance (sf.tree), the deployed site was missing this content.

## 🚀 Next Steps

1. **Commit and Push the Fix:**
   ```bash
   git add .github/workflows/build-docs.yml
   git commit -m "fix: change workflow to build SaaS-Factory instance instead of API-Reference"
   git push origin main
   ```

2. **Verify Deployment:**
   - Go to the Actions tab in GitHub
   - Watch the "Build documentation" workflow run
   - Once completed, visit: `https://saas-factory-labs.github.io/Saas-Factory/`
   - You should now see the SaaS-Factory documentation

3. **Check Content:**
   - Verify README.md appears
   - Verify Architectural Decision Record is accessible
   - Verify Use Cases are available
   - Verify AppBlueprint documentation is present

## 🔍 Alternative: Building Both Instances

If you want to build **both instances** (API-Reference AND SaaS-Factory) and make them both accessible on the deployed site, you would need to:

1. Use a matrix strategy to build both instances separately
2. Merge the built artifacts into a single deployment
3. Create an index page that links to both instances

This is more complex but can be implemented if needed. For now, focusing on the SaaS-Factory instance makes sense since that's where the main content resides.

## 📚 Instance Structure

**SaaS-Factory Instance (sf.tree):**
- README.md - Project overview
- Architectural-decision-record.md
- Use-Cases.md (with User Signup and Login)
- AppBlueprint documentation
  - Code_README.md (Development Workflow)
  - Shared-Modules (all module documentation)

**API-Reference Instance (ar.tree):**
- Code section (CHANGELOG, packages)
- Currently not being deployed

## ✨ Result

After this fix is deployed, your GitHub Pages site will properly display the SaaS-Factory documentation with all the content we've been organizing throughout this session!

