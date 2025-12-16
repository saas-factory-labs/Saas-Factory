---
title: RazorPress Content Directory Setup
---

This document explains how RazorPress documentation is configured to avoid file duplication while maintaining a single source of truth in the `docs/content/` directory.

## Problem

RazorPress loads markdown files from its `_pages` directory, but we want to maintain all documentation in `docs/content/` with subdirectories for organization (architecture, development, getting-started, guides). Copying files would create duplication and sync issues.

## Solution: Directory Junctions

We use **directory junctions** (on Windows) or **symbolic links** (on Linux/macOS) to make the content directories accessible to RazorPress without duplicating files.

### Windows Setup (Directory Junctions)

Directory junctions don't require administrator privileges on Windows:

```powershell
# Run from repository root
$contentPath = "docs\content"
$pagesPath = "docs\RazorPress\RazorPress\_pages"
$dirs = @("architecture", "development", "getting-started", "guides")

foreach ($dir in $dirs) {
    $source = Join-Path $contentPath $dir
    $target = Join-Path $pagesPath $dir
    
    if ((Test-Path $source) -and -not (Test-Path $target)) {
        cmd /c mklink /J "$target" "$source"
        Write-Output "✓ Created junction: $dir"
    }
}
```

### Linux/macOS Setup (Symbolic Links)

Symbolic links work natively without special permissions:

```bash
# Run from repository root
cd docs/RazorPress/RazorPress/_pages

ln -s ../../../content/architecture architecture
ln -s ../../../content/development development
ln -s ../../../content/getting-started getting-started
ln -s ../../../content/guides guides

echo "✓ Created symlinks to content directories"
```

## Verification

Check that junctions/symlinks were created successfully:

**Windows:**
```powershell
Get-ChildItem "docs\RazorPress\RazorPress\_pages" -Directory | 
    Where-Object { $_.Attributes -band [System.IO.FileAttributes]::ReparsePoint } | 
    Select-Object Name, Target
```

**Linux/macOS:**
```bash
ls -la docs/RazorPress/RazorPress/_pages/
```

You should see the directories marked as junctions/symlinks pointing to the content directories.

## How It Works

1. **Content Location**: All markdown files live in `docs/content/` subdirectories
2. **Junctions**: Directory junctions in `_pages/` point to content subdirectories
3. **RazorPress**: Loads files from `_pages/` which transparently reads through junctions
4. **Routing**: Multi-segment URLs work via `/{*slug}` catch-all route in `Page.cshtml`
5. **Sidebar**: Links use lowercase slugs matching directory structure (e.g., `/development/feature-development-guide`)

## RazorPress Configuration

The `Configure.Ssg.cs` file is configured to load from `_pages`:

```csharp
includes.LoadFrom("_includes");
pages.LoadFrom("_pages");  // Reads through junctions to content/
whatsNew.LoadFrom("_whatsnew");
videos.LoadFrom("_videos");
```

## GitHub Actions Setup

The deployment workflow creates symlinks before building:

```yaml
- name: Create directory junctions for content
  run: |
    cd docs/RazorPress/RazorPress/_pages
    ln -s ../../../content/architecture architecture
    ln -s ../../../content/development development
    ln -s ../../../content/getting-started getting-started
    ln -s ../../../content/guides guides
```

## Benefits

✅ **Single Source of Truth**: All content in `docs/content/`  
✅ **No Duplication**: Junctions/symlinks avoid file copies  
✅ **Easy Maintenance**: Edit once in content directory  
✅ **Organized Structure**: Content logically organized by category  
✅ **Clean Git History**: Only content changes tracked, not duplicates  

## Troubleshooting

### Pages Not Loading

If pages aren't loading after setup:

1. Verify junctions exist: `Get-ChildItem` or `ls -la` commands above
2. Check RazorPress logs: Should show "Found X pages" where X > 0
3. Restart dev server: `dotnet watch` in RazorPress directory
4. Verify routing: Ensure `Page.cshtml` has `@page "/{*slug}"` (catch-all route)

### Sidebar Links Not Working

1. Check sidebar.json links use lowercase slugs
2. Verify slug format matches directory structure: `/directory/filename-without-extension`
3. Example: `Quick-start.md` in `getting-started/` → `/getting-started/quick-start`

### CI/CD Build Failures

If GitHub Actions fails to create symlinks:

1. Ensure symlink creation step runs before build
2. Verify paths are relative: `../../../content/directory`
3. Check Linux permissions (symlinks don't need special permissions)

## File Organization

```
docs/
├── content/                          # Single source of truth
│   ├── architecture/
│   │   └── Architectural-decision-record.md
│   ├── development/
│   │   ├── Feature-Development-Guide.md
│   │   ├── 01-adding-domain-entities.md
│   │   ├── 02-adding-repositories.md
│   │   └── 03-creating-api-controllers.md
│   ├── getting-started/
│   │   └── Quick-start.md
│   └── guides/
│       ├── authentication/
│       └── deployment/
└── RazorPress/
    └── RazorPress/
        └── _pages/
            ├── architecture/         # Junction → content/architecture
            ├── development/          # Junction → content/development
            ├── getting-started/      # Junction → content/getting-started
            ├── guides/               # Junction → content/guides
            ├── sidebar.json
            └── index.md
```

## Related Configuration

- **Routing**: [Page.cshtml](../../../docs/RazorPress/RazorPress/Pages/Page.cshtml) - `@page "/{*slug}"`
- **Loading**: [Configure.Ssg.cs](../../../docs/RazorPress/RazorPress/Configure.Ssg.cs)
- **Sidebar**: [sidebar.json](../../../docs/RazorPress/RazorPress/_pages/sidebar.json)
- **Deployment**: [deploy-combined-github-pages.yml](../../../.github/workflows/deploy-combined-github-pages.yml)
