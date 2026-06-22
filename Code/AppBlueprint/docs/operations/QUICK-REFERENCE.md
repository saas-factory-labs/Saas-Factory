# Docker Hardening - Quick Reference Card

## 🚀 What Changed?

### Images
- **Before:** `mcr.microsoft.com/dotnet/aspnet:10.0` (Microsoft standard)
- **After:** `dhi.io/dotnet:10-aspnet` (Docker Hardened Images)
- **Size:** 50-60% smaller (220MB → 90-110MB)
- **Security:** Near-zero CVEs, 70-80% fewer packages
- **Provenance:** SLSA Level 3 with signed SBOMs

### Ports
- **Before:** Port 80 (privileged)
- **After:** Port 8080 (non-privileged)
- **Action:** Update deployment configs!

### Features
- ✅ No shell (enhanced security)
- ✅ No package manager (reduced attack surface)
- ✅ Non-root user (principle of least privilege)
- ✅ Diagnostics disabled (production-ready)

## ⚡ Quick Commands

```powershell
# Build hardened image
docker build -t myapp:hardened -f Dockerfile .

# Run container
docker run -d -p 8080:8080 --name myapp myapp:hardened

# Test health endpoint
Invoke-WebRequest http://localhost:8080/health

# Verify non-root
docker run --rm myapp:hardened id
# Output: uid=1654(app) gid=1654(app)

# Try to access shell (should fail)
docker run --rm myapp:hardened /bin/sh
# Output: Error (no shell available)
```

## 🔧 Update Deployment Configs

### Docker Compose
```yaml
services:
  web:
    ports:
      - "80:8080"  # Changed from 80:80
```

### Kubernetes
```yaml
spec:
  ports:
  - port: 80
    targetPort: 8080  # Changed from 80
```

## 📋 Checklist

- [ ] Update port mappings in deployment files
- [ ] Add health check endpoints to ASP.NET Core apps
- [ ] Test in development environment
- [ ] Update load balancer/ingress rules
- [ ] Update CI/CD pipelines
- [ ] Train team on debugging without shell

## 🆘 Troubleshooting

### Can't access shell?
**Expected!** Chiseled images have no shell. Use:
```powershell
# Copy files out for inspection
docker cp myapp:/app/appsettings.json .

# Or use a debug sidecar
docker run --rm --network container:myapp curlimages/curl sh
```

### Health checks failing?
Implement ASP.NET Core health endpoints:
```csharp
// Program.cs
app.MapHealthChecks("/health");
```

See `HEALTH-CHECK-EXAMPLES.md` for details.

### Need to debug?
For development, use standard images:
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS debug
# Full Ubuntu with shell and tools
```

## 📚 Documentation

- **Full Guide:** `DOCKER-SECURITY-HARDENING.md`
- **Health Checks:** `HEALTH-CHECK-EXAMPLES.md`
- **Summary:** `DOCKER-HARDENING-SUMMARY.md`

## 🎯 Key Benefits

| Benefit | Impact |
|---------|--------|
| **Security** | Near-zero CVEs with continuous patching |
| **Performance** | 50-60% smaller images = faster deployments |
| **Compliance** | FIPS/STIG ready, SLSA Level 3 provenance |
| **Supply Chain** | Transparent SBOMs, verified provenance |
| **Cost** | Less bandwidth, less storage, FREE |
| **Trust** | Docker-maintained, open source (Apache 2.0) |


## ⚠️ Breaking Changes

1. **Port 8080** - Update all configs
2. **No shell** - Use alternatives for debugging
3. **.NET 10** - Verify compatibility

## 🔐 Security Features

✅ Docker Hardened Images (dhi.io)
✅ Near-zero CVEs
✅ SLSA Level 3 provenance
✅ Transparent, signed SBOMs
✅ Non-root user (UID 1654)
✅ Non-privileged port (8080)
✅ Diagnostics disabled
✅ Minimal attack surface
✅ FIPS/STIG compliance ready

---

**Questions?** Check the full documentation or ask the team!
