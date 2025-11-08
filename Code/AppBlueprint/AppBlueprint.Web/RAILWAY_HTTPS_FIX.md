# Railway HTTPS Certificate Fix

## Problem
The Web application was failing to start in Railway with the error:
```
System.InvalidOperationException: Unable to configure HTTPS endpoint. 
No server certificate was specified, and the default developer certificate 
could not be found or is out of date.
```

## Root Cause
The application was trying to configure HTTPS (port 443) in production (Railway) without a valid certificate. Railway handles TLS termination at the edge/load balancer level, so the application container should only serve HTTP traffic.

## Solution
Modified `Program.cs` to conditionally configure HTTPS based on the environment:

### Before
```csharp
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80);
    // ALWAYS tried to configure HTTPS on port 443
    options.ListenAnyIP(443, listenOptions =>
    {
        // Certificate loading logic...
        listenOptions.UseHttps();
    });
});
```

### After
```csharp
builder.WebHost.ConfigureKestrel(options =>
{
    // Always listen on HTTP port 80
    options.ListenAnyIP(80);
    
    // Only configure HTTPS in Development mode
    // In production (Railway), TLS is handled at the edge/load balancer
    if (builder.Environment.IsDevelopment())
    {
        options.ListenAnyIP(443, listenOptions =>
        {
            // Certificate loading logic...
            listenOptions.UseHttps();
        });
    }
    else
    {
        Console.WriteLine("[Web] Production mode - HTTPS disabled (handled by Railway edge)");
    }
});
```

## Changes Made

### File: `Program.cs` (Lines 73-119)
- Wrapped HTTPS configuration in `if (builder.Environment.IsDevelopment())`
- Added console logging to indicate HTTPS is disabled in production
- HTTPS configuration remains unchanged for local development

## Behavior

### Development Environment
- Listens on **HTTP port 80** and **HTTPS port 443**
- Uses custom certificate if available (`web-service.pfx`)
- Falls back to default developer certificate
- Full HTTPS support for local debugging

### Production Environment (Railway)
- Listens on **HTTP port 80 ONLY**
- HTTPS is completely disabled
- Railway handles TLS termination at the edge
- Public traffic goes through Railway's load balancer with proper TLS

## Architecture

```
Client (HTTPS)
    ↓
Railway Edge (TLS Termination)
    ↓
Web Container (HTTP only, port 80)
    ↓
API Container (HTTP, port 8091)
```

## Verification

### Local Development
```bash
dotnet run
# Output should show:
# [Web] HTTPS configured with custom certificate
# OR
# [Web] HTTPS configured with default dev certificate
```

### Railway Deployment
```bash
# Application log should show:
# [Web] Production mode - HTTPS disabled (handled by Railway edge)
```

## Related Files
- `Program.cs` - Kestrel configuration
- `Dockerfile` - Exposes port 80, sets ASPNETCORE_URLS
- `RAILWAY_API_CONNECTION.md` - Updated with HTTPS notes

## Testing Checklist
- [x] Code compiles without errors
- [x] Local development still supports HTTPS
- [x] Production mode disables HTTPS configuration
- [x] Dockerfile only exposes port 80
- [x] Environment detection works correctly
- [x] Console logging indicates current mode

## Impact
- **Local Development**: No change - HTTPS works as before
- **Railway Deployment**: Fixes startup error, application now starts successfully
- **Security**: No impact - TLS still enforced at Railway edge
- **Performance**: Slight improvement - no unnecessary certificate loading in production

