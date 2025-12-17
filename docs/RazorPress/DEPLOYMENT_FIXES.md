# RazorPress Deployment Fixes - December 2024

Documentation of all fixes applied to resolve deployment issues with the RazorPress documentation site at https://saas-factory-labs.github.io/Saas-Factory/docs/

---

## Issue 1: CSS Styling Not Loading ✅ FIXED

**Problem:** Deployed site showed unstyled HTML with missing CSS, 0.1KB stylesheet, and Tailwind parse errors.

**Root Causes:**
1. **Invalid Tailwind v4 syntax in v3 project** - `@theme` directive in `tailwind.input.css` is only supported in Tailwind v4, but project uses v3.4.17
2. **Build order issue** - Tailwind CLI ran BEFORE prerender, so it couldn't scan the generated HTML files in `dist/`

**Solutions:**
- **Commit ec91b07:** Removed `@theme` directive, used v3-compatible CSS variable syntax
- **Commit 76403e5:** Reordered workflow so Tailwind builds AFTER prerender to scan `dist/**/*.html`

**Files Modified:**
- `RazorPress/wwwroot/css/tailwind.input.css` - Removed `@theme`, added explicit CSS variables
- `.github/workflows/deploy-combined-github-pages.yml` - Moved Tailwind step after prerender

---

## Issue 2: Hero Section Content Missing ✅ FIXED

**Problem:** Home page loaded with correct CSS but hero section, Key Objectives, and all custom content was completely missing from deployed site.

**Root Causes:**
1. **Func delegates don't serialize** - `Index.cshtml` used `Func<dynamic?, object>` pattern which works in development but fails during `RazorSsg.PrerenderAsync()` static site generation
2. **File conflict** - Both `Pages/Index.cshtml` AND `_pages/index.md` existed, causing markdown renderer to overwrite the custom page

**Solutions:**
- **Commit f2304dc:** 
  - Moved ALL hero HTML directly into `Index.cshtml` body (not as Func)
  - Deleted `_pages/index.md` to prevent rendering conflict
  - Updated `appsettings.json` with correct GitHub Pages URLs (commit 9c3a378)

**Technical Details:**
```csharp
// ❌ DOESN'T WORK - Func doesn't serialize during prerender
Func<dynamic?, object> header = @<div>...hero HTML...</div>;
@await Html.PartialAsync("DocsPage", new Shared.DocsPage { Header = header });

// ✅ WORKS - Direct HTML in .cshtml body
<div class="not-prose pb-16">
    <div class="mx-auto max-w-7xl">
        <h1>Saas Factory</h1>
        <p>Solution for Deploying and Managing Any Type of SaaS Application</p>
        <!-- All hero content directly here -->
    </div>
</div>
```

**Files Modified:**
- `RazorPress/Pages/Index.cshtml` - Custom hero page with direct HTML
- `RazorPress/_pages/index.md` - **DELETED** (was conflicting with Index.cshtml)
- `RazorPress/appsettings.json` - Updated URLs:
  - `PublicBaseUrl`: `https://saas-factory-labs.github.io/Saas-Factory/docs`
  - `BaseHref`: `/Saas-Factory/docs/`

---

## Issue 3: Development Section Not Visible in Sidebar ✅ FIXED

**Problem:** Development section only appeared after navigating to another page, not visible on homepage.

**Root Cause:** 
`Index.cshtml` had hardcoded sidebar with only 2 links instead of loading the dynamic sidebar from `sidebar.json`.

**Solution:**
- **Commit c07ea6d:** Replaced hardcoded sidebar HTML with `Markdown.GetSidebar()` call to load dynamic sidebar

**Code Change:**
```csharp
// ❌ BEFORE - Hardcoded sidebar (only 2 items)
<nav class="sidebar">
    <a href="/">Getting Started</a>
    <ul>
        <li><a href="/getting-started/quick-start">Quick Start</a></li>
        <li><a href="/architecture/...">Architecture</a></li>
    </ul>
</nav>

// ✅ AFTER - Dynamic sidebar from sidebar.json
@{
    var defaultMenu = new MarkdownMenu {
        Icon = Markdown.DefaultMenuIcon,
        Text = "Getting Started",
        Link = "/",
    };
    var sidebar = Markdown.GetSidebar("", defaultMenu);
}

<nav class="sidebar">
    @foreach (var menu in sidebar) {
        // Renders ALL sections including Development
    }
</nav>
```

**Files Modified:**
- `RazorPress/Pages/Index.cshtml` - Added dynamic sidebar rendering matching `DocsPage.cshtml` pattern

---

## Testing Steps

