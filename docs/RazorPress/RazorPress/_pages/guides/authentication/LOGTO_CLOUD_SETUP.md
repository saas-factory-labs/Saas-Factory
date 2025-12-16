# Logto Cloud Setup - Quick Guide

Get your AppBlueprint application running with Logto Cloud authentication in 5 minutes!

## Why Logto Cloud?

✅ **No Infrastructure** - Fully managed service  
✅ **Fast Setup** - Up and running in minutes  
✅ **Free Tier** - Generous free tier for development  
✅ **Global CDN** - Fast authentication worldwide  
✅ **Automatic Updates** - Always on the latest version  

---

## Step-by-Step Setup

### 1. Create Your Logto Cloud Account

1. Go to **[cloud.logto.io](https://cloud.logto.io)**
2. Sign up with your email or GitHub account
3. Verify your email address

### 2. Create a Tenant

1. After signing in, you'll be prompted to create your first tenant
2. Choose a tenant name (e.g., "appblueprint-dev")
3. Select a region closest to you
4. Click **Create**

Your tenant URL will be: `https://[your-tenant-id].logto.app`

### 3. Create an Application

1. In the Logto Console, navigate to **Applications**
2. Click **Create Application**
3. Select **Traditional Web App**
4. Name it "AppBlueprint Web"
5. Click **Create Application**

### 4. Configure Application Settings

In your new application's settings page:

#### A. Redirect URIs
Add these URLs (for local development):
```
http://localhost:8092/callback
https://localhost:443/callback
```

#### B. Post Sign-out Redirect URIs
Add these URLs:
```
http://localhost:8092
https://localhost:443
```

#### C. CORS Allowed Origins
Add these URLs:
```
http://localhost:8092
https://localhost:443
```

#### D. Enable ROPC Grant Type ⚠️ IMPORTANT

1. Scroll down to **Advanced Settings**
2. Find **Grant Types**
3. **Enable "Resource Owner Password Credentials (ROPC)"**
4. Click **Save Changes**

> ⚠️ **Note**: ROPC is required for password-based login. For production, consider implementing Authorization Code Flow with PKCE for better security.

### 5. Get Your Credentials

From the application overview/settings page, you'll see:

- **App ID** (your Client ID) - Click to copy
- **App Secret** (your Client Secret) - Click to reveal and copy
- **Endpoint** (your tenant URL) - Example: `https://abc123.logto.app`

**Keep these secure!** You'll need them in the next step.

### 6. Create a Test User

1. Navigate to **User Management** in the Logto Console
2. Click **Create User**
3. Fill in the details:
   - **Username**: `testuser`
   - **Email**: `test@example.com`
   - **Password**: Choose a strong password (remember this!)
4. Click **Create**

### 7. Configure Your Application

You have two options:

#### Option A: Using User Secrets (Recommended - Keeps secrets secure)

Open PowerShell and run:

```powershell
cd C:\Development\Development-Projects\saas-factory-labs\Code\AppBlueprint\AppBlueprint.Web

dotnet user-secrets set "Authentication:Provider" "Logto"
dotnet user-secrets set "Authentication:Logto:Endpoint" "https://YOUR-TENANT-ID.logto.app"
dotnet user-secrets set "Authentication:Logto:ClientId" "YOUR-APP-ID"
dotnet user-secrets set "Authentication:Logto:ClientSecret" "YOUR-APP-SECRET"
dotnet user-secrets set "Authentication:Logto:Scope" "openid profile email offline_access"
```

**Replace:**
- `YOUR-TENANT-ID` with your actual tenant ID
- `YOUR-APP-ID` with your App ID
- `YOUR-APP-SECRET` with your App Secret

#### Option B: Using appsettings.Development.json (Quick but less secure)

Edit `Code/AppBlueprint/AppBlueprint.Web/appsettings.Development.json`:

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
      "Scope": "openid profile email offline_access"
    }
  }
}
```

**Replace the placeholder values with your actual credentials from Step 5.**

> ⚠️ **Important**: If you use this option, make sure NOT to commit your secrets to source control!

### 8. Test Your Setup

1. **Build your application:**
   ```powershell
   cd C:\Development\Development-Projects\saas-factory-labs\Code\AppBlueprint\AppBlueprint.Web
   dotnet build
   ```

2. **Run your application** (if not already running)

3. **Navigate to the login page**

4. **Login with your test user:**
   - Email: `test@example.com`
   - Password: (the password you set in Step 6)

5. **Success!** 🎉 You should be authenticated via Logto Cloud!

---

## Verifying Your Setup

### Check Logs
Look for these log messages when your app starts:

```
[AppBlueprint.Infrastructure.Authorization] Using authentication provider: Logto
[AppBlueprint.Infrastructure.Authorization] Logto endpoint configured: https://YOUR-TENANT.logto.app
```

### Check Authentication
In your browser's developer tools (F12), you should see:
- Network requests to your Logto endpoint
- A successful token response
- JWT tokens stored

### Troubleshooting

If login fails, check:

1. ✅ **ROPC Grant Type is enabled** in Logto app settings
2. ✅ **User exists** in Logto with the correct credentials
3. ✅ **Endpoint URL is correct** (should start with `https://`)
4. ✅ **ClientId and ClientSecret are correct**
5. ✅ **No typos** in your configuration

