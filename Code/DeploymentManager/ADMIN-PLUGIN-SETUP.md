# DeploymentManager — Admin Portal Plugin Setup

How to package a private app's admin module as a NuGet package and have DeploymentManager.Web
consume it automatically on every Railway deploy.

---

## Architecture overview

```
┌─────────────────────────────────────┐      ┌──────────────────────────────────────────┐
│  Private repo (e.g. Trubador/       │      │  Public repo (saas-factory-labs/         │
│  Boligportal)                       │      │  Saas-Factory)                            │
│                                     │      │                                          │
│  SaaSFactory.Boligportal.Admin      │      │  AppBlueprint.AdminPortalKernel          │
│    ├─ Implements IAdminPortalModule │      │    └─ Shared plugin contract (DLL)       │
│    └─ HintPath refs kernel DLLs     │      │                                          │
│                                     │      │  DeploymentManager.Web                   │
│  GitHub Actions                     │      │    ├─ Loads plugins from /app/plugins/   │
│    publish-nuget.yml                │      │    └─ Dockerfile                         │
│    └─ Publishes .nupkg to           │      │                                          │
│       GitHub Packages (Trubador)    │      │  GitHub Actions                          │
│                                     │      │    deploy-deploymentmanager.yml          │
└─────────────────────────────────────┘      │    ├─ Downloads plugin .nupkg            │
                                             │    ├─ Extracts DLL → plugins/            │
          GitHub Packages (Trubador)         │    └─ railway up --ci                    │
          SaaSFactory.Boligportal.Admin ─────┼──►                                       │
                                             │  Railway                                 │
                                             │    └─ Docker build (Dockerfile)          │
                                             │         COPY plugins/ → /app/plugins/    │
                                             └──────────────────────────────────────────┘
```

**Deploy flow (end-to-end):**

1. Push to Boligportal repo → `publish-nuget.yml` packs and pushes `.nupkg` to GitHub Packages under Trubador.
2. Trigger `deploy-deploymentmanager.yml` in Saas-Factory repo (manual dispatch or after Boligportal publish).
3. Workflow runs `download-admin-plugins.ps1` → downloads the `.nupkg`, extracts `SaaSFactory.Boligportal.Admin.dll` into `Code/DeploymentManager/plugins/`.
4. `railway up --ci` uploads the full repo (including the populated `plugins/`) to Railway's cloud builder.
5. Railway runs the Dockerfile → `COPY Code/DeploymentManager/plugins/ /app/plugins/` bakes the DLL into the image.
6. Container starts; DeploymentManager.Web calls `AddAdminPortalPlugins()` which scans `/app/plugins/` and loads the module.

---

## Part 1 — Private app repo (Boligportal)

### 1.1 Project file (`SaaSFactory.Boligportal.Admin.csproj`)

```xml
<PropertyGroup>
  <IsPackable>true</IsPackable>
  <PackageId>SaaSFactory.Boligportal.Admin</PackageId>
  <Version>1.0.0</Version>
  <!-- Local dev: points to sibling Saas-Factory checkout -->
  <AdminPortalKernelBinDir>
    ..\..\..\..\Development-Projects\Saas-Factory\Code\AppBlueprint\Shared-Modules\
    AppBlueprint.AdminPortalKernel\bin\$(Configuration)\net10.0\
  </AdminPortalKernelBinDir>
</PropertyGroup>

<!-- Compile-time only — kernel ships with DeploymentManager.Web, not this package -->
<ItemGroup>
  <Reference Include="AppBlueprint.AdminPortalKernel">
    <HintPath>$(AdminPortalKernelBinDir)AppBlueprint.AdminPortalKernel.dll</HintPath>
    <Private>false</Private>
    <ExcludeAssets>runtime</ExcludeAssets>
  </Reference>
  <!-- repeat for other kernel DLLs that are referenced -->
</ItemGroup>
```

`Private=false` / `ExcludeAssets=runtime` means the kernel DLLs are **not** bundled into the `.nupkg` — they are already present in DeploymentManager.Web at runtime.

### 1.2 NuGet.Config (Boligportal repo root)

```xml
<packageSourceMapping>
  <packageSource key="trubador-github">
    <package pattern="AppBlueprint.*" />
    <package pattern="SaaSFactory.*" />   <!-- add this line -->
  </packageSource>
</packageSourceMapping>
```

### 1.3 GitHub Actions — `publish-nuget.yml`

Location: `.github/workflows/publish-nuget.yml` in the **Boligportal repo root** (not a subfolder).

Key points:
- Checks out Saas-Factory (public, no token needed) to build the kernel DLL on CI.
- Overrides `AdminPortalKernelBinDir` via `-p:AdminPortalKernelBinDir=...` so the HintPath resolves on CI.
- Pushes to `https://nuget.pkg.github.com/Trubador/index.json` using the automatic `GITHUB_TOKEN`.

Trigger: GitHub Release published **or** manual dispatch with a version input.

---

## Part 2 — Saas-Factory repo (DeploymentManager)

### 2.1 Plugin download script

`Code/DeploymentManager/Scripts/download-admin-plugins.ps1`

Reads `$env:ADMIN_PLUGIN_PACKAGES` (format: `PackageId:Version, ...`), authenticates to GitHub Packages using `$env:PLUGINS_FEED_TOKEN`, downloads each `.nupkg`, and extracts the `net10.0/*.dll` into the `-OutputDir` folder.

### 2.2 `.dockerignore` exception

