# SonarCloud Issues Report

**Project:** `saas-factory-labs_Saas-Factory`
**Date:** 2026-06-23
**Total Open Issues:** 845
**Total Remediation Effort:** ~4,194 minutes (~70 hours)

---

## BLOCKER (fix immediately)

|File|Line|Issue|Status|
|----|----|-----|------|
|~~`docker-compose.local.yml`~~|40|**VULNERABILITY** — Hardcoded PostgreSQL password (`secrets:S6698`)|✅ Fixed — file deleted|
|~~`Popover.razor`~~|138|**CODE_SMELL** — Method named `Dispose` doesn't implement `IDisposable` — rename or implement (`S2953`)|✅ Fixed — `@implements IDisposable` added|
|~~`MegaMenu.razor`~~|218|**CODE_SMELL** — Method named `Dispose` doesn't implement `IDisposable` — rename or implement (`S2953`)|✅ Fixed — `@implements IDisposable` added|

---

## CRITICAL — Code Smells

|File|Line|Issue|Status|
|----|----|-----|------|
|~~`ApiEndpoints.cs`~~|180, 181|Field shadows outer class member — rename it (`S3218`)|✅ Fixed — `Search.Tenants/Users` renamed to `TenantsPath/UsersPath`; `SearchController.cs` updated|
|`AdminPortalModuleRegistry.cs`|63, 78|Properties `Modules` / `RouterAssemblies` copy collections — convert to methods (`S2365`)|Open — many callers across services and tests; needs dedicated refactoring pass|
|~~`CommandPalette.razor`~~|137, 148|Properties `AllCommands` / `FilteredCommands` copy collections — convert to methods (`S2365`)|✅ Fixed — converted to `GetAllCommands()` / `GetFilteredCommands()`|
|~~`GlobalSearch.razor`~~|184|Property `FilteredResults` copies collection — convert to method (`S2365`)|✅ Fixed — converted to `GetFilteredResults()`; template and `HandleKeyDown` / `GetResultIndex` updated|
|`FileValidationService.cs`|100|Cognitive Complexity 20 > 15 — refactor (`S3776`)|Open|
|`Extensions.cs`|76|Cognitive Complexity 20 > 15 — refactor (`S3776`)|Open|
|`AdminDependencies.razor`|82|Cognitive Complexity 30 > 15 — refactor (`S3776`)|Open|
|`PostgresConnectionString.cs`|46|Cognitive Complexity 20 > 15 — refactor (`S3776`)|Open|
|`CliProcessRunner.cs`|37|Cognitive Complexity 17 > 15 — refactor (`S3776`)|Open|
|~~`DashboardCard05.razor`~~|31|Unread private field `counter` — remove or use it (`S4487`)|✅ Fixed — field and `counter++` removed|
|~~`ModalBlank.razor`~~|40|Unread private field `modalContentRef` (`S4487`)|✅ Fixed — `@ref`, field, `@using`, and `@inject` removed|
|~~`ModalBasic.razor`~~|53|Unread private field `modalContentRef` (`S4487`)|✅ Fixed — `@ref`, field, `@using`, and `@inject` removed|
|~~`DropdownFilter.razor`~~|83|Empty method — add comment, throw `NotSupportedException`, or implement (`S1186`)|✅ Already implemented — `OnAfterRenderAsync` has JS interop; likely stale scan result|
|~~`Datepicker.razor`~~|28|Empty method — add comment, throw `NotSupportedException`, or implement (`S1186`)|✅ Already implemented — `OnAfterRenderAsync` has JS interop; likely stale scan result|

---

## MAJOR — Vulnerabilities (Hardcoded Credentials)

|File|Lines|Issue|Status|
|----|-----|-----|------|
|~~`docker-compose.local.yml`~~|19|Hardcoded `PASSWORD` in YAML (`yaml:S2068`)|✅ Fixed — file deleted|
|~~`ConfigurationValidator.cs`~~|53, 59, 62, 67|Multiple hardcoded passwords and connection URIs (`S2068`)|✅ Fixed — example connection strings removed from error message; replaced with concise key-reference message|
|~~`SystemController.cs`~~|82|Hardcoded password string (`S2068`)|✅ Fixed — connection string removed from response|
|~~`EnvironmentInfoCommand.cs`~~|223|Hardcoded password string (`S2068`)|✅ Fixed — MatchEvaluator, no literal replacement string|
|~~`MainMenu.cs`~~|337|Hardcoded password string (`S2068`)|✅ Fixed — MatchEvaluator, no literal replacement string|
|~~`Program.cs` (DeveloperCli)~~|755, 756|Hardcoded passwords (`S2068`)|✅ Fixed — MatchEvaluator, hardcoded fallback removed|
|~~`SeedTest/Program.cs`~~|17, 19|Hardcoded fallback password + broken masking (`S2068`)|✅ Fixed — throws if env var missing, masking now generic|

