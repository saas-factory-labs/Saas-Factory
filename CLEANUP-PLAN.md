# Repo Cleanup Plan

Audit of obsolete, duplicate, and AI-session-artifact files across the repo.
Review each section and delete what you're comfortable with.

---

## HIGH — Delete with confidence

### Firebase docs (not in the tech stack — Logto is used)
- [ ] `Code/AppBlueprint/FIREBASE-ENV-SETUP.md`
- [ ] `Code/AppBlueprint/FIREBASE-ENV-VARS-NEEDED.md`
- [ ] `Code/AppBlueprint/FIREBASE-SETUP-GUIDE.md`

### Dating app docs (out of scope for a generic SaaS factory)
- [ ] `Code/AppBlueprint/DATING-APP-INTEGRATION-GUIDE.md`
- [ ] `Code/AppBlueprint/DATING-APP-QUICK-START.md`

### Duplicate issue templates (`.md` format superseded by `.yml` in `ISSUE_TEMPLATES/`)
- [ ] `.github/ISSUE_TEMPLATE/bug_report.md`
- [ ] `.github/ISSUE_TEMPLATE/feature_request.md`
- [ ] `.github/ISSUE_TEMPLATE/` (directory itself, once files removed)

### Broken docs sync script (hardcoded absolute path to one developer's machine)
- [ ] `docs/sync-documentation.ps1`

---

## MEDIUM — AI session artifacts / one-off scripts

### Dated fix/analysis docs (info lives in git history, these are stale session summaries)
- [X] `Code/AppBlueprint/CODE-QUALITY-IMPROVEMENTS-2026-01-02.md`
- [ ] `Code/AppBlueprint/FILE-DELETE-FIX-2026-02-02.md`
- [ ] `Code/AppBlueprint/SIGNUP-FLOW-AUTHENTICATION-FIX.md`
- [ ] `Code/AppBlueprint/OPTION-4-IMPLEMENTATION-GUIDE.md` (planning doc, decision was made)
- [ ] `Code/AppBlueprint/TODO-ANALYSIS.md`

### One-off RLS setup scripts (ran once during initial DB setup, no ongoing purpose)
- [ ] `Code/AppBlueprint/create-rls-migration.ps1`
- [ ] `Code/AppBlueprint/run-rls-setup.ps1`

### Diagnostic test scripts (explicitly temporary, named accordingly)
- [ ] `Code/AppBlueprint/AppBlueprint.AppHost/test-doppler-injection.ps1`
- [ ] `Code/AppBlueprint/AppBlueprint.ApiService/test-tenant-middleware.ps1`

### Incomplete placeholder script
- [ ] `docs/search-server/create-search-api-key.ps1` (`$RAILWAY_URL = ""` — never filled in)

### Stale GitHub Actions SHA reference (SHAs are outdated, workflows already encode the correct ones)
- [ ] `.github/GITHUB_ACTIONS_SHA_REFERENCE.md`

---

## MEDIUM — Duplicate/overlapping docs (consolidate or delete extras)

### UiKit — 3 usage docs for one library (pick one, delete the rest)
- [ ] `Code/AppBlueprint/Shared-Modules/AppBlueprint.UiKit/USAGE.md` (235 lines — shortest, likely earliest draft)
- [ ] `Code/AppBlueprint/Shared-Modules/AppBlueprint.UiKit/USAGE-GUIDE.md` (519 lines)
- [ ] `Code/AppBlueprint/Shared-Modules/AppBlueprint.UiKit/THEME-USAGE-GUIDE.md` (1086 lines — most complete, keep this one?)

### Notification system — 4 docs on same topic (heavy overlap across sessions)
- [ ] `Code/AppBlueprint/NOTIFICATION-SYSTEM-SUMMARY.md`
- [ ] `Code/AppBlueprint/NOTIFICATION-BELL-INTEGRATION.md`
- [ ] `Code/AppBlueprint/NOTIFICATIONS-QUICK-REFERENCE.md`
- [ ] `Code/AppBlueprint/MULTI-CHANNEL-NOTIFICATIONS-COMPARISON.md`

### Docker security — summary is redundant alongside full guide
- [ ] `Code/AppBlueprint/DOCKER-HARDENING-SUMMARY.md` (keep `DOCKER-SECURITY-HARDENING.md`)

### SignalR — two docs written across sessions covering same feature
- [ ] `Code/AppBlueprint/SIGNALR-IMPLEMENTATION.md`
- [ ] `Code/AppBlueprint/SIGNALR-AUTHORIZATION-GUIDE.md` (consolidate into one or delete both)

### Logto/auth — two overlapping guides
- [ ] `Code/AppBlueprint/LOGTO-AUTHENTICATION-SETUP.md`
- [ ] `Code/AppBlueprint/Shared-Modules/AUTHENTICATION_GUIDE.md` (consolidate into one)

---

## LOW — Minor clutter

- [ ] `build-artifacts/` at repo root — contains only a `README.md`, no actual artifacts; misleading directory name
- [ ] `Code/AppBlueprint/setup-dev-certs.sh` — bash variant of the PowerShell cert script, 14 lines vs 31, likely never tested on this Windows-primary project
- [ ] `Code/AppBlueprint/configure-aspire-telemetry.ps1` — verify if Aspire telemetry is now configured via `appsettings.json`/AppHost; if so this is dead
