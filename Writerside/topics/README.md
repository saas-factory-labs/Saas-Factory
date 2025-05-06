# 🏗️ SaaS Factory

_Solution for Deploying and Managing Any Type of SaaS application_

The solution includes the app blueprint project that serves as a blueprint for deploying SaaS applications and a
Deployment Manager to deploy and manage each SaaS application centrally for optimal efficiency.


https://learn.microsoft.com/en-us/nuget/consume-packages/central-package-management

https://learn.microsoft.com/en-us/nuget/concepts/security-best-practices


## 🔢 Project status

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Trubador_SaaS-Factory&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Trubador_SaaS-Factory)
**Quality Gate Status**

[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=Trubador_SaaS-Factory&metric=bugs)](https://sonarcloud.io/summary/new_code?id=Trubador_SaaS-Factory)
**Bugs**

[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=Trubador_SaaS-Factory&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=Trubador_SaaS-Factory)
**Code Smells**

[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Trubador_SaaS-Factory&metric=coverage)](https://sonarcloud.io/summary/new_code?id=Trubador_SaaS-Factory)
**Coverage**

[![Duplicated Lines](https://sonarcloud.io/api/project_badges/measure?project=Trubador_SaaS-Factory&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=Trubador_SaaS-Factory)
**Duplicated Lines (%)**

## 📄 App Blueprint {collapsible="true"}

_Blueprint for Deploying Any Type of SaaS_

Blueprint for deploying any type of B2B or B2C SaaS to accelerate development speed and time-to-market by reducing the
need for boilerplate code and infrastructure setup.

### 🛜 Port Mappings (Production - Docker compose)

<table>
            <tr>
                <td>Project</td>
                <td>Url</td>
            </tr>
            <tr>
                <td>Web</td>
                <td>https://localhost:8083, http://localhost:8082</td>
            </tr>            
            <tr>
                <td>Api</td>
                <td>https://localhost:8081/api, http://localhost:8080/api</td>
            </tr>
            <tr>
                <td>Gateway</td>
                <td>https://localhost:8085/gw/health, http://localhost:8084/gw/health</td>
            </tr>
</table>        

### Database Schema Diagram

**Database schema diagram for `Appblueprint-Dev-Db`:**

[![Explore database with Azimutt](https://img.shields.io/badge/PostgreSQL-browse_online-gray?labelColor=4169E1&logo=postgresql&logoColor=fff&style=flat)](https://azimutt.app/create?sql=https://diagram-hosting-proxy.casper-c7c.workers.dev/schema.sql)

### Application Projects

[App Blueprint Web project](AppBlueprint.Web_README.md)

[App Blueprint API project](AppBlueprint.ApiService_README.md)

[App Blueprint Developer Cli project](AppBlueprint.DeveloperCli_README.md)

[App Blueprint Application Gateway project](AppBlueprint.AppGateway_README.md)

[App Blueprint AppHost project](AppBlueprint.AppHost_README.md)

[App Blueprint Test project](AppBlueprint.Tests_README.md)

### Application Modules

[App Blueprint Architecture test module](AppBlueprint.ArchitectureTests_README.md)

[App Blueprint Application module](AppBlueprint.Baseline.Application_README.md)

[App Blueprint Service defaults module](AppBlueprint.ServiceDefaults_README.md)

[App Blueprint Ui Kit module](AppBlueprint.UiKit_README.md)

[App Blueprint Application infrastructure module](AppBlueprint.Infrastructure_README.md)

### Cloudflare Worker projects

 <!-- [Cloudflare Worker App Blueprint project](AppBlueprint.Worker_README.md) -->


## 🛂 Deployment Manager {collapsible="true"}

### Application Projects {id="deploymentmanager_application-projects"}

[Deployment Manager Web project](AppBlueprint.Web_README.md)

[Deployment Manager API project](AppBlueprint.ApiService_README.md)

[Deployment Manager Developer Cli project](AppBlueprint.DeveloperCli_README.md)

[Deployment Manager AppHost project](AppBlueprint.AppHost_README.md)

[Deployment Manager Test project](AppBlueprint.Tests_README.md)

### Application Modules {id="deploymentmanager_application-modules"}

[Deployment Manager Shared Kernel Module](AppBlueprint.Web_README.md)

## 👨‍💻 Development Workflow

_Getting started guide to develop both on the App Blueprint and Deployment Manager projects_

[Development Workflow](Code_README.md)

## 🎯 Vision

Deploy a new SaaS app project with one command via the Developer CLI, adding application-specific features while
fundamental components are preconfigured. Complete deployment within 30 minutes.

## 🌲 Purpose

The purpose is to deploy SaaS products for personal and business use, achieve financial freedom, and escape corporate
constraints.

## 📈 Objectives

- Fun to work with and develop
- Consolidated shared infrastructure
- Cost-effective, secure, and fast deployment
- Familiar tech stack (maximize C# usage)
- Automated processes for CI/CD, disaster recovery, and documentation
- Support for multiple environments: Dev, QA, staging, and production

## 🤔 Complexities

- Keeping multiple SaaS applications updated
- Balancing high-level vision with low-level troubleshooting
- Determining MVP deployment timelines
- Deciding when and how to include a Deployment Manager in future versions

## 🔗 Related Repositories

[Hosting](https://github.com/Trubador/Hosting)

[Milan Clean Architecture](https://github.com/Trubador/milanjanovich-clean-architecture-course)

[Didact](https://github.com/DidactHQ/didact-engine)

[Blazor Diagrams](https://github.com/Blazor-Diagrams/Blazor.Diagrams)


## 📁 File Structure

```
├─ .github                            # GitHub workflows
├─ Code                               # Application source code
│  ├─ AppBlueprint                    # SaaS Blueprint projects
│     ├─ ApiService                   # .NET API (REST API)    
│     ├─ AppGateway                   # .NET API Gateway with YARP proxy
│     ├─ AppHost                      # .NET Aspire AppHost
│     ├─ DeveloperCli                 # .NET CLI for developers
│     ├─ ServiceDefaults              # .NET Aspire service defaults
│     ├─ Tests                        # .NET tests (unit, integration, architecture, bunit)
│     ├─ Web                          # .NET Blazor server web app
│     ├─ Shared-Modules               # Clean Architecture shared modules
│       ├─ Api.Client.Sdk             # .NET architecture tests
│       ├─ Application                # .NET architecture tests
│       ├─ Contracts                  # .NET architecture tests
│       ├─ Domain                     # .NET architecture tests
│       ├─ Infrastructure             # .NET architecture tests
│       ├─ Presentation.ApiModule     # .NET architecture tests
│       ├─ SharedKernel               # .NET Aspire shared kernel
│       ├─ UiKit                      # .NET architecture tests

```
## ℹ️ Definitions

- **Entity**
- **DTO**
    - Request
    - Response
- **Database**
    - Migrations
    - Contexts

## 🔗 App Projects

[File Discovery](https://github.com/Trubador/File-Discovery)

[GroceryBot](https://github.com/Trubador/GroceryBot)

[Cooligram](https://github.com/Trubador/Cooligram)

[Honest dating](https://github.com/Trubador/Free-Dating)

[Better parking](https://github.com/Trubador/BetterParking)

[Probro](https://github.com/Trubador/Probro)

[Boligportal](https://github.com/Trubador/Boligportal)





