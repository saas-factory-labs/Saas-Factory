# SonarCloud Code Smell Report

**Project:** saas-factory-labs/Saas-Factory  
**Date:** 2026-06-23  
**Status filter:** OPEN  
**Type filter:** CODE_SMELL  
**Total issues:** 83  
**Total estimated effort:** 377 minutes

---

## Summary by Severity

| Severity | Count |
|----------|-------|
| Critical | 2 |
| Major | 58 |
| Minor | 15 |
| Info | 8 |

## Summary by Rule

| Rule | Description | Count |
|------|-------------|-------|
| `csharpsquid:S125` | Remove commented-out code | 30 |
| `docker:S8431` | Use version tag OR digest, not both | 14 |
| `powershelldre:S8620` | Remove trailing whitespace | 6 |
| `csharpsquid:S3236` | Remove argument hiding caller info | 4 |
| `external_roslyn:ASP0015` | Use typed header property instead of string | 5 |
| `plsql:OrderByExplicitAscCheck` | Add explicit ASC to ORDER BY | 3 |
| `csharpsquid:S107` | Method exceeds 7 parameters | 2 |
| `csharpsquid:S3776` | Cognitive complexity too high | 2 |
| `csharpsquid:S1133` | Remove deprecated code | 2 |
| `csharpsquid:S1192` | Define constant for repeated string | 2 |
| `csharpsquid:S1135` | Complete TODO comment | 2 |
| `external_roslyn:CA1054` | Use `System.Uri` instead of `string` for URI params | 1 |
| `external_roslyn:CA1024` | Use property instead of method | 1 |
| `external_roslyn:CA1716` | Rename parameter conflicting with reserved keyword | 1 |
| `external_roslyn:CS8602` | Dereference of possibly null reference | 1 |
| `csharpsquid:S2139` | Log or rethrow caught exception | 1 |
| `csharpsquid:S2342` | Rename enum to match naming convention | 1 |
| `csharpsquid:S3241` | Change return type to void | 1 |
| `csharpsquid:S3267` | Simplify loop with LINQ `.Where()` | 1 |
| `css:S1874` | Deprecated CSS property value | 1 |
| `css:S4666` | Duplicate CSS selector | 1 |

---

## Critical Issues (2)

| # | File | Line | Rule | Message | Effort |
|---|------|------|------|---------|--------|
| 1 | `AppBlueprint.Web/WebhookUrlValidator.cs` | 79 | `csharpsquid:S3776` | Refactor this method to reduce Cognitive Complexity from 25 to 15 | 6 min |
| 2 | `AppBlueprint.ServiceDefaults/Extensions.cs` | 76 | `csharpsquid:S3776` | Refactor this method to reduce Cognitive Complexity from 20 to 15 | 6 min |

---

## Major Issues (58)

### Docker — Image tag + digest conflict (14 issues)

> Rule `docker:S8431`: Use either the version tag or the digest for the image, not both.

| File | Lines |
|------|-------|
| `AppBlueprint.ApiService/Dockerfile` | 3, 12 |
| `AppBlueprint.ApiService/Dockerfile.railway` | 3, 11 |
| `AppBlueprint.AppGateway/Dockerfile` | 2, 10 |
| `AppBlueprint.AppHost/Dockerfile` | 4, 11 |
| `AppBlueprint.Tests/Dockerfile` | 2, 7 |
| `AppBlueprint.Web/Dockerfile` | 3, 15 |
| `AppBlueprint.Web/Dockerfile.railway` | 3, 11 |

**Effort:** 5 min each — 70 min total

---

### Commented-out code — C# (28 issues)

> Rule `csharpsquid:S125`: Remove commented-out code blocks.

