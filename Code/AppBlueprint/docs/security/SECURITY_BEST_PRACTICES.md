# Security Best Practices for AppBlueprint

This guide outlines critical security practices when using AppBlueprint for B2C/B2B SaaS applications.

## 🔐 Authentication & Secrets Management

### Never Commit Secrets to Version Control

**PROHIBITED in appsettings.json:**
- ❌ **OAuth Client Secrets** - Used to authenticate your application
- ❌ **API Keys** - Third-party service authentication tokens
- ❌ **JWT Signing Keys** - Secret keys for token generation/validation
- ❌ **Database Connection Strings** - With passwords/credentials
- ❌ **Encryption Keys** - For data encryption at rest
- ❌ **Service Account Credentials** - Cloud provider secrets

**SAFE to commit (these are public identifiers):**
- ✅ **OAuth Client IDs** - Public application identifiers visible in OAuth URLs
- ✅ **OAuth Endpoints/Domains** - Public URLs (e.g., `login.auth0.com`)
- ✅ **Tenant IDs** - Public identifiers for multi-tenant setups

**Why Client IDs are public:**
- They appear in OAuth redirect URLs: `https://auth.example.com/authorize?client_id=abc123&...`
- Anyone using your app can inspect network traffic and see the Client ID
- They cannot authenticate without the Client Secret
- OAuth2 RFC 6749 explicitly defines them as non-confidential

**"Security by obscurity" caveat:** Some organizations keep even public identifiers out of source control to avoid advertising their infrastructure setup to potential attackers. This is a valid defense-in-depth approach but provides minimal real security benefit since the information is exposed during normal application use.

### Approved Secret Storage Methods

#### Local Development
```bash
# Use dotnet user-secrets
dotnet user-secrets init
dotnet user-secrets set "Authentication:Logto:ClientSecret" "secret-value"
```

#### Production (Azure)
```csharp
// Program.cs - Add Azure Key Vault
if (builder.Environment.IsProduction())
{
    var keyVaultUri = new Uri(builder.Configuration["KeyVaultEndpoint"]!);
    builder.Configuration.AddAzureKeyVault(
        keyVaultUri,
        new DefaultAzureCredential()
    );
}
```

#### Production (AWS)
```csharp
// Use AWS Secrets Manager
using Amazon.SecretsManager;
using Amazon.SecretsManager.Extensions.Caching;

var cache = new SecretsManagerCache();
var secret = await cache.GetSecretString("app/secrets");
```

#### Container Deployments
```yaml
# docker-compose.yml - Use environment variables
services:
  web:
    environment:
      - Authentication__Provider=Logto
      - Authentication__Logto__ClientSecret=${LOGTO_SECRET}
    env_file:
      - .env.secrets  # Add to .gitignore!
```

## 🛡️ Authentication Security

### Use Production-Grade OAuth2/OIDC Providers

**Recommended:**
- ✅ Logto (open-source, self-hosted option)
- ✅ Auth0 (enterprise, battle-tested)
- ✅ Azure AD B2C (Microsoft identity)
- ✅ AWS Cognito (AWS ecosystem)

**NOT Recommended for Production:**
- ❌ JWT provider (simple, lacks OAuth2 flow)
- ❌ Mock provider (testing only)

### Enable Multi-Factor Authentication (MFA)

Configure MFA in your authentication provider:

```json
{
  "Authentication": {
    "Provider": "Logto",
    "Logto": {
      "MfaRequired": true,
      "MfaMethods": ["totp", "sms"]
    }
  }
}
```

### Implement Token Rotation

```csharp
// Refresh tokens before expiration
var result = await _authProvider.RefreshTokenAsync(refreshToken);
if (result.IsSuccess)
{
    await _tokenStorage.StoreTokenAsync(result.AccessToken);
}
```

## 🔒 Data Protection

### Enable HTTPS Only

```csharp
// Program.cs
app.UseHttpsRedirection();

if (builder.Environment.IsProduction())
{
    app.UseHsts(); // HTTP Strict Transport Security
}
```

### Configure CORS Properly

```csharp
// Only allow specific origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("Production", policy =>
    {
        policy.WithOrigins(
            "https://yourdomain.com",
            "https://www.yourdomain.com"
        )
        .AllowCredentials()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});
```

### Encrypt Sensitive Data at Rest

```csharp
// Use IDataProtectionProvider for sensitive fields
public class SecureUserData
{
    [ProtectedData]
    public string? SocialSecurityNumber { get; set; }
    
    [ProtectedData]
    public string? PaymentMethodToken { get; set; }
}
```

