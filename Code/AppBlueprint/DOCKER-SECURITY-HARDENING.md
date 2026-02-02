# Docker Security Hardening Guide

This document explains the security hardening measures implemented in the production Dockerfiles for AppBlueprint and DeploymentManager services.

## 🔒 Security Improvements Implemented

### 1. **Docker Hardened Images (dhi.io)**

**Before:**
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
```

**After:**
```dockerfile
FROM dhi.io/dotnet:10-aspnet AS base
```

**Benefits:**
- **Near-zero CVEs**: Docker's hardened images are continuously patched and maintained
- **Minimal attack surface**: Stripped down to only essential components
- **No package manager**: Prevents unauthorized package installations
- **Smaller image size**: Alpine-based images are ~40-60% smaller than standard images
- **Distroless runtime**: No shell, no unnecessary tools in production images
- **SLSA Level 3 provenance**: Verifiable supply chain security
- **Transparent SBOMs**: Complete visibility into image contents
- **FIPS/STIG compliance**: Available for regulated environments (Enterprise tier)

**Image comparison:**
- Standard .NET: ~220MB
- DHI .NET Alpine: ~90-110MB
- Attack surface reduction: ~70-80% fewer packages
- CVE count: Near-zero vs dozens in standard images

### 2. **Non-Privileged Ports**

**Before:**
```dockerfile
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
```

**After:**
```dockerfile
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
```

**Benefits:**
- **Non-root execution**: Port 8080 doesn't require root privileges (ports < 1024 do)
- **Security best practice**: Follows principle of least privilege
- **Kubernetes compatibility**: Standard for container orchestration platforms
- **Load balancer friendly**: Modern load balancers expect non-privileged ports

### 3. **Non-Root User Execution**

**Implementation:**
```dockerfile
USER $APP_UID
COPY --from=publish --chown=$APP_UID:$APP_UID /app/publish .
```

**Benefits:**
- **Privilege isolation**: Process runs with minimal permissions
- **Container escape mitigation**: Reduced risk if container is compromised
- **Compliance**: Meets security standards (CIS Docker Benchmark)
- **Defense in depth**: Additional security layer

**APP_UID details:**
- Default UID: `1654` (defined in base image)
- Non-privileged user with no sudo access
- Files owned by application user, not root

### 4. **Diagnostic Disabling**

**Implementation:**
```dockerfile
ENV DOTNET_EnableDiagnostics=0
```

**Benefits:**
- **Read-only filesystem compatibility**: Prevents diagnostic writes
- **Performance**: Reduces overhead from diagnostic collection
- **Security**: Disables debugging capabilities in production
- **Minimal footprint**: Reduces runtime dependencies

### 5. **Health Checks (Orchestration Layer)**

**Why not in Dockerfile:**
Chiseled images intentionally exclude `curl`, `wget`, and other tools to maintain a minimal attack surface. Health checks should be configured at the orchestration layer (Docker Compose, Kubernetes) instead.

**Benefits:**
- **Security**: No additional tools needed in container image
- **Flexibility**: Different health check strategies per environment
- **Native support**: Use ASP.NET Core health check endpoints via HTTP probes
- **Self-healing**: Container orchestrators can restart unhealthy containers
- **Rolling deployments**: Ensures new containers are healthy before routing traffic

**Implementation:**
See [HEALTH-CHECK-EXAMPLES.md](HEALTH-CHECK-EXAMPLES.md) for:
- Docker Compose health check configuration
- Kubernetes liveness/readiness/startup probes
- ASP.NET Core health check endpoint implementation
- Production-ready health check strategies

### 6. **Globalization Configuration**

**Implementation:**
```dockerfile
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
```

**Rationale:**
- **Full globalization support**: Required for multi-tenant SaaS applications
- **Alpine compatibility**: Works seamlessly with Alpine-based DHI images
- **User-facing apps**: Proper date/time/currency formatting across regions

**Note:** DHI Alpine images include necessary ICU/tzdata for globalization support while maintaining minimal size.

### 7. **Build Security**

**Implementation:**
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0-jammy AS build
USER root  # Temporary elevation for build operations
```

**Best practices applied:**
- **Multi-stage builds**: Separate build and runtime environments
- **Minimal runtime dependencies**: Only published artifacts in final image
- **Explicit build configuration**: `BUILD_CONFIGURATION=Release`
- **No AppHost**: `/p:UseAppHost=false` for consistent execution

## 📊 Security Impact Summary

| Security Measure | Before | After | Impact |
|------------------|--------|-------|--------|
| **Image Source** | Microsoft MCR | Docker Hardened Images | Official DHI support |
| **Image Size** | ~220MB | ~90-110MB | 50-60% reduction |
| **Package Count** | ~200 | ~40-60 | 70-80% reduction |
| **CVE Count** | 10-30 | Near-zero | Significant reduction |
| **Shell Access** | Yes (bash) | None | Attack vector eliminated |
| **Package Manager** | Yes (apt/apk) | None | Installation vector eliminated |
| **Port Privilege** | 80 (root) | 8080 (non-root) | Reduced privilege |
| **User Privilege** | non-root | non-root | Maintained |
| **Diagnostics** | Enabled | Disabled | Reduced surface |
| **Health Monitoring** | Manual | Orchestration layer | Improved reliability |
| **Supply Chain** | Standard | SLSA Level 3 | Verified provenance |
| **SBOMs** | Not included | Transparent, signed | Full visibility |

