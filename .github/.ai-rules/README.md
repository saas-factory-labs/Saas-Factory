# AI Rules Directory

This directory contains domain-specific rules and guidelines for AI assistants working with the SaaS Factory codebase.

## Organization

The rules are organized by domain area to provide targeted guidance:

- **[Baseline](./baseline/README.md)** - Fundamental rules that apply to all code (code style, entity modeling, etc.)
- **[Backend](./backend/README.md)** - Rules for C# backend development (API controllers, repositories, external integrations)
- **[Frontend](./frontend/README.md)** - Rules for Blazor/Razor frontend development (UI design, component patterns)
- **[Infrastructure](./infrastructure/README.md)** - Rules for infrastructure code (Pulumi, Docker, deployment)
- **[Developer CLI](./developer-cli/README.md)** - Rules for the AppBlueprint.DeveloperCli project
- **[Tests](./tests/README.md)** - Rules for writing unit, integration, and UI tests
- **[Development Workflow](./development-workflow/README.md)** - Guidelines for development processes

## How to Use

When working on code in the AppBlueprint project:

1. Always consult the **Baseline** rules first - they apply to all code
2. Then consult the domain-specific rules relevant to your task:
   - Backend work → consult Backend rules
   - UI work → consult Frontend rules
   - CLI work → consult Developer CLI rules
   - Infrastructure → consult Infrastructure rules
3. Always write tests following the **Tests** rules
4. Follow the **Development Workflow** for PR and development processes

## Rule File Format

Each domain directory contains:
- `README.md` - Overview and core principles for that domain
- Specific rule files (`.md`) - Detailed guidelines for particular aspects

## Maintaining Rules

- Keep rules clear, concise, and actionable
- Update rules when learning new patterns or best practices
- Suggest rule changes when discovering deviations
- Ensure examples are accurate and follow current patterns
- Link related rules across domains when relevant