## 🎯 Multi-Tenancy Security

### Tenant Isolation

```csharp
// Always validate tenant access
public class TenantScopedRepository<T> where T : ITenantScopedEntity
{
    public async Task<T?> GetByIdAsync(TenantId tenantId, TId id)
    {
        // Enforce tenant filter at database level
        return await _dbContext.Set<T>()
            .Where(e => e.TenantId == tenantId && e.Id == id)
            .FirstOrDefaultAsync();
    }
}
```

### Row-Level Security (PostgreSQL)

```sql
-- Enable RLS on tenant-scoped tables
ALTER TABLE todos ENABLE ROW LEVEL SECURITY;

-- Create policy for tenant isolation
CREATE POLICY tenant_isolation ON todos
    USING (tenant_id = current_setting('app.tenant_id')::uuid);
```

### Prevent Tenant Enumeration

```csharp
// Don't reveal tenant existence in error messages
if (!await _tenantService.ExistsAsync(tenantId))
{
    // Generic error - don't reveal tenant doesn't exist
    return Unauthorized("Access denied");
}
```

## 🚨 Security Headers

### Add Security Headers

```csharp
// Program.cs
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Append(
        "Content-Security-Policy",
        "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'"
    );
    
    await next();
});
```

## 📝 Audit Logging

### Log Security Events

```csharp
public class AuditLogger
{
    public void LogAuthenticationSuccess(string userId, string tenantId)
    {
        _logger.LogInformation(
            "User {UserId} authenticated successfully for tenant {TenantId}",
            userId,
            tenantId
        );
    }
    
    public void LogAuthenticationFailure(string email, string reason)
    {
        _logger.LogWarning(
            "Authentication failed for {Email}: {Reason}",
            email,
            reason
        );
    }
    
    public void LogUnauthorizedAccess(string userId, string resource)
    {
        _logger.LogWarning(
            "User {UserId} attempted unauthorized access to {Resource}",
            userId,
            resource
        );
    }
}
```

## 🔍 Security Testing

### Implement Security Tests

```csharp
[Test]
public async Task Unauthorized_User_Cannot_Access_Other_Tenant_Data()
{
    // Arrange
    var tenantA = TenantId.NewId();
    var tenantB = TenantId.NewId();
    var userFromTenantB = await CreateUserAsync(tenantB);
    var todoInTenantA = await CreateTodoAsync(tenantA);
    
    // Act
    var result = await _todoService.GetByIdAsync(
        userFromTenantB.Id,
        tenantB,
        todoInTenantA.Id
    );
    
    // Assert
    result.Should().BeNull();
}
```

## 📋 Security Checklist

Before deploying to production:

### Authentication
- [ ] Using production OAuth2/OIDC provider (not Mock/JWT)
- [ ] MFA enabled for admin accounts
- [ ] Token refresh implemented
- [ ] Secure token storage (HTTP-only cookies or secure storage)
- [ ] Session timeout configured

### Secrets Management
- [ ] No secrets in appsettings.json
- [ ] Azure Key Vault or AWS Secrets Manager configured
- [ ] User secrets for local development
- [ ] CI/CD secrets stored securely
- [ ] .gitignore includes secret files

### Network Security
- [ ] HTTPS enforced (no HTTP)
- [ ] HSTS enabled
- [ ] CORS configured with specific origins
- [ ] Security headers added
- [ ] Rate limiting implemented

### Data Protection
- [ ] Sensitive fields encrypted at rest
- [ ] Database backups encrypted
- [ ] Personal data anonymization strategy
- [ ] GDPR compliance measures

### Multi-Tenancy
- [ ] Row-level security enabled (PostgreSQL)
- [ ] Tenant isolation tested
- [ ] No tenant enumeration vulnerabilities
- [ ] Cross-tenant access tests pass

### Monitoring
- [ ] Security event logging
- [ ] Failed authentication alerts
- [ ] Unusual access pattern detection
- [ ] Log aggregation (e.g., Application Insights, CloudWatch)

### Compliance
- [ ] Privacy policy published
- [ ] Terms of service published
- [ ] Cookie consent implemented
- [ ] Data export functionality (GDPR)
- [ ] Account deletion functionality (GDPR)

## 🔗 Further Reading

- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [Azure Security Best Practices](https://learn.microsoft.com/en-us/azure/security/fundamentals/best-practices-and-patterns)
- [AWS Security Best Practices](https://aws.amazon.com/architecture/security-identity-compliance/)
- [NIST Cybersecurity Framework](https://www.nist.gov/cyberframework)
