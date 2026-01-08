# Database Hybrid Mode - Documentation Index

This directory contains comprehensive documentation for AppBlueprint's flexible Database Context configuration.

## 📚 Documentation Files

### 1. **[DATABASE_HYBRID_MODE_SETUP.md](./DATABASE_HYBRID_MODE_SETUP.md)** ⭐ START HERE
**Purpose:** Quick reference for the AppBlueprint demo app Hybrid Mode configuration  
**Audience:** Developers working on the demo app  
**Length:** ~350 lines  
**Contents:**
- Why AppBlueprint uses Hybrid Mode
- Current configuration (already applied)
- How it works in code
- Production deployment options
- Troubleshooting guide
- Verification steps

**Use when:** Setting up or understanding the demo app's B2C/B2B dynamic dashboard feature

---

### 2. **[Shared-Modules/DATABASE_CONTEXT_FLEXIBILITY_GUIDE.md](./Shared-Modules/DATABASE_CONTEXT_FLEXIBILITY_GUIDE.md)**
**Purpose:** Complete guide to all DbContext configuration options  
**Audience:** All developers, architects  
**Length:** ~930 lines  
**Contents:**
- Overview of Baseline, B2C, B2B, Hybrid modes
- Complete configuration reference
- 6 detailed usage examples:
  1. B2C Consumer SaaS
  2. B2B Organization SaaS
  3. Minimal Microservice (Baseline)
  4. Marketplace Platform (Hybrid)
  5. **Demo App with Dynamic Dashboards (Hybrid)** - NEW!
  6. Custom Feature-Specific Context (Dating App)
- Testing with TestContainers
- Advanced scenarios (multiple databases, read replicas)
- Troubleshooting guide
- Best practices

**Use when:** Need comprehensive understanding of all context types and configuration options

---

### 3. **[Shared-Modules/DatabaseContexts/Examples/README.md](./Shared-Modules/DatabaseContexts/Examples/README.md)**
**Purpose:** Configuration file examples for different app types  
**Audience:** Developers setting up new projects  
**Length:** ~430 lines  
**Contents:**
- Quick start guide
- appsettings.b2c.example.json
- appsettings.b2b.example.json
- appsettings.hybrid.example.json
- appsettings.baseline.example.json
- Step-by-step configuration instructions
- Testing and verification steps

**Use when:** Need copy-paste configuration templates

---

## 🔍 Quick Reference

### When to Use Each Context Type

