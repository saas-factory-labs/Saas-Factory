<h1 align="center">  🏗️ SaaS Factory </h1>

<h4> Blueprint for deploying any type of B2B/B2C SaaS to massively accelerate development speed and time to market of a new SaaS web application by reducing the to create boilerplate code and infrastructure.
 </h4>

 <h5 align="center">

<p> Database schema diagram for appblueprintdb </p>

[![Explore database with Azimutt](https://img.shields.io/badge/PostgreSQL-browse_online-gray?labelColor=4169E1&logo=postgresql&logoColor=fff&style=flat)](https://azimutt.app/create?sql=https://diagram-hosting-proxy.casper-c7c.workers.dev/schema.sql)

![alt text](image.png)

<a href="https://appblueprint-web-production-production.up.railway.app/dashboard"> View live demo site </a>

</h5>


## 🔢 Project Status

### CI/CD & Build Status

[![SonarCloud Analysis](https://github.com/saas-factory-labs/Saas-Factory/actions/workflows/sonarcloud-analysis.yaml/badge.svg)](https://github.com/saas-factory-labs/Saas-Factory/actions/workflows/sonarcloud-analysis.yaml?query=branch%3Amain)
[![Deploy to Railway](https://github.com/saas-factory-labs/Saas-Factory/actions/workflows/deploy-to-railway.yml/badge.svg)](https://github.com/saas-factory-labs/Saas-Factory/actions/workflows/deploy-to-railway.yml?query=branch%3Amain)
[![Docker Scout Vulnerability Scan](https://github.com/saas-factory-labs/Saas-Factory/actions/workflows/docker-scout-vulnerability-scan.yml/badge.svg)](https://github.com/saas-factory-labs/Saas-Factory/actions/workflows/docker-scout-vulnerability-scan.yml?query=branch%3Amain)
[![Publish NuGet Packages](https://github.com/saas-factory-labs/Saas-Factory/actions/workflows/publish-nuget-packages.yml/badge.svg)](https://github.com/saas-factory-labs/Saas-Factory/actions/workflows/publish-nuget-packages.yml?query=branch%3Amain)

### GitHub Issues & Project Management

[![GitHub issues with enhancement label](https://img.shields.io/github/issues-raw/saas-factory-labs/Saas-Factory/enhancement?label=enhancements&logo=github&color=%23A2EEEF)](https://github.com/saas-factory-labs/Saas-Factory/issues?q=is%3Aissue+is%3Aopen+label%3Aenhancement)
[![GitHub issues with bug label](https://img.shields.io/github/issues-raw/saas-factory-labs/Saas-Factory/bug?label=bugs&logo=github&color=red)](https://github.com/saas-factory-labs/Saas-Factory/issues?q=is%3Aissue+is%3Aopen+label%3Abug)
[![GitHub open issues](https://img.shields.io/github/issues-raw/saas-factory-labs/Saas-Factory?label=open%20issues&logo=github)](https://github.com/saas-factory-labs/Saas-Factory/issues?q=is%3Aissue+is%3Aopen)

### Code Quality & Security (SonarCloud)

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=saas-factory-labs_Saas-Factory&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=saas-factory-labs_Saas-Factory)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=saas-factory-labs_Saas-Factory&metric=security_rating)](https://sonarcloud.io/component_measures?id=saas-factory-labs_Saas-Factory&metric=Security)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=saas-factory-labs_Saas-Factory&metric=reliability_rating)](https://sonarcloud.io/component_measures?id=saas-factory-labs_Saas-Factory&metric=Reliability)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=saas-factory-labs_Saas-Factory&metric=sqale_rating)](https://sonarcloud.io/component_measures?id=saas-factory-labs_Saas-Factory&metric=Maintainability)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=saas-factory-labs_Saas-Factory&metric=bugs)](https://sonarcloud.io/summary/new_code?id=saas-factory-labs_Saas-Factory)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=saas-factory-labs_Saas-Factory&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=saas-factory-labs_Saas-Factory)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=saas-factory-labs_Saas-Factory&metric=vulnerabilities)](https://sonarcloud.io/project/issues?id=saas-factory-labs_Saas-Factory&resolved=false&types=VULNERABILITY)
[![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=saas-factory-labs_Saas-Factory&metric=sqale_index)](https://sonarcloud.io/component_measures?id=saas-factory-labs_Saas-Factory&metric=sqale_index)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=saas-factory-labs_Saas-Factory&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=saas-factory-labs_Saas-Factory)

---

## 📚 Documentation

**[📖 View Complete Documentation](https://saas-factory-labs.github.io/Saas-Factory/docs/)**

The comprehensive documentation includes:
- 🚀 **Getting Started Guide** - Quick setup and installation
- 🏗️ **Architecture Overview** - System design and patterns
- 📝 **Development Workflow** - Building and deploying
- 🔧 **Configuration Guide** - Environment setup and customization
- 📦 **Shared Modules** - Reusable components and libraries
- 🎯 **Use Cases** - User flows and feature guides

---

 ## 🎯 Project Overview

<summary><h4>Vision</h4></summary>

Deploy a fully functional SaaS application in under 30 minutes using a single command via the Developer CLI, with all foundational features already in place - ready for you to add your application-specific features.



<summary><h4>Purpose</h4></summary>

Provide a production-ready SaaS blueprint that eliminates months of boilerplate development, allowing developers to focus on building unique features that differentiate their product rather than rebuilding common infrastructure.


<summary><h4>Objectives</h4></summary>

**Development Experience**
- Enjoyable and productive development workflow
- Familiar tech stack (C# wherever possible)
- Minimal technical debt through standardized implementations

**Infrastructure & Deployment**
- Consolidated shared infrastructure across all SaaS projects
- Fast continuous deployment (automatic deployment after passing automated QA)
- Cloud-agnostic architecture (easily migrate to Digital Ocean, Hetzner, Linode, Render, Railway, etc.)

**Architecture & Quality**
- Monorepo structure for deployment manager and SaaS application boilerplate
- Modular, flexible structure with proper tracking of code and database migrations
- Automated processes for testing, documentation, and deployment
- Multiple environments: Dev, QA (automated), Staging, and Production

**Operations**
- Cost-effective infrastructure management
- Enterprise-grade security implementation
- Disaster recovery capabilities
- Comprehensive audit logging

<summary><h4>Challenges & Considerations</h4></summary>

**Technical Challenges**
- Maintaining consistency across multiple deployed SaaS applications to minimize technical debt
- Balancing high-level architectural vision with detailed low-level implementation

**Strategic Decisions**
- **MVP Readiness**: Defining criteria for the first production SaaS Factory deployment
- **Deployment Manager**: Should this be deferred to version 2.0? How to consolidate and migrate existing deployed applications?

---

### 🛠️  Prerequisites

<details>

<summary>Install prerequisites for development on Windows</summary>
	
1.	Open a PowerShell terminal as Administrator and run the following command to install Windows Subsystem for Linux (required for Docker):
  
    `wsl --install`

2. Restart your computer if prompted.

3. Install .NET, Git, Docker Desktop, Node.js, Azure CLI, and GitHub CLI using winget (available only on Windows 11):

    ```powershell
    @(
        "Microsoft.DotNet.SDK.9",
        "Git.Git",
        "Docker.DockerDesktop",
        "OpenJS.NodeJS",
        "npm install wrangler --save-dev"
        "GitHub.cli"    	
    ) | ForEach-Object { winget install --accept-package-agreements --accept-source-agreements --id $_ }
    
    "gh extension install https://github.com/nektos/gh-act"
    ```
</details>

# 🗂️ File structure in the git repository

SaaS-Factory is a [monorepo](https://en.wikipedia.org/wiki/Monorepo) containing all application code, infrastructure, tools, libraries, documentation, etc. 
A monorepo is a powerful way to organize a codebase, used by Google, Facebook, Uber, Microsoft, etc.

> **📋 Repository Organization:** See [REPOSITORY_ORGANIZATION.md](REPOSITORY_ORGANIZATION.md) for details on the recent reorganization and directory structure improvements.

```bash
├─ .github                            # GitHub workflows, CI/CD, and Copilot instructions
├─ build-artifacts                    # Build output, logs, and temporary files (gitignored)
├─ Code                               # Contains the application source code
│  ├─ AppBlueprint        
│     ├─ Appblueprint.AppHost         # .NET Aspire project starting app and all dependencies in Docker
│      ├─ Appblueprint.Gateway        # Blazor Server app utilizing Mudblazor components
│      ├─ Appblueprint.Web            # Blazor Server app utilizing Mudblazor components
│      ├─ Appblueprint.ApiService     # .Net Rest/GraphQL API
│      ├─ Appblueprint.Workers        # Background workers for long-running tasks and event processing
│      ├─ Appblueprint.SharedKernel   # Shared code between all projects
│      ├─ Appblueprint.UiKit          # Shared UI components
│      ├─ Appblueprint.Tests          # Tests for all projects
├─ docs                               # Documentation and guides
│  ├─ guides                          # Testing guides and quick references
├─ scripts                            # Utility scripts
│  ├─ powershell                      # PowerShell scripts for Windows development
├─ Writerside                         # Comprehensive documentation source
```