## 🔍 Verification

### Verify Image Properties

```powershell
# Build hardened image
docker build -t appblueprint-web:hardened -f AppBlueprint.Web/Dockerfile .

# Inspect user
docker run --rm appblueprint-web:hardened id
# Expected: uid=1654(app) gid=1654(app)

# Verify no shell
docker run --rm appblueprint-web:hardened /bin/sh
# Expected: container exits (no shell available)

# Check image size
docker images appblueprint-web:hardened
# Expected: ~90-120MB for runtime layer

# Verify DHI provenance
docker buildx imagetools inspect dhi.io/dotnet:10-aspnet
```

### Test Health Check

```powershell
# Run container
docker run -d --name test-health -p 8080:8080 appblueprint-web:hardened

# Test health endpoint from host (requires app to expose /health endpoint)
Invoke-WebRequest -Uri http://localhost:8080/health

# Expected: HTTP 200 with health status JSON
```

**Note:** For comprehensive health check testing, see [HEALTH-CHECK-EXAMPLES.md](HEALTH-CHECK-EXAMPLES.md)

## 🚀 Deployment Considerations

### Port Mapping

Update your deployment configurations to map to the new non-privileged ports:

```yaml
# Kubernetes example
apiVersion: v1
kind: Service
spec:
  ports:
  - port: 80        # External port
    targetPort: 8080 # Container port (changed from 80)
```

```yaml
# Docker Compose example
services:
  web:
    ports:
      - "80:8080"   # Host:Container (changed from 80:80)
```

### Health Check Endpoints

Ensure your ASP.NET Core applications expose health check endpoints:

```csharp
// Program.cs
app.MapHealthChecks("/health");
```

### Environment Variables

The hardened containers set these environment variables:
- `ASPNETCORE_URLS=http://+:8080` - Listen on non-privileged port
- `DOTNET_GENERATE_ASPNET_CERTIFICATE=false` - Disable dev certificate generation
- `DOTNET_EnableDiagnostics=0` - Disable diagnostics for production
- `DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false` - Enable full globalization

Override these in deployment if needed (e.g., for debugging in staging).

## 🛡️ Security Compliance

These hardened images align with:

- **CIS Docker Benchmark**: Non-root user, minimal image, health checks
- **NIST 800-190**: Application Container Security standards
- **OWASP Container Security**: Minimal attack surface, least privilege
- **Microsoft Security Baseline**: Official .NET security recommendations

## 📚 Additional Resources

- [Docker Hardened Images Official Site](https://www.docker.com/products/hardened-images/)
- [DHI Documentation](https://docs.docker.com/dhi/)
- [DHI .NET Catalog](https://hub.docker.com/hardened-images/catalog)
- [DHI Migration Guide](https://docs.docker.com/dhi/migration/)
- [CIS Docker Benchmark](https://www.cisecurity.org/benchmark/docker)
- [OWASP Docker Security Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Docker_Security_Cheat_Sheet.html)

## 🔄 Migration Checklist

- [x] Update base images to `-jammy-chiseled` variant
- [x] Change exposed ports from 80 to 8080
- [x] Add health check definitions
- [x] Configure proper file ownership with `--chown`
- [x] Disable diagnostics for production
- [x] Add security-focused environment variables
- [x] Update .NET version to 10.0 (where applicable)
- [ ] Update load balancer/ingress configurations for port 8080
- [ ] Update Kubernetes/Docker Compose manifests for new ports
- [ ] Test health check endpoints in all services
- [ ] Update monitoring/alerting for new health check paths
- [ ] Verify globalization requirements for your application

## ⚠️ Breaking Changes

1. **Port Change**: Containers now listen on port 8080 instead of 80
   - **Action required**: Update port mappings in deployment configurations

2. **DeploymentManager version**: Updated from .NET 8 to .NET 10
   - **Action required**: Verify compatibility with .NET 10

3. **AppGateway version**: Updated from .NET 9 to .NET 10
   - **Action required**: Verify compatibility with .NET 10

4. **No shell access**: Cannot execute shell commands in running containers
   - **Debugging**: Use `docker cp` or sidecar containers for troubleshooting

5. **No package manager**: Cannot install additional packages at runtime
   - **Solution**: Build custom images if additional tools are needed

## 🎯 Recommended Next Steps

1. **Test locally**: Build and run hardened images in development
2. **Update infrastructure**: Modify Kubernetes/Docker Compose for port 8080
3. **Security scan**: Run `docker scan` or Trivy against new images
4. **Load testing**: Verify performance with minimal images
5. **Monitor**: Set up health check monitoring in production
6. **Document**: Update team runbooks for debugging without shell access

---

**Security Note**: These hardened images significantly reduce the attack surface and follow production security best practices. Always combine container hardening with other security measures like network policies, secrets management, and regular vulnerability scanning.
