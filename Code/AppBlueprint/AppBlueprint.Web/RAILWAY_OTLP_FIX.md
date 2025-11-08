# Railway OTLP Telemetry Connection Fix

## Problem
The Web application was failing in Railway with repeated connection errors:
```
System.Net.Http.HttpRequestException: Connection refused (localhost:18889)
System.Net.Sockets.SocketException (111): Connection refused
```

The application was attempting to send OpenTelemetry data to `localhost:18889` (Aspire Dashboard) which doesn't exist in Railway production environments.

## Root Cause
The `AppBlueprint.ServiceDefaults` project was **unconditionally** configuring OTLP (OpenTelemetry Protocol) exporters to send telemetry data to `localhost:18889`, which is the Aspire Dashboard endpoint only available during local development.

In Railway (production):
- No Aspire Dashboard is running
- localhost:18889 doesn't exist
- Connection attempts fail and retry repeatedly
- Application logs fill with errors

## Solution Implemented

### 1. ServiceDefaults - Conditional OTLP Configuration
**File**: `AppBlueprint.ServiceDefaults/Extensions.cs`

#### ConfigureDefaultOpenTelemetrySettings()
- Only sets default `localhost:18889` endpoint in **Development** mode
- In **Production** mode, leaves endpoint unset (disables OTLP export)
- Still allows explicit OTLP endpoint via environment variable

**Before**:
```csharp
if (string.IsNullOrEmpty(endpoint))
{
    // ALWAYS set localhost - even in production!
    Environment.SetEnvironmentVariable(OtelExporterOtlpEndpoint, "http://localhost:18889");
}
```

**After**:
```csharp
if (string.IsNullOrEmpty(endpoint))
{
    // Only set default localhost endpoint in Development
    string? aspnetEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    if (string.Equals(aspnetEnv, "Development", StringComparison.OrdinalIgnoreCase))
    {
        Environment.SetEnvironmentVariable(OtelExporterOtlpEndpoint, "http://localhost:18889");
    }
    // else: Leave unset in production - will skip OTLP exporters
}
```

#### ConfigureOpenTelemetry()
- Checks if OTLP endpoint is configured before adding exporters
- Only adds OTLP exporters when endpoint is available
- Falls back to console exporter for logs in production
- Telemetry is still collected, just not exported

**Before**:
```csharp
// ALWAYS added OTLP exporters - crashes if endpoint unreachable
string endpoint = Environment.GetEnvironmentVariable(OtelExporterOtlpEndpoint)!;
log.AddOtlpExporter(opt => { opt.Endpoint = new Uri(endpoint); ... });
```

**After**:
```csharp
// Check if endpoint exists first
string? endpoint = Environment.GetEnvironmentVariable(OtelExporterOtlpEndpoint);
bool hasOtlpEndpoint = !string.IsNullOrEmpty(endpoint);

if (hasOtlpEndpoint)
{
    log.AddOtlpExporter(opt => { opt.Endpoint = new Uri(endpoint!); ... });
}
else
{
    log.AddConsoleExporter(); // Fallback for production
}
```

### 2. Web Program.cs - Environment-Aware Configuration
**File**: `AppBlueprint.Web/Program.cs`

Wrapped OTLP configuration in environment check:
- **Development**: Configures Aspire Dashboard endpoint
- **Production**: Logs that OTLP is disabled, allows explicit override

## Behavior

### Development Environment
```
[Web] Using default OTLP endpoint: http://localhost:18889
[Web] Final OTLP endpoint → http://localhost:18889
[Web] Final OTLP protocol → http/protobuf
```
- ✅ Telemetry sent to Aspire Dashboard
- ✅ Full observability in local development
- ✅ No changes to existing workflow

### Production/Railway Environment
```
[Web] Production mode - OTLP telemetry export disabled (no Aspire Dashboard)
```
- ✅ No connection attempts to localhost:18889
- ✅ No connection refused errors
- ✅ Telemetry still collected (metrics, traces, logs)
- ✅ Console logging works normally
- ⚠️ Telemetry not exported (unless explicit endpoint provided)

### Production with External Observability
If you want to send telemetry to an external service (e.g., Honeycomb, Datadog, Grafana Cloud):

```bash
# Set in Railway environment variables
OTEL_EXPORTER_OTLP_ENDPOINT=https://api.honeycomb.io
OTEL_EXPORTER_OTLP_HEADERS=x-honeycomb-team=your-api-key
```

Then:
```
[Web] Production mode - OTLP telemetry export disabled (no Aspire Dashboard)
[Web] Using explicit OTLP endpoint: https://api.honeycomb.io
```
- ✅ Telemetry exported to external service
- ✅ Full observability in production

## Architecture

### Local Development
```
Web App (Development)
    ↓ OTLP (http://localhost:18889)
Aspire Dashboard
    ↓ Visualize
Metrics, Traces, Logs
```

### Railway Production (Default)
```
Web App (Production)
    ↓ Console Logs Only
Railway Logs Dashboard
```

