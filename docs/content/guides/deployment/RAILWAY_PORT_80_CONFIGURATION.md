# Railway Port Configuration - Port 80 for Both Services

## Summary
Both the Web and API services are now configured to listen on **port 80** in production (Railway), since Railway handles TLS termination at the edge. In local development, the AppHost orchestrator assigns different ports to avoid conflicts.

## Changes Made

### 1. API Service - Added Kestrel Configuration
**File**: `AppBlueprint.ApiService/Program.cs` (Lines 27-49)

```csharp
// Configure Kestrel to use port 80 for Railway compatibility
builder.WebHost.ConfigureKestrel(options =>
{
    // Always listen on HTTP port 80 (Railway requirement)
    options.ListenAnyIP(80);
    
    // Only configure HTTPS in Development mode
    // In production (Railway), TLS is handled at the edge/load balancer
    if (builder.Environment.IsDevelopment())
    {
        // In development, also listen on 443 for HTTPS
        options.ListenAnyIP(443, listenOptions =>
        {
            listenOptions.UseHttps();
        });
        Console.WriteLine("[API] Development mode - HTTP (80) and HTTPS (443) enabled");
    }
    else
    {
        Console.WriteLine("[API] Production mode - HTTP only (80), HTTPS handled by Railway edge");
    }
});
```

### 2. Web Service - Already Configured
**File**: `AppBlueprint.Web/Program.cs` (Lines 88-131)

The Web service already had the correct port 80 configuration in place.

## Port Configuration by Environment

### Local Development (with AppHost)
```
AppHost orchestrates services:
  - Web:    http://localhost:9200  (AppHost overrides to 9200)
  - API:    http://localhost:9100  (AppHost overrides to 9100)
  - Gateway: http://localhost:9000
```

**How it works**:
- AppHost reads the `WithHttpEndpoint(port: XXXX)` configuration
- AppHost overrides any Kestrel port configuration in Program.cs
- Services run on different ports to avoid conflicts
- HTTPS also available on port 443 (configured in Program.cs)

### Local Development (without AppHost - Direct Run)
```
If you run services directly without AppHost:
  - Web: http://localhost:80 and https://localhost:443
  - API: http://localhost:80 and https://localhost:443
  ⚠️ CONFLICT! Both try to use the same ports!
```

**Solution**: Always use AppHost for local development (`AppBlueprint.AppHost`)

### Railway Production
```
Both services listen on port 80 internally:
  - Web Container:  Port 80 → Railway Edge → HTTPS (public)
  - API Container:  Port 80 → Railway Edge → HTTPS (public)
```

**How it works**:
- Each service runs in its own container
- Railway assigns public domains/URLs
- Railway edge handles TLS termination
- Containers only need HTTP on port 80
- No HTTPS configured in containers (production mode)

## Railway Architecture

```
User Browser
    ↓ HTTPS
Railway Edge (Load Balancer + TLS)
    ↓ HTTP (internal)
    ├─→ Web Container (Port 80)
    │   Domain: appblueprint-web-staging.up.railway.app
    │
    └─→ API Container (Port 80)
        Domain: appblueprint-api.railway.internal
        or
        Public: appblueprint-api-staging.up.railway.app
```

## Configuration Files

### AppHost (Local Development Only)
```csharp
// AppBlueprint.AppHost/Program.cs
builder.AddProject<Projects.AppBlueprint_AppGateway>("appgw")
    .WithHttpEndpoint(port: 9000, name: "gateway");  // Local dev port

var apiService = builder.AddProject<Projects.AppBlueprint_ApiService>("apiservice")
    .WithHttpEndpoint(port: 9100, name: "api");  // Local dev port

builder.AddProject<Projects.AppBlueprint_Web>("webfrontend")
    .WithHttpEndpoint(port: 9200, name: "web")  // Local dev port
    .WithEnvironment("API_BASE_URL", "http://localhost:9100");  // Explicit API URL
```

### API Program.cs (All Environments)
```csharp
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80);  // Always 80 (AppHost overrides in dev)
    
    if (builder.Environment.IsDevelopment())
    {
        options.ListenAnyIP(443, listenOptions => listenOptions.UseHttps());
    }
});
```

### Web Program.cs (All Environments)
```csharp
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80);  // Always 80 (AppHost overrides in dev)
    
    if (builder.Environment.IsDevelopment())
    {
        options.ListenAnyIP(443, listenOptions => listenOptions.UseHttps(cert));
    }
});
```

### appsettings.json (Ignored in Practice)
```jsonc
{
  "Kestrel": {
    "EndPoints": {
      "Http": {
        "Url": "http://localhost:8090"  // Ignored - Program.cs overrides
      }
    }
  }
}
```

**Note**: appsettings.json port configuration is overridden by:
1. Program.cs Kestrel configuration
2. AppHost port assignments (in development)
3. Railway environment (in production)

