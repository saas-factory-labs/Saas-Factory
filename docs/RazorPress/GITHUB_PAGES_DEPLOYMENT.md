# GitHub Pages Deployment Configuration

This document explains the configuration changes made to deploy RazorPress documentation to GitHub Pages at a subdirectory path (`/Saas-Factory/docs/`).

## Problem Overview

When deploying to GitHub Pages at a subdirectory (not root), several issues occurred:

1. **CSS/JS 404 Errors**: Asset paths like `/css/app.css` resolved to domain root instead of `/Saas-Factory/docs/css/app.css`
2. **Navigation Link Failures**: Internal page links used absolute paths that ignored the base href
3. **Subdirectory Page 404s**: Pages in subdirectories (e.g., `getting-started/quick-start`) were not being rendered
4. **Blazor WASM 404s**: Landing page `_framework` files were ignored by GitHub Pages' default Jekyll processing

## Solution Architecture

### 1. BaseHref Configuration

**Files Modified:**
- `appsettings.json`
- `appsettings.Production.json`

**Changes:**

Added `BaseHref` property to support subdirectory deployment:

```json
// appsettings.json (default)
{
  "BaseHref": "/"
}

// appsettings.Production.json (GitHub Pages)
{
  "BaseHref": "/Saas-Factory/docs/"
}
```

**Purpose:** Provides environment-specific base path configuration for asset and navigation URLs.

---

### 2. Layout Template Updates

**Files Modified:**
- `Pages/Shared/_Layout.cshtml`
- `Pages/Error.cshtml`

**Changes:**

Injected `AppConfig` and updated `<base href>` tag to use configurable BaseHref:

```razor
@inject AppConfig AppConfig

<head>
    <base href="@(RazorSsg.GetBaseHref() ?? AppConfig.BaseHref ?? "/")"/>
    <!-- rest of head content -->
</head>
```

**Purpose:** Ensures HTML `<base>` tag reflects the deployment environment's base path.

---

### 3. Path Rewriting Post-Processing

**File Modified:** `Configure.Ssg.cs`

**Changes:**

Added post-processing step after `RazorSsg.PrerenderAsync` to rewrite absolute paths:

```csharp
// After prerender completes, post-process HTML files to include baseHref in absolute paths
var baseHref = AppConfig.Instance.BaseHref?.TrimEnd('/') ?? "";
if (!string.IsNullOrEmpty(baseHref))
{
    var htmlFiles = Directory.GetFiles(distDir, "*.html", SearchOption.AllDirectories);
    var processed = 0;
    var regex1 = new Regex(@"(href|src)=""/(css|mjs|lib|img|js|pages)/", RegexOptions.Compiled);
    var regex2 = new Regex(@"""/(mjs|lib)/", RegexOptions.Compiled);
    var regex3 = new Regex(@"href=""/""", RegexOptions.Compiled);
    
    foreach (var file in htmlFiles)
    {
        var content = File.ReadAllText(file);
        var newContent = content;
        
        // Rewrite asset paths
        newContent = regex1.Replace(newContent, $"$1=\"{baseHref}/$2/");
        
        // Rewrite import map paths
        newContent = regex2.Replace(newContent, $"\"{baseHref}/$1/");
        
        // Rewrite root href
        newContent = regex3.Replace(newContent, $"href=\"{baseHref}/\"");
        
        if (newContent != content)
        {
            File.WriteAllText(file, newContent);
            processed++;
        }
    }
}
```

**Regex Patterns:**

| Pattern | Purpose | Example Transformation |
|---------|---------|------------------------|
| `(href&#124;src)="/(css&#124;mjs&#124;lib&#124;img&#124;js&#124;pages)/` | Asset paths | `/css/app.css` → `/Saas-Factory/docs/css/app.css` |
| `"/(mjs&#124;lib)/` | Import map paths | `"/mjs/app.mjs"` → `"/Saas-Factory/docs/mjs/app.mjs"` |
| `href="/"` | Root navigation | `href="/"` → `href="/Saas-Factory/docs/"` | |

**Purpose:** Browsers treat paths starting with `/` as absolute from domain root, ignoring the `<base>` tag. This post-processing rewrites all absolute paths to include the baseHref prefix.

---

### 4. Subdirectory Page Rendering

**File Modified:** `Pages/Page.cshtml`

**Changes:**

Changed `GetStaticProps` implementation from `GetVisiblePages()` to `GetAll()`:

```csharp
// BEFORE (only rendered root-level pages)
public List<MarkdownFileInfo> GetStaticProps(MarkdownPagesBase markdown)
{
    var allPages = markdown.GetVisiblePages();
    return allPages;
}

// AFTER (renders all pages including subdirectories)
public List<MarkdownFileInfo> GetStaticProps(MarkdownPagesBase markdown)
{
    var allPages = markdown.GetAll();
    return allPages;
}
```

