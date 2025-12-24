# Admin Access Security Hardening

## Overview

The `IAdminTenantAccessService` provides read-only access to tenant data for support purposes. While it includes defense-in-depth protections (RBAC, `.AsNoTracking()`, RLS policies, audit logging), it's still a powerful capability that requires additional security hardening.

This document covers attack vectors and practical mitigation strategies.

---

## Attack Vectors & Mitigations

### 1. Privilege Escalation

**Attack**: Attacker gains or creates SuperAdmin role through vulnerability.

**Mitigations**:

#### A. Implement Multi-Factor Authentication (MFA) for Admins

```csharp
// Infrastructure/Services/AdminTenantAccessService.cs
public async Task<TResult> ExecuteReadOnlyAsAdminAsync<TResult>(
    string tenantId,
    string reason,
    Func<Task<TResult>> queryAction)
{
    // ... existing code ...
    
    // ✅ Add: Verify MFA is enabled for admin
    if (!_currentUserService.HasMfaEnabled())
    {
        _logger.LogWarning(
            "ADMIN_ACCESS_DENIED | AdminUserId={AdminUserId} | Reason=MFA_NOT_ENABLED",
            _currentUserService.UserId);
        
        throw new UnauthorizedAccessException(
            "Multi-factor authentication is required for admin access");
    }
    
    // ... rest of method ...
}
```

#### B. Separate Admin Accounts (No Dual-Use)

```csharp
// Application/Services/ICurrentUserService.cs
public interface ICurrentUserService
{
    string? UserId { get; }
    string? Email { get; }
    bool IsInRole(string role);
    IEnumerable<string> Roles { get; }
    
    // ✅ Add: Check if account is admin-only (no regular user access)
    bool IsAdminOnlyAccount();
}

// Infrastructure/Services/AdminTenantAccessService.cs
// Add to ExecuteReadOnlyAsAdminAsync():
if (!_currentUserService.IsAdminOnlyAccount())
{
    throw new UnauthorizedAccessException(
        "Admin access requires a dedicated admin account (no dual-use accounts)");
}
```

**Implementation**:
- Store `IsAdminOnly` flag in user profile
- Admin users cannot have tenant-specific data
- Prevents compromised regular user accounts from being elevated to admin

#### C. Role-Based Access Control (Already Implemented) ✅

```csharp
// ✅ Already implemented in AdminTenantAccessService
if (!_currentUserService.IsInRole("SuperAdmin"))
{
    throw new UnauthorizedAccessException("Only SuperAdmins can access other tenants' data");
}
```

---

### 2. Session Hijacking

**Attack**: Attacker steals admin JWT token and uses it for unauthorized access.

**Mitigations**:

#### A. Short-Lived Admin Sessions

```csharp
// Infrastructure/Authentication/WebAuthenticationExtensions.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            
            // ✅ Add: Short token lifetime for admin users
            ClockSkew = TimeSpan.Zero // No tolerance for expired tokens
        };
        
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var user = context.Principal;
                if (user?.IsInRole("SuperAdmin") == true)
                {
                    // ✅ Enforce max 15-minute sessions for admins
                    var expClaim = user.FindFirst("exp");
                    if (expClaim != null)
                    {
                        var exp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim.Value));
                        var issued = DateTimeOffset.FromUnixTimeSeconds(
                            long.Parse(user.FindFirst("iat")?.Value ?? "0"));
                        
                        if ((exp - issued).TotalMinutes > 15)
                        {
                            context.Fail("Admin tokens must expire within 15 minutes");
                        }
                    }
                }
                return Task.CompletedTask;
            }
        };
    });
```

#### B. IP Address Allowlisting for Admin Access

```csharp
// Infrastructure/Middleware/AdminIpWhitelistMiddleware.cs
public sealed class AdminIpWhitelistMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AdminIpWhitelistMiddleware> _logger;
    private readonly HashSet<IPAddress> _allowedIps;

    public AdminIpWhitelistMiddleware(
        RequestDelegate next,
        ILogger<AdminIpWhitelistMiddleware> logger,
        IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        
        // Load from configuration
        string[]? ips = configuration.GetSection("Security:AdminAllowedIPs").Get<string[]>();
        _allowedIps = ips?.Select(IPAddress.Parse).ToHashSet() ?? new HashSet<IPAddress>();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Check if user is admin
        if (context.User?.IsInRole("SuperAdmin") == true)
        {
            IPAddress? remoteIp = context.Connection.RemoteIpAddress;
            
            // ✅ Verify IP is in allowlist
            if (remoteIp != null && !_allowedIps.Contains(remoteIp))
            {
                _logger.LogWarning(
                    "ADMIN_ACCESS_BLOCKED | AdminUserId={AdminUserId} | IP={IpAddress} | Reason=IP_NOT_ALLOWED",
                    context.User.FindFirst("sub")?.Value,
                    remoteIp);
                
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Admin access not allowed from this IP address");
                return;
            }
        }

        await _next(context);
    }
}

// appsettings.json
{
  "Security": {
    "AdminAllowedIPs": [
      "203.0.113.42",      // Office IP
      "198.51.100.100"     // VPN IP
    ]
  }
}
```

