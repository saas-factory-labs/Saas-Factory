# Admin IP Whitelist Setup Guide

## Overview

This guide shows how to configure IP whitelisting for admin access to prevent unauthorized access from untrusted locations.

## 1. Configuration

Add to your `appsettings.json` or `appsettings.Production.json`:

```json
{
  "Security": {
    "AdminIpWhitelist": {
      "Enabled": true,
      "AllowedIps": [
        "203.0.113.10",        // Office IP
        "198.51.100.25",       // VPN IP
        "192.0.2.50"           // Admin workstation IP
      ]
    }
  }
}
```

### Development Configuration

For local development (`appsettings.Development.json`):

```json
{
  "Security": {
    "AdminIpWhitelist": {
      "Enabled": false  // Disable in development for convenience
    }
  }
}
```

**Note:** Localhost (127.0.0.1 and ::1) is always allowed automatically.

## 2. Register Middleware

Add to your `Program.cs` in **AppBlueprint.Web** and **AppBlueprint.ApiService**:

```csharp
// After app.UseAuthentication() and app.UseAuthorization()
app.UseAuthentication();
app.UseAuthorization();

// ✅ Add admin IP whitelist (must be after authentication/authorization)
app.UseAdminIpWhitelist();

app.MapControllers();
```

**Important:** The middleware must be placed:
- ✅ **AFTER** `UseAuthentication()` and `UseAuthorization()` (needs user identity)
- ✅ **BEFORE** `MapControllers()` or `MapRazorPages()`

## 3. How It Works

### Detection

The middleware automatically detects admin routes:
- `/api/admin/*` paths
- Paths containing `/tenant-data`
- Any route where user has `SuperAdmin` role

### IP Check Flow

```
1. Request arrives
2. Is admin route? → No → Allow
3. Is SuperAdmin? → No → Allow (let authorization handle)
4. Extract IP address
5. Is IP whitelisted? → No → Block with 403
6. Is IP whitelisted? → Yes → Allow + Log
```

### Response When Blocked

```json
{
  "error": "Forbidden",
  "message": "Admin access denied: Your IP address is not whitelisted for admin operations"
}
```

**HTTP Status:** 403 Forbidden

## 4. Finding Your IP Address

### Windows (PowerShell)
```powershell
# Public IP (recommended for production)
Invoke-RestMethod -Uri "https://api.ipify.org?format=json"

# Local network IP
(Get-NetIPAddress -AddressFamily IPv4 | Where-Object {$_.InterfaceAlias -notlike "*Loopback*"}).IPAddress
```

### Linux/Mac
```bash
# Public IP
curl https://api.ipify.org

# Local network IP
hostname -I | awk '{print $1}'
```

### From Browser
Visit: https://whatismyipaddress.com/

## 5. Production Setup

### Step 1: Identify Admin IPs

1. **Office Network**: Get your office's public IP from ISP
2. **VPN**: Get VPN exit IP addresses
3. **Admin Workstations**: If using static IPs, add individual IPs
4. **Cloud Services**: If running admin tools in cloud, add cloud IPs

### Step 2: Update Production Configuration

```bash
# Using Azure App Service Configuration
az webapp config appsettings set --name your-app --resource-group your-rg --settings \
  Security__AdminIpWhitelist__Enabled=true \
  Security__AdminIpWhitelist__AllowedIps__0=203.0.113.10 \
  Security__AdminIpWhitelist__AllowedIps__1=198.51.100.25

# Using Environment Variables
export Security__AdminIpWhitelist__Enabled=true
export Security__AdminIpWhitelist__AllowedIps__0=203.0.113.10
export Security__AdminIpWhitelist__AllowedIps__1=198.51.100.25
```

### Step 3: Test

```powershell
# Test from allowed IP (should succeed)
Invoke-RestMethod -Uri "https://your-app.com/api/admin/tenants" `
  -Headers @{Authorization="Bearer $token"}

# Test from blocked IP (should return 403)
# Use different network or VPN to test
```

## 6. Monitoring

### Log Examples

**Allowed Access:**
```
ADMIN_IP_ALLOWED | AdminUserId=admin@example.com | IpAddress=203.0.113.10 | Path=/api/admin/tenants
```

**Blocked Access:**
```
ADMIN_IP_BLOCKED | AdminUserId=admin@example.com | IpAddress=198.51.100.99 | Path=/api/admin/tenants | Reason=IP_NOT_WHITELISTED
```

### Query Blocked Attempts

```sql
-- PostgreSQL example (if using structured logging to database)
SELECT 
    timestamp,
    admin_user_id,
    ip_address,
    path
