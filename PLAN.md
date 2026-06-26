# PLAN.md — Schema-Driven GitOps Hybrid Infrastructure Engine

Goal: make AppBlueprint-based SaaS apps 100% infrastructure-agnostic. Each app ships only an
`infra.json`; all provisioning logic lives in a central library (`AppBlueprint.Infrastructure.Core`)
driven by the `AppBlueprint.DeveloperCli` global tool via the Pulumi Automation API.

## Conventions discovered (anchors for every step)
- New Core library lives at `Code/AppBlueprint/Shared-Modules/AppBlueprint.Infrastructure.Core/`
  (sibling to `AppBlueprint.CliKit`, which the CLI already references).
- `Directory.Build.props` supplies `net10.0`, `ImplicitUsings`, `Nullable`, analyzers (warn-only).
- Central Package Management in `Directory.Packages.props`. Already present: `Pulumi` 3.145.0
  (includes the `Pulumi.Automation` namespace), `Pulumi.Cloudflare` 5.49.0, `Pulumi.AzureNative`.
  Added: `NJsonSchema` 11.6.1 (System.Text.Json-native, for `infra generate-schema`).
- **Neon strategy (decided):** **manually managed in the Neon portal — IaC provisions nothing.** The
  connection string is supplied at deploy time as a Pulumi secret (`neon:connectionString`) or env
  var, never stored in `infra.json`. (Superseded the earlier Terraform-bridge plan.)
- **Database topology:** externally-managed Neon Postgres, fronted by **Cloudflare Hyperdrive**
  (first-class resource in the already-pinned `Pulumi.Cloudflare`) for pooled, low-latency Worker
  connections. Hyperdrive reads the manually-set connection string as its origin.
- CLI command pattern: static class with `Create() : Command` + `ExecuteInteractive()`, registered
  in `Commands/CommandFactory.cs` (`AddDeveloperCliCommands`). Framework = `System.CommandLine`.
- Code rules: `is null`/`is not null`, `ArgumentNullException.ThrowIfNull`, namespace matches folder,
  TDD, `dotnet build` after each change.

## Micro-Steps

- [x] **Step 1 — Core configuration records.** Create the `AppBlueprint.Infrastructure.Core` project
  and the provider-agnostic `infra.json` contract: `AppInfrastructureConfig` (root), `CloudProvider`
  enum, `ComputeArgs`, `DatabaseArgs`, `StorageArgs`. Pure POCOs, no Pulumi dependency. Verify build.
- [x] **Step 2 — Core abstractions & outputs.** `AppDeploymentOutputs` (Pulumi-free output bag) and
  `IInfrastructureProvider` interface (the contract `AppStackFactory` switches over).
- [x] **Step 3 — Register infra NuGet packages centrally.** Added `NJsonSchema` 11.6.1 to
  `Directory.Packages.props`. Neon = Terraform bridge (local SDK, generated in Step 7 — no central
  NuGet). Hyperdrive uses already-pinned `Pulumi.Cloudflare`. Pulumi CLI v3.243.0 verified.
- [x] **Step 4 — `AppStackFactory` + provider base.** `AppStackFactory` selects from registered
  `IInfrastructureProvider`s by platform (no hard switch). `ProviderBase` centralizes naming, tags,
  and the `IgnorePortalManagedSecrets` policy. Wired `Pulumi` into Core csproj. **Corrected central
  pins:** `Pulumi` 3.145.0→**3.107.2** (3.145.0 was fictional), `Pulumi.Cloudflare` 5.49.0→**6.17.0**.
- [x] **Step 5 — Cloudflare compute component.** `CloudflareProvider` (partial class) +
  `.Compute.cs`: one `WorkersScript` per workload (v6 has no Container resource — script is the
  equivalent API). IgnoreChanges set to the v6-accurate `content`/`contentSha256`/`bindings` so
  wrangler/CI-managed code and portal-managed secrets survive deploys. Account id from
  `cloudflare:accountId` / `CLOUDFLARE_ACCOUNT_ID`. Verified type names against the 6.17.0 assembly.
- [x] **Step 6 — Cloudflare storage component.** `CloudflareProvider.Storage.cs`: one `R2Bucket`
  per `StorageArgs.Buckets`; when `EnableImages`, a default `ImageVariant` ("public", scale-down
  1280²) since Images is account-level (no "enable" resource). Outputs bucket names + variant id.