#### C. Device Fingerprinting

```csharp
// Infrastructure/Services/DeviceFingerprintService.cs
public sealed class DeviceFingerprintService
{
    public string GenerateFingerprint(HttpContext context)
    {
        // Combine multiple device characteristics
        var components = new[]
        {
            context.Request.Headers.UserAgent.ToString(),
            context.Request.Headers.AcceptLanguage.ToString(),
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            context.Request.Headers["sec-ch-ua"].ToString()
        };
        
        string combined = string.Join("|", components);
        return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(combined)));
    }
}

// Store fingerprint in JWT claims during admin login
// Verify fingerprint hasn't changed during admin operations
```

---

### 3. SQL Injection via tenantId Parameter

**Attack**: Pass malicious SQL in `tenantId` parameter to bypass RLS or extract data.

**Mitigations**:

#### A. Input Validation (Already Implemented via Parameterized Queries) ✅

```csharp
// ✅ Already safe - uses parameterized queries
await _dbContext.Database.ExecuteSqlRawAsync(@"
    SELECT set_config('app.current_tenant_id', {0}, FALSE);
    SELECT set_config('app.is_admin', 'true', FALSE);
", tenantId);
```

**Why it's safe**: `{0}` placeholder is automatically parameterized by EF Core.

#### B. Additional Format Validation

```csharp
// Infrastructure/Services/AdminTenantAccessService.cs
public async Task<TResult> ExecuteReadOnlyAsAdminAsync<TResult>(
    string tenantId,
    string reason,
    Func<Task<TResult>> queryAction)
{
    ArgumentNullException.ThrowIfNull(tenantId);
    ArgumentNullException.ThrowIfNull(reason);
    
    // ✅ Add: Validate tenant ID format (ULID or GUID)
    if (!IsValidTenantIdFormat(tenantId))
    {
        _logger.LogWarning(
            "ADMIN_ACCESS_DENIED | AdminUserId={AdminUserId} | InvalidTenantId={TenantId}",
            _currentUserService.UserId,
            tenantId);
        
        throw new ArgumentException(
            "Tenant ID must be a valid ULID or GUID format", 
            nameof(tenantId));
    }
    
    // ... rest of method ...
}

private static bool IsValidTenantIdFormat(string tenantId)
{
    // Example: Validate ULID format (26 characters, alphanumeric)
    return tenantId.Length == 26 && 
           tenantId.All(c => char.IsLetterOrDigit(c));
}
```

---

### 4. Bulk Data Extraction

**Attack**: Admin queries thousands of tenants to extract all data.

**Mitigations**:

#### A. Rate Limiting for Admin Operations

```csharp
// Infrastructure/Middleware/AdminRateLimitMiddleware.cs
using System.Collections.Concurrent;

public sealed class AdminRateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AdminRateLimitMiddleware> _logger;
    private static readonly ConcurrentDictionary<string, AdminAccessLog> _accessLog = new();

    private sealed class AdminAccessLog
    {
        public List<DateTimeOffset> AccessTimes { get; } = new();
    }

    public async Task InvokeAsync(HttpContext context, ICurrentUserService currentUserService)
    {
        if (currentUserService.IsInRole("SuperAdmin"))
        {
            string adminUserId = currentUserService.UserId ?? "unknown";
            AdminAccessLog log = _accessLog.GetOrAdd(adminUserId, _ => new AdminAccessLog());

            lock (log.AccessTimes)
            {
                // Remove accesses older than 1 hour
                DateTimeOffset oneHourAgo = DateTimeOffset.UtcNow.AddHours(-1);
                log.AccessTimes.RemoveAll(t => t < oneHourAgo);

                // ✅ Limit: Max 10 different tenants per hour
                if (log.AccessTimes.Count >= 10)
                {
                    _logger.LogWarning(
                        "ADMIN_RATE_LIMIT_EXCEEDED | AdminUserId={AdminUserId} | Count={Count}",
                        adminUserId,
                        log.AccessTimes.Count);

                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.Response.WriteAsync(
                        "Rate limit exceeded: Maximum 10 tenant accesses per hour");
                    return;
                }

                log.AccessTimes.Add(DateTimeOffset.UtcNow);
            }
        }

        await _next(context);
    }
}

// Register in Program.cs
app.UseMiddleware<AdminRateLimitMiddleware>();
```

