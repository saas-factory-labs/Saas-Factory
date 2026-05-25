# Migration Isolation Strategy

When AppBlueprint packages are installed as NuGet dependencies by downstream SaaS apps
(property rental portal, dating app, parking app, etc.), the apps need to run their own
EF Core migrations without colliding with the base AppBlueprint migrations.

## Current approach: Option D — Separate history tables (active)

Each AppBlueprint DbContext writes its applied migrations to a dedicated tracking table
instead of the default `__EFMigrationsHistory`:

| DbContext | History table |
|---|---|
| `ApplicationDbContext` | `__AppBlueprintMigrationsHistory` |
| `BaselineDbContext` | `__AppBlueprintBaselineMigrationsHistory` |
| `B2BDbContext` | `__AppBlueprintB2BMigrationsHistory` |
| `B2CdbContext` | `__AppBlueprintB2CMigrationsHistory` |

Downstream apps use the default `__EFMigrationsHistory` table (or name their own).
Both sets of migrations coexist in the same database with no collision.

**Deployment sequence for a consumer app:**

```bash
# 1. Apply AppBlueprint base migrations
dotnet ef database update --project path/to/AppBlueprint.Infrastructure \
  --startup-project path/to/YourApp

# 2. Apply your app-specific migrations
dotnet ef database update --project path/to/YourApp.Infrastructure
```

**When adding a new AppBlueprint migration:**

```bash
dotnet ef migrations add MigrationName \
  --project Shared-Modules/AppBlueprint.Infrastructure \
  --startup-project AppBlueprint.ApiService \
  --context BaselineDbContext
```

**Limitation:** Consumer apps cannot add columns to AppBlueprint-owned tables
(e.g. `Users`, `Tenants`) via their own EF migrations. For that, see Option B or C below.

---

## Option A — Migration bundles

Compile migrations into a self-contained binary and ship it alongside the package.
Consumer apps run the binary at deploy time; they never interact with the migration files.

```bash
dotnet ef migrations bundle \
  --self-contained -r linux-x64 \
  --output ./artifacts/migrate-appblueprint \
  --project Shared-Modules/AppBlueprint.Infrastructure \
  --startup-project AppBlueprint.ApiService
```

Publish `migrate-appblueprint` as a GitHub Release asset alongside each NuGet package version.

**Pros:**
- Consumers have zero migration surface — no EF tooling needed at deploy time
- Bundle is fast and self-contained (no dotnet SDK required on the host)
- Cleanest contract: the package owns its schema fully

**Cons:**
- A new bundle must be generated and published for every schema change
- Consumers cannot extend base tables (Users, Tenants, etc.) via EF
- CI pipeline must build the bundle per target runtime (`linux-x64`, `linux-arm64`, etc.)
- Harder to debug migration failures inside the bundle

**Best for:** Production deployment pipelines, container-based deployments, apps where
the base schema is stable and unlikely to be extended.

---

## Option B — MigrationsAssembly in consumer project

The package ships with no migration files. The DbContext is configured to resolve
migrations from the consuming assembly using a generic startup type:

```csharp
// In AppBlueprint.Infrastructure ServiceCollectionExtensions
options.UseNpgsql(connectionString, o =>
    o.MigrationsAssembly(typeof(TStartup).Assembly.FullName)
     .MigrationsHistoryTable("__EFMigrationsHistory"));
```

The consumer runs `dotnet ef migrations add Initial` and gets a single migration
that creates all base tables plus their own tables together.

**Pros:**
- Consumer has full EF control — can add columns to Users, Tenants, or any base table
- Single migration history, single `dotnet ef database update`
- Standard EF workflow, no new tooling concepts

**Cons:**
- Each consumer re-creates all base tables from scratch in their own migration file
- When the package releases a schema change, consumers must add a new migration manually
  and understand what changed
- Requires the package to expose a `TStartup` registration pattern (API change)

**Best for:** Apps that need to extend base entities (e.g. a dating app adding
`ProfilePhotoUrl` to the `Users` table).

---

## Option C — Separate `AppBlueprint.Infrastructure.Migrations` NuGet package

Extract all migration files into a second package:
`SaaS-Factory.AppBlueprint.Infrastructure.Migrations`

The main Infrastructure package's DbContext points to this assembly:

```csharp
options.UseNpgsql(connectionString, o =>
    o.MigrationsAssembly("SaaS-Factory.AppBlueprint.Infrastructure.Migrations")
     .MigrationsHistoryTable("__AppBlueprintMigrationsHistory"));
```

Consumers who want the managed base schema install both packages.
Consumers who want full control skip the migrations package and use Option B.

**Pros:**
- Clean separation — Infrastructure package is stable; migrations package versions independently
- Consumers can opt in or out of managed migrations
- Pattern familiar from other OSS libraries (e.g. Identity, OpenIddict)

**Cons:**
- Two packages to publish and keep in sync on every schema change
- Must document the version compatibility matrix between the two packages
- Consumers who skip the migrations package still cannot extend base tables without Option B

**Best for:** When the base schema stabilises and migration churn slows down;
good intermediate step before Option A.

---

## Recommended evolution path

```
Now      → Option D  (separate history tables, zero structural change)
6 months → Option C  (extract migrations package, cleaner consumer story)
1 year   → Option A  (migration bundles for production, zero consumer burden)
          + Option B  available for apps that need to extend base entities
```