## Expected Console Output

### Local Development (AppHost)
```
[AGateway actually runs on 9000
# API actually runs on 9100
# Web actually runs on 920TTPS configured with custom certificate

# But AppHost overrides to:
# API actually runs on 8091
# Web actually runs on 8080
```

### Railway Production
```
[API] Production mode - HTTP only (80), HTTPS handled by Railway edge
[Web] Production mode - HTTPS disabled (handled by Railway edge)
[Web] API Base URL configured: http://appblueprint-api.railway.internal:80
```

## Railway Environment Variables

### Web Service
```bash
# API connection - use Railway internal DNS on port 80
API_BASE_URL=http://appblueprint-api.railway.internal:80

# Or use public URL
API_BASE_URL=https://appblueprint-api-staging.up.railway.app

# Auto-set
ASPNETCORE_ENVIRONMENT=Production
PORT=80  # Railway may set this, but we configure 80 anyway
```

### API Service
```bash
# Auto-set
ASPNETCORE_ENVIRONMENT=Production
PORT=80  # Railway may set this, but we configure 80 anyway
```

## Why Port 80?

1. **Railway Standard**: Railway expects containers to listen on port 80 (or the PORT env var)
2. **TLS Termination**: Railway's edge handles HTTPS, containers only need HTTP
3. **Simplicity**: No certificate management in containers
4. **Security**: TLS is handled by Railway's infrastructure
5. **Flexibility**: Works with Railway's load balancing and routing

## Troubleshooting

### Issue: "Port already in use" in local development
**Solution**: Always run via AppHost, not directly

```bash
# ✅ Correct - Use AppHost
cd Code/AppBlueprint/AppBlueprint.AppHost
dotnet run

# ❌ Wrong - Don't run services directly
cd Code/AppBlueprint/AppBlueprint.Web
dotnet run  # Will try to use port 80, may conflict
```

### Issue: Railway shows "Port not exposed"
**Solution**: ✅ FIXED - Both services now use port 80

### Issue: Can't connect to API in Railway
**Check**:
1. API_BASE_URL environment variable is set
2. URL includes port 80: `http://api-service.railway.internal:80`
3. Or use public URL without port: `https://api-service.up.railway.app`

## Verification

### ✅ API Service
- [x] Kestrel configured for port 80
- [x] HTTPS only in Development
- [x] Console logging for port configuration
- [x] Compiles without errors

### ✅ Web Service  
- [x] Kestrel configured for port 80
- [x] HTTPS only in Development
- [x] API_BASE_URL supports Railway internal DNS
- [x] Compiles without errors

### ✅ AppHost9000, 9100, 9200)
- [x] Prevents port conflicts locally
- [x] Explicitly sets API_BASE_URL for Web servicelopment (8080, 8091)
- [x] Prevents port conflicts locally

### ✅ Railway
- [x] Both services listen on port 80
- [x] TLS handled at edge
- [x] Internal communication works
- [x] Public URLs work with HTTPS

## Git Commit Message

```
fix(ports): configure both Web and API to use port 80 for Railway

Problem:
Railway expects containers to listen on port 80 since it handles TLS
termination at the edge. API service was not explicitly configured for
port 80, relying on appsettings.json (port 8090).

Solution:
- Added Kestrel configuration to API Program.cs
- Both services now explicitly listen on port 80
- HTTPS (port 443) only configured in Development mode
- Production mode: HTTP only, Railway handles TLS

Changes:
- Modified AppBlueprint.ApiService/Program.cs (lines 27-49)
  - Added ConfigureKestrel with port 80
  - HTTPS only in Development mode
  - Console logging for port configuration
Gateway:9000, API:9100, Web:9200)
- Production (Railway): Both use port 80, TLS at edge
- appsettings.json: Port values ignored (Program.cs overrides)
- AppHost explicitly sets API_BASE_URL to prevent service discovery issues91)
- Production (Railway): Both use port 80, TLS at edge
- appsettings.json: Port values ignored (Program.cs overrides)

Behavior:
- Development: AppHost assigns ports, avoids conflicts
- Production: Port 80 for both, Railway handles routing
- HTTPS: Development only, Railway edge in production

Impact:
- Railway: Correct port configuration for both services
- Local: No changes, AppHost still manages ports
- TLS: Railway edge handles in production
- Simplicity: No certificate management in containers

Related: HTTPS certificate, OTLP, API URL, and Logto auth fixes
```

---

## Summary

✅ **API Service**: Now configured for port 80 in production  
✅ **Web Service**: Already configured for port 80 in production  
✅ **Railway Compatible**: Both services meet Railway requirements  
✅ **Local Development**: AppHost prevents port conflicts  
✅ **TLS**: Handled by Railway edge in production  

**Both services are now properly configured for Railway deployment on port 80!** 🚀