#### B. Query Result Limits

```csharp
// Infrastructure/Services/AdminTenantAccessService.cs
public async Task<TResult> ExecuteReadOnlyAsAdminAsync<TResult>(
    string tenantId,
    string reason,
    Func<Task<TResult>> queryAction)
{
    // ... existing validation ...
    
    try
    {
        // Set RLS session variables
        await _dbContext.Database.ExecuteSqlRawAsync(@"
            SELECT set_config('app.current_tenant_id', {0}, FALSE);
            SELECT set_config('app.is_admin', 'true', FALSE);
            SELECT set_config('statement_timeout', '10000', FALSE);  -- ✅ 10 second query timeout
        ", tenantId);

        TResult result = await queryAction();

        // ✅ Add: Warn if result set is suspiciously large
        if (result is ICollection collection && collection.Count > 1000)
        {
            _logger.LogWarning(
                "ADMIN_LARGE_RESULT_SET | AdminUserId={AdminUserId} | TenantId={TenantId} | " +
                "Count={Count} | Reason={Reason}",
                adminUserId,
                tenantId,
                collection.Count,
                reason);
        }

        return result;
    }
    // ... rest of method ...
}
```

---

### 5. Credential Stuffing / Brute Force

**Attack**: Attacker attempts to guess admin credentials through repeated login attempts.

**Mitigations**:

#### A. Account Lockout Policy

```csharp
// Infrastructure/Authentication/AdminAccountLockoutService.cs
public sealed class AdminAccountLockoutService
{
    private static readonly ConcurrentDictionary<string, FailedLoginAttempts> _attempts = new();

    private sealed class FailedLoginAttempts
    {
        public int Count { get; set; }
        public DateTimeOffset LockoutUntil { get; set; }
    }

    public bool IsLockedOut(string email, out DateTimeOffset lockoutUntil)
    {
        if (_attempts.TryGetValue(email, out FailedLoginAttempts? attempts))
        {
            if (attempts.LockoutUntil > DateTimeOffset.UtcNow)
            {
                lockoutUntil = attempts.LockoutUntil;
                return true;
            }
        }

        lockoutUntil = DateTimeOffset.MinValue;
        return false;
    }

    public void RecordFailedAttempt(string email)
    {
        FailedLoginAttempts attempts = _attempts.GetOrAdd(email, _ => new FailedLoginAttempts());
        
        attempts.Count++;
        
        // ✅ Progressive lockout: 5 failed = 15 min, 10 failed = 1 hour, 15 failed = 24 hours
        if (attempts.Count >= 15)
            attempts.LockoutUntil = DateTimeOffset.UtcNow.AddHours(24);
        else if (attempts.Count >= 10)
            attempts.LockoutUntil = DateTimeOffset.UtcNow.AddHours(1);
        else if (attempts.Count >= 5)
            attempts.LockoutUntil = DateTimeOffset.UtcNow.AddMinutes(15);
    }

    public void ResetAttempts(string email)
    {
        _attempts.TryRemove(email, out _);
    }
}
```

#### B. CAPTCHA for Admin Login

```csharp
// After 2 failed attempts, require CAPTCHA for admin logins
// Use hCaptcha or Google reCAPTCHA v3
```

---

### 6. Insider Threat (Malicious Admin)

**Attack**: Legitimate admin with bad intentions abuses access.

**Mitigations**:

#### A. Mandatory Justification with Ticket Validation

```csharp
// Infrastructure/Services/TicketValidationService.cs
public sealed class TicketValidationService
{
    private readonly HttpClient _httpClient;

    public async Task<bool> ValidateTicketAsync(string reason)
    {
        // Extract ticket number from reason (e.g., "Support ticket #12345")
        Match match = Regex.Match(reason, @"#(\d+)", RegexOptions.IgnoreCase);
        
        if (!match.Success)
            return false; // No ticket number provided

        string ticketNumber = match.Groups[1].Value;

        // ✅ Verify ticket exists in ticketing system (e.g., Zendesk, Jira)
        HttpResponseMessage response = await _httpClient.GetAsync(
            $"https://your-ticketing-system.com/api/tickets/{ticketNumber}");

        return response.IsSuccessStatusCode;
    }
}

// Infrastructure/Services/AdminTenantAccessService.cs
public async Task<TResult> ExecuteReadOnlyAsAdminAsync<TResult>(
    string tenantId,
    string reason,
    Func<Task<TResult>> queryAction)
{
    // ... existing validation ...
    
    // ✅ Add: Validate support ticket exists
    if (!await _ticketValidationService.ValidateTicketAsync(reason))
    {
        _logger.LogWarning(
            "ADMIN_ACCESS_DENIED | AdminUserId={AdminUserId} | Reason=INVALID_TICKET | " +
            "ProvidedReason={Reason}",
            _currentUserService.UserId,
            reason);
        
        throw new ArgumentException(
            "Reason must include a valid support ticket number (e.g., 'Support ticket #12345')",
            nameof(reason));
    }
    
    // ... rest of method ...
}
```

