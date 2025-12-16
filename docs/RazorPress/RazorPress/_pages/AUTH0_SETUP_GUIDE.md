# Auth0 Setup Guide for AppBlueprint

This guide will walk you through setting up Auth0 authentication for your AppBlueprint application.

## Overview

Your application now uses a **pluggable authentication system** that supports multiple providers:
- **Mock Provider** (default for development - accepts any credentials)
- **Auth0 Provider** (enterprise authentication)
- **Logto Provider** (open-source authentication)

The system automatically selects the provider based on your `appsettings.json` configuration.

---

## Quick Start - Current Configuration

Your application is currently configured to use the **Mock Provider** for development, which allows you to test authentication without setting up an external service.

To test with Mock Provider:
1. Use any email and password combination
2. The system will accept any non-empty credentials
3. A mock JWT token will be generated

---

## Setting Up Auth0

### Step 1: Create an Auth0 Account

1. Go to [https://auth0.com](https://auth0.com)
2. Sign up for a free account (supports up to 7,000 active users)
3. Complete the onboarding process

### Step 2: Create an Application

1. In the Auth0 Dashboard, go to **Applications** → **Applications**
2. Click **Create Application**
3. Choose a name (e.g., "AppBlueprint Web")
4. Select **Regular Web Applications**
5. Click **Create**

### Step 3: Configure Application Settings

In your new application's settings:

1. **Allowed Callback URLs**: Add your application URLs
   ```
   https://localhost:443/callback,
   http://localhost:8092/callback
   ```

2. **Allowed Logout URLs**: Add your application URLs
   ```
   https://localhost:443,
   http://localhost:8092
   ```

3. **Allowed Web Origins**: Add your application URLs
   ```
   https://localhost:443,
   http://localhost:8092
   ```

4. **Save Changes**

### Step 4: Get Your Credentials

From the application's **Settings** tab, copy:
- **Domain** (e.g., `dev-abc123.us.auth0.com`)
- **Client ID** (e.g., `xYz123AbC456DeF789`)
- **Client Secret** (click "Show" to reveal)

### Step 5: Create an API

1. Go to **Applications** → **APIs**
2. Click **Create API**
3. Enter:
   - **Name**: AppBlueprint API
   - **Identifier**: `https://api.appblueprint.local` (this will be your Audience)
   - **Signing Algorithm**: RS256
4. Click **Create**

### Step 6: Configure Your Application

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
    "Provider": "Auth0",
    "Auth0": {
      "Domain": "https://YOUR-AUTH0-DOMAIN.auth0.com",
      "ClientId": "YOUR-CLIENT-ID",
      "ClientSecret": "YOUR-CLIENT-SECRET",
      "Audience": "https://api.appblueprint.local"
    }
  }
}
```

**Replace:**
- `YOUR-AUTH0-DOMAIN` with your Auth0 domain (e.g., `dev-abc123.us.auth0.com`)
- `YOUR-CLIENT-ID` with your Client ID
- `YOUR-CLIENT-SECRET` with your Client Secret
- The `Audience` with your API identifier

### Step 7: Create Test Users

1. Go to **User Management** → **Users**
2. Click **Create User**
3. Enter email and password
4. Click **Create**

### Step 8: Test Authentication

1. Start your application
2. Navigate to the login page
3. Use the test user credentials you created
4. You should be authenticated through Auth0!

---

## Using User Secrets (Recommended for Development)

Instead of storing secrets in `appsettings.Development.json`, use User Secrets:

### PowerShell Commands:

```powershell
cd Code\AppBlueprint\AppBlueprint.Web