Check logs for detailed error messages:
```powershell
# The logs will show in your application console
# Look for lines starting with [AppBlueprint.Infrastructure.Authorization]
```

---

## Next Steps

### 🎨 Customize the Sign-In Experience

1. In Logto Console, go to **Sign-in Experience**
2. Customize:
   - **Branding**: Add your logo, change colors
   - **Sign-in Methods**: Enable social sign-in (Google, GitHub, etc.)
   - **Passwordless**: Enable email/SMS codes
   - **Terms of Service**: Add your policies

### 🔐 Enable Multi-Factor Authentication (MFA)

1. Go to **Sign-in Experience** → **Multi-factor Auth**
2. Enable **TOTP (Authenticator App)**
3. Configure settings and save

### 👥 Add Social Sign-In

1. Go to **Connectors**
2. Click **Add Social Connector**
3. Choose a provider (Google, GitHub, Microsoft, etc.)
4. Follow the setup wizard
5. Enable in your sign-in experience

### 📊 Monitor Usage

1. Go to **Dashboard** to see:
   - Active users
   - Sign-in activities
   - Popular sign-in methods
   - Error rates

---

## Production Deployment

When you're ready for production:

### 1. Create a Production Tenant

1. In Logto Cloud, create a new tenant for production
2. Choose a production-appropriate name
3. Select the region closest to your users

### 2. Update Production Configuration

Use environment variables in your production deployment:

```
Authentication__Provider=Logto
Authentication__Logto__Endpoint=https://your-prod-tenant.logto.app
Authentication__Logto__ClientId=your-prod-app-id
Authentication__Logto__ClientSecret=your-prod-app-secret
Authentication__Logto__Scope=openid profile email offline_access
```

### 3. Update Redirect URIs

In production application settings, add your production URLs:
```
https://yourdomain.com/callback
https://yourdomain.com
```

### 4. Security Checklist

- ✅ Use Authorization Code Flow with PKCE (instead of ROPC)
- ✅ Enable MFA for all users
- ✅ Configure rate limiting
- ✅ Set up webhooks for security events
- ✅ Review audit logs regularly
- ✅ Rotate secrets periodically

---

## Switching Between Mock and Logto

Your application makes switching easy:

### Development/Testing (Mock)
```json
{
  "Authentication": {
    "Provider": "Mock"
  }
}
```

Or with user secrets:
```powershell
dotnet user-secrets set "Authentication:Provider" "Mock"
```

### Production (Logto)
```json
{
  "Authentication": {
    "Provider": "Logto",
    "Logto": {
      "Endpoint": "https://your-tenant.logto.app",
      "ClientId": "your-app-id",
      "ClientSecret": "your-app-secret",
      "Scope": "openid profile email offline_access"
    }
  }
}
```

---

## Pricing

Logto Cloud pricing is very developer-friendly:

### Free Tier (Perfect for Development & Small Apps)
- ✅ **50,000 MAU** (Monthly Active Users)
- ✅ Unlimited applications
- ✅ Social sign-in
- ✅ MFA
- ✅ Email support

### Pro Tier (For Growing Apps)
- ✅ **Unlimited MAU**
- ✅ Everything in Free
- ✅ Custom domain
- ✅ Advanced features
- ✅ Priority support

Check [cloud.logto.io/pricing](https://cloud.logto.io/pricing) for current pricing.

---

## Common Issues & Solutions

### "Invalid grant type" Error
**Solution**: Enable ROPC grant type in application settings (Step 4D)

### "Invalid credentials" Error
**Solution**: 
- Verify user exists in Logto Console
- Check username/email and password are correct
- Ensure user account is not locked

### Network/Connection Issues
**Solution**:
- Verify your Endpoint URL is correct
- Check internet connection
- Ensure no firewall blocking Logto Cloud

### Tokens Not Refreshing
**Solution**: Add `offline_access` to your scope:
```json
"Scope": "openid profile email offline_access"
```

---

## Getting Help

### Logto Resources
- 📚 [Documentation](https://docs.logto.io)
- 💬 [Discord Community](https://discord.gg/logto)
- 🐙 [GitHub Issues](https://github.com/logto-io/logto/issues)
- 📧 Email support through Logto Console

### Check Your Implementation
- Review [Authorization README](../Shared-Modules/AppBlueprint.Infrastructure/Authorization/README.md)
- Check example configs in `Authorization/Examples/`

---

## Summary Checklist

Before you start, make sure you have:

- ✅ Created Logto Cloud account
- ✅ Created a tenant
- ✅ Created an application (Traditional Web App)
- ✅ Enabled ROPC grant type
- ✅ Added redirect URIs
- ✅ Created a test user
- ✅ Copied App ID, App Secret, and Endpoint
- ✅ Updated your appsettings or user secrets
- ✅ Tested login with your test user

---

**You're all set!** Enjoy your Logto Cloud authentication! 🚀

For the complete guide with self-hosting options, see [LOGTO_SETUP_GUIDE.md](./LOGTO_SETUP_GUIDE.md)

---

**Last Updated**: 2025-10-30