### Local Prerender Test
```bash
cd docs/RazorPress/RazorPress
rm -rf dist
dotnet run --AppTasks=prerender

# Verify content
grep "Key Objectives" dist/index.html
grep "Development" dist/index.html
ls -lh dist/css/app.css  # Should be ~48KB
```

### Deployment Test
1. Commit and push changes to feature branch
2. Merge to main branch
3. GitHub Actions builds and deploys automatically
4. Verify at: https://saas-factory-labs.github.io/Saas-Factory/docs/
5. Check browser console for errors
6. Verify all sidebar sections visible

---

## Important Learnings

### RazorPress Static Site Generation

1. **Func delegates don't serialize** - `RazorSsg.PrerenderAsync()` can't serialize Func<> delegates
   - ✅ Use: Direct HTML in .cshtml body
   - ✅ Use: Markdown content in `_pages/`
   - ❌ Avoid: Func delegates for dynamic content

2. **File route conflicts** - Having both `Pages/X.cshtml` AND `_pages/x.md` causes issues
   - RazorPress renders both during prerender
   - Later render overwrites earlier one
   - Solution: Use EITHER .cshtml OR .md, not both

3. **Custom pages need explicit sidebar** - Don't inherit DocsPage sidebar automatically
   - Must call `Markdown.GetSidebar()` explicitly
   - Must render sidebar using same pattern as `DocsPage.cshtml`

### Tailwind CSS in CI/CD

1. **Build order matters** - Tailwind scans HTML to generate utility classes
   - ✅ Correct: Prerender → Tailwind build
   - ❌ Wrong: Tailwind build → Prerender

2. **Version compatibility** - Syntax differs between Tailwind versions
   - `@theme` directive is v4-only
   - v3 requires explicit CSS variables

3. **Content paths** - Tailwind needs to scan the RIGHT files
   - `content: ['./dist/**/*.html']` not `./Pages/**/*.cshtml`
   - Prerendered HTML contains actual classes used

### GitHub Pages Subdirectory

1. **BaseHref configuration needed**
   - Landing page: `<base href="/Saas-Factory/" />`
   - Documentation: `<base href="/Saas-Factory/docs/" />`
   - Must update in both source AND during build

2. **Path rewriting** - Post-process HTML to rewrite internal links
   - Workflow step: `sed` commands to update paths
   - Updates all relative URLs for subdirectory

---

## Troubleshooting Guide

### CSS Not Loading or Wrong Size

**Symptoms:**
- app.css is 0.1KB or very small
- Unstyled HTML displayed
- Tailwind utilities missing

**Fix:**
1. Check build order: Tailwind must run AFTER prerender
2. Verify content paths scan `dist/**/*.html`
3. Remove any Tailwind v4 syntax (`@theme`)
4. Check generated CSS file size (~48KB expected)

### Content Missing on Deployment

**Symptoms:**
- Content renders locally but not on deployment
- Hero section or custom content missing
- Markdown content appears instead

**Fix:**
1. Check for Func delegates in custom pages - replace with direct HTML
2. Delete conflicting markdown files (_pages/index.md)
3. Test prerender locally - content should be in dist/index.html
4. Verify appsettings.json has correct PublicBaseUrl

### Sidebar Not Showing All Sections

**Symptoms:**
- Some navigation sections missing
- Sections appear after clicking link

**Fix:**
1. Add `Markdown.GetSidebar()` call in custom page
2. Render sidebar using foreach loop like DocsPage
3. Check _pages/sidebar.json has all sections defined

---

## Related Files

- **Workflow:** `.github/workflows/deploy-combined-github-pages.yml`
- **Config:** `docs/RazorPress/RazorPress/appsettings.json`
- **Tailwind:** `docs/RazorPress/RazorPress/tailwind.config.js`
- **CSS Input:** `docs/RazorPress/RazorPress/wwwroot/css/tailwind.input.css`
- **Homepage:** `docs/RazorPress/RazorPress/Pages/Index.cshtml`
- **Sidebar:** `docs/RazorPress/RazorPress/_pages/sidebar.json`

---

## Commit History

- **ec91b07** - Remove @theme directive (Tailwind v3 compatibility)
- **76403e5** - Build Tailwind AFTER prerender (build order fix)
- **9c3a378** - Update appsettings.json URLs (GitHub Pages config)
- **f2304dc** - Move HTML to Index.cshtml, delete index.md (content fix)
- **c07ea6d** - Add dynamic sidebar to Index.cshtml (navigation fix)

---

**Status:** All issues resolved ✅  
**Last Updated:** December 17, 2024  
**Deployed:** https://saas-factory-labs.github.io/Saas-Factory/docs/