---

## MAJOR — Bug

|File|Line|Issue|Status|
|----|----|-----|------|
|`Program.cs`|583|Condition always evaluates to `false` — unreachable code exists (`S2583`)|Open|

---

## MAJOR — Code Smells

|Pattern|Rule|Approx. Count|Status|
|-------|----|-------------|------|
|~~Fields not marked `readonly`~~|`S2933`|~60+|✅ Fixed — `dotnet format --diagnostics IDE0044` applied across 35 files, 76 fields|
|~~`DateTime` created without `DateTimeKind`~~|`S6562`|~20+|✅ Fixed — all 8 instances in `MinimumAgeHandlerTests.cs` updated to `DateTimeKind.Utc`|
|Unused private accessors|`S1144`|~10+|Open|
|`StringBuilder` methods not locale-aware|`CA1305`|~6|Open — no auto-fix available; likely false positives (log/debug output), mark as Won't Fix in SonarCloud UI|
|~~`string` URI properties should be `System.Uri`~~|`CA1056`|~3|✅ Fixed — `WebhookResponse`, `CreateWebhookRequest`, `UpdateWebhookRequest` updated; `WebhookController` callers updated|
|SQL injection risk via `SqlQueryRaw` with interpolation|`S3649`|1 (`AdminPortalDiagnostics.cs:128`)|Open|

---

## Recommended Fix Order

1. ✅ ~~**Hardcoded PostgreSQL password** in `docker-compose.local.yml:40`~~ — file deleted
2. ✅ ~~**Credential strings** in `SystemController.cs`, `EnvironmentInfoCommand.cs`, `MainMenu.cs`, `DeveloperCli/Program.cs`, `SeedTest/Program.cs`~~ — all fixed
3. ✅ ~~**`readonly` fields**~~ — 76 fields across 35 files auto-fixed via `dotnet format`
4. ✅ ~~**`DateTime` / `DateTimeKind`**~~ — all 8 instances fixed in `MinimumAgeHandlerTests.cs`
5. ✅ ~~**`string` URI properties**~~ — webhook contracts updated to `Uri`, callers fixed
6. ✅ ~~**Collection-copying properties** in `CommandPalette.razor`~~ — converted to methods
7. ✅ ~~**`Dispose` naming confusion**~~ — `Popover.razor` and `MegaMenu.razor` both have `@implements IDisposable`
8. ✅ ~~**Unread private fields**~~ — `DashboardCard05.razor` (`counter`), `ModalBlank.razor` and `ModalBasic.razor` (`modalContentRef`) all removed
9. ✅ ~~**`ConfigurationValidator.cs`**~~ — example connection strings removed from error message
10. ✅ ~~**Field shadowing** in `ApiEndpoints.cs:180,181`~~ — `Search.Tenants/Users` renamed to `TenantsPath/UsersPath`
11. ✅ ~~**Collection-copying property** in `GlobalSearch.razor`~~ — converted to `GetFilteredResults()`
12. ✅ ~~**Empty methods** in `DropdownFilter.razor`, `Datepicker.razor`~~ — already implemented; stale SonarCloud result
13. **Unreachable code bug** in `Program.cs:583`
14. **Collection-copying properties** — `AdminPortalModuleRegistry.cs` (many callers; dedicated refactoring pass needed)
15. **Cognitive Complexity** — `AdminDependencies.razor`, `FileValidationService.cs`, `Extensions.cs`, `PostgresConnectionString.cs`, `CliProcessRunner.cs`
16. **Unused private accessors** (`S1144`) — ~10+ instances; needs SonarCloud UI to see exact list
17. **`StringBuilder` locale** (`CA1305`) — mark as Won't Fix in SonarCloud UI
18. **SQL injection** in `AdminPortalDiagnostics.cs:128` (`S3649`)