**Impact:**
- **Before**: 12 root-level pages rendered
- **After**: 80+ pages rendered including subdirectories:
  - `architecture/*`
  - `development/*`
  - `getting-started/*`
  - `guides/*`

**Purpose:** `GetVisiblePages()` filters out pages with `/` in their slug, only returning root-level pages. `GetAll()` returns all visible pages regardless of directory depth.

---

### 5. Jekyll Bypass for Blazor WASM

**File Modified:** `.github/workflows/deploy-combined-github-pages.yml`

**Changes:**

Added `.nojekyll` file creation in the deployment workflow:

```yaml
- name: Create combined deployment directory
  run: |
    # ... existing code ...
    
    # Add .nojekyll to prevent GitHub Pages from ignoring _framework directory
    echo "🔥 Adding .nojekyll file..."
    touch combined-site/.nojekyll
    
    # ... rest of deployment ...
```

**Additional Verification Steps:**

```yaml
# Verify base href in generated HTML
echo "🔍 Checking base href in index.html..."
grep -o '<base href="[^"]*"' combined-site/docs/index.html || echo "⚠️ Base href not found"

# Verify subdirectory pages exist
echo "🔍 Verifying subdirectory pages..."
if [ -f "combined-site/docs/getting-started/quick-start.html" ]; then
  echo "✅ Subdirectory pages rendered correctly"
else
  echo "⚠️ Subdirectory pages missing"
fi
```

**Purpose:** 
- GitHub Pages uses Jekyll by default, which ignores directories starting with underscore (`_`)
- Blazor WASM uses `_framework/` directory for runtime files
- `.nojekyll` file disables Jekyll processing, allowing `_framework/` files to be served
- Verification steps ensure configuration is applied correctly

---

## Testing Checklist

After deployment, verify:

- [ ] **CSS Loads**: Open browser DevTools Network tab, verify CSS files load from correct path
- [ ] **Navigation Works**: Click internal links, verify they navigate correctly
- [ ] **Subdirectory Pages**: Access pages like `/docs/getting-started/quick-start`
- [ ] **Blazor WASM Loads**: Landing page at root loads without `_framework` 404 errors
- [ ] **Import Maps**: Check console for module loading errors
- [ ] **Base Href**: View page source, verify `<base href="/Saas-Factory/docs/">` present

**Important:** Test in incognito/private browser window to avoid cached 404 responses.

---

## Environment Comparison

| Environment | BaseHref | Behavior |
|------------|----------|----------|
| **Development** (localhost) | `/` | Standard root-relative paths |
| **Production** (GitHub Pages) | `/Saas-Factory/docs/` | Paths prefixed with repository/docs |

---

## File Summary

| File | Type | Purpose |
|------|------|---------|
| `appsettings.json` | Config | Default BaseHref configuration |
| `appsettings.Production.json` | Config | Production BaseHref override |
| `Configure.Ssg.cs` | Code | Post-processing path rewriting logic |
| `Pages/Shared/_Layout.cshtml` | Template | Base href injection in HTML |
| `Pages/Error.cshtml` | Template | Base href injection in error page |
| `Pages/Page.cshtml` | Template | Subdirectory page rendering |
| `.github/workflows/deploy-combined-github-pages.yml` | CI/CD | Jekyll bypass + verification |

---

## Commits

```
93477b9 - Fix RazorPress docs CSS loading for GitHub Pages deployment
dabe731 - Fix navigation links in RazorPress docs
7ca1472 - Fix subdirectory page rendering in RazorPress
6b91492 - Fix Blazor WASM landing page by adding .nojekyll file
```

---

## Key Learnings

1. **Base Href Limitations**: HTML `<base>` tag does NOT affect paths starting with `/` - browsers treat these as absolute from domain root
2. **GetVisiblePages Filtering**: Filters out pages with `/` in slug, unsuitable for subdirectory navigation
3. **Jekyll on GitHub Pages**: Automatically enabled, requires `.nojekyll` to disable underscore directory filtering
4. **Browser Caching**: 404 responses are aggressively cached - always test deployments in incognito mode
5. **Post-Processing Necessity**: Client-side path rewriting is insufficient; server-generated HTML must contain correct paths

---

## Future Improvements

- Consider moving BaseHref configuration to build-time variable instead of runtime config
- Evaluate ServiceStack's built-in subdirectory deployment support (if available)
- Add automated tests for path rewriting logic
- Investigate CDN cache invalidation for faster deployment verification