FROM logs
WHERE message LIKE '%ADMIN_IP_BLOCKED%'
ORDER BY timestamp DESC
LIMIT 100;
```

### Grafana Dashboard Query

```promql
# Count of blocked admin access attempts
sum(rate(admin_ip_blocked_total[5m])) by (ip_address)

# Alert if > 5 blocked attempts from same IP in 5 minutes
sum(rate(admin_ip_blocked_total[5m])) by (ip_address) > 5
```

## 7. Security Considerations

### ✅ DO

- ✅ **Use static IPs** for production admin access
- ✅ **Enable in production** - set `Enabled: true`
- ✅ **Monitor blocked attempts** - alert on suspicious activity
- ✅ **Document IPs** - maintain list of why each IP is whitelisted
- ✅ **Regular review** - audit whitelist quarterly
- ✅ **Use VPN** - route admin traffic through corporate VPN
- ✅ **Test blocking** - verify non-whitelisted IPs are blocked

### ❌ DON'T

- ❌ **Don't use dynamic IPs** - residential ISPs change IPs frequently
- ❌ **Don't whitelist broad ranges** - be specific (e.g., avoid 0.0.0.0/0)
- ❌ **Don't disable in production** - defeats the purpose
- ❌ **Don't share IPs** - one IP per location/service
- ❌ **Don't forget to update** - remove IPs when no longer needed

### IPv6 Support

The middleware automatically handles:
- IPv4 addresses (e.g., 203.0.113.10)
- IPv6 addresses (e.g., 2001:db8::1)
- IPv4-mapped IPv6 addresses (e.g., ::ffff:203.0.113.10)

## 8. Troubleshooting

### "Admin access denied: Your IP address is not whitelisted"

**Cause:** Your IP is not in the allowed list.

**Solution:**
1. Find your current IP: `curl https://api.ipify.org`
2. Add to configuration
3. Restart application
4. Verify: Check logs for "Admin IP whitelist: Added {IpAddress}"

### Middleware Not Working

**Check:**
1. ✅ Middleware registered in `Program.cs`?
2. ✅ Placed after `UseAuthentication()` and `UseAuthorization()`?
3. ✅ Configuration section named correctly (`Security:AdminIpWhitelist`)?
4. ✅ `Enabled: true` in configuration?
5. ✅ Route matches admin pattern (`/api/admin/*` or `/tenant-data`)?

### Behind Reverse Proxy (Nginx, Cloudflare, etc.)

If using a reverse proxy, configure forwarded headers:

```csharp
// Program.cs - Add BEFORE UseAdminIpWhitelist()
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
    KnownNetworks = { /* your proxy IPs */ },
    KnownProxies = { /* your proxy IPs */ }
});

app.UseAdminIpWhitelist();
```

**Why:** Reverse proxies mask the real client IP. This ensures `RemoteIpAddress` is set correctly.

## 9. Integration with Other Security Layers

This middleware works with other admin security measures:

```
Request Flow:
1. Authentication (JWT validation)
2. Authorization (SuperAdmin role check)
3. ✅ IP Whitelist (this middleware)
4. MFA Verification (in AdminTenantAccessService)
5. Rate Limiting (AdminRateLimitMiddleware)
6. Audit Logging (AdminTenantAccessService)
```

All layers must pass for admin access to succeed.

## 10. Quick Reference

### Configuration Template

```json
{
  "Security": {
    "AdminIpWhitelist": {
      "Enabled": true,
      "AllowedIps": [
        "YOUR_OFFICE_IP",
        "YOUR_VPN_IP",
        "YOUR_HOME_IP"
      ]
    }
  }
}
```

### Program.cs Template

```csharp
var builder = WebApplication.CreateBuilder(args);
// ... services configuration ...

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAdminIpWhitelist(); // ✅ Add here
app.MapControllers();

app.Run();
```

### Test Commands

```powershell
# Get your IP
curl https://api.ipify.org

# Test admin endpoint
curl -H "Authorization: Bearer YOUR_TOKEN" https://your-app.com/api/admin/tenants

# View logs
docker logs your-container | grep "ADMIN_IP"
```

## Summary

✅ **Implemented** - IP whitelist middleware created  
✅ **Configurable** - Enable/disable per environment  
✅ **Automatic** - Detects admin routes and SuperAdmin role  
✅ **Secure** - Blocks non-whitelisted IPs with 403  
✅ **Logged** - All attempts logged for monitoring  
✅ **Production-ready** - Handles IPv4, IPv6, and proxies  

This adds a critical defense layer preventing admin access from untrusted networks.
