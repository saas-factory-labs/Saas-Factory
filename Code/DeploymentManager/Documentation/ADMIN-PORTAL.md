# Admin Portal — host operations

DeploymentManager is a CSP-style shell: you log in once, and from the **Admin Portals**
sidebar section you open the admin portal for an individual deployed SaaS app
(boligportal, dating app, …) to administer its users — like logging into a customer's
Azure tenant from the Microsoft CSP partner portal.

Each app's admin portal is a runtime-loaded **plugin** (`SaaSFactory.<App>.Admin.dll`)
built in that app's private repo. The generic machinery lives in
`AppBlueprint.AdminPortalKernel`; see its README for authoring a module.

```
login → DeploymentManager → Admin Portals ▸ Dating App
      → /apps/dating/admin  (dashboard, users, tenants, audit)
```

Single Logto session covers everything (the portals are embedded in the shell — no
second login). Everything is gated to the `DeploymentManagerAdmin` role and, in
production, additionally to the admin IP whitelist; in production Cloudflare Access sits
in front of the container (ops layer, out of code scope).

## Adding an app's admin portal

1. The app team publishes `SaaSFactory.<App>.Admin` to the private GitHub Packages feed
   (or, locally, drops the dll in the shared `local-plugins` folder).
2. Make the dll available to the host:
   - **Local:** set `AdminPortal:PluginsPath` to the `local-plugins` folder
     (`appsettings.Development.json` already points at the repo-root sibling). Run the
     shell with `dotnet watch`.
   - **Production:** add `<App>.Admin:<version>` to the `ADMIN_PLUGIN_PACKAGES` GitHub
     Actions variable; the deploy pipeline downloads it into the image `plugins/` folder
     (`AdminPortal__PluginsPath=/app/plugins`).
3. Configure the app's database connection string (see below).
4. The portal appears automatically under **Admin Portals** in the sidebar.

The shell **fails to start** if a loaded plugin has any security violation (un-gated page
or controller, `[AllowAnonymous]`, a route outside `/apps/{slug}/`) or if a registered
module has no connection string — an internal admin tool must refuse to boot misconfigured.

## Environment variables

| Variable | Purpose |
|---|---|
| `AdminPortal__PluginsPath` | Folder scanned for plugin dlls (`/app/plugins` in the image) |
| `AdminPortal__Modules__{slug}__ConnectionString` | The app's own Postgres (Neon/Railway); dedicated per-app DB user with SELECT on Users/Tenants and UPDATE on Users |
| `ConnectionStrings__DefaultConnection` | DeploymentManager's own database — hosts the `dm_admin_audit` log |
| `Security__AdminIpWhitelist__Enabled` / `__AllowedIps__0…` | Production admin IP whitelist (localhost always allowed) |

## Migrations / deploy ordering

The audit table `dm_admin_audit` is owned by **DeploymentManager.ApiService**'s
migrations (via `ConfigureAdminPortalKernel()` in `DeploymentManagerDbContext`) and
created by its startup `MigrateAsync`. DeploymentManager.Web reads/writes the table
through a standalone context that never migrates. **Deploy/migrate ApiService before Web
first records an admin action** (already the implicit Railway order).

## Security model recap

- Pages: `[Authorize(Roles = DeploymentManagerAdmin)]` via each module's
  `Components/Pages/_Imports.razor` (RCL pages don't inherit the host's).
- Controllers: same role attribute; loaded plugin controllers are hosted by the shell
  and scanned by the inspector at startup.
- Regression tests: `DeploymentManager.Tests/AdminPortalSecurityTests.cs` runs the
  inspector over the kernel + sample module; the existing `ControllerAuthorizationTests`
  cover the ApiService controllers.
- Every admin read context and every write action is written to `dm_admin_audit` with a
  mandatory reason; a failed audit write aborts the operation.
