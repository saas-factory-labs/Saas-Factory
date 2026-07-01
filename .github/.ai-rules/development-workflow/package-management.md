---
description: NuGet package management rules and version policy
globs:
  - "**/Directory.Packages.props"
  - "**/*.csproj"
---

# Package Management

## Central Package Management

All package versions are defined in `Directory.Packages.props`. Project `.csproj` files reference packages without specifying a version.

To add or update a package:
1. Use `Nuget MCP server` to query the correct release version (no known vulnerabilities).
2. Ask the user before upgrading or downgrading any package.
3. Update the version in `Directory.Packages.props`.
4. Add `<PackageReference>` (without version) to the project `.csproj`.

## NU1510 — Remove unnecessary package references

Do not explicitly reference packages already included in the .NET SDK or available transitively. This reduces maintenance burden and avoids version conflicts.

Common packages to avoid adding explicitly:
- `System.Text.Json` (included in .NET 6+)
- `Microsoft.Extensions.*` packages when already available through `Microsoft.AspNetCore.App`
- Any package already referenced by a direct dependency (transitive references)

Before adding a package reference, verify it is not already available transitively.

## NU1902 — Keep packages updated and secure

Regularly check for and fix security vulnerabilities in NuGet packages. Use `Nuget MCP server` to find latest stable versions without known vulnerabilities. Upgrade to patch security issues promptly, and ensure all packages in a family use consistent versions (e.g., all OpenTelemetry packages at the same version).

## Prefer stable versions over prerelease

For production code, always use stable (non-RC, non-preview) package versions when available. Use `Nuget MCP server` with `allow_prerelease=false` to find stable releases. Only use prerelease packages when no stable version exists or when explicitly required for bleeding-edge framework features.
