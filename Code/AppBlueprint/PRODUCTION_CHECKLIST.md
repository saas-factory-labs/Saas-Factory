# Production Checklist

Pre-launch checklist for deploying an AppBlueprint-based SaaS application.
Work through each section in order — later sections depend on earlier ones being done.

---

## 1. Secrets & configuration

- [ ] No secrets committed to source control — verify with `git log -p | grep -i secret`
- [ ] All secrets stored in Railway environment variables or Azure Key Vault, not in `appsettings.json`
- [ ] `appsettings.Development.json` is in `.gitignore` and absent from the NuGet package
- [ ] Connection strings use environment variables (`DATABASE_CONNECTION_STRING`), not hardcoded values
- [ ] Stripe, Resend, Cloudflare R2 keys are environment-scoped (staging vs production keys differ)
- [ ] Azure Key Vault or AWS Secrets Manager wired up for production secrets rotation

> Reference: [Shared-Modules/SECURITY_BEST_PRACTICES.md](Shared-Modules/SECURITY_BEST_PRACTICES.md)
> Reference: [Shared-Modules/CONFIGURATION_EXTERNALIZATION_STRATEGY.md](Shared-Modules/CONFIGURATION_EXTERNALIZATION_STRATEGY.md)

---

## 2. Authentication

- [ ] Logto (or chosen provider) application created and configured for the production domain
- [ ] Redirect URIs in Logto set to `https://yourdomain.com/callback` and `https://yourdomain.com/signout-callback-logto` — **not** hardcoded staging URIs
- [ ] Post-logout redirect URI set to `https://yourdomain.com/`
- [ ] `Logto:Endpoint`, `Logto:AppId`, `Logto:AppSecret`, `Logto:Resource` set as environment variables
- [ ] Cookie secure policy is `Always` in production (not `None`)
- [ ] Data Protection keys persisted to filesystem or Azure Blob Storage (not ephemeral in-memory)
- [ ] Authentication provider set via `Authentication:Provider` config key, not hardcoded
- [ ] Firebase, Auth0, or other secondary providers configured if used (see factory)

> Reference: [LOGTO-AUTHENTICATION-SETUP.md](LOGTO-AUTHENTICATION-SETUP.md)
> Reference: [Shared-Modules/AUTHENTICATION_GUIDE.md](Shared-Modules/AUTHENTICATION_GUIDE.md)
> Reference: [Shared-Modules/AppBlueprint.Infrastructure/Authorization/README.md](Shared-Modules/AppBlueprint.Infrastructure/Authorization/README.md)

---

## 3. Database & migrations

- [ ] Railway (or target host) PostgreSQL database provisioned for the production environment
- [ ] `DATABASE_CONNECTION_STRING` environment variable set in production Railway service
- [ ] AppBlueprint base migrations applied: `dotnet ef database update --context BaselineDbContext`
- [ ] App-specific migrations applied separately (they use `__EFMigrationsHistory`, AppBlueprint uses its own tables)
- [ ] Row-Level Security SQL script run: `psql ... -f SetupRowLevelSecurity.sql`
- [ ] RLS health check endpoint (`/health`) returns healthy for the `rls` tag
- [ ] Database backup strategy in place before every migration run
- [ ] Rollback procedure tested in staging before production migration

> Reference: [Shared-Modules/MIGRATION_STRATEGY.md](Shared-Modules/MIGRATION_STRATEGY.md) — migration isolation options (Option D active, Options A/B/C for future)
> Reference: [Shared-Modules/MIGRATION_ROLLBACK_GUIDE.md](Shared-Modules/MIGRATION_ROLLBACK_GUIDE.md)
> Reference: [DATABASE_HYBRID_MODE_SETUP.md](DATABASE_HYBRID_MODE_SETUP.md)
> Reference: [Shared-Modules/MULTI_TENANCY_GUIDE.md](Shared-Modules/MULTI_TENANCY_GUIDE.md)

---

## 4. Multi-tenancy & data isolation

- [ ] `MultiTenancy:Strategy` is `SharedDatabase` (only supported strategy)
- [ ] `MultiTenancy:EnableRowLevelSecurity` is `true`
- [ ] `MultiTenancy:EnableQueryFilters` is `true`
- [ ] `MultiTenancy:ValidateTenantExists` is `true`
- [ ] Tenant ID is always sourced from JWT claims (`TenantResolutionStrategy.JwtClaim`) for authenticated requests
- [ ] Admin bypass (`AdminTenantAccessService`) is restricted to admin roles only
- [ ] RLS policies cover all tenant-scoped tables — verify with `SetupRowLevelSecurity.sql`

> Reference: [Shared-Modules/MULTI_TENANCY_GUIDE.md](Shared-Modules/MULTI_TENANCY_GUIDE.md)
> Reference: [Shared-Modules/SECURITY_BEST_PRACTICES.md](Shared-Modules/SECURITY_BEST_PRACTICES.md)

---

## 5. NuGet package publishing

- [ ] `NUGET_API_KEY` GitHub secret set (scoped to `SaaS-Factory.AppBlueprint.*`)
- [ ] `GITHUB_TOKEN` has `contents: write` permission for the release workflow
- [ ] Semantic-release is configured (`.releaserc.json` present at repo root)
- [ ] At least one conventional commit (`feat:` or `fix:`) merged to `main` to trigger first release
- [ ] `auto-release.yml` dispatches `publish-nuget-packages.yml` on successful release (wired via `gh workflow run`)
- [ ] All 8 packages build and pack cleanly: `dotnet pack --configuration Release`
- [ ] Package version resolves from GitVersion, not hardcoded `1.0.0`
- [ ] Published packages verified on NuGet.org before consumer apps reference them