dotnet user-secrets set "Authentication:Auth0:Domain" "https://your-domain.auth0.com"
dotnet user-secrets set "Authentication:Auth0:ClientId" "your-client-id"
dotnet user-secrets set "Authentication:Auth0:ClientSecret" "your-client-secret"
dotnet user-secrets set "Authentication:Auth0:Audience" "https://api.appblueprint.local"
dotnet user-secrets set "Authentication:Provider" "Auth0"
```

This keeps your credentials secure and out of source control.

---

## Production Configuration

For production, use environment variables or Azure Key Vault:

### Environment Variables:

```
Authentication__Provider=Auth0
Authentication__Auth0__Domain=https://your-domain.auth0.com
Authentication__Auth0__ClientId=your-client-id
Authentication__Auth0__ClientSecret=your-client-secret
Authentication__Auth0__Audience=https://api.appblueprint.com
```

### Azure App Service Configuration:

Add these in **Configuration** → **Application settings**:
- `Authentication:Provider` = `Auth0`
- `Authentication:Auth0:Domain` = `https://your-domain.auth0.com`
- `Authentication:Auth0:ClientId` = `your-client-id`
- `Authentication:Auth0:ClientSecret` = `your-client-secret`
- `Authentication:Auth0:Audience` = `https://api.appblueprint.com`

---

## Switching Between Providers

### Use Mock Provider (Development/Testing):
```json
{
  "Authentication": {
    "Provider": "Mock"
  }
}
```

### Use Auth0 (Production):
```json
{
  "Authentication": {
    "Provider": "Auth0",
    "Auth0": {
      // ... configuration
    }
  }
}
```

### Use Logto (Alternative):
```json
{
  "Authentication": {
    "Provider": "Logto",
    "Logto": {
      "Endpoint": "https://your-logto-instance.logto.app",
      "ClientId": "your-client-id",
      "ClientSecret": "your-client-secret",
      "Scope": "openid profile email"
    }
  }
}
```

---

## Troubleshooting

### Authentication Fails

1. **Check logs**: Look for authentication errors in the console output
2. **Verify credentials**: Ensure Domain, ClientId, ClientSecret, and Audience are correct
3. **Test in Auth0 Dashboard**: Try logging in through Auth0's test login feature
4. **Check callback URLs**: Ensure your application URLs are in Auth0's allowed lists

### "Invalid authentication provider" Error

- Check that `Provider` is set to exactly `"Auth0"` (case-sensitive)
- Valid values: `Mock`, `Auth0`, `Logto`

### Token Issues

- Verify the Audience matches your API identifier in Auth0
- Check token expiration settings in Auth0
- Enable debug logging: `"AppBlueprint.Infrastructure.Authorization": "Debug"`

### Network Issues

- Ensure your application can reach `https://YOUR-DOMAIN.auth0.com`
- Check firewall and proxy settings

---

## Advanced Configuration

### Custom Scopes

Add additional scopes in your configuration:
```json
{
  "Authentication": {
    "Auth0": {
      "Scope": "openid profile email read:data write:data"
    }
  }
}
```

### Token Refresh

The system automatically handles token refresh using the refresh token provided by Auth0.

### Multi-Tenant

For multi-tenant setups, you can configure different Auth0 tenants per customer by dynamically loading configuration.

---

## Code Usage

Your existing code continues to work unchanged:

```csharp
@inject IUserAuthenticationProvider AuthProvider

// Login
var success = await AuthProvider.LoginAsync(email, password);

// Check authentication
var isAuthenticated = AuthProvider.IsAuthenticated();

// Logout
await AuthProvider.LogoutAsync();
```

The factory pattern automatically handles provider selection based on configuration.

---

## Additional Resources

- [Auth0 Documentation](https://auth0.com/docs)
- [Auth0 Pricing](https://auth0.com/pricing)
- [Auth0 Quickstarts](https://auth0.com/docs/quickstarts)
- [Auth0 Community](https://community.auth0.com/)

---

## Need Help?

- Check the README at `Code/AppBlueprint/Shared-Modules/AppBlueprint.Infrastructure/Authorization/README.md`
- Review example configurations in `Code/AppBlueprint/Shared-Modules/AppBlueprint.Infrastructure/Authorization/Examples/`
- Look at the authentication tests in `AppBlueprint.Tests/Infrastructure/UserAuthenticationProviderTests.cs`

---

**Last Updated**: 2025-10-30