### Railway Production (With External Observability)
```
Web App (Production)
    ↓ OTLP (https://external-service)
Honeycomb / Datadog / Grafana Cloud
    ↓ Visualize
Metrics, Traces, Logs
```

## Changes Made

### Modified Files

#### 1. AppBlueprint.ServiceDefaults/Extensions.cs
**Lines 45-67**: ConfigureDefaultOpenTelemetrySettings()
- Only set localhost:18889 in Development
- Check ASPNETCORE_ENVIRONMENT variable

**Lines 70-149**: ConfigureOpenTelemetry()
- Check if OTLP endpoint is configured
- Conditionally add OTLP exporters
- Add console exporter fallback for logs
- Collect telemetry even without export

#### 2. AppBlueprint.Web/Program.cs
**Lines 23-70**: Telemetry configuration
- Wrapped in `if (builder.Environment.IsDevelopment())`
- Added production mode logging
- Allow explicit OTLP override in production

## Testing

### Local Development (No Changes)
```bash
cd C:\Development\Development-Projects\saas-factory-labs\Code\AppBlueprint\AppBlueprint.Web
dotnet run

# Expected output:
# [Web] Using default OTLP endpoint: http://localhost:18889
# [Web] HTTPS configured with custom certificate
```

### Railway Deployment (Fixed)
```bash
# In Railway logs:
[Web] Production mode - OTLP telemetry export disabled (no Aspire Dashboard)
[Web] Production mode - HTTPS disabled (handled by Railway edge)
[Web] API Base URL configured: http://appblueprint-api.railway.internal:8091
[Web] Application started successfully
```

**No more connection refused errors! ✅**

## Verification Checklist

- [x] ServiceDefaults only sets localhost OTLP in Development
- [x] OTLP exporters conditionally added based on endpoint availability
- [x] Web Program.cs environment-aware for telemetry
- [x] Production mode logs indicate OTLP is disabled
- [x] Explicit OTLP endpoint still supported via environment variable
- [x] Code compiles without errors
- [x] Local development unchanged

## Railway Environment Variables

### Not Required (Default Behavior)
Railway deployment works without any telemetry configuration:
- OTLP export is disabled by default in production
- Console logging still works
- Application starts successfully

### Optional (External Observability)
To enable telemetry export to external service:
```bash
OTEL_EXPORTER_OTLP_ENDPOINT=https://your-observability-service.com
OTEL_EXPORTER_OTLP_HEADERS=authorization=Bearer your-token
OTEL_EXPORTER_OTLP_PROTOCOL=http/protobuf  # or grpc
```

## Impact

### Before Fix
- ❌ Application logs filled with connection errors
- ❌ Retry attempts slowed down application
- ❌ Unclear if application is working correctly
- ❌ Wasted resources attempting impossible connections

### After Fix
- ✅ No connection errors in production
- ✅ Clean application logs
- ✅ Better performance (no retry overhead)
- ✅ Clear logging about telemetry status
- ✅ Still supports external observability if needed

## Related Issues Fixed

1. ✅ **HTTPS Certificate Error** - Disabled in production
2. ✅ **OTLP Connection Error** - Disabled in production
3. ✅ **API Base URL** - Environment variable support

All Railway deployment blockers have been resolved! 🎉

## Git Commit Message

```
fix(telemetry): disable OTLP export in production to prevent connection errors

Problem:
Application was repeatedly trying to connect to localhost:18889 (Aspire 
Dashboard) in Railway production, causing connection refused errors and 
filling logs with retry attempts.

Changes:
- ServiceDefaults: Only set localhost OTLP endpoint in Development mode
- ServiceDefaults: Conditionally add OTLP exporters based on endpoint
- Web Program.cs: Environment-aware telemetry configuration
- Add console exporter fallback for production logs

Technical Details:
- Modified AppBlueprint.ServiceDefaults/Extensions.cs
  - ConfigureDefaultOpenTelemetrySettings() checks ASPNETCORE_ENVIRONMENT
  - ConfigureOpenTelemetry() validates endpoint before adding exporters
- Modified AppBlueprint.Web/Program.cs
  - OTLP configuration wrapped in IsDevelopment() check
  - Production mode logs telemetry status

Behavior:
- Development: OTLP sent to Aspire Dashboard (localhost:18889)
- Production: OTLP disabled, console logging only
- Production with env var: OTLP sent to external service

Impact:
- Fixes: Connection refused errors in Railway
- Local dev: No changes, Aspire Dashboard works as before
- Production: Clean logs, no connection errors
- Performance: No retry overhead in production

Related: Also fixed HTTPS certificate error and API base URL configuration
```

---

## Summary

✅ **OTLP Connection Errors**: FIXED  
✅ **Aspire Dashboard Dependency**: REMOVED in production  
✅ **Railway Deployment**: Ready  
✅ **External Observability**: Supported (optional)  
✅ **Local Development**: Unchanged  

**The Web application will now start cleanly in Railway without telemetry connection errors!** 🚀