#### B. Peer Review for Sensitive Operations

```csharp
// For high-sensitivity tenants (e.g., VIP customers, regulated industries)
// Require second admin to approve access request

public sealed class AdminApprovalService
{
    public async Task<bool> RequiresApproval(string tenantId)
    {
        // Check if tenant is flagged as high-sensitivity
        var tenant = await _dbContext.Tenants
            .Where(t => t.Id == tenantId)
            .Select(t => new { t.IsVip, t.IsRegulated })
            .FirstOrDefaultAsync();

        return tenant?.IsVip == true || tenant?.IsRegulated == true;
    }

    public async Task RequestApproval(string adminUserId, string tenantId, string reason)
    {
        // Create approval request
        // Send notification to senior admin
        // Wait for approval before granting access
    }
}
```

#### C. Real-Time Alerting for Admin Access

```csharp
// Infrastructure/Services/AdminAlertingService.cs
public sealed class AdminAlertingService
{
    private readonly IEmailService _emailService;
    private readonly ILogger<AdminAlertingService> _logger;

    public async Task SendAdminAccessAlert(
        string adminUserId,
        string tenantId,
        string reason)
    {
        // ✅ Send real-time alert to security team
        await _emailService.SendEmailAsync(
            to: "security-team@company.com",
            subject: $"🚨 Admin Tenant Access: {adminUserId}",
            body: $@"
                Admin User: {adminUserId}
                Tenant ID: {tenantId}
                Reason: {reason}
                Timestamp: {DateTimeOffset.UtcNow:yyyy-MM-dd HH:mm:ss UTC}
                
                This is an automated security notification.
            ");
    }
}
```

---

### 7. Token Replay Attack

**Attack**: Attacker captures admin JWT token and reuses it later.

**Mitigations**:

#### A. Single-Use Tokens for Admin Operations

```csharp
// Infrastructure/Services/AdminNonceService.cs
public sealed class AdminNonceService
{
    private static readonly ConcurrentDictionary<string, HashSet<string>> _usedNonces = new();

    public string GenerateNonce()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }

    public bool IsNonceUsed(string adminUserId, string nonce)
    {
        HashSet<string> nonces = _usedNonces.GetOrAdd(adminUserId, _ => new HashSet<string>());
        
        lock (nonces)
        {
            return !nonces.Add(nonce); // Returns false if nonce already exists
        }
    }

    public void CleanupExpiredNonces()
    {
        // Run periodically to prevent memory growth
        // Nonces older than 1 hour can be removed
    }
}

// Infrastructure/Services/AdminTenantAccessService.cs
// Add nonce parameter and validation:
public async Task<TResult> ExecuteReadOnlyAsAdminAsync<TResult>(
    string tenantId,
    string reason,
    string nonce, // ✅ Add: single-use nonce
    Func<Task<TResult>> queryAction)
{
    // ✅ Verify nonce hasn't been used
    if (_nonceService.IsNonceUsed(_currentUserService.UserId!, nonce))
    {
        throw new UnauthorizedAccessException("Nonce has already been used");
    }
    
    // ... rest of method ...
}
```

---

### 8. Audit Log Tampering

**Attack**: Attacker with database access deletes or modifies audit logs.

**Mitigations**:

#### A. Append-Only Audit Table (Already Implemented) ✅

```sql
-- ✅ Already implemented in SetupRowLevelSecurity.sql
CREATE TABLE "AdminAuditLog" (
    "Id" SERIAL PRIMARY KEY,
    -- ... fields ...
);

-- Grant INSERT only (no UPDATE or DELETE)
GRANT INSERT ON "AdminAuditLog" TO app_user;
```

#### B. External Audit Log Sink

