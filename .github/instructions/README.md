# GitHub Copilot Instructions Directory

This directory contains granular, path-specific instructions for GitHub Copilot coding agent.

## How It Works

Files in this directory use YAML frontmatter to specify which files or directories they apply to. GitHub Copilot will automatically use the relevant instructions based on the files you're working with.

## Instruction Files

- **[backend-api.instructions.md](./backend-api.instructions.md)** - API controllers, endpoints, and ApiService project
- **[frontend-blazor.instructions.md](./frontend-blazor.instructions.md)** - Blazor components, pages, Web and AppGateway projects
- **[testing.instructions.md](./testing.instructions.md)** - Test files and testing practices
- **[developer-cli.instructions.md](./developer-cli.instructions.md)** - Developer CLI tool development

## File Format

Each instruction file follows this format:

```markdown
---
applies_to:
  - "path/pattern/**"
  - "**/*.specific.cs"
---

# Title

Instructions content here...
```

## Relationship to `.ai-rules`

These instruction files complement the more comprehensive documentation in `../.ai-rules/`:

- **`.github/instructions/`** - Quick reference, targeted guidance, examples
- **`.github/.ai-rules/`** - Comprehensive rules, detailed patterns, architecture

## Adding New Instructions

When creating new instruction files:

1. Use descriptive filenames ending in `.instructions.md`
2. Add YAML frontmatter with `applies_to` paths
3. Keep content focused and actionable
4. Include practical examples
5. Link to comprehensive rules in `.ai-rules` for details
6. Update this README to list the new file

## Best Practices

- Keep instructions concise and scannable
- Use code examples liberally
- Focus on common patterns and gotchas
- Cross-reference comprehensive rules
- Update when patterns change
