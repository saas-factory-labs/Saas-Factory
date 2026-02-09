# Health Check Configuration Examples

Since chiseled (distroless) images don't include `curl` or `wget` to maintain a minimal attack surface, health checks should be configured at the orchestration layer.

## Docker Compose Example

```yaml
version: '3.8'

services:
  web:
    image: appblueprint-web:latest
    ports:
      - "80:8080"
    healthcheck:
      test: ["CMD-SHELL", "dotnet --info > /dev/null 2>&1 || exit 1"]
      interval: 30s
      timeout: 3s
      retries: 3
      start_period: 10s
    environment:
      - ASPNETCORE_URLS=http://+:8080
      - ASPNETCORE_ENVIRONMENT=Production

  apiservice:
    image: appblueprint-apiservice:latest
    ports:
      - "8080:8080"
    healthcheck:
      test: ["CMD-SHELL", "dotnet --info > /dev/null 2>&1 || exit 1"]
      interval: 30s
      timeout: 3s
      retries: 3
      start_period: 10s
    depends_on:
      - postgres
    environment:
      - ASPNETCORE_URLS=http://+:8080
      - ASPNETCORE_ENVIRONMENT=Production

  gateway:
    image: appblueprint-gateway:latest
    ports:
      - "8081:8080"
    healthcheck:
      test: ["CMD-SHELL", "dotnet --info > /dev/null 2>&1 || exit 1"]
      interval: 30s
      timeout: 3s
      retries: 3
      start_period: 10s
    environment:
      - ASPNETCORE_URLS=http://+:8080
      - ASPNETCORE_ENVIRONMENT=Production

  postgres:
    image: postgres:16-alpine
    environment:
      POSTGRES_PASSWORD: example
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 10s
      timeout: 5s
      retries: 5
```

**Note:** The `dotnet --info` command is a simple check that the .NET runtime is available. For production, you should implement proper ASP.NET Core health check endpoints (see below).

## Kubernetes Example

### Deployment with Liveness and Readiness Probes

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: appblueprint-web
  namespace: production
spec:
  replicas: 3
  selector:
    matchLabels:
      app: appblueprint-web
  template:
    metadata:
      labels:
        app: appblueprint-web
    spec:
      securityContext:
        runAsNonRoot: true
        runAsUser: 1654
        fsGroup: 1654
        seccompProfile:
          type: RuntimeDefault
      containers:
      - name: web
        image: appblueprint-web:latest
        imagePullPolicy: Always
        ports:
        - containerPort: 8080
          name: http
          protocol: TCP
        
        # Liveness probe - restart container if this fails
        livenessProbe:
          httpGet:
            path: /health/live
            port: 8080
            scheme: HTTP
          initialDelaySeconds: 10
          periodSeconds: 30
          timeoutSeconds: 3
          successThreshold: 1
          failureThreshold: 3
        
        # Readiness probe - remove from service endpoints if this fails
        readinessProbe:
          httpGet:
            path: /health/ready
            port: 8080
            scheme: HTTP
          initialDelaySeconds: 5
          periodSeconds: 10
          timeoutSeconds: 3
          successThreshold: 1
          failureThreshold: 3
        
        # Startup probe - gives app time to start before liveness kicks in
        startupProbe:
          httpGet:
            path: /health/startup
            port: 8080
            scheme: HTTP
          initialDelaySeconds: 0
          periodSeconds: 5
          timeoutSeconds: 3
          successThreshold: 1
          failureThreshold: 12  # 12 * 5s = 60s max startup time
        
        resources:
          requests:
            cpu: 100m
            memory: 128Mi
          limits:
            cpu: 500m
            memory: 512Mi
        
        env:
        - name: ASPNETCORE_URLS
          value: "http://+:8080"
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        
        securityContext:
          allowPrivilegeEscalation: false
          readOnlyRootFilesystem: true
          runAsNonRoot: true
          runAsUser: 1654
          capabilities:
            drop:
            - ALL
        
        volumeMounts:
        - name: tmp
          mountPath: /tmp
        - name: aspnet-temp
          mountPath: /app/.aspnet
      
      volumes:
      - name: tmp
        emptyDir: {}
      - name: aspnet-temp
        emptyDir: {}
---
apiVersion: v1
kind: Service
metadata:
  name: appblueprint-web
  namespace: production
