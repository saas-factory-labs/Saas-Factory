# AppBlueprint.AdminPortalKernel

Generic machinery for per-app **admin portals** hosted by the DeploymentManager shell.
The shell (a public, internal-only Blazor Server app) loads one thin **admin module**
(plugin) per deployed SaaS app — boligportal, dating app, etc. Each module ships from its
own **private** repo as a NuGet package / dll, so admin pages and admin API endpoints are
never deployed inside the public-facing SaaS apps.

This package provides, generically (works for any AppBlueprint-based app since they all
share the baseline `"Users"`/`"Tenants"` schema):

- The `IAdminPortalModule` contract + registry + runtime plugin loader.
- Routable pages defined once and reused by every module:
  `/apps/{slug}/admin`, `/apps/{slug}/admin/users`, `/apps/{slug}/admin/tenants`,
  `/apps/{slug}/admin/audit`.
- User administration (search / view / activate / deactivate), tenant overview,
  dashboard stats and an immutable audit log.
- `AdminPortalSecurityInspector`: reflection checks that fail the build (and host startup)
  if any page or controller is not gated to `DeploymentManagerAdmin`.

UI is **Tailwind / Cruip (via AppBlueprint.UiKit)** — no MudBlazor.

## Authoring an admin module (in your private app repo)

### 1. Project file

A Razor Class Library referencing only this kernel package. `EnableDynamicLoading`
makes the dll self-contained for runtime loading:

```xml
<Project Sdk="Microsoft.NET.Sdk.Razor">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <EnableDynamicLoading>true</EnableDynamicLoading>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="SaaS-Factory.AppBlueprint.AdminPortalKernel" Version="<host-version>" />
  </ItemGroup>
</Project>
```

### 2. Module class

```csharp
public sealed class DatingAdminModule : IAdminPortalModule
{
    public string Slug => "dating";          // lowercase letters/digits/hyphens
    public string DisplayName => "Dating App";
}
```

The four generic pages now work for `dating` as soon as the host has a connection
string configured. Add your own pages under `Components/Pages/` — they **must** route
under `/apps/dating/` and be role-gated.

### 3. Mandatory `Components/Pages/_Imports.razor`

RCL pages do **not** inherit the host's `_Imports.razor`, so the role gate must live in
your module:

```razor
@using Microsoft.AspNetCore.Authorization
@using AppBlueprint.Application.Constants
@attribute [Authorize(Roles = Roles.DeploymentManagerAdmin)]
```

### 4. Guard it in your own tests

```csharp
var violations = AdminPortalSecurityInspector.InspectModuleAssembly(
    typeof(DatingAdminModule).Assembly, ["dating"]);
violations.Should().BeEmpty();
```

## Version alignment (hard requirement)

Modules are loaded into the host's **default** AssemblyLoadContext and render in-process,
so they must be compiled against the **same versions** the host uses:

| Component | Version |
|---|---|
| .NET | `net10.0` |
| `SaaS-Factory.AppBlueprint.AdminPortalKernel` | match the DeploymentManager host |
| `AppBlueprint.UiKit` (Tailwind/Cruip styles) | match the host |

Mismatches surface at restore (`NU1107`) or at load (the host logs each plugin's
assembly version). Modules **must not** ship their own static web assets — they use the
host's UiKit CSS/JS.

## App database access & RLS

The host resolves each module's app database from
`AdminPortal:Modules:{slug}:ConnectionString`
(Railway env var `AdminPortal__Modules__{slug}__ConnectionString`).

Use a **dedicated per-app DB user** that can `SELECT` on `"Users"`/`"Tenants"` and
`UPDATE "Users"."IsActive"`. The kernel always sets the RLS session variables
(`app.is_admin`, `app.current_tenant_id`) used by AppBlueprint's policies; if the app DB
enables RLS, ensure its cross-tenant SELECT policy honours `app.is_admin = 'true'`.
Every read context and every write action is recorded in the host's `dm_admin_audit`
table with the acting admin, app slug, target, reason and UTC timestamp.

## Shipping the module

- **Local dev (Flow 1):** post-build copy `bin/<cfg>/net10.0/SaaSFactory.<App>.Admin.dll`
  (+ `.pdb`) to the shared `local-plugins` folder (sibling of the repo root). The host's
  `dotnet watch` restarts and picks it up. A reference implementation lives at
  `Code/DeploymentManager/Samples/SaaSFactory.Sample.Admin` with
  `copy-sample-plugin.ps1`.
- **CI/CD (Flow 2):** publish the module as a nupkg to your private GitHub Packages feed.
  The public repo's deploy pipeline runs
  `Code/DeploymentManager/Scripts/download-admin-plugins.ps1` to bake the dlls into the
  DeploymentManager image's `plugins/` folder. Plugin names live in GitHub Actions
  variables, never in committed code.

Post-build copy snippet for the module csproj:

```xml
<Target Name="CopyToLocalPlugins" AfterTargets="Build"
        Condition="Exists('$(LocalPluginsPath)')">
  <Copy SourceFiles="$(TargetDir)$(AssemblyName).dll;$(TargetDir)$(AssemblyName).pdb"
        DestinationFolder="$(LocalPluginsPath)" />
</Target>
```
