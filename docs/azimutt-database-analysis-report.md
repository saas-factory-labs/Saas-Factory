# 🔍 Azimutt Database Analysis Report

> **Database:** `appblueprintdb`
> **Report Date:** 2026-02-23
> **Azimutt CLI Version:** 0.1.38
> **Tool:** [Azimutt](https://azimutt.app/) — Next-Gen ERD & Database Linter
> **Workflow:** [View latest run](https://github.com/saas-factory-labs/Saas-Factory/actions/workflows/azimutt-database-analysis.yml)

---

## Summary

| Severity | Count |
|----------|------:|
| 🔴 High   | 53 |
| 🟠 Medium | 30 |
| 🔵 Low    | 1 |
| 💡 Hint   | 21 |
| **Total** | **105** |

> Scanned **49 entities**, **55 relations**, **0 queries**, **0 types**.

---

## 🔴 High Severity (53 violations)

### Duplicated Index — 20 violations

- Index IX_ApiLogs_ApiKeyId on public.ApiLogs(ApiKeyId) can be deleted, it's covered by: IX_ApiLogs_ApiKeyId_StatusCode(ApiKeyId, StatusCode).
- Index IX_ApiLogs_SessionId on public.ApiLogs(SessionId) can be deleted, it's covered by: IX_ApiLogs_SessionId_StatusCode(SessionId, StatusCode).
- Index IX_AuditLogs_Category on public.AuditLogs(Category) can be deleted, it's covered by: IX_AuditLogs_Category_ModifiedAt(Category, ModifiedAt).
- _+ 17 more_

### Misaligned Relation — 33 violations

- Relation FK_Accounts_Users_OwnerId link attributes different types: public.Accounts(OwnerId): character varying(1024) != public.Users(Id): character varying(40)
- Relation FK_Addresses_Cities_CityId link attributes different types: public.Addresses(CityId): character varying(1024) != public.Cities(Id): character varying(40)
- Relation FK_Addresses_Countries_CountryId link attributes different types: public.Addresses(CountryId): character varying(1024) != public.Countries(Id): character varying(40)
- _+ 30 more_

---

## 🟠 Medium Severity (30 violations)

### Entity With Too Heavy Indexes — 4 violations

- Entity public.Tenants has too heavy indexes (11x data size, 12 indexes).
- Entity public.FileMetadata has too heavy indexes (6x data size, 6 indexes).
- Entity public.WebhookEvents has too heavy indexes (6x data size, 6 indexes).
- _+ 1 more_

### Missing Relation — 26 violations

- Create a relation from public.Accounts(UserId) to public.Users(Id).
- Create a relation from public.Accounts(TenantId) to public.Tenants(Id).
- Create a relation from public.AdminAuditLog(AdminUserId) to public.Users(Id).
- _+ 23 more_

---

## 🔵 Low Severity (1 violation)

### Inconsistent Entity Name — 1 violation

- Entity public.__EFMigrationsHistory doesn't follow naming convention camel-upper.

---

## 💡 Hint Severity (21 violations)

### Inconsistent Attribute Type — 21 violations

- Attribute Id has several types: character varying(50) in public.FileMetadata(Id), text in public.SignupAuditLog(Id), integer in public.AdminAuditLog(Id) and 2 others, character varying(1024) in public.Accounts(Id) and 19 others, character varying(40) in public.AuditLogs(Id) and 22 others.
- Attribute Name has several types: character varying(1024) in public.Accounts(Name) and 2 others, character varying(200) in public.Countries(Name) and 4 others, character varying(100) in public.Cities(Name) and 7 others.
- Attribute Email has several types: text in public.SignupAuditLog(Email), character varying(255) in public.Accounts(Email) and 1 other, character varying(100) in public.Tenants(Email) and 1 other.
- _+ 18 more_

---

## ℹ️ Notes

- `pg_stat_statements` is **not enabled**. Enabling it on the PostgreSQL instance will allow Azimutt to analyse slow & degrading queries.
- To receive the full report as JSON, pass `--email your@email.com` to the CLI.

---

_Generated automatically by [Azimutt CLI](https://www.npmjs.com/package/azimutt) · [Re-run analysis](https://github.com/saas-factory-labs/Saas-Factory/actions/workflows/azimutt-database-analysis.yml)_
