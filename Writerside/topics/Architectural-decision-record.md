# 📙 Architectural Decision Record for SaaS Factory

## 🏗️ Key Architectural Decisions

### API Architecture: Separate API per Project [APPROVED]

**Decision:** Each SaaS app project will have its own separate API.

#### Original Shared API Concept
- Shared API for all deployed SaaS apps
- Shared API for Deployment Manager
- Shared API for App Blueprint

#### Rationale for Separate APIs

**Disadvantages of Shared API:**
- Higher complexity
- Single point of failure for all projects
- Risk of database cross-contamination and data leakage between SaaS apps

**Advantages of Shared API (rejected):**
- Lower technical debt with no code drift between AppBlueprint and app projects
- No duplicated API code
- Easier to maintain and update

---

## 📦 Core Components

### Deployment Manager
**Purpose:** Command center to control deployment, monitoring and management of deployed SaaS apps

**Capabilities:**
- Shared Infrastructure with deployed SaaS apps
- Integration application landscape
- Customer management and growth metrics tracking

### App Blueprint
**Purpose:** Template for deploying a SaaS App

**Use Case:** Serves as the foundation for all new SaaS applications

### Deployed SaaS App
**Purpose:** Personal or commercial App (B2C, B2B, or Developer-first)

**Example:** FileDiscovery application

---

## 🎯 Application Architecture

### Design Principles
- **Domain Driven Design (DDD)** - Business logic organized around domain concepts
- **Clean Architecture** - Separation of concerns and dependency inversion
  - Low technical debt
  - High reliability

---

## 📂 Project Structure

### Deployment Manager Modules

#### DeploymentManager.Web
- Command center portal

#### DeploymentManager.Api
- REST API
- GraphQL API

#### DeploymentManager.Codeflow
- Dependency Tracker

#### DeploymentManager.Shared
- Shared DTOs
- Shared Enums
- Shared Entities

#### Shared Infrastructure
- Cross-cutting infrastructure components

### Deployed SaaS App Structure (App-Blueprint Template)

#### FileDiscovery.Web
- Customer management portal
- Partner portal
- Admin portal

#### FileDiscovery.Api
- REST API
- GraphQL API

---

## 🛠️ Tech Stack

### Frontend

#### Blazor
**Why Blazor:**
- .NET familiarity
- High performance
- Can run everywhere (WebAssembly or hosted server web app)
- Strong integration with .NET backend technologies

**UI Framework:**
- **MudBlazor** - Fast user interface development
  - 3rd party mudextensions: https://codebeam-mudextensions.pages.dev

---

### Backend

#### .NET
**Why .NET:**
- Familiarity → High productivity
- High performance

**Additional Components:**
- **YARP Proxy** - Reverse proxy
- **Authentication solution** (e.g., Supertokens)

---

### Infrastructure

#### Database & Storage
- **Railway** - PostgreSQL databases
- **Cloudflare R2** - Blob storage
- **Redis Cloud** - Caching

#### Search & Analytics
- **Algolia** - Search service
- **Microsoft Clarity** - User behavior analytics
- **Google Analytics** - Web analytics
- **Google Tag Manager** - Tag management

#### External Services
- **Stripe** - Payment processing
- **Resend** - Email delivery
- **Logsnag** - Event tracking
- **Cookiebot** - Cookie consent management
- **Google reCAPTCHA** - Bot protection

#### Cloud Services
- **Cloudflare** - CDN and security
- **Azure Key Vault** - Secret management
- **Grafana Cloud** - Monitoring and visualization

---

### Monitoring & Observability

- **OpenTelemetry** - Distributed tracing and metrics

---

### CI/CD

- **GitHub Actions** - Workflow automation
- **Pulumi Automation API** - Infrastructure as code
- **Cloudcostify API** - Cost tracking
- **SonarCloud** - Code quality and security scanning

---

### Testing

#### Frameworks & Libraries
- **NSubstitute** - Mocking framework
- **xUnit** - Unit testing framework
- **Bogus** - Test data generation

#### Testing Types
- Architecture testing
- Function testing
- Integration testing
- **Performance testing (Grafana K6)**
  - Smoke tests
  - Load tests

---

### Documentation

#### Tools & Standards
- Deployment Manager dependency tracking map
- **Dive** - Docker image analysis
- GitHub Copilot automatic documentation
- Automatic Swagger REST API documentation
- Automatic GraphQL schema documentation
- OpenTelemetry dependencies map

#### Standards
- Naming conventions
- **SonarCloud Quality Gate**
  - Technical debt < 1 hour
- Project file structure guidelines

---

### Security

#### Security Measures
- **CORS** - Cross-Origin Resource Sharing
- **CSRF** - Cross-Site Request Forgery protection
- **HSTS** - HTTP Strict Transport Security
- **CSP** - Content Security Policy
- **Zero trust** architecture & no hardcoded credentials
- **IP whitelisting**
- Continuous monitoring
- Regular pentesting
- **Cloudflare DDoS protection**
