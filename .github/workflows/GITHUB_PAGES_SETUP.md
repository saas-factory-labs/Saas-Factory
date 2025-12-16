# GitHub Pages Setup Instructions

## ✅ Workflow Updated

The `build-docs.yml` workflow has been successfully converted from Cloudflare Pages deployment to GitHub Pages deployment.

## 🔧 Required GitHub Repository Configuration

To enable GitHub Pages deployment, you need to configure the following settings in your GitHub repository:

### Step 1: Enable GitHub Pages

1. Go to your repository on GitHub: `https://github.com/saas-factory-labs/Saas-Factory`
2. Navigate to **Settings** → **Pages** (in the left sidebar)
3. Under **Source**, select:
   - **Source**: `GitHub Actions` (instead of "Deploy from a branch")
4. Click **Save**

### Step 2: Verify Workflow Permissions (Optional)

The workflow already includes the necessary permissions in the YAML file, but you can verify repository-level permissions:

1. Go to **Settings** → **Actions** → **General**
2. Scroll to **Workflow permissions**
3. Ensure either:
   - "Read and write permissions" is selected, OR
   - "Read repository contents and packages permissions" is selected (the workflow defines its own permissions)

## 📝 What Changed

### Removed:
- ❌ Git commit and push steps that committed built docs back to the repository
- ❌ Cloudflare Pages deployment approach

### Added:
- ✅ GitHub Pages permissions configuration
- ✅ Concurrency control to prevent deployment conflicts
- ✅ `upload-pages-artifact` action to prepare documentation for GitHub Pages
- ✅ Separate `deploy` job that deploys to GitHub Pages
- ✅ GitHub Pages environment with deployment URL output

## 🚀 Deployment Flow

1. **Build Job**:
   - Checks out repository
   - Builds Writerside documentation
   - Unzips the documentation to `webHelpAR2-all/` directory
   - Uploads documentation as GitHub Pages artifact
   - Saves backup artifact

2. **Deploy Job**:
   - Waits for build job to complete
   - Deploys the artifact to GitHub Pages
   - Outputs the deployment URL

## 🌐 Accessing Your Documentation

After the workflow runs successfully, your documentation will be available at:

```
https://saas-factory-labs.github.io/Saas-Factory/
```

## 🔍 Monitoring Deployments

1. Go to **Actions** tab in your repository
2. Click on the "Build documentation" workflow
3. View the deployment status and URL in the deploy job
4. Go to **Settings** → **Pages** to see deployment history

## 📊 Benefits of GitHub Pages over Cloudflare Pages (in this context)

- ✅ No need to commit built artifacts back to repository
- ✅ Cleaner git history (no automated commits)
- ✅ Integrated with GitHub Actions (no external service setup)
- ✅ Automatic HTTPS with github.io domain
- ✅ Deployment history tracked in GitHub
- ✅ Environment protection rules can be applied

## 🎯 Next Steps

1. Enable GitHub Pages in repository settings (see Step 1 above)
2. Commit and push the updated workflow file
3. Trigger the workflow manually or push changes to `Writerside/**` files
4. Monitor the workflow execution in the Actions tab
5. Access your documentation at the GitHub Pages URL

## 🐛 Troubleshooting

### Issue: "pages build and deployment" workflow failing

**Solution**: Make sure GitHub Pages is enabled with "GitHub Actions" as the source (not "Deploy from a branch").

### Issue: 404 error when accessing the documentation

**Solution**: 
- Check that the workflow completed successfully
- Verify the unzipped documentation path contains an `index.html` file
- Ensure the artifact path in `upload-pages-artifact` is correct

### Issue: Permission denied errors

**Solution**: Verify that the workflow has the correct permissions (already configured in the YAML file).

## 📚 Additional Resources

- [GitHub Pages Documentation](https://docs.github.com/en/pages)
- [Deploying with GitHub Actions](https://docs.github.com/en/pages/getting-started-with-github-pages/configuring-a-publishing-source-for-your-github-pages-site#publishing-with-a-custom-github-actions-workflow)
- [Writerside Documentation](https://www.jetbrains.com/help/writerside/)

