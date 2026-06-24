# AGENTS.md

This file provides behavioral guidelines for AI agents working with code in this repository.

## For GitHub Copilot
Follow the instructions at `.github/copilot-instructions.md`

## For Claude Code / Claude Chat
Follow the instructions at `CLAUDE.md`

## Behavioral Guidelines

1. **Think before coding** - State assumptions explicitly before making changes
2. **Read before you write** - Understand existing patterns by reading related code first
3. **Goal-driven execution** - Define success criteria before iterating
4. **Checkpoint significant steps** - Summarize what was done/verified after major changes
5. **Fail loud** - Surface uncertainty, don't hide it; ask when unsure

## Build, Test, and Format

```powershell
# Build (verify compilation)
cd Code\AppBlueprint
dotnet build

# Build specific project
cd Code\AppBlueprint\AppBlueprint.Web
dotnet build

# Test (run all tests)
cd Code\AppBlueprint
dotnet test

# Test specific project
cd Code\AppBlueprint\AppBlueprint.Tests
dotnet test

# Format code
cd Code\AppBlueprint
dotnet format
```

## Source of Truth

| Category | Location |
|----------|----------|
| **Primary rules** | `.github/copilot-instructions.md` |
| **Backend patterns** | `.github/.ai-rules/backend/` |
| **Architecture rules** | `.github/.ai-rules/baseline/` |
| **Frontend patterns** | `.github/.ai-rules/frontend/` |
| **Test patterns** | `.github/.ai-rules/tests/` |
| **Claude agents** | `.github/.claude/agents/` |
| **Claude rules** | `.github/.claude/rules/` |

## Agent Roles

| Agent | Purpose |
|-------|---------|
| `backend-reviewer` | Reviews backend code for security, performance, and quality |
| `frontend-reviewer` | Reviews Blazor/frontend code for accessibility and patterns |
| `architect` | Enforces clean architecture and cross-layer dependencies |
| `qa-reviewer` | Verifies test coverage and test quality |

## Core Principles

1. **Preserve existing code** - Prefer editing over creating new files
2. **Follow established patterns** - Match existing code style in the codebase
3. **Test-first development** - Write tests before implementation (TDD)
4. **Verify changes** - Run `dotnet build` after modifications
5. **No over-engineering** - Stay within scope of the task

## Working with This Codebase

### Architecture
- **Clean Architecture** with Domain-Driven Design principles
- **CQRS pattern** - Commands and Queries in Application layer
- **Repository pattern** - Interfaces in Domain, implementations in Infrastructure

### Key Directories
```
Code/AppBlueprint/
├── AppBlueprint.AppHost/          # .NET Aspire orchestrator
├── AppBlueprint.Web/              # Blazor Server frontend
├── AppBlueprint.ApiService/       # REST API backend
└── Shared-Modules/                # Clean Architecture layers
    ├── AppBlueprint.Domain/       # Entities, Value Objects
    ├── AppBlueprint.Application/  # Use Cases, Commands, Queries
    ├── AppBlueprint.Infrastructure/ # EF Core, Repositories
    └── AppBlueprint.Presentation.ApiModule/ # API endpoints
```

### Before Implementing
1. Read `CLAUDE.md` for project-specific guidance
2. Consult relevant rules in `.github/.ai-rules/`
3. Check existing patterns in the codebase
4. Write tests first, then implement

### After Implementing
1. Run `dotnet build` to verify compilation
2. Run `dotnet test` to verify tests pass
3. Verify no security vulnerabilities introduced
4. Check that changes follow established patterns