```csharp
// Infrastructure/Sinks/ExternalAuditLogSink.cs
public sealed class ExternalAuditLogSink : ILogEventSink
{
    private readonly HttpClient _httpClient;

    public void Emit(LogEvent logEvent)
    {
        // ✅ Send admin access logs to external SIEM (e.g., Splunk, DataDog)
        if (logEvent.MessageTemplate.Text.Contains("ADMIN_TENANT_ACCESS"))
        {
            // Forward to external system that admin cannot modify
            _httpClient.PostAsJsonAsync(
                "https://siem.company.com/api/logs",
                new
                {
                    timestamp = logEvent.Timestamp,
                    level = logEvent.Level.ToString(),
                    message = logEvent.MessageTemplate.Text,
                    properties = logEvent.Properties
                });
        }
    }
}
```

#### C. Database Trigger for Audit Log Protection

```sql
-- Prevent deletion of audit logs
CREATE OR REPLACE FUNCTION prevent_audit_log_deletion()
RETURNS TRIGGER AS $$
BEGIN
    RAISE EXCEPTION 'Deleting audit logs is not allowed';
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER prevent_audit_deletion
BEFORE DELETE ON "AdminAuditLog"
FOR EACH ROW
EXECUTE FUNCTION prevent_audit_log_deletion();

-- Log any attempted deletions
CREATE OR REPLACE FUNCTION log_audit_log_tampering()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO "SecurityIncidents" ("Type", "Details", "Timestamp")
    VALUES ('AUDIT_LOG_TAMPERING_ATTEMPT', 
            'Attempted to delete audit log ID: ' || OLD."Id", 
            NOW());
    RAISE EXCEPTION 'Security incident logged';
END;
$$ LANGUAGE plpgsql;
```

---

## Monitoring & Alerting

### 1. Real-Time Anomaly Detection

```sql
-- Query to detect suspicious patterns (run every 5 minutes)
SELECT 
    "AdminUserId",
    COUNT(DISTINCT "TenantId") as unique_tenants_accessed,
    COUNT(*) as total_accesses,
    MIN("Timestamp") as first_access,
    MAX("Timestamp") as last_access
FROM "AdminAuditLog"
WHERE "Timestamp" >= NOW() - INTERVAL '1 hour'
GROUP BY "AdminUserId"
HAVING COUNT(DISTINCT "TenantId") > 10  -- ✅ Alert: >10 tenants/hour
    OR COUNT(*) > 50;  -- ✅ Alert: >50 accesses/hour
```

### 2. Dashboards

Create Grafana/Datadog dashboards showing:
- Admin access count over time
- Top admins by access frequency
- Failed admin login attempts
- Tenants accessed multiple times
- Large result set warnings

### 3. Weekly Security Review

```sql
-- Generate weekly admin access report
SELECT 
    "AdminUserId",
    "TenantId",
    "Reason",
    "Timestamp"
FROM "AdminAuditLog"
WHERE "Timestamp" >= NOW() - INTERVAL '7 days'
ORDER BY "Timestamp" DESC;
```

---

## Configuration Checklist

- [ ] Enable MFA for all SuperAdmin accounts
- [ ] Configure short token lifetimes (15 minutes for admins)
- [ ] Set up IP allowlisting for admin access
- [ ] Implement rate limiting (10 tenants/hour)
- [ ] Validate support ticket numbers in `reason` parameter
- [ ] Configure real-time alerts to security team
- [ ] Set up external audit log sink (SIEM)
- [ ] Create anomaly detection SQL job (run every 5 minutes)
- [ ] Build Grafana dashboard for admin access monitoring
- [ ] Schedule weekly security review of admin access logs
- [ ] Implement account lockout policy (5 failed = 15 min lockout)
- [ ] Add device fingerprinting to JWT claims
- [ ] Create runbook for responding to suspicious admin activity

---

## Incident Response

If suspicious admin activity is detected:

1. **Immediate**: Disable affected admin account
2. **Investigate**: Review `AdminAuditLog` for all accesses
3. **Notify**: Alert affected tenants if PII was accessed
4. **Remediate**: Rotate admin credentials, review firewall rules
5. **Document**: Create incident report with timeline
6. **Improve**: Update security controls based on lessons learned

---

## Summary

**Defense-in-Depth Layers** for Admin Access:

1. ✅ **Authentication**: MFA, short sessions, IP allowlisting
2. ✅ **Authorization**: RBAC (SuperAdmin only), admin-only accounts
3. ✅ **Input Validation**: Tenant ID format checks, SQL injection protection
4. ✅ **Rate Limiting**: Max 10 tenants/hour, query timeouts
5. ✅ **Audit Logging**: Append-only table, external SIEM sink
6. ✅ **Monitoring**: Real-time alerts, anomaly detection, weekly reviews
7. ✅ **Process**: Ticket validation, peer review for sensitive tenants

**Result**: Even if one layer fails, multiple others prevent unauthorized access.
