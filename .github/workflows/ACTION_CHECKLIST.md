# 🚀 Quick Start: Deploy Landing Page + Documentation Together

## ✅ Action Checklist

### Step 1: Disable Old Workflows (Choose One)

**Option A - Rename (Recommended - keeps them for reference)**
```bash
cd .github/workflows
mv build-docs.yml build-docs.yml.disabled
mv deploy-landingpage.yml deploy-landingpage.yml.disabled
```

**Option B - Delete (Clean approach)**
```bash
cd .github/workflows
rm build-docs.yml
rm deploy-landingpage.yml
```

### Step 2: Commit and Push

```bash
# If you renamed files (Option A)
git add .github/workflows/*.yml.disabled
git add .github/workflows/deploy-combined-github-pages.yml
git add .github/workflows/COMBINED_DEPLOYMENT_GUIDE.md
git add .github/workflows/ACTION_CHECKLIST.md
git commit -m "feat: combine landing page and documentation deployment

- Add combined workflow for GitHub Pages deployment
- Disable separate landing page and docs workflows  
- Landing page at root, documentation at /docs
- Prevents deployment conflicts"
git push origin main

# OR if you deleted files (Option B)
git rm .github/workflows/build-docs.yml
git rm .github/workflows/deploy-landingpage.yml
git add .github/workflows/deploy-combined-github-pages.yml
git add .github/workflows/COMBINED_DEPLOYMENT_GUIDE.md
git add .github/workflows/ACTION_CHECKLIST.md
git commit -m "feat: combine landing page and documentation deployment

- Add combined workflow for GitHub Pages deployment
- Remove separate landing page and docs workflows
- Landing page at root, documentation at /docs
- Prevents deployment conflicts"
git push origin main
```

### Step 3: Monitor Deployment

1. Go to **Actions** tab in GitHub
2. Watch the "Deploy Landing Page + Documentation to GitHub Pages" workflow
3. Wait for it to complete (usually 3-5 minutes)

### Step 4: Verify URLs

After deployment succeeds, test both URLs:

- **Landing Page**: https://saas-factory-labs.github.io/Saas-Factory/
- **Documentation**: https://saas-factory-labs.github.io/Saas-Factory/docs/

### Step 5: (Optional) Add Cross-Navigation

**In Landing Page HTML** - Add link to documentation:
```html
<a href="/Saas-Factory/docs/" class="btn btn-primary">View Documentation</a>
```

**In Documentation** - Add link to landing page in your header or navigation

## 🎯 What This Achieves

✅ Landing page at root URL (`/`)
✅ Documentation at `/docs` subdirectory  
✅ No more overwriting conflicts
✅ Both deploy together in single workflow
✅ Clean, professional URL structure

## 📍 File Structure After Deployment

```
https://saas-factory-labs.github.io/Saas-Factory/
├── index.html              ← Landing page (Blazor WASM)
├── _framework/             ← Blazor framework
├── css/                    ← Landing page styles
├── 404.html               ← SPA routing
└── docs/                   ← Documentation (Writerside)
    ├── index.html          ← Docs home
    ├── topics/
    └── images/
```

## 🔧 Troubleshooting

**Problem**: Old workflows still running after disabling

**Solution**: 
```bash
# Make sure files are actually renamed/deleted
ls -la .github/workflows/

# Should NOT see:
# - build-docs.yml
# - deploy-landingpage.yml

# Should see:
# - deploy-combined-github-pages.yml
# - build-docs.yml.disabled (if you renamed)
# - deploy-landingpage.yml.disabled (if you renamed)
```

**Problem**: Can't access `/docs` URL

**Solution**: Check the workflow logs to verify documentation was built and copied correctly.

## ⚡ Quick Commands Summary

```bash
# 1. Disable old workflows
mv .github/workflows/build-docs.yml .github/workflows/build-docs.yml.disabled
mv .github/workflows/deploy-landingpage.yml .github/workflows/deploy-landingpage.yml.disabled

# 2. Commit and push
git add .github/workflows/*.yml.disabled .github/workflows/deploy-combined-github-pages.yml .github/workflows/*.md
git commit -m "feat: combine landing page and documentation deployment"
git push origin main

# 3. Monitor at: https://github.com/saas-factory-labs/Saas-Factory/actions
```

That's it! Your landing page and documentation will now be deployed together. 🎉

