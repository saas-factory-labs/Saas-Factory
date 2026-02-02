# AppBlueprint Framework - Missing Production Services Roadmap

This document tracks the remaining infrastructure services needed to make AppBlueprint a complete, production-ready B2C/B2B SaaS framework.

## Status Overview

| Service | Priority | Status | Complexity | Estimated Effort |
|---------|----------|--------|------------|------------------|
| ✅ Real-Time Communication (SignalR) | P0 - Critical | ✅ **COMPLETE** | Medium | 4-6 hours |
| Background Jobs | P0 - Critical | 📋 Planned | Medium | 6-8 hours |
| Full-Text Search | P1 - High | 📋 Planned | Medium | 4-6 hours |
| File Storage Service | P1 - High | 📋 Planned | Medium | 6-8 hours |
| ✅ Email Template System | P1 - High | ✅ **COMPLETE** | Medium | 4-6 hours |
| Payment Webhook Infrastructure | P2 - Medium | 📋 Planned | High | 8-10 hours |
| Multi-Channel Notifications | P2 - Medium | 📋 Planned | High | 8-10 hours |
| Rate Limiting | P2 - Medium | 📋 Planned | Low | 2-4 hours |
| Caching Abstraction | P2 - Medium | 📋 Planned | Medium | 4-6 hours |
| GDPR Automation Tools | P3 - Low | 📋 Planned | High | 10-12 hours |
| Advanced Tenant Analytics | P3 - Low | 📋 Planned | Medium | 6-8 hours |
| Audit Logging Infrastructure | P3 - Low | 📋 Planned | Medium | 4-6 hours |
| Feature Flag System | P3 - Low | 📋 Planned | Low | 3-4 hours |
| Health Check Dashboard | P3 - Low | 📋 Planned | Low | 2-3 hours |
| API Versioning Infrastructure | P3 - Low | 📋 Planned | Low | 2-3 hours |

**Total Estimated Effort:** 69-93 hours (9-12 working days)

---

## 🎯 Phase 1: Critical Infrastructure (P0)

### ✅ 1. Real-Time Communication (SignalR) - **COMPLETE**

**Implementation:**
- ✅ `TenantScopedHub<THub>` base class with JWT authentication
- ✅ Automatic tenant/user group management
- ✅ Demo chat hub with full functionality
- ✅ Blazor UI component with real-time features
- ✅ MessagePack protocol for efficient serialization
- ✅ Comprehensive documentation

**Files Created:**
- `AppBlueprint.Infrastructure/SignalR/TenantScopedHub.cs`
- `AppBlueprint.Infrastructure/SignalR/DemoChatHub.cs`
- `AppBlueprint.UiKit/Components/Pages/RealtimeChat.razor`
- `SIGNALR-IMPLEMENTATION.md`

**Usage Examples:**
```csharp
public class NotificationHub : TenantScopedHub<NotificationHub>
{
    public async Task SendNotification(string message)
    {
        await SendToTenantAsync("ReceiveNotification", message);
    }
}
```

---

### 📋 2. Background Jobs Infrastructure

**Current State:** Missing entirely  
**Priority:** P0 - Critical (blocks async operations)  
**Complexity:** Medium  
**Estimated Effort:** 6-8 hours  

