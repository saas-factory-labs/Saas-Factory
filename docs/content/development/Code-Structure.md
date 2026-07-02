---
title: Code Structure
---

# 🗂️ File structure in the git repository

SaaS-Factory is a [monorepo](https://en.wikipedia.org/wiki/Monorepo) containing all application code, infrastructure, tools, libraries, documentation, etc.
A monorepo is a powerful way to organize a codebase, used by Google, Facebook, Uber, Microsoft, etc.

The monorepo hosts three deployable apps that share the platform core: **AppBlueprint** (the reusable SaaS blueprint itself), **DeploymentManager** (an internal tool that deploys and centrally manages every SaaS Factory-based app, and hosts their per-app admin portals as plugins), and **Landingpage** (the public marketing/landing site).

```bash
├─ .github                                  # GitHub workflows, CI/CD, and Copilot instructions
├─ build-artifacts                          # Build output, logs, and temporary files (gitignored)
├─ docs                                     # Cross-repo documentation, guides, and diagrams
│  ├─ content                               # Docs site content (architecture, getting-started, guides, specs)
│  └─ search-server                         # Typesense search server + scraper powering the docs site
├─ scripts                                  # Utility scripts (SQL setup/maintenance, PowerShell helpers)
├─ Code                                     # Application source code
│  ├─ AppBlueprint                          # The reusable SaaS blueprint (product code)
│  │  ├─ AppBlueprint.AppHost               # .NET Aspire project orchestrating the app and its dependencies
│  │  ├─ AppBlueprint.AppGateway            # YARP reverse proxy / API gateway
│  │  ├─ AppBlueprint.Web                   # Blazor Server app utilizing MudBlazor components
│  │  ├─ AppBlueprint.ApiService            # .NET REST API
│  │  ├─ AppBlueprint.ServiceDefaults       # Shared Aspire service configuration
│  │  ├─ AppBlueprint.DeveloperCli          # CLI tools for scaffolding and management
│  │  ├─ AppBlueprint.Tests                 # Tests for all AppBlueprint projects
│  │  ├─ Cloudflare-Workers                 # Edge worker(s) supporting the AppBlueprint app
│  │  ├─ docs                               # AppBlueprint-specific architecture/operations/security/troubleshooting docs
│  │  └─ Shared-Modules                     # Clean Architecture shared modules, published as NuGet packages
│  │     ├─ AppBlueprint.Domain                  # Entities, value objects, aggregates, domain logic
│  │     ├─ AppBlueprint.Application             # Use cases, commands, queries, DTOs (CQRS)
│  │     ├─ AppBlueprint.Infrastructure          # Composition root wiring up the Infrastructure.* modules below
│  │     ├─ Infrastructure                       # Infrastructure split into focused, independently versioned modules
│  │     │  ├─ AppBlueprint.Infrastructure.Core           # Cross-cutting infrastructure abstractions
│  │     │  ├─ AppBlueprint.Infrastructure.Persistence    # EF Core DbContext, repositories, migrations (PostgreSQL)
│  │     │  ├─ AppBlueprint.Infrastructure.Authentication # Logto integration and auth handlers
│  │     │  ├─ AppBlueprint.Infrastructure.Payments       # Stripe integration
│  │     │  ├─ AppBlueprint.Infrastructure.Email          # Resend integration
│  │     │  ├─ AppBlueprint.Infrastructure.Notifications  # Notification delivery
│  │     │  ├─ AppBlueprint.Infrastructure.Storage        # Cloudflare R2 / Azure Blob storage
│  │     │  ├─ AppBlueprint.Infrastructure.Search         # Full-text search integration
│  │     │  ├─ AppBlueprint.Infrastructure.Realtime       # Realtime/SignalR services
│  │     │  └─ AppBlueprint.Infrastructure.Compliance     # GDPR export/deletion and audit logging
│  │     ├─ AppBlueprint.Presentation.ApiModule  # Minimal API endpoints and versioning
│  │     ├─ AppBlueprint.Contracts               # Shared contracts and interfaces
│  │     ├─ AppBlueprint.SharedKernel            # Shared kernel code across all projects
│  │     ├─ AppBlueprint.UiKit                   # Reusable Tailwind/Cruip UI components
│  │     ├─ AppBlueprint.CliKit                  # Shared CLI building blocks used by DeveloperCli
│  │     ├─ AppBlueprint.AdminPortalKernel       # Generic admin-portal plugin host, consumed by DeploymentManager
│  │     └─ AppBlueprint.Api.Client.Sdk          # Kiota-generated API client SDK
│  ├─ DeploymentManager                     # Internal tool: deploys/manages SaaS Factory apps and their admin portals
│  │  ├─ DeploymentManager.AppHost          # .NET Aspire orchestrator
│  │  ├─ DeploymentManager.Web              # Blazor Server shell that loads per-app admin-portal plugins
│  │  ├─ DeploymentManager.ApiService       # REST API backend
│  │  ├─ DeploymentManager.CloudInfrastructure # Pulumi infrastructure-as-code
│  │  ├─ DeploymentManager.Codeflow         # GraphQL/DGraph-based code dependency graph tooling
│  │  ├─ DeploymentManager.Tests            # Tests for all DeploymentManager projects
│  │  ├─ Samples                            # Reference admin-portal plugin implementation
│  │  └─ plugins                            # Runtime folder for downloaded/copied admin-portal plugin dlls
│  ├─ Landingpage                           # Blazor WebAssembly marketing/landing page (static hosting)
│  ├─ Cloudflare-Workers                    # Standalone Cloudflare Worker(s) (OpenAPI/Hono), independent of AppBlueprint
│  └─ SaaSFactory.Testing                   # Cross-cutting test helpers (e.g. CodeQL test runner)
```