| Context Type | Use Case | Documentation |
|-------------|----------|---------------|
| **Hybrid** (Demo App) | Apps with both B2C and B2B features, dynamic UIs | [DATABASE_HYBRID_MODE_SETUP.md](./DATABASE_HYBRID_MODE_SETUP.md) |
| **B2C** | Consumer apps (fitness, finance, personal tools) | [Section in Flexibility Guide](./Shared-Modules/DATABASE_CONTEXT_FLEXIBILITY_GUIDE.md#example-1-b2c-consumer-saas-default) |
| **B2B** | Enterprise apps (CRM, project management) | [Section in Flexibility Guide](./Shared-Modules/DATABASE_CONTEXT_FLEXIBILITY_GUIDE.md#example-2-b2b-organization-saas) |
| **Baseline** | Microservices, custom contexts | [Section in Flexibility Guide](./Shared-Modules/DATABASE_CONTEXT_FLEXIBILITY_GUIDE.md#example-3-minimal-microservice-baseline-only) |

### Configuration Quick Copy

**Hybrid Mode (Demo App):**
```bash
DatabaseContext__ContextType="B2C"
DatabaseContext__EnableHybridMode="true"
```

**B2C Only:**
```bash
DatabaseContext__ContextType="B2C"
```

**B2B Only:**
```bash
DatabaseContext__ContextType="B2B"
```

**Baseline Only:**
```bash
DatabaseContext__BaselineOnly="true"
```

---

## 🎯 Common Tasks

### Task: "I want to understand why AppBlueprint uses Hybrid Mode"
→ Read: [DATABASE_HYBRID_MODE_SETUP.md](./DATABASE_HYBRID_MODE_SETUP.md) (Section: "Why Hybrid Mode?")

### Task: "I need to configure a new B2C consumer app"
→ Read: [Examples README](./Shared-Modules/DatabaseContexts/Examples/README.md) (appsettings.b2c.example.json)

### Task: "I'm building a marketplace with buyers and sellers"
→ Read: [Flexibility Guide](./Shared-Modules/DATABASE_CONTEXT_FLEXIBILITY_GUIDE.md) (Example 4: Marketplace Platform)

### Task: "How do I create a custom context for my feature module?"
→ Read: [Flexibility Guide](./Shared-Modules/DATABASE_CONTEXT_FLEXIBILITY_GUIDE.md) (Example 6: Dating App)

### Task: "I'm getting 'Unable to resolve service for type B2BDbContext' errors"
→ Read: [Hybrid Mode Setup - Troubleshooting](./DATABASE_HYBRID_MODE_SETUP.md#troubleshooting)

### Task: "I need to configure different databases for B2C and B2B"
→ Read: [Flexibility Guide - Advanced Scenarios](./Shared-Modules/DATABASE_CONTEXT_FLEXIBILITY_GUIDE.md#multiple-databases)

---

## 📍 Current AppBlueprint Configuration

**Status:** ✅ Hybrid Mode **ACTIVE**

**Configured in:**
- `AppBlueprint.ApiService/Properties/launchSettings.json`
- `AppBlueprint.Web/Properties/launchSettings.json`
- `AppBlueprint.Web/Program.cs` (migrated from legacy to flexible config)

**What's registered:**
- ✅ BaselineDbContext
- ✅ B2CdbContext
- ✅ ApplicationDbContext
- ✅ B2BDbContext

**Use cases enabled:**
- ✅ B2C user signup flow → Personal dashboard
- ✅ B2B user signup flow → Organization dashboard with Teams/API Keys
- ✅ TodoAppKernel (extends B2BDbContext)
- ✅ UnitOfWork (requires both contexts)
- ✅ Dynamic UI based on user account type

---

## 🔗 Related Documentation

- [🏗️ Clean Architecture Dependencies](./.github/.ai-rules/baseline/clean-architecture-dependencies.md) - Layer dependency rules
- [🔐 Multi-Tenancy Guide](./Shared-Modules/MULTI_TENANCY_GUIDE.md) - Tenant isolation with RLS
- [🧪 Testing Guide](./.github/.ai-rules/tests/README.md) - Unit, integration, UI tests
- [⚙️ Backend Guidelines](./.github/.ai-rules/backend/README.md) - C# backend patterns

---

## 📊 Documentation Coverage Summary

| Topic | Coverage | Status |
|-------|----------|--------|
| Hybrid Mode (Demo App) | Dedicated guide + examples | ✅ Complete |
| B2C Configuration | Full guide + examples | ✅ Complete |
| B2B Configuration | Full guide + examples | ✅ Complete |
| Baseline Only | Full guide + examples | ✅ Complete |
| Custom Contexts | Dating app example | ✅ Complete |
| Marketplace Apps | Full example with code | ✅ Complete |
| Configuration Options | All options documented | ✅ Complete |
| Troubleshooting | Common issues + solutions | ✅ Complete |
| Production Deployment | Environment vars, Key Vault, Docker | ✅ Complete |
| Testing | TestContainers integration | ✅ Complete |
| Migration Guide | Legacy → Flexible config | ✅ Complete |

---

## 🚀 Next Steps

1. **New to AppBlueprint?** Start with [DATABASE_HYBRID_MODE_SETUP.md](./DATABASE_HYBRID_MODE_SETUP.md)
2. **Building a new app?** Choose your context type and read the relevant section in [DATABASE_CONTEXT_FLEXIBILITY_GUIDE.md](./Shared-Modules/DATABASE_CONTEXT_FLEXIBILITY_GUIDE.md)
3. **Need configuration templates?** Copy from [Examples README](./Shared-Modules/DatabaseContexts/Examples/README.md)
4. **Troubleshooting?** Check troubleshooting sections in both main guides

---

**Last Updated:** January 3, 2026  
**AppBlueprint Version:** .NET 10 + Aspire 13.0.0  
**Configuration Status:** Hybrid Mode Active ✅