| File | Line(s) |
|------|---------|
| `CertificateExtensions.cs` | 1 |
| `AppBlueprint.ServiceDefaults/Extensions.cs` | 202 |
| `RegexValidation.cs` | 1 |
| `UpdateAddressRequest.cs` | 21, 24, 27 |
| `AddressResponse.cs` | 17, 33, 36, 39 |
| `CreateAuditLogRequest.cs` | 33 |
| `UpdateAuditLogRequest.cs` | 35, 38 |
| `AuditLogResponse.cs` | 22, 33, 36 |
| `Entity.cs` | 1 |
| `AppBlueprint.SharedKernel/Program.cs` | 16 |
| `Role.cs` | 1 |
| `ContactPersonValidation.cs` | 1 |
| `CustomerAccountValidation.cs` | 1 |
| `CustomerValidation.cs` | 1 |
| `EmailValidation.cs` | 1 |
| `FileValidation.cs` | 1 |
| `IntegrationValidation.cs` | 1 |
| `OrganizationValidation.cs` | 1 |
| `PhoneNumberValidation.cs` | 1 |
| `ProfileValidation.cs` | 1 |
| `TeamValidation.cs` | 1 |
| `UserValidation.cs` | 1 |
| `WebhookValidation.cs` | 1 |

**Effort:** 5 min each — 140 min total

---

### PowerShell — Trailing whitespace (6 issues)

> Rule `powershelldre:S8620`: Remove trailing whitespace from line.

| File | Lines |
|------|-------|
| `create-rls-migration.ps1` | 27, 30 |
| `configure-aspire-telemetry.ps1` | 104, 119, 125, 133 |

**Effort:** 1 min each — 6 min total

---

### Exception handling (1 issue)

| File | Line | Rule | Message | Effort |
|------|------|------|---------|--------|
| `SignupService.cs` | 159 | `csharpsquid:S2139` | Either log this exception and handle it, or rethrow it with contextual information | 15 min |

---

### Method parameter count (2 issues)

> Rule `csharpsquid:S107`: Reduce method parameters to 7 or fewer.

| File | Line | Message | Effort |
|------|------|---------|--------|
| `IMultiChannelNotificationService.cs` | 13 | Method has 9 parameters, which exceeds 7 authorized | 20 min |
| `IMultiChannelNotificationService.cs` | 27 | Method has 8 parameters, which exceeds 7 authorized | 20 min |

---

### API / Type design (5 issues)

| File | Line | Rule | Message | Effort |
|------|------|------|---------|--------|
| `WebhookUrlValidator.cs` | 18 | `external_roslyn:CA1054` | Change parameter type from `string` to `System.Uri` | 0 min |
| `PIITypeRegistry.cs` | 47 | `external_roslyn:CA1024` | Use a property instead of `GetXxx()` method | 0 min |
| `IEmailTemplateService.cs` | 29 | `external_roslyn:CA1716` | Rename parameter `To` to avoid reserved keyword conflict | 0 min |
| `UserService.cs` | 256 | `external_roslyn:CS8602` | Dereference of possibly null reference | 0 min |
| `GDPRType.cs` | 4 | `csharpsquid:S2342` | Rename enumeration to match naming convention | 5 min |

---

### CSS (2 issues)

| File | Line | Rule | Message | Effort |
|------|------|------|---------|--------|
| `app.css` | 298 | `css:S1874` | Expected "button" to be "auto" (deprecated value) | 15 min |
| `app.css` | 6448 | `css:S4666` | Duplicate selector `.flatpickr-day.startRange.endRange` | 1 min |

---

## Minor Issues (15)

### Caller info arguments (4 issues)

> Rule `csharpsquid:S3236`: Remove redundant `[CallerMemberName]` / `[CallerFilePath]` argument that hides actual caller information.

| File | Line | Effort |
|------|------|--------|
| `AppBlueprint.ServiceDefaults/Extensions.cs` | 24 | 2 min |
| `AppBlueprint.ServiceDefaults/Extensions.cs` | 78 | 2 min |
| `AppBlueprint.ServiceDefaults/Extensions.cs` | 166 | 2 min |
| `AppBlueprint.ServiceDefaults/Extensions.cs` | 173 | 2 min |

