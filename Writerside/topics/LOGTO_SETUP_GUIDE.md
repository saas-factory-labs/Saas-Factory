# Logto Setup Guide for AppBlueprint

This guide will walk you through setting up Logto authentication for your AppBlueprint application.

## Overview

Logto is an open-source identity infrastructure alternative to Auth0. It provides:
- ✅ **Open Source** - Self-hostable or use their cloud service
- ✅ **Developer-friendly** - Modern UI and great DX
- ✅ **Cost-effective** - Free for unlimited MAUs (Monthly Active Users) on self-hosted
- ✅ **Feature-rich** - Social sign-in, MFA, passwordless, and more

Your application is already configured to support Logto through the pluggable authentication system.

---

## Quick Start Options

You have two options for using Logto:

### Option 1: Logto Cloud (Recommended for Quick Start)
- Fastest way to get started
- Managed hosting
- Free tier available
- No infrastructure to maintain

### Option 2: Self-Hosted Logto
- Full control over your data
- Free for unlimited users
- Requires Docker or Kubernetes
- Best for production/enterprise

---

## Option 1: Logto Cloud Setup

### Step 1: Create a Logto Cloud Account

1. Go to [https://cloud.logto.io](https://cloud.logto.io)
2. Sign up for a free account
3. Create a new tenant (e.g., "appblueprint-dev")
4. Your tenant URL will be something like: `https://abc123.logto.app`

### Step 2: Create an Application

1. In the Logto Console, go to **Applications**
2. Click **Create Application**
3. Select **Traditional Web App**
4. Enter a name: "AppBlueprint Web"
5. Click **Create Application**

### Step 3: Configure Application Settings

In your application settings:

1. **Redirect URIs**: Add your callback URLs
   ```
   https://localhost:443/callback
   http://localhost:8092/callback
   ```

2. **Post Sign-out Redirect URIs**: Add your logout URLs
   ```
   https://localhost:443
   http://localhost:8092
   ```

3. **CORS Allowed Origins**: Add your application URLs
   ```
   https://localhost:443
   http://localhost:8092
   ```

4. **Save changes**

### Step 4: Get Your Credentials

From the application details page, copy:
- **App ID** (this is your Client ID)
- **App Secret** (this is your Client Secret)
- **Endpoint** (your tenant URL, e.g., `https://abc123.logto.app`)

### Step 5: Enable Resource Owner Password Credentials (ROPC) Flow

⚠️ **Important**: By default, Logto may not enable the password grant type for security reasons.

1. In your application settings, find **Grant Types**
2. Enable **Resource Owner Password Credentials (ROPC)**
3. Save changes

> **Note**: For production, consider using Authorization Code Flow with PKCE instead of ROPC for better security.

### Step 6: Create Test Users

1. Go to **User Management** in the Logto Console
2. Click **Create User**
3. Enter:
   - **Username**: testuser
   - **Email**: test@example.com
   - **Password**: Choose a strong password
4. Click **Create**

### Step 7: Configure Your Application

Update `Code/AppBlueprint/AppBlueprint.Web/appsettings.Development.json`:

```json
{
  "DetailedErrors": true,
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Debug",
      "Microsoft.Hosting.Lifetime": "Information",
      "AppBlueprint.Infrastructure.Authorization": "Debug"
    }
  },
  "Authentication": {
    "Provider": "Logto",
    "Logto": {
      "Endpoint": "https://YOUR-TENANT-ID.logto.app",
      "ClientId": "YOUR-APP-ID",
      "ClientSecret": "YOUR-APP-SECRET",
      "Scope": "openid profile email"
    }
  }
}
```

**Replace:**
- `YOUR-TENANT-ID` with your actual tenant ID (e.g., `abc123`)
- `YOUR-APP-ID` with your App ID
- `YOUR-APP-SECRET` with your App Secret

### Step 8: Test Authentication

1. Start your application
2. Navigate to the login page
3. Use your test user credentials
4. You should be authenticated through Logto!

---

## Option 2: Self-Hosted Logto Setup

### Step 1: Install Logto Using Docker Compose

Create a `docker-compose.yml` file for Logto:

```yaml
version: '3.9'

services:
  logto:
    image: svhd/logto:latest
    container_name: logto
    ports:
      - "3001:3001"
      - "3002:3002"
    environment:
      - TRUST_PROXY_HEADER=1
      - DB_URL=postgresql://postgres:password@postgres:5432/logto
      - ENDPOINT=http://localhost:3001
      - ADMIN_ENDPOINT=http://localhost:3002
    depends_on:
      postgres:
        condition: service_healthy

  postgres:
    image: postgres:14-alpine
    container_name: logto_postgres
    environment:
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: password
      POSTGRES_DB: logto
    volumes:
      - logto_data:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 10s
      timeout: 5s
      retries: 5

volumes:
  logto_data:
```

### Step 2: Start Logto

```powershell
docker-compose up -d
```

### Step 3: Access Logto Admin Console

1. Open your browser to `http://localhost:3002`
2. Create an admin account
3. Complete the initial setup wizard

### Step 4: Configure Application

Follow steps 2-8 from Option 1, but use:
- **Endpoint**: `http://localhost:3001`

---

## Using User Secrets (Recommended for Development)

Store your credentials securely using .NET User Secrets:

```powershell
cd Code\AppBlueprint\AppBlueprint.Web

dotnet user-secrets set "Authentication:Logto:Endpoint" "https://your-tenant.logto.app"
dotnet user-secrets set "Authentication:Logto:ClientId" "your-app-id"
dotnet user-secrets set "Authentication:Logto:ClientSecret" "your-app-secret"
dotnet user-secrets set "Authentication:Logto:Scope" "openid profile email"
dotnet user-secrets set "Authentication:Provider" "Logto"
```

This keeps your credentials secure and out of source control.

---

## Production Configuration

### Environment Variables

```
Authentication__Provider=Logto
Authentication__Logto__Endpoint=https://your-production-tenant.logto.app
Authentication__Logto__ClientId=your-app-id
Authentication__Logto__ClientSecret=your-app-secret
Authentication__Logto__Scope=openid profile email offline_access
```

### Azure App Service Configuration

Add these in **Configuration** → **Application settings**:
- `Authentication:Provider` = `Logto`
- `Authentication:Logto:Endpoint` = `https://your-tenant.logto.app`
- `Authentication:Logto:ClientId` = `your-app-id`
- `Authentication:Logto:ClientSecret` = `your-app-secret`
- `Authentication:Logto:Scope` = `openid profile email offline_access`

---

## Advanced Features

### Social Sign-In

Logto supports multiple social connectors:

1. In Logto Console, go to **Connectors**
2. Add connectors for:
   - Google
   - GitHub
   - Microsoft
   - Facebook
   - Apple
   - And many more...

### Passwordless Authentication

Enable passwordless options:
- Email verification codes
- SMS verification codes
- Magic links

### Multi-Factor Authentication (MFA)

1. Go to **Sign-in Experience** → **Multi-factor Auth**
2. Enable TOTP (Time-based One-Time Password)
3. Configure backup codes

### Custom Branding

1. Go to **Sign-in Experience** → **Branding**
2. Customize:
   - Logo
   - Primary color
   - Dark mode
   - Custom CSS

### Custom Scopes and Claims

Add custom user attributes:

1. Go to **User Management** → **User Attributes**
2. Add custom attributes
3. Include them in token claims
4. Update your scope configuration:

```json
{
  "Authentication": {
    "Logto": {
      "Scope": "openid profile email custom:organization custom:role"
    }
  }
}
```

---

## Switching Between Providers

Your application makes it easy to switch authentication providers:

### Use Mock Provider (Development/Testing)
```json
{
  "Authentication": {
    "Provider": "Mock"
  }
}
```

### Use Logto
```json
{
  "Authentication": {
    "Provider": "Logto",
    "Logto": {
      "Endpoint": "https://your-tenant.logto.app",
      "ClientId": "your-app-id",
      "ClientSecret": "your-app-secret",
      "Scope": "openid profile email"
    }
  }
}
```

### Use Auth0
```json
{
  "Authentication": {
    "Provider": "Auth0",
    "Auth0": {
      "Domain": "https://your-domain.auth0.com",
      "ClientId": "your-client-id",
      "ClientSecret": "your-client-secret",
      "Audience": "https://your-api"
    }
  }
}
```

---

## Troubleshooting

### Authentication Fails

1. **Check logs**: Look for errors in console output with debug logging enabled
2. **Verify credentials**: Ensure Endpoint, ClientId, and ClientSecret are correct
3. **Check ROPC grant**: Verify Resource Owner Password Credentials is enabled
4. **Test in Logto Console**: Try the built-in test feature

### "Grant type not supported" Error

- Enable **Resource Owner Password Credentials (ROPC)** in your application settings
- For production, consider implementing Authorization Code Flow instead

### CORS Issues

- Add your application URLs to **CORS Allowed Origins** in Logto
- Ensure the protocol (http/https) matches exactly

### Token Refresh Issues

- Include `offline_access` in your scope to get refresh tokens
- Check token expiration settings in Logto

### Network/Connection Issues

- Verify your application can reach the Logto endpoint
- Check firewall rules and network configuration
- For self-hosted: Ensure Logto containers are running (`docker ps`)

### Self-Hosted Database Issues

```powershell
# Check Logto logs
docker logs logto

# Check PostgreSQL logs
docker logs logto_postgres

# Restart services
docker-compose restart
```

---

## Code Usage

Your existing code works without any changes:

```csharp
@inject IUserAuthenticationProvider AuthProvider

// Login
var success = await AuthProvider.LoginAsync(email, password);
if (success)
{
    // User is authenticated
    NavigationManager.NavigateTo("/dashboard");
}

// Check authentication status
var isAuthenticated = AuthProvider.IsAuthenticated();

// Logout
await AuthProvider.LogoutAsync();
```

---

## Logto vs Auth0 Comparison

| Feature | Logto | Auth0 |
|---------|-------|-------|
| **Pricing** | Free (self-hosted) or Cloud | Free tier (7,000 MAU) |
| **Open Source** | ✅ Yes | ❌ No |
| **Self-Hosting** | ✅ Yes | ❌ No |
| **Social Sign-in** | ✅ Yes | ✅ Yes |
| **MFA** | ✅ Yes | ✅ Yes |
| **Passwordless** | ✅ Yes | ✅ Yes |
| **Custom UI** | ✅ Fully customizable | ⚠️ Limited on free tier |
| **API Support** | ✅ Full REST API | ✅ Full REST API |
| **Community** | Growing | Established |
| **Enterprise Support** | Available | Available |

---

## Migration from Auth0 to Logto

If you're migrating from Auth0:

### 1. Export Users from Auth0
- Use Auth0 Management API to export users
- Export includes hashed passwords (if using Auth0 database)

### 2. Import Users to Logto
- Use Logto Management API
- Script the migration process
- Consider a gradual migration strategy

### 3. Update Configuration
- Simply change `"Provider": "Logto"` in your configuration
- Update the provider-specific settings
- No code changes required!

### 4. Test Thoroughly
- Verify all authentication flows work
- Test token refresh
- Validate user attributes/claims

---

## Production Deployment Considerations

### For Self-Hosted Logto

1. **Use PostgreSQL with backups**
   - Configure automated backups
   - Use managed PostgreSQL service (AWS RDS, Azure Database, etc.)

2. **Use HTTPS**
   - Configure SSL certificates
   - Use reverse proxy (nginx, Traefik)

3. **Scale horizontally**
   - Run multiple Logto instances
   - Use load balancer

4. **Monitor and log**
   - Set up application monitoring
   - Configure log aggregation
   - Set up alerts

### Example Production Docker Compose

```yaml
version: '3.9'

services:
  logto:
    image: svhd/logto:latest
    deploy:
      replicas: 2
      resources:
        limits:
          cpus: '2'
          memory: 2G
    environment:
      - TRUST_PROXY_HEADER=1
      - DB_URL=postgresql://user:pass@your-managed-postgres:5432/logto
      - ENDPOINT=https://auth.yourdomain.com
      - ADMIN_ENDPOINT=https://admin.yourdomain.com
    networks:
      - logto_network

  nginx:
    image: nginx:alpine
    ports:
      - "443:443"
      - "80:80"
    volumes:
      - ./nginx.conf:/etc/nginx/nginx.conf
      - ./certs:/etc/nginx/certs
    depends_on:
      - logto
    networks:
      - logto_network

networks:
  logto_network:
```

---

## Kubernetes Deployment

Logto can be deployed to Kubernetes:

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: logto
spec:
  replicas: 3
  selector:
    matchLabels:
      app: logto
  template:
    metadata:
      labels:
        app: logto
    spec:
      containers:
      - name: logto
        image: svhd/logto:latest
        env:
        - name: DB_URL
          valueFrom:
            secretKeyRef:
              name: logto-secrets
              key: database-url
        - name: ENDPOINT
          value: "https://auth.yourdomain.com"
        ports:
        - containerPort: 3001
        - containerPort: 3002
---
apiVersion: v1
kind: Service
metadata:
  name: logto-service
spec:
  selector:
    app: logto
  ports:
  - name: app
    port: 3001
  - name: admin
    port: 3002
  type: LoadBalancer
```

---

## Additional Resources

### Official Logto Resources
- [Logto Documentation](https://docs.logto.io)
- [Logto GitHub Repository](https://github.com/logto-io/logto)
- [Logto Discord Community](https://discord.gg/logto)
- [Logto Cloud](https://cloud.logto.io)

### Integration Guides
- [Logto API Reference](https://docs.logto.io/api)
- [SDK Documentation](https://docs.logto.io/sdk)
- [Connector Documentation](https://docs.logto.io/connectors)

### Video Tutorials
- [Getting Started with Logto](https://www.youtube.com/channel/UCQtX8VvYnJvFM3RxmVvYOOw)

---

## Getting Help

### Community Support
- Join the [Logto Discord](https://discord.gg/logto)
- GitHub Issues: [Report bugs or request features](https://github.com/logto-io/logto/issues)
- Stack Overflow: Tag your questions with `logto`

### Enterprise Support
- Contact Logto for enterprise support plans
- Available for both cloud and self-hosted deployments

---

## Security Best Practices

1. **Use HTTPS in production** - Always use TLS/SSL
2. **Rotate secrets regularly** - Change ClientSecret periodically
3. **Enable MFA** - Require multi-factor authentication for sensitive operations
4. **Implement rate limiting** - Protect against brute force attacks
5. **Monitor authentication logs** - Track suspicious activity
6. **Use short-lived tokens** - Configure appropriate token expiration
7. **Implement refresh token rotation** - Enhance security for long-lived sessions
8. **Validate redirect URIs** - Only allow trusted callback URLs

---

## Next Steps

1. ✅ Choose between Logto Cloud or Self-Hosted
2. ✅ Follow the setup steps above
3. ✅ Update your `appsettings.Development.json`
4. ✅ Create test users
5. ✅ Test authentication in your application
6. ✅ Explore advanced features (social login, MFA, etc.)
7. ✅ Plan your production deployment

---

**Last Updated**: 2025-10-30

**Questions?** Check the [Authorization README](Code/AppBlueprint/Shared-Modules/AppBlueprint.Infrastructure/Authorization/README.md) or open an issue!