spec:
  type: ClusterIP
  selector:
    app: appblueprint-web
  ports:
  - port: 80
    targetPort: 8080
    protocol: TCP
    name: http
```

## ASP.NET Core Health Check Implementation

Add health check endpoints to your ASP.NET Core applications:

### Program.cs

```csharp
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add health checks
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("Service is running"))
    .AddNpgSql(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "postgres",
        failureStatus: HealthStatus.Degraded,
        tags: new[] { "db", "postgres" })
    .AddUrlGroup(
        new Uri("http://apiservice:8080/health/ready"),
        name: "apiservice",
        failureStatus: HealthStatus.Degraded,
        tags: new[] { "api" });

var app = builder.Build();

// Map health check endpoints
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new
        {
            status = report.Status.ToString(),
            duration = report.TotalDuration,
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                duration = e.Value.Duration
            })
        });
    }
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live") || check.Name == "self"
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => true  // All checks must pass
});

app.MapHealthChecks("/health/startup", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("startup") || check.Name == "self"
});

app.Run();
```

### NuGet Package Required

```xml
<PackageReference Include="AspNetCore.HealthChecks.NpgSql" Version="8.0.0" />
<PackageReference Include="AspNetCore.HealthChecks.Uris" Version="8.0.0" />
```

## Health Check Endpoints Explained

| Endpoint | Purpose | Kubernetes Probe |
|----------|---------|------------------|
| `/health` | Overall health status with details | None (monitoring/metrics) |
| `/health/live` | Is the app alive? (restart if fails) | livenessProbe |
| `/health/ready` | Is the app ready for traffic? | readinessProbe |
| `/health/startup` | Has the app finished starting? | startupProbe |


### Probe Strategy

1. **Startup Probe**: Protects slow-starting apps from being killed during initialization
   - Only runs at container startup
   - Once succeeds, never runs again
   - Example checks: database migrations, config loading

2. **Liveness Probe**: Detects deadlocks and unrecoverable states
   - Should be simple and fast
   - Only checks if app process is responsive
   - Example checks: basic HTTP response, thread pool not exhausted

3. **Readiness Probe**: Determines if app can handle requests
   - Can fail temporarily (e.g., during deployments of dependencies)
   - Removes pod from service endpoints when failing
   - Example checks: database connectivity, external API availability

## Alternative: HTTP Probe from Sidecar

If you need more sophisticated health checks with curl/wget, use a sidecar container:

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: appblueprint-web
spec:
  template:
    spec:
      containers:
      - name: web
        image: appblueprint-web:latest
        ports:
        - containerPort: 8080
      
      # Sidecar for health checks
      - name: health-checker
        image: curlimages/curl:latest
        command: 
        - sh
        - -c
        - |
          while true; do
            if curl -f http://localhost:8080/health/ready; then
              echo "Health check passed"
            else
              echo "Health check failed"
            fi
            sleep 10
          done
```

**Note:** Sidecars add complexity and resource overhead. Prefer Kubernetes HTTP probes when possible.

## Testing Health Checks Locally

```powershell
# Start container
docker run -d -p 8080:8080 --name test-app appblueprint-web:latest

# Test health endpoint (from host)
Invoke-WebRequest -Uri http://localhost:8080/health

# Check Docker health status (if healthcheck defined in docker-compose)
docker ps --filter "name=test-app" --format "table {{.Names}}\t{{.Status}}"

# View detailed health information
docker inspect test-app | ConvertFrom-Json | Select-Object -ExpandProperty State | Select-Object -ExpandProperty Health
```

## Production Recommendations

1. **Always implement proper health check endpoints** in your ASP.NET Core apps
2. **Use distinct probes** for startup, liveness, and readiness
3. **Keep liveness checks simple** - only verify the app can respond
4. **Make readiness checks comprehensive** - verify all dependencies
5. **Set appropriate timeouts** - balance responsiveness vs. false positives
6. **Monitor health check failures** - alert on patterns, not individual failures
7. **Test failure scenarios** - ensure probes behave correctly during outages

## Security Note

Health check endpoints should:
- **Not expose sensitive information** (connection strings, secrets)
- **Not be authenticated** (probes can't handle auth)
- **Be accessible only from cluster** (use NetworkPolicies)
- **Rate limit if exposed** (prevent DoS on health endpoints)
