# GitHub Configuration Directory

This directory contains GitHub-specific configuration files, workflows, and AI assistant instructions for the SaaS Factory project.

## 📁 Directory Structure

### GitHub Copilot Instructions

#### Main Instructions
- **[copilot-instructions.md](./copilot-instructions.md)** - Primary instructions for GitHub Copilot coding agent

#### Granular Instructions (Path-Specific)
- **[instructions/](./instructions/)** - Path-specific instruction files with YAML frontmatter
  - Automatically applied based on the files being edited
  - Quick reference with practical examples
  - See [instructions/README.md](./instructions/README.md) for details

#### Comprehensive AI Rules
- **[.ai-rules/](./.ai-rules/)** - Detailed domain-specific rules and guidelines
  - [Baseline](./.ai-rules/baseline/README.md) - Fundamental rules for all code
  - [Backend](./.ai-rules/backend/README.md) - C# backend development
  - [Frontend](./.ai-rules/frontend/README.md) - Blazor/Razor UI development
  - [Infrastructure](./.ai-rules/infrastructure/README.md) - Pulumi, Docker, deployment
  - [Developer CLI](./.ai-rules/developer-cli/README.md) - CLI tool development
  - [Tests](./.ai-rules/tests/README.md) - Testing guidelines
  - [Development Workflow](./.ai-rules/development-workflow/README.md) - PR processes

### GitHub Features

- **[workflows/](./workflows/)** - GitHub Actions CI/CD workflows
- **[ISSUE_TEMPLATES/](./ISSUE_TEMPLATES/)** - Issue templates for bug reports, features, etc.
- **[PULL_REQUEST_TEMPLATE.md](./PULL_REQUEST_TEMPLATE.md)** - PR template
- **[SECURITY.md](./SECURITY.md)** - Security policy and vulnerability reporting
- **[dependabot.yml](./dependabot.yml)** - Dependabot configuration for dependency updates

## 🤖 How GitHub Copilot Uses These Instructions

### Instruction Hierarchy

1. **Primary Instructions**: `copilot-instructions.md` - Always loaded first
2. **Path-Specific Instructions**: `instructions/*.instructions.md` - Applied when working with matching files
3. **Comprehensive Rules**: `.ai-rules/**/*.md` - Referenced for detailed guidance

### YAML Frontmatter Targeting

Instruction files use YAML frontmatter to specify which paths they apply to:

```yaml
---
applies_to:
  - "Code/AppBlueprint/AppBlueprint.Web/**"
  - "**/*.razor"
---
```

When you edit files matching these patterns, Copilot automatically applies the relevant instructions.

## 📝 Best Practices for AI Instructions

### For Contributors

When updating instructions:

1. **Keep it clear and actionable** - Avoid ambiguity
2. **Include examples** - Show, don't just tell
3. **Link related rules** - Cross-reference for details
4. **Test the instructions** - Ensure they work as expected
5. **Update all relevant files** - Maintain consistency

### For Maintainers

When maintaining the instruction structure:

1. **Main instructions** - Update for project-wide changes
2. **Path-specific** - Add new `.instructions.md` files for new patterns
3. **Comprehensive rules** - Expand `.ai-rules` for detailed guidance
4. **Validation** - Ensure all links work and files are referenced correctly

## 🔗 Related Documentation

- [Main Repository README](../README.md)
- [Writerside Documentation](../Writerside/topic/README.md)
- [AppBlueprint Documentation](../Code/AppBlueprint/)

## 📚 Resources

- [GitHub Copilot Best Practices](https://docs.github.com/en/copilot/tutorials/coding-agent/get-the-best-results)
- [Custom Instructions Documentation](https://github.blog/changelog/2025-07-23-github-copilot-coding-agent-now-supports-instructions-md-custom-instructions/)
- [Path-Specific Instructions](https://docs.github.com/en/copilot/customizing-copilot/adding-custom-instructions-for-github-copilot)

## 🆘 Support

If you have questions about the AI instructions or need help:

1. Check the relevant instruction files first
2. Review the comprehensive rules in `.ai-rules`
3. Create an issue with the `documentation` label
4. Ask in the team chat for clarification

---

**Last Updated**: November 2025
**Maintained By**: SaaS Factory Team