`D:\Development\Development-Projects\Saas-Factory\.dockerignore` must allow plugin DLLs through the Docker build context:

```
!Code/DeploymentManager/plugins/*.dll
!Code/DeploymentManager/plugins/*.pdb
```

### 2.3 Dockerfile (`Code/DeploymentManager/DeploymentManager.Web/Dockerfile`)

Build context is the **repo root** (Railway default). Key additions:

```dockerfile
# Bake plugins into the image (populated by CI before railway up)
RUN mkdir -p /app/plugins
COPY Code/DeploymentManager/plugins/ /app/plugins/

# Suppress NuGet pack for packable AppBlueprint sub-projects (no README on CI)
RUN dotnet build "DeploymentManager.Web.csproj" \
    -c "$BUILD_CONFIGURATION" \
    -o /app/build \
    -p:GeneratePackageOnBuild=false

# publish stage also needs the flag
RUN dotnet publish "DeploymentManager.Web.csproj" \
    -c "$BUILD_CONFIGURATION" \
    -o /app/publish \
    /p:UseAppHost=false \
    -p:GeneratePackageOnBuild=false

# Final stage — copy plugins alongside the published app
COPY --from=build --chown=$APP_UID:$APP_UID /app/plugins ./plugins/
ENV ADMIN_PORTAL_PLUGINS_PATH=/app/plugins
```

### 2.4 `railway.toml` (repo root)

```toml
[build]
builder = "DOCKERFILE"
dockerfilePath = "Code/DeploymentManager/DeploymentManager.Web/Dockerfile"

[deploy]
startCommand = "dotnet DeploymentManager.Web.dll"
restartPolicyType = "ON_FAILURE"
restartPolicyMaxRetries = 3
```

No `healthcheckPath` — Railway's healthcheck will fail during cold starts with plugin loading.

### 2.5 GitHub Actions — `deploy-deploymentmanager.yml`

`.github/workflows/deploy-deploymentmanager.yml`

```yaml
- name: Download admin plugins
  shell: pwsh
  env:
    ADMIN_PLUGIN_PACKAGES: ${{ inputs.plugin_packages || vars.ADMIN_PLUGIN_PACKAGES }}
    ADMIN_PLUGINS_FEED_OWNER: Trubador
    PLUGINS_FEED_TOKEN: ${{ secrets.PLUGINS_FEED_TOKEN }}
  run: |
    pwsh Code/DeploymentManager/Scripts/download-admin-plugins.ps1 `
      -OutputDir Code/DeploymentManager/plugins

- name: Install Railway CLI
  run: |
    npm install -g @railway/cli@5.15.0
    railway --version

- name: Deploy to Railway
  env:
    RAILWAY_TOKEN: ${{ secrets.RAILWAY_TOKEN_DEPLOYMENTMANAGER }}
  run: |
    railway up \
      --service "${{ vars.RAILWAY_DM_SERVICE }}" \
      --ci \
      ${{ inputs.debug == 'true' && '--verbose' || '' }}
```

**Required GitHub Actions secrets** (set in Saas-Factory repo → Settings → Secrets):

| Secret | Description |
|--------|-------------|
| `RAILWAY_TOKEN_DEPLOYMENTMANAGER` | Railway project-scoped token (Railway → Project → Settings → Tokens) |
| `PLUGINS_FEED_TOKEN` | GitHub PAT with `read:packages` scope on the Trubador org |

**Required GitHub Actions variables** (Settings → Variables):

| Variable | Example value |
|----------|---------------|
| `RAILWAY_DM_SERVICE` | `Deployment Manager Web` |
| `ADMIN_PLUGIN_PACKAGES` | `SaaSFactory.Boligportal.Admin:1.0.0` |

---

## Part 3 — Railway environment variables

Set under **DeploymentManager.Web service → Variables** in the Railway dashboard.

ASP.NET Core maps `Section:Key` config to `Section__Key` env vars (double underscore).

### Required

| Variable | Description |
|----------|-------------|
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string for the DeploymentManager database (hosts `dm_admin_audit` log) |

### Per admin module

| Variable | Description |
|----------|-------------|
| `AdminPortal__Modules__boligportal__ConnectionString` | PostgreSQL connection string for the Boligportal app database (read-only access) |

### Security

| Variable | Default | Description |
|----------|---------|-------------|
| `AdminPortal__Security__RequireMfaClaim` | `true` | Set to `false` to disable the MFA claim requirement (useful during initial setup) |

### Connection string format (Npgsql)

```
Host=your-host.railway.internal;Port=5432;Database=railway;Username=postgres;Password=xxx;SSL Mode=Require;Trust Server Certificate=true
```

> Railway injects `DATABASE_URL` (postgres:// URL format) for attached Postgres services, but Npgsql requires the explicit key=value format above.

---

## Part 4 — Adding a new admin plugin

1. Create `SaaSFactory.<AppName>.Admin` project in the private app repo implementing `IAdminPortalModule`.
2. Add `publish-nuget.yml` to that repo (copy from Boligportal, adjust `PROJECT_PATH`).
3. Publish a release to push the `.nupkg` to GitHub Packages under Trubador.
4. Add `SaaSFactory.<AppName>.Admin:1.0.0` to the `ADMIN_PLUGIN_PACKAGES` variable in the Saas-Factory repo.
5. Add `AdminPortal__Modules__<appslug>__ConnectionString` to Railway.
6. Re-run `deploy-deploymentmanager.yml`.
