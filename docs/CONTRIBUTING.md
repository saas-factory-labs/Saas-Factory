# Contributing to SaaS Factory

We welcome contributions from the community. Here are some guidelines to help you get started.

The project's architecture is still actively evolving, but we take the utmost care to review and land contributions rather than let them stall — most reasonable pull requests get implemented. Before starting on a large or non-obvious change, please [open an issue or discussion](https://github.com/saas-factory-labs/Saas-Factory/issues) first, so we can align on approach and make sure your effort doesn't collide with an in-progress refactor.

## Commit Message Convention

This project uses **Conventional Commits** for automated semantic versioning. Please read our [Commit Convention Guide](COMMIT_CONVENTION.md) to understand how to format your commit messages correctly.

## How to Contribute

1. Fork the repository.
2. Create a branch with a descriptive, lowercase, dash-separated name (e.g. `fix-tenant-lookup-cache`).
3. Make your changes, following the code style configured in `.editorconfig` and the patterns in `.github/.ai-rules/`.
4. Add or update tests for your change (see the [testing guides](guides/) and the project's TDD approach — TUnit, FluentAssertions, NSubstitute).
5. Commit following our [commit conventions](COMMIT_CONVENTION.md).
6. Rebase onto the latest `development` branch before pushing.
7. Push to your fork and open a pull request against `development` using the [PR template](../.github/PULL_REQUEST_TEMPLATE.md).

Please adhere to primarily contributing C# code to the projects since the project is centered around the .NET/C# framework:

- Blazor Web Application
- API
- Application Gateway
- Developer CLI
- Class libraries

The code is modeled based on Clean Architecture principles.

Python and TypeScript are used only in specific niche use cases such as Machine Learning with Python, since it has greater maturity in that field. For such use cases, [ML.NET](https://dotnet.microsoft.com/en-us/apps/ai/ml-dotnet) is preferred as an initial implementation before reaching for Python.

It is very important not to commit any secrets to the code, as these will be flagged in the pull request by an automated GitHub Action that scans for secrets.

## Code of Conduct

Please adhere to the [Code of Conduct](CODE_OF_CONDUCT.md) in all your interactions with the project.

## Reporting Issues

Use the GitHub [issues](https://github.com/saas-factory-labs/Saas-Factory/issues) to report bugs or suggest new features.