**Why It's Needed:**
- Email sending should be asynchronous (don't block HTTP requests)
- Report generation (PDF, CSV exports)
- Data cleanup jobs (e.g., delete old sessions, expired tokens)
- Scheduled tenant tasks (billing runs, reminder emails)
- Webhook delivery retries
- Bulk operations (batch user imports, mass notifications)

**Proposed Solution:**

**Option A: Hangfire (Recommended)**
- Persistent job storage (PostgreSQL)
- Built-in dashboard
- Cron scheduling
- Automatic retries
- Tenant-scoped job filtering

**Option B: Quartz.NET**
- More lightweight
- Less dependency overhead
- Requires custom dashboard

**Implementation Plan:**

```csharp
// 1. Base class for tenant-scoped jobs
public abstract class TenantScopedJob
{
    protected TenantId TenantId { get; private set; }
    
    public void SetTenantContext(TenantId tenantId)
    {
        TenantId = tenantId;
    }
}

// 2. Job service abstraction
public interface IBackgroundJobService
{
    Task<string> EnqueueAsync<TJob>(Expression<Action<TJob>> methodCall) where TJob : TenantScopedJob;
    Task<string> ScheduleAsync<TJob>(Expression<Action<TJob>> methodCall, TimeSpan delay) where TJob : TenantScopedJob;
    Task<string> RecurringAsync<TJob>(string jobId, Expression<Action<TJob>> methodCall, string cronExpression) where TJob : TenantScopedJob;
}

// 3. Example job
public class SendWelcomeEmailJob : TenantScopedJob
{
    private readonly IEmailService _emailService;
    
    public async Task Execute(string userId, string emailAddress)
    {
        // TenantId is automatically set by framework
        await _emailService.SendWelcomeEmailAsync(emailAddress);
    }
}

// 4. Usage
await _backgroundJobService.EnqueueAsync<SendWelcomeEmailJob>(
    job => job.Execute(userId, email)
);
```

**Files to Create:**
- `AppBlueprint.Infrastructure/BackgroundJobs/TenantScopedJob.cs`
- `AppBlueprint.Infrastructure/BackgroundJobs/IBackgroundJobService.cs`
- `AppBlueprint.Infrastructure/BackgroundJobs/HangfireBackgroundJobService.cs`
- `AppBlueprint.Infrastructure/BackgroundJobs/TenantJobFilter.cs` (ensures jobs run with correct tenant context)
- `AppBlueprint.Infrastructure/Extensions/ServiceCollectionExtensions.cs` (AddAppBlueprintBackgroundJobs)
- Demo page: `AppBlueprint.UiKit/Components/Pages/BackgroundJobsDemo.razor`

**Configuration:**
```csharp
builder.Services.AddAppBlueprintBackgroundJobs(options =>
{
    options.UsePostgreSql(connectionString);
    options.DashboardPath = "/admin/jobs"; // Require admin role
    options.MaxRetryAttempts = 3;
});
```

**Security Considerations:**
- Dashboard access restricted to admin users
- Tenant context automatically injected (prevent cross-tenant job execution)
- Job serialization security (don't expose sensitive data in job arguments)

---

## 🔥 Phase 2: High-Priority Services (P1)

### 📋 3. Full-Text Search Infrastructure

**Current State:** Basic EF Core queries only  
**Priority:** P1 - High (critical for user experience)  
**Complexity:** Medium  
**Estimated Effort:** 4-6 hours  

**Why It's Needed:**
- Property search by name, description, location
- User search in admin panels
- Document/content search
- Autocomplete suggestions
- Faceted search (filters)

**Proposed Solution:**

**Use PostgreSQL Full-Text Search** (already have PostgreSQL)
- No external dependencies
- Tenant isolation via RLS (automatic)
- Supports multiple languages
- Built-in ranking

**Implementation Plan:**

```csharp
// 1. Search service abstraction
public interface ISearchService<TEntity> where TEntity : class
{
    Task<SearchResult<TEntity>> SearchAsync(SearchQuery query);
    Task IndexAsync(TEntity entity);
    Task ReindexAsync(IEnumerable<TEntity> entities);
}

// 2. Search query builder
public class SearchQuery
{
    public string Query { get; set; } = string.Empty;
    public int PageSize { get; set; } = 20;
    public int PageNumber { get; set; } = 1;
    public Dictionary<string, object> Filters { get; set; } = new();
    public SearchRankingOptions Ranking { get; set; } = SearchRankingOptions.Default;
}

// 3. PostgreSQL implementation
public class PostgreSqlSearchService<TEntity> : ISearchService<TEntity>
{
    // Uses tsvector and tsquery
    // Automatically includes tenant ID in queries
}

// 4. Entity configuration
public class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        // Add search vector column
        builder.Property<NpgsqlTsVector>("SearchVector")
            .HasComputedColumnSql(
                "to_tsvector('english', coalesce(name,'') || ' ' || coalesce(description,''))",
                stored: true
            );
        
        builder.HasIndex("SearchVector")
            .HasMethod("GIN");
    }
}
```

**Files to Create:**
- `AppBlueprint.Infrastructure/Search/ISearchService.cs`
- `AppBlueprint.Infrastructure/Search/PostgreSqlSearchService.cs`
- `AppBlueprint.Infrastructure/Search/SearchQuery.cs`
- `AppBlueprint.Infrastructure/Search/SearchResult.cs`
- Demo page: `AppBlueprint.UiKit/Components/Pages/SearchDemo.razor`

**Migration Required:**
```sql
-- Add search vectors to searchable entities
ALTER TABLE properties ADD COLUMN search_vector tsvector 
    GENERATED ALWAYS AS (
        to_tsvector('english', coalesce(name,'') || ' ' || coalesce(description,''))
    ) STORED;

CREATE INDEX idx_properties_search ON properties USING GIN(search_vector);
```

---

### 📋 4. File Storage Service (Production-Ready)

**Current State:** Basic interface exists, no real implementation  
**Priority:** P1 - High (needed for user uploads)  
**Complexity:** Medium  
**Estimated Effort:** 6-8 hours  

**Why It's Needed:**
- Property images, documents
- User avatars
- PDF reports, invoices
- Bulk import files (CSV)
- User-generated content

**Current Limitation:**
```csharp
// Exists but not implemented
public interface IFileStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);
    Task<Stream> DownloadFileAsync(string fileKey);
    Task DeleteFileAsync(string fileKey);
}
```

**Proposed Enhancement:**

```csharp
// Enhanced file storage with tenant isolation and metadata
public interface IFileStorageService
{
    // Upload with automatic tenant scoping
    Task<StoredFile> UploadAsync(UploadFileRequest request);
    
    // Download with tenant validation
    Task<FileDownloadResult> DownloadAsync(string fileKey);
    
    // Generate signed URLs for direct access (24-hour expiry)
    Task<string> GetSignedUrlAsync(string fileKey, TimeSpan expiry);
    
    // List files for tenant
    Task<IEnumerable<StoredFile>> ListFilesAsync(FileListQuery query);
    
    // Delete with tenant validation
    Task DeleteAsync(string fileKey);
    
    // Bulk operations
    Task DeleteManyAsync(IEnumerable<string> fileKeys);
}

public record StoredFile(
    string FileKey,           // Unique identifier (Ulid)
    string OriginalFileName,
    string ContentType,
    long SizeInBytes,
    TenantId TenantId,
    UserId UploadedBy,
    DateTime UploadedAt,
    string? Tags,
    Dictionary<string, string> Metadata
);

public record UploadFileRequest(
    Stream FileStream,
    string FileName,
    string ContentType,
    string? Folder = null,
    Dictionary<string, string>? Metadata = null
);
```

**Implementation Options:**

**Option A: AWS S3** (Recommended for production)
```csharp
public class S3FileStorageService : IFileStorageService
{
    // Bucket structure: {bucket}/{tenantId}/{folder}/{fileKey}
    // Automatic tenant isolation
    // Signed URLs for secure direct access
}
```

**Option B: Azure Blob Storage**
```csharp
public class AzureBlobStorageService : IFileStorageService
{
    // Container per tenant or single container with tenant prefix
}
```

**Option C: Local File System** (Development only)
```csharp
public class LocalFileStorageService : IFileStorageService
{
    // Store in: {basePath}/{tenantId}/{folder}/{fileKey}
    // Development/testing only - not for production
}
```

**Files to Create:**
- `AppBlueprint.Infrastructure/FileStorage/IFileStorageService.cs` (enhanced)
- `AppBlueprint.Infrastructure/FileStorage/S3FileStorageService.cs`
- `AppBlueprint.Infrastructure/FileStorage/LocalFileStorageService.cs`
- `AppBlueprint.Infrastructure/FileStorage/StoredFile.cs`
- `AppBlueprint.Infrastructure/FileStorage/FileUploadValidator.cs` (security)
- Demo page: `AppBlueprint.UiKit/Components/Pages/FileUploadDemo.razor`

**Security Considerations:**
- File type validation (whitelist allowed extensions)
- Virus scanning integration point
- File size limits (configurable per tenant)
- Automatic tenant isolation (file keys include tenant ID)
- Signed URLs expire after 24 hours

**Configuration:**
```csharp
builder.Services.AddAppBlueprintFileStorage(options =>
{
    options.Provider = FileStorageProvider.S3;
    options.S3BucketName = "appblueprint-files";
    options.MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB
    options.AllowedExtensions = new[] { ".jpg", ".png", ".pdf", ".docx" };
});
```

---

### ✅ 5. Email Template System - **COMPLETE**

**Implementation:**
- ✅ `IEmailTemplateService` interface for rendering and sending templated emails
- ✅ `RazorEmailTemplateService` using RazorLight for Razor template compilation
- ✅ Strongly-typed email models (WelcomeEmailModel, PasswordResetEmailModel, OrderConfirmationEmailModel, etc.)
- ✅ Generic framework templates (_Layout.cshtml, WelcomeEmail.cshtml, PasswordReset.cshtml, OrderConfirmation.cshtml)
- ✅ Template override support (deployed apps can customize templates)
- ✅ Embedded resources for framework templates
- ✅ Comprehensive documentation

**Files Created:**
- `AppBlueprint.Application/Interfaces/IEmailTemplateService.cs`
- `AppBlueprint.Infrastructure/Services/Email/RazorEmailTemplateService.cs`
- `AppBlueprint.Infrastructure/Services/Email/Templates/_Layout.cshtml`
- `AppBlueprint.Infrastructure/Services/Email/Templates/WelcomeEmail.cshtml`
- `AppBlueprint.Infrastructure/Services/Email/Templates/PasswordReset.cshtml`
- `AppBlueprint.Infrastructure/Services/Email/Templates/OrderConfirmation.cshtml`
- `AppBlueprint.Infrastructure/Extensions/EmailTemplateServiceExtensions.cs`
- `AppBlueprint.Contracts/Baseline/Email/EmailTemplateModels.cs`
- `AppBlueprint.Infrastructure/Services/Email/README.md`

**Usage Examples:**
```csharp
// Register service (with template override support)
builder.Services.AddEmailTemplateService(
    Path.Combine(builder.Environment.ContentRootPath, "Templates")
);

// Send templated email
var model = new WelcomeEmailModel(
    UserName: "John Doe",
    EmailAddress: "john@example.com",
    TenantName: "Acme Corp",
    ActivationLink: "https://app.example.com/activate?token=abc123"
);

await _emailTemplateService.SendTemplatedEmailAsync(
    from: "noreply@acmecorp.com",
    to: "john@example.com",
    subject: "Welcome to Acme Corp!",
    templateName: "WelcomeEmail",
    model: model
);
```

**Architecture:**
- Framework provides generic templates as embedded resources
- Deployed apps can override templates by placing custom .cshtml files in `/Templates/` folder
- Template resolution: checks app's custom templates first, falls back to framework defaults
- Compile-time safety with strongly-typed models and IntelliSense support

---

### 📋 5. Email Template System

**Current State:** IEmailService exists, but no templating  
**Priority:** P1 - High (needed for transactional emails)  
**Complexity:** Medium  
**Estimated Effort:** 4-6 hours  

**Why It's Needed:**
- Welcome emails
- Password reset emails
- Invoice/receipt emails
- Booking confirmations
- Weekly digest emails
- Admin notifications

**Proposed Solution:**

```csharp
// 1. Template service
public interface IEmailTemplateService
{
    Task<string> RenderTemplateAsync<TModel>(string templateName, TModel model);
    Task SendTemplatedEmailAsync<TModel>(string to, string templateName, TModel model);
}

// 2. Template models
public record WelcomeEmailModel(
    string UserName,
    string ActivationLink,
    string TenantName
);

public record BookingConfirmationModel(
    string PropertyName,
    DateTime CheckInDate,
    DateTime CheckOutDate,
    decimal TotalPrice,
    string BookingReference
);

// 3. Template storage
// Option A: Embedded resources (.cshtml files)
// Option B: Database (allows tenant customization)
// Option C: Hybrid (defaults embedded, overrides in DB)

// 4. Usage
await _emailTemplateService.SendTemplatedEmailAsync(
    user.Email,
    "WelcomeEmail",
    new WelcomeEmailModel(user.Name, activationUrl, tenant.Name)
);
```

**Template Engine Options:**
- **RazorLight** (recommended - familiar Razor syntax)
- **Scriban** (lightweight, sandboxed)
- **Handlebars.NET** (simple, logic-less)

**Files to Create:**
- `AppBlueprint.Infrastructure/Email/IEmailTemplateService.cs`
- `AppBlueprint.Infrastructure/Email/RazorEmailTemplateService.cs`
- `AppBlueprint.Infrastructure/Email/Templates/WelcomeEmail.cshtml`
- `AppBlueprint.Infrastructure/Email/Templates/PasswordReset.cshtml`
- `AppBlueprint.Infrastructure/Email/Templates/BookingConfirmation.cshtml`
- `AppBlueprint.Infrastructure/Email/Templates/_Layout.cshtml` (base template)
- Demo page: `AppBlueprint.UiKit/Components/Pages/EmailTemplatesDemo.razor`

**Template Example:**
```html
@model WelcomeEmailModel
<!DOCTYPE html>
<html>
<head>
    <title>Welcome to @Model.TenantName</title>
</head>
<body>
    <h1>Welcome, @Model.UserName!</h1>
    <p>Click here to activate your account:</p>
    <a href="@Model.ActivationLink">Activate Account</a>
</body>
</html>
```

---

## 🚀 Phase 3: Medium-Priority Services (P2)

### 📋 6. Payment Webhook Infrastructure

**Priority:** P2 - Medium (needed for production billing)  
**Complexity:** High  
**Estimated Effort:** 8-10 hours  

**Implementation:**
- Signature verification (Stripe, PayPal)
- Idempotency handling (prevent duplicate processing)
- Retry logic for failed webhooks
- Webhook event store (audit log)
- Dead letter queue for failed events

---

### 📋 7. Multi-Channel Notifications

**Priority:** P2 - Medium  
**Complexity:** High  
**Estimated Effort:** 8-10 hours  

**Channels:**
- In-app notifications (database + SignalR)
- Push notifications (FCM, APNS)
- SMS notifications (Twilio)
- Email notifications (already have email service)

**Features:**
- User notification preferences (per channel)
- Notification templates
- Read/unread tracking
- Batch notifications

---

### 📋 8. Rate Limiting Middleware

**Priority:** P2 - Medium  
**Complexity:** Low  
**Estimated Effort:** 2-4 hours  

**Implementation:**
- Tenant-based rate limits
- User-based rate limits
- Endpoint-specific limits
- Sliding window algorithm
- Redis-backed for multi-server

---

### 📋 9. Caching Abstraction

**Priority:** P2 - Medium  
**Complexity:** Medium  
**Estimated Effort:** 4-6 hours  

**Implementation:**
- `ICacheService` abstraction
- In-memory cache (development)
- Redis cache (production)
- Tenant-scoped cache keys
- Cache invalidation patterns

---

## 🌟 Phase 4: Nice-to-Have Services (P3)

### 📋 10. GDPR Automation Tools

**Priority:** P3 - Low (depends on market)  
**Complexity:** High  
**Estimated Effort:** 10-12 hours  

**Features:**
- Data export (all user data in JSON/CSV)
- Right to be forgotten (cascade delete)
- Consent management
- Data retention policies
- Audit trail

---

### 📋 11. Advanced Tenant Analytics

**Priority:** P3 - Low  
**Complexity:** Medium  
**Estimated Effort:** 6-8 hours  

**Features:**
- Usage metrics per tenant
- API call tracking
- Feature adoption tracking
- Custom dashboards

---

### 📋 12. Audit Logging Infrastructure

**Priority:** P3 - Low  
**Complexity:** Medium  
**Estimated Effort:** 4-6 hours  

**Features:**
- Track all entity changes
- Who/what/when/where logging
- Compliance reporting

---

### 📋 13. Feature Flag System

**Priority:** P3 - Low  
**Complexity:** Low  
**Estimated Effort:** 3-4 hours  

**Features:**
- Tenant-level flags
- User-level flags
- A/B testing support

---

### 📋 14. Health Check Dashboard

**Priority:** P3 - Low  
**Complexity:** Low  
**Estimated Effort:** 2-3 hours  

**Features:**
- Database connectivity
- External service health
- Custom health checks
- Metrics export

---

### 📋 15. API Versioning Infrastructure

**Priority:** P3 - Low  
**Complexity:** Low  
**Estimated Effort:** 2-3 hours  

**Features:**
- URL-based versioning (/api/v1/, /api/v2/)
- Header-based versioning
- Deprecation warnings
- Version routing

---

## Implementation Priority Order

**Recommended Order:**
1. ✅ **SignalR Real-Time Communication** (COMPLETE)
2. **Background Jobs** (critical for async operations)
3. **Full-Text Search** (critical for UX)
4. **File Storage Service** (needed for user uploads)
5. **Email Template System** (needed for transactional emails)
6. **Rate Limiting** (security essential)
7. **Caching Abstraction** (performance critical)
8. **Payment Webhook Infrastructure** (for billing)
9. **Multi-Channel Notifications** (enhanced UX)
10. **Feature Flag System** (deployment flexibility)
11. **Audit Logging** (compliance)
12. **Health Check Dashboard** (operations)
13. **API Versioning** (future-proofing)
14. **GDPR Automation** (market-dependent)
15. **Advanced Analytics** (growth phase)

---

## Framework vs Application Responsibility

### ✅ **Framework Provides**
- Base classes and abstractions (e.g., `TenantScopedJob`, `TenantScopedHub`)
- Service registration and configuration
- Tenant/user context injection
- Authentication/authorization integration
- Demo pages showing usage patterns
- Comprehensive documentation

### 📝 **Applications Implement**
- Domain-specific job types (e.g., `SendBookingReminderJob`)
- Custom search fields and ranking
- Business-specific file validation rules
- Email templates specific to their domain
- Custom notification types
- Application-specific metrics

---

## Testing Strategy

Each service implementation should include:
- ✅ Unit tests for core logic
- ✅ Integration tests with real dependencies
- ✅ Demo page for visual verification
- ✅ Multi-tenant isolation tests
- ✅ Security tests (authorization, input validation)
- ✅ Performance tests (for high-volume scenarios)

---

## Success Criteria

A service is considered "complete" when:
- ✅ Base infrastructure implemented and tested
- ✅ Tenant isolation verified
- ✅ Authentication/authorization integrated
- ✅ Demo page created and functional
- ✅ Documentation written
- ✅ Service registration extension method created
- ✅ No breaking changes to existing framework code
- ✅ Follows clean architecture principles
- ✅ Security review completed

---

## Next Steps

1. ✅ Complete SignalR implementation testing
2. 📋 Begin Background Jobs implementation (Hangfire integration)
3. 📋 Create project tracking board for remaining services
4. 📋 Prioritize based on property rental app requirements

---

**Last Updated:** February 1, 2026  
**Current Focus:** 🎯 SignalR Real-Time Communication  
**Next Up:** 🔄 Background Jobs Infrastructure
