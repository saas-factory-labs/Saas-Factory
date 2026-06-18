# Repo Cleanup Plan

Audit of obsolete, duplicate, and AI-session-artifact files across the repo.
Review each section and delete what you're comfortable with.

---

## HIGH — Delete with confidence

### Broken docs sync script (hardcoded absolute path to one developer's machine)
- [ ] `docs/sync-documentation.ps1`

---

## MEDIUM — AI session artifacts / one-off scripts

### Dated fix/analysis docs (info lives in git history, these are stale session summaries)
- [ ] `Code/AppBlueprint/SIGNUP-FLOW-AUTHENTICATION-FIX.md`
- [ ] `Code/AppBlueprint/TODO-ANALYSIS.md`

### One-off RLS setup scripts (ran once during initial DB setup, no ongoing purpose)
- [ ] `Code/AppBlueprint/create-rls-migration.ps1`
- [ ] `Code/AppBlueprint/run-rls-setup.ps1`

### Diagnostic test scripts (explicitly temporary, named accordingly)
- [ ] `Code/AppBlueprint/AppBlueprint.AppHost/test-doppler-injection.ps1`
- [ ] `Code/AppBlueprint/AppBlueprint.ApiService/test-tenant-middleware.ps1`

### Incomplete placeholder script
- [ ] `docs/search-server/create-search-api-key.ps1` (`$RAILWAY_URL = ""` — never filled in)

---

## LOW — Minor clutter

- [ ] `build-artifacts/` at repo root — contains only a `README.md`, no actual artifacts; misleading directory name
- [ ] `Code/AppBlueprint/setup-dev-certs.sh` — bash variant of the PowerShell cert script, 14 lines vs 31, likely never tested on this Windows-primary project
- [ ] `Code/AppBlueprint/configure-aspire-telemetry.ps1` — verify if Aspire telemetry is now configured via `appsettings.json`/AppHost; if so this is dead