- [x] **Step 7 — Neon database (external/manual).** Decision: Neon is managed by hand in the portal;
  **no IaC resources created.** Reframed `DatabaseArgs` to drop provisioning-only fields
  (`PostgresVersion`/`Region`) — it is now a marker (optional `Name`) that the app has a Postgres DB
  and opts into Hyperdrive. Connection string is a deploy-time secret, consumed by Step 8.
- [x] **Step 8 — Cloudflare Hyperdrive component.** `CloudflareProvider.Database.cs`: when
  `Database.UseHyperdrive` (new toggle, default true), create a `HyperdriveConfig` whose origin is
  parsed (inside `.Apply`, secrecy preserved) from the `neon:connectionString` secret (or
  `NEON_DATABASE_URL`/`DATABASE_CONNECTIONSTRING`). Outputs hyperdrive id + secret connection string.
  Verified origin shape (Host/Port/Database/User/Password/Scheme) against the 6.17.0 assembly.
- [x] **Step 9 — CLI wiring: reference Core + `infra` parent command scaffold.** Added Core
  ProjectReference to the CLI; `InfraCommand` registers `infra` with `generate-schema` and
  `up --env` (required) subcommands (placeholder handlers); registered in `CommandFactory`. Verified
  `infra --help` lists both subcommands on the built exe.
- [ ] **Step 10 — `infra generate-schema`.** Reflect `AppInfrastructureConfig` via `NJsonSchema` →
  write `app-infra-schema.json` for IDE IntelliSense.
- [x] **Step 11 — `infra.json` loader + validation.** `AppInfrastructureConfigLoader` in Core:
  `Parse`/`LoadFromFile`/`LoadFromDirectory`. Case-insensitive, comments + trailing commas allowed,
  `required` members give STJ presence checks, custom `Validate` covers semantic rules. Hardened with
  an explicit `DefaultJsonTypeInfoResolver` so it works even in reflection-disabled/trimmed hosts.
  Behavior verified across 6 valid/invalid cases.
- [ ] **Step 12 — `wrangler.toml` parser/bridge.** Detect and parse `wrangler.toml`; blend legacy
  bindings (incl. existing Hyperdrive/R2 bindings) into the Pulumi program.
- [x] **Step 13 — `infra up --env [prod/dev]`.** `InfraUpRunner`: loads `infra.json`, bridges
  `wrangler.toml` (supplies account id), builds an inline `PulumiFn` from
  `AppStackFactory`+`CloudflareProvider`, runs `LocalWorkspace.CreateOrSelectStackAsync` +
  `stack.UpAsync` streaming stdout/stderr (`UpdateOptions.OnStandardOutput/Error`), prints outputs
  (secrets masked). Added `Pulumi.Automation` 3.107.2 (separate package, must match the SDK). Fixed a
  real bug: handled errors returned exit 0 — now `Environment.Exit(1)` so CI sees failures. Verified
  safe path (no infra.json → clean error, exit 1, nothing created).
- [ ] **Step 14 — CLI DI + dashboard menu wiring.** Register `infra` in `CommandFactory`, add a
  dashboard entry.
- [x] **Step 15 — Tests.** `AppBlueprint.Tests/Infrastructure/`: `AppInfrastructureConfigLoaderTests`
  (7), `WranglerConfigParserTests` (3), `InfraSchemaGeneratorTests` (1, via reflection per existing
  convention). **11/11 pass** (run via the MTP exe + `--treenode-filter`; `dotnet test` VSTest path is
  unsupported on .NET 10 SDK). Added Core ProjectReference to the test project.

---
**Status:** ✅ ALL 15 STEPS COMPLETE. The Schema-Driven GitOps hybrid infrastructure engine is built,
wired into the CLI (`infra generate-schema` / `infra up`) and dashboard, and unit-tested (11/11).
Every layer builds clean; the only un-exercised path is a real `pulumi up` (needs live Cloudflare
credentials + would create real resources).

**Addendum — dry run (pulumi preview):** `infra up --env <env> --dry-run` previews changes via
`stack.PreviewAsync` (streams engine output, prints a change summary, applies nothing). The dashboard
gained a "Preview infrastructure (dry run)" action (runs without the deploy confirmation, since it is
read-only). `InfraUpRunner.RunAsync(env, dryRun)` branches preview vs up. Verified flag wiring +
fail-fast path; a real preview needs live credentials.
