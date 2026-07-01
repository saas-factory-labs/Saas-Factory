---
description: Development workflow guidelines and environment setup
globs: []
---

# Development Workflow

## Environment

- Development is primarily done on Windows using PowerShell. When running commands in the CLI, always use PowerShell syntax — do not use bash or cmd.
- Do **not** use `&&` to chain commands in PowerShell — run commands separately in sequence.
- Mac/Linux must also be supported in scripts and workflows.
- Doppler CLI is used to inject environment variables for local development.

## Rules Files

- [Package Management](./package-management.md) - NuGet version policy, security updates, and stable vs. prerelease guidance.
- [Pull Request](./pull-request.md) - Pull request workflow guidelines.

## PowerShell Formatting (S8620)

Remove trailing whitespace from PowerShell scripts. Configure your editor to trim trailing whitespace on save.

