<h1 align="center">  🏗️ SaaS Factory </h1>

<h4> A comprehensive platform framework that combines opinionated architecture, production-ready infrastructure, and developer-friendly tooling to deploy enterprise-grade B2B/B2C SaaS applications in minutes instead of months.
 </h4>

<p align="center">
  <img src="concept.png" alt="Concept diagram: your SaaS application built on centrally-updated NuGet packages, core building blocks, shared infrastructure modules, and cloud-agnostic hosting" width="600"/>
</p>

SaaS Factory is a **Platform Framework**, not a clone-and-forget boilerplate: the core ships as versioned **NuGet packages**, so you pull in bug fixes, security patches, and new features the way an EV receives over-the-air updates — without losing your application-specific customizations.

## ⚖️ SaaS Factory vs. Alternatives

| Framework / Platform | Ecosystem | Core Update Model | Architecture Philosophy | Infrastructure & Deployment |
| :--- | :--- | :--- | :--- | :--- |
| **SaaS Factory** | .NET (C#) | **Centralized NuGet Packages** (Dynamically updatable core platform) | Lightweight, pragmatic, Domain-Driven-Design (DDD), Clean Architecture and Laravel-inspired | Cloud-agnostic, cost-effective (.NET Aspire + YARP) |
| **ABP Framework** | .NET (C#) | **Centralized NuGet Packages** (Updatable framework layers) | Heavy Enterprise, strict Domain-Driven Design (DDD) | Highly abstract, enterprise-scale, steep learning curve |
| **Bullet Train** | Ruby on Rails | **RubyGems Packages** (Updatable via core framework gems) | "The Rails Way", extreme convention over configuration | Monolithic, optimized for maximum developer velocity |
| **Laravel Spark / Jetstream** | PHP | **Composer Packages** (Billing & Auth decoupled as packages) | Highly expressive, rapid application development | Traditional or serverless PHP, optimized for single-app instances |
| **SaaS Pegasus** | Python (Django) | **Boilerplate / Scaffolding** (One-time generation, manual upgrades) | Clean Django architecture, batteries included | Traditional Python stack, heavy emphasis on recent AI/LLM tooling |

Read the full [Vision & Positioning](docs/content/architecture/vision.md) doc for the automotive-factory metaphor behind the architecture, the reasoning for each design choice, and a deeper comparison against ABP, Bullet Train, Laravel Spark, and SaaS Pegasus.

---

 <h5 align="center">

<p> Database schema diagram for appblueprintdb </p>

[![Explore database with Azimutt](https://img.shields.io/badge/PostgreSQL-browse_schema-gray?labelColor=4169E1&logo=postgresql&logoColor=fff&style=flat)](https://azimutt.app/create?sql=https://raw.githubusercontent.com/saas-factory-labs/Saas-Factory/refs/heads/development/scripts/schema.sql)
[![Azimutt Database Analysis](https://img.shields.io/badge/Azimutt-database_analysis-gray?labelColor=7C3AED&logo=postgresql&logoColor=fff&style=flat)](docs/azimutt-database-analysis-report.md)

![Database Schema Diagram](docs/images/image.png)

<a href="https://demo.saasfactorylabs.com">Visit the live demo site </a>

</h5>


## 🔢 Project Status

### CI/CD & Build Status

<!--[![SonarCloud Analysis](https://github.com/saas-factory-labs/Saas-Factory/actions/workflows/sonarcloud-analysis.yaml/badge.svg)](https://github.com/saas-factory-labs/Saas-Factory/actions/workflows/sonarcloud-analysis.yaml?query=branch%3Amain) -->
[![Azimutt Database Analysis](https://github.com/saas-factory-labs/Saas-Factory/actions/workflows/azimutt-database-analysis.yml/badge.svg)](https://github.com/saas-factory-labs/Saas-Factory/actions/workflows/azimutt-database-analysis.yml?query=branch%3Amain)
<!-- [![Deploy to Railway](https://github.com/saas-factory-labs/Saas-Factory/actions/workflows/deploy-to-railway.yml/badge.svg)](https://github.com/saas-factory-labs/Saas-Factory/actions/workflows/deploy-to-railway.yml?query=branch%3Amain) -->
<!-- [![Docker Scout Vulnerability Scan](https://github.com/saas-factory-labs/Saas-Factory/actions/workflows/docker-scout-vulnerability-scan.yml/badge.svg)](https://github.com/saas-factory-labs/Saas-Factory/actions/workflows/docker-scout-vulnerability-scan.yml?query=branch%3Amain) -->
[![Publish NuGet Packages](https://github.com/saas-factory-labs/Saas-Factory/actions/workflows/publish-nuget-packages.yml/badge.svg)](https://github.com/saas-factory-labs/Saas-Factory/actions/workflows/publish-nuget-packages.yml?query=branch%3Amain)

### GitHub Issues & Project Management

[![GitHub issues with enhancement label](https://img.shields.io/github/issues-raw/saas-factory-labs/Saas-Factory/enhancement?label=enhancements&logo=github&color=%23A2EEEF)](https://github.com/saas-factory-labs/Saas-Factory/issues?q=is%3Aissue+is%3Aopen+label%3Aenhancement)
[![GitHub issues with bug label](https://img.shields.io/github/issues-raw/saas-factory-labs/Saas-Factory/bug?label=bugs&logo=github&color=red)](https://github.com/saas-factory-labs/Saas-Factory/issues?q=is%3Aissue+is%3Aopen+label%3Abug)
[![GitHub open issues](https://img.shields.io/github/issues-raw/saas-factory-labs/Saas-Factory?label=open%20issues&logo=github)](https://github.com/saas-factory-labs/Saas-Factory/issues?q=is%3Aissue+is%3Aopen)

### Quality & security analysis

<!-- [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=saas-factory-labs_Saas-Factory&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=saas-factory-labs_Saas-Factory) -->
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=saas-factory-labs_Saas-Factory&metric=security_rating)](https://sonarcloud.io/component_measures?id=saas-factory-labs_Saas-Factory&metric=Security)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=saas-factory-labs_Saas-Factory&metric=reliability_rating)](https://sonarcloud.io/component_measures?id=saas-factory-labs_Saas-Factory&metric=Reliability)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=saas-factory-labs_Saas-Factory&metric=sqale_rating)](https://sonarcloud.io/component_measures?id=saas-factory-labs_Saas-Factory&metric=Maintainability)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=saas-factory-labs_Saas-Factory&metric=bugs)](https://sonarcloud.io/summary/new_code?id=saas-factory-labs_Saas-Factory)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=saas-factory-labs_Saas-Factory&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=saas-factory-labs_Saas-Factory)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=saas-factory-labs_Saas-Factory&metric=vulnerabilities)](https://sonarcloud.io/project/issues?id=saas-factory-labs_Saas-Factory&resolved=false&types=VULNERABILITY)
[![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=saas-factory-labs_Saas-Factory&metric=sqale_index)](https://sonarcloud.io/component_measures?id=saas-factory-labs_Saas-Factory&metric=sqale_index)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=saas-factory-labs_Saas-Factory&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=saas-factory-labs_Saas-Factory)
<!-- [![Quality: SonarCloud](https://sonarcloud.io/api/project_badges/measure?project=saas-factory-labs_Saas-Factory&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=saas-factory-labs_Saas-Factory) -->
[![Security: CodeQL](https://github.com/saas-factory-labs/Saas-Factory/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/saas-factory-labs/Saas-Factory/actions/workflows/codeql-analysis.yml)
<!-- [![Security: Snyk](https://github.com/saas-factory-labs/Saas-Factory/actions/workflows/snyk-analysis.yaml/badge.svg)](https://github.com/saas-factory-labs/Saas-Factory/actions/workflows/snyk-analysis.yaml) -->
 <!-- [![Quality: SonarCloud](https://sonarcloud.io/api/project_badges/measure?project=saas-factory-labs_Saas-Factory&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=saas-factory-labs_Saas-Factory) -->

## 📚 Documentation

- 🚀 **[Getting Started / Quick Start](docs/content/getting-started/Quick-start.md)** - Prerequisites, running the app, and day-to-day dev commands
- 🏗️ **[Vision & Positioning](docs/content/architecture/vision.md)** - Why the platform is built this way, and how it compares to alternatives
- 🗂️ **[Code Structure](docs/content/development/Code-Structure.md)** - Full repository and module breakdown
- 📝 **[Feature Development Guide](docs/content/development/Feature-Development-Guide.md)** - Adding domain entities, repositories, and API endpoints
- 🎯 **[Use Cases](docs/content/guides/Use-Cases.md)** - User flows and feature guides

## 🤝 Contributing

Contributions are highly welcome and much appreciated. See [CONTRIBUTING.md](docs/CONTRIBUTING.md) for how to get started, our commit message convention, and the [Code of Conduct](docs/CODE_OF_CONDUCT.md).

---

### 🛠️  Quick Start

```powershell
cd Code\AppBlueprint\AppBlueprint.AppHost
dotnet run
```

This starts the .NET Aspire AppHost, which orchestrates the Web, API, Gateway, and their dependencies (PostgreSQL, mail server, etc.) in Docker with a single command.

Requires the .NET 10 SDK, Docker, Node.js, and the GitHub CLI. Full prerequisite install steps for **Windows, macOS, and Linux** are in the [Quick Start guide](docs/content/getting-started/Quick-start.md).

# 🗂️ File structure in the git repository

SaaS-Factory is a [monorepo](https://en.wikipedia.org/wiki/Monorepo) hosting three deployable apps that share the platform core: **AppBlueprint** (the reusable SaaS blueprint itself), **DeploymentManager** (an internal tool that deploys and centrally manages every SaaS Factory-based app, and hosts their per-app admin portals as plugins), and **Landingpage** (the public marketing/landing site).

```bash
├─ .github            # GitHub workflows, CI/CD, and Copilot instructions
├─ build-artifacts    # Build output, logs, and temporary files (gitignored)
├─ docs               # Documentation site content, guides, and diagrams
├─ scripts            # Utility scripts (SQL setup/maintenance, PowerShell helpers)
├─ Code               # Application source code
│  ├─ AppBlueprint         # The reusable SaaS blueprint (product code)
│  ├─ DeploymentManager    # Internal tool: deploys/manages SaaS Factory apps and their admin portals
│  ├─ Landingpage          # Blazor WebAssembly marketing/landing page (static hosting)
│  ├─ Cloudflare-Workers   # Standalone Cloudflare Worker(s) (OpenAPI/Hono), independent of AppBlueprint
│  └─ SaaSFactory.Testing  # Cross-cutting test helpers (e.g. CodeQL test runner)
```

See the [full code structure breakdown](docs/content/development/Code-Structure.md) for the complete module tree, including all Shared-Modules and their NuGet packages.

<!--
## 👥 Maintainers

SaaS Factory is actively developed and maintained by:

<table>
  <tr>
    <td align="center">
      <a href="https://github.com/Trubador">
        <img src="https://github.com/Trubador.png" width="100px;" alt="Casper Rubæk Mølvadgaard"/><br />
        <sub><b>Casper Rubæk Mølvadgaard</b></sub>
      </a><br />
      Lead Architect & Primary Maintainer
    </td>
    <td align="center">
      <a href="https://github.com/hornvieh3u">
        <img src="https://github.com/hornvieh3u.png" width="100px;" alt="hornvieh3u"/><br />
        <sub><b>hornvieh3u</b></sub>
      </a><br />
      Contributor
    </td>
  </tr>
</table>
-->
