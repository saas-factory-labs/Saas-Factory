# Solution: Deploy Both Landing Page and Documentation to GitHub Pages

## 🎯 Problem

GitHub Pages only supports **one deployment per repository**. You had two workflows competing for the same deployment:
1. `deploy-landingpage.yml` - Deploys Blazor landing page
2. `build-docs.yml` - Deploys Writerside documentation

Whichever workflow ran last would overwrite the other, so you could only see one at a time.

## ✅ Solution: Combined Deployment

I've created a **new combined workflow** that deploys both in a single GitHub Pages site:

- **Landing Page**: Deployed at root URL (`/`)
- **Documentation**: Deployed at `/docs` subdirectory

### URL Structure

After deployment:
- **Landing Page**: `https://saas-factory-labs.github.io/Saas-Factory/`
- **Documentation**: `https://saas-factory-labs.github.io/Saas-Factory/docs/`

## 📝 Implementation Steps

### Step 1: Disable Old Workflows

Rename or disable the old conflicting workflows to prevent them from running:

```bash
# Option A: Rename them (keeps them for reference)
mv .github/workflows/build-docs.yml .github/workflows/build-docs.yml.disabled
mv .github/workflows/deploy-landingpage.yml .github/workflows/deploy-landingpage.yml.disabled

# Option B: Delete them (if you don't need them)
rm .github/workflows/build-docs.yml
rm .github/workflows/deploy-landingpage.yml
```

### Step 2: Use the New Combined Workflow

The new workflow file is: `.github/workflows/deploy-combined-github-pages.yml`

**What it does:**
1. ✅ Builds the Blazor landing page
2. ✅ Builds the Writerside documentation
3. ✅ Combines them into a single deployment directory:
   - Landing page files at root
   - Documentation files in `/docs` subdirectory
4. ✅ Deploys everything to GitHub Pages

### Step 3: Update Navigation Links

You'll want to add links between your landing page and documentation:

**In Landing Page** (add link to docs):
```html
<a href="/Saas-Factory/docs/">Documentation</a>
```

**In Documentation** (add link back to landing page):
Add a link in your Writerside navigation or homepage back to:
```
https://saas-factory-labs.github.io/Saas-Factory/
```

### Step 4: Commit and Deploy

```bash
# Add the new workflow
git add .github/workflows/deploy-combined-github-pages.yml

# Disable old workflows (choose one approach)
git add .github/workflows/*.yml.disabled  # If you renamed them
# OR
git rm .github/workflows/build-docs.yml .github/workflows/deploy-landingpage.yml  # If you deleted them

# Commit and push
git commit -m "feat: combine landing page and documentation deployment to GitHub Pages"
git push origin main
```

## 🔄 How It Works

The combined workflow:

1. **Triggers on changes to:**
   - `Code/Landingpage/**` (landing page changes)
   - `Writerside/**` (documentation changes)
   - Manual trigger via `workflow_dispatch`

2. **Build Process:**
   ```
   Build Landing Page → Build Documentation → Combine Both → Deploy
   ```

3. **Directory Structure:**
   ```
   combined-site/
   ├── index.html              (Landing page root)
   ├── _framework/             (Blazor framework files)
   ├── css/                    (Landing page styles)
   ├── js/                     (Landing page scripts)
   ├── 404.html               (SPA routing fallback)
   └── docs/                   (Documentation subdirectory)
       ├── index.html          (Documentation home)
       ├── topics/
       └── images/
   ```

## 🎨 Alternative Solutions (Not Implemented)

### Alternative 1: Separate Repository for Documentation
- Create a new repo like `saas-factory-docs`
- Deploy docs to: `https://saas-factory-labs.github.io/saas-factory-docs/`
- **Pros**: Complete separation, independent deployments
- **Cons**: Harder to maintain, need to sync changes

### Alternative 2: Use GitHub Project Pages
- Keep landing page in main repo
- Use a separate `gh-pages` branch for documentation
- **Pros**: Traditional GitHub Pages approach
- **Cons**: More complex branch management

### Alternative 3: Custom Domain with Subdomain
- Landing page: `saas-factory.com`
- Documentation: `docs.saas-factory.com`
- **Pros**: Professional URLs, complete separation
- **Cons**: Requires custom domain and DNS configuration

## 📊 Comparison

| Solution | Landing Page URL | Documentation URL | Complexity | Recommended |
|----------|-----------------|-------------------|------------|-------------|
| **Combined (Implemented)** | `/` | `/docs` | Low | ✅ Yes |
| Separate Repo | `/` | `/docs/` (different repo) | Medium | No |
| gh-pages Branch | `/` | `/` (branch-based) | High | No |
| Custom Domain | Custom | `docs.custom` | High | Future |

## 🚀 Next Steps

1. ✅ **Disable or delete** old workflows (`build-docs.yml` and `deploy-landingpage.yml`)
2. ✅ **Commit and push** the new combined workflow
3. ✅ **Monitor the workflow** run in GitHub Actions
4. ✅ **Test both URLs** after deployment:
   - Landing page: `https://saas-factory-labs.github.io/Saas-Factory/`
   - Documentation: `https://saas-factory-labs.github.io/Saas-Factory/docs/`
5. ✅ **Update navigation** to add links between landing page and documentation

## 🐛 Troubleshooting

### Issue: Old workflow still running

**Solution**: Make sure to disable or delete the old workflow files. GitHub Actions won't automatically disable them.

### Issue: 404 on /docs

**Solution**: Check that the documentation was successfully built and copied to the `docs` subdirectory in the workflow logs.

### Issue: Landing page styles broken

**Solution**: Verify that the base href is correctly set to `/Saas-Factory/` in the landing page's index.html.

## 📚 Resources

- [GitHub Pages Documentation](https://docs.github.com/en/pages)
- [GitHub Actions - Deploying with GitHub Actions](https://docs.github.com/en/pages/getting-started-with-github-pages/configuring-a-publishing-source-for-your-github-pages-site#publishing-with-a-custom-github-actions-workflow)
- [Blazor WASM Deployment](https://learn.microsoft.com/en-us/aspnet/core/blazor/host-and-deploy/webassembly)

## ✨ Benefits of This Solution

✅ **Single workflow** - Easier to maintain
✅ **Atomic deployments** - Both update together
✅ **No conflicts** - Single source of truth for GitHub Pages
✅ **Clean URLs** - Professional structure with `/docs` subdirectory
✅ **Efficient** - Only rebuilds what changed (based on path filters)

Your landing page and documentation will now coexist peacefully on the same GitHub Pages site! 🎉