---

### SQL — Implicit sort order (3 issues)

> Rule `plsql:OrderByExplicitAscCheck`: Add `ASC` to `ORDER BY` to make sort direction explicit.

| File | Lines | Effort |
|------|-------|--------|
| `SetupRowLevelSecurity-Simple.sql` | 123, 131, 131 | 5 min each |

---

### Duplicate string literals (2 issues)

> Rule `csharpsquid:S1192`: Extract repeated string literals into a named constant.

| File | Line | Message | Effort |
|------|------|---------|--------|
| `UserService.cs` | 69 | `"User not found"` used 4 times | 4 min |
| `DataExportService.cs` | 17 | `"Implementation moved to Infrastructure layer"` used 5 times | 4 min |

---

### Design / usability (3 issues)

| File | Line | Rule | Message | Effort |
|------|------|------|---------|--------|
| `ServiceCollectionExtensions.cs` | 66 | `csharpsquid:S3241` | Change return type to void; no callers use returned value | 2 min |
| `CloudflareIpFilterMiddleware.cs` | 174 | `csharpsquid:S3267` | Simplify loop using LINQ `.Where()` | 5 min |
| `GDPRType.cs` | 4 | `csharpsquid:S2342` | Rename enum to match naming convention | 5 min |

---

### Deprecated code (2 issues)

> Rule `csharpsquid:S1133`: Remove `[Obsolete]`-marked code or update callers.

| File | Line | Effort |
|------|------|--------|
| `CloudflareR2Options.cs` | 65 | 10 min |
| `TodoEntity.cs` | 8 | 10 min |

---

## Info Issues (8)

### ASP.NET typed header properties (5 issues)

> Rule `external_roslyn:ASP0015`: Use the typed property accessor instead of string literal header name.

| File | Line | Header |
|------|------|--------|
| `AppBlueprint.AppGateway/Program.cs` | 126 | `X-Content-Type-Options` |
| `AppBlueprint.AppGateway/Program.cs` | 129 | `X-Frame-Options` |
| `AppBlueprint.AppGateway/Program.cs` | 132 | `X-XSS-Protection` |
| `AppBlueprint.AppGateway/Program.cs` | 151 | `Content-Security-Policy` |
| `AppBlueprint.AppGateway/Program.cs` | 163 | `Strict-Transport-Security` |

---

### TODO comments (2 issues)

> Rule `csharpsquid:S1135`: Complete or remove TODO comments.

| File | Lines |
|------|-------|
| `ServiceCollectionExtensions.cs` | 31, 32 |

---

### Deprecated code (1 issue)

| File | Line | Rule |
|------|------|------|
| `TodoEntity.cs` | 8 | `csharpsquid:S1133` |

---

## Recommended Fix Order

| Priority | Category | Issues | Est. Effort |
|----------|----------|--------|-------------|
| 1 | Critical cognitive complexity (`S3776`) | 2 | 12 min |
| 2 | Exception swallowing (`S2139`) | 1 | 15 min |
| 3 | Dockerfile tag+digest conflict (`S8431`) | 14 | 70 min |
| 4 | PowerShell trailing whitespace (`S8620`) | 6 | 6 min |
| 5 | Commented-out code (`S125`) | 30 | 150 min |
| 6 | Method parameter count (`S107`) | 2 | 40 min |
| 7 | API design (`CA1054`, `CA1024`, `CA1716`, `CS8602`) | 4 | ~5 min |
| 8 | Duplicate string literals (`S1192`) | 2 | 8 min |
| 9 | SQL sort direction (`OrderByExplicitAscCheck`) | 3 | 15 min |
| 10 | Caller info args (`S3236`) | 4 | 8 min |
| 11 | CSS issues | 2 | 16 min |
| 12 | Info / style issues | 8 | 0–10 min |
