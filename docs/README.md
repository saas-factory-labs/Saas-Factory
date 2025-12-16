# Documentation

This directory contains the main documentation files for the SaaS Factory project.

## Structure

All documentation markdown files are stored in the `content/` directory, organized by category. This maintains consistency across different documentation tools (SSG, static site generators).

### Content Organization

```
content/
  getting-started/     - Quick start guides and onboarding
  architecture/        - Architectural decisions and system design
  guides/              - Specific guides and tutorials
    authentication/    - Authentication and JWT setup guides
    deployment/        - Railway, Docker, and deployment guides
  development/         - Development workflow and code structure
```

### Current Documentation Files

**Getting Started:**
- [content/getting-started/Quick-start.md](content/getting-started/Quick-start.md) - Quick start guide

**Architecture:**
- [content/architecture/Architectural-decision-record.md](content/architecture/Architectural-decision-record.md) - Key architectural decisions

**General Guides:**
- [content/guides/Migration.md](content/guides/Migration.md) - Migration guides for framework and dependency upgrades
- [content/guides/Versioning.md](content/guides/Versioning.md) - Versioning strategy using GitVersion
- [content/guides/](content/guides/) - Additional testing and reference guides

**Authentication (12 guides):**
- Auth0 setup and configuration
- Blazor JWT implementation
- Logto authentication setup (5 guides)
- JWT testing and validation

**Deployment (11 guides):**
- Railway deployment (7 guides)
- Docker configuration (2 guides)
- Cloud database and HTTPS setup

**Development:**
- [content/development/Code-Structure.md](content/development/Code-Structure.md) - Overview of Code directory structure

### Other Directories

- `RazorPress/` - RazorPress SSG implementation (copies documentation from content/)

## Documentation Workflow

1. **Add new documentation**: Create or update markdown files in `docs/content/` (organized by category)
2. **Sync to SSG tools**: Run `./sync-documentation.ps1` to copy files to:
   - `RazorPress/RazorPress/_pages/`
   - `Writerside/topics/`
3. **Update navigation**: 
   - RazorPress: Edit `RazorPress/RazorPress/_pages/sidebar.json`
   - Writerside: Edit `Writerside/sf.tree`

### Why This Structure?

This structure ensures that:
- Documentation files remain tool-agnostic in `content/`
- Easy migration between different SSG tools (RazorPress, Writerside, etc.)
- Single source of truth for documentation content
- SSG tools (RazorPress, Writerside) reference these files via sync script
- Consistent documentation across all platforms

## Migration from Writerside

When importing documentation from Writerside (`/Writerside/topics/`):\n1. Copy the markdown file to appropriate `docs/content/` subdirectory
2. Add the file path to the sync script (`sync-documentation.ps1`)
3. Run the sync script to copy to both RazorPress and Writerside topics
4. Update both RazorPress sidebar and Writerside tree configurations