> Reference: [Shared-Modules/NUGET_PUBLISHING_GUIDE.md](Shared-Modules/NUGET_PUBLISHING_GUIDE.md)

---

## 6. CI/CD pipeline

- [ ] `deploy-to-railway.yml` targets .NET 10 (`DOTNET_VERSION: '10.0'`)
- [ ] `dotnet-ef` installed at version `10.*` in migration job
- [ ] All tests pass on the `main` branch before deploy
- [ ] Staging deploy and smoke test runs before production deploy
- [ ] `RAILWAY_TOKEN_STAGING` and `RAILWAY_TOKEN_PRODUCTION` secrets set
- [ ] `RAILWAY_DATABASE_URL_STAGING` and `RAILWAY_DATABASE_URL_PRODUCTION` secrets set
- [ ] GitHub environment protection rules configured for `production` (require manual approval)
- [ ] SBOM generation (`generate-sbom.yml`) runs on release

---

## 7. Security

- [ ] XSS prevention: no `eval()`, no raw HTML injection from user input in Razor components
- [ ] HTTPS enforced — `RequireHttpsMetadata` is `true` in production
- [ ] HSTS headers configured
- [ ] Admin IP allowlist configured for admin-only endpoints
- [ ] API key authentication filter applied to internal API endpoints
- [ ] SonarCloud analysis (`sonarcloud-analysis.yaml`) passing with no critical issues
- [ ] Docker Scout vulnerability scan (`docker-scout-vulnerability-scan.yml`) passing
- [ ] GitGuardian (`git-guardian.yml`) enabled and passing
- [ ] OpenSSF Scorecard (`scorecard.yml`) reviewed
- [ ] Known vulnerable transitive packages pinned in `Directory.Packages.props` (`Snappier`, `SharpCompress`, `Kiota.Abstractions`)

> Reference: [Shared-Modules/SECURITY_BEST_PRACTICES.md](Shared-Modules/SECURITY_BEST_PRACTICES.md)
> Reference: [AppBlueprint.Web/XSS-PREVENTION-GUIDE.md](AppBlueprint.Web/XSS-PREVENTION-GUIDE.md)
> Reference: [admin-ip-whitelist-setup.md](admin-ip-whitelist-setup.md)

---

## 8. External services

- [ ] **Stripe**: live-mode API key set; webhook endpoint registered at `https://yourdomain.com/api/stripe/webhook`; webhook secret set as environment variable
- [ ] **Resend**: production API key set; sending domain verified in Resend dashboard
- [ ] **Cloudflare R2**: production bucket created; `R2_ACCESS_KEY_ID`, `R2_SECRET_ACCESS_KEY`, `R2_BUCKET_NAME`, `R2_ENDPOINT` set
- [ ] **Logto**: production application created (separate from staging/dev application)
- [ ] **Firebase**: production Firebase project configured if using Firebase auth
- [ ] All external service webhooks point to the production domain, not staging

> Reference: [CLOUDFLARE-R2-IMPLEMENTATION.md](CLOUDFLARE-R2-IMPLEMENTATION.md)
> Reference: [EMAIL-TEMPLATES-README.md](EMAIL-TEMPLATES-README.md)

---

## 9. Observability

- [ ] OpenTelemetry exporter configured for production (Aspire Dashboard, Grafana, or equivalent)
- [ ] Serilog sinks configured for production log aggregation (not just Console)
- [ ] Health check endpoint (`/health`) publicly accessible and monitored
- [ ] Uptime monitoring configured for the production URL
- [ ] Stripe webhook delivery failures alerted (Stripe dashboard → Webhooks → configure alerts)
- [ ] Railway service restart alerts configured

> Reference: [AppBlueprint.SharedKernel/Telemetry/SETUP.md](AppBlueprint.SharedKernel/Telemetry/SETUP.md)
> Reference: [AppBlueprint.SharedKernel/Telemetry/README.md](AppBlueprint.SharedKernel/Telemetry/README.md)

---

## 10. Pre-launch smoke test

- [ ] Sign up flow completes end-to-end (Logto redirect → callback → tenant created → dashboard)
- [ ] Sign out clears session and redirects to login
- [ ] Authenticated API call returns data scoped to the correct tenant (not another tenant's data)
- [ ] RLS: direct `psql` query without `SET app.current_tenant_id` returns zero rows for tenant-scoped tables
- [ ] File upload to R2 succeeds and URL is accessible
- [ ] Transactional email (signup confirmation) delivered via Resend
- [ ] Stripe checkout session creates and redirects correctly
- [ ] `/health` endpoint returns `Healthy` for all checks including `rls` and `db`

---

## Production recommendation: migration strategy

The current active strategy is **Option D** (separate `__AppBlueprint*MigrationsHistory` tables per DbContext).
This is suitable for initial launches.

For mature production deployments consider evolving to:
- **Option C** (separate migrations NuGet package) — when base schema stabilises
- **Option A** (migration bundles) — for zero-downtime, tooling-free deploys

Full rationale and upgrade path: [Shared-Modules/MIGRATION_STRATEGY.md](Shared-Modules/MIGRATION_STRATEGY.md)
