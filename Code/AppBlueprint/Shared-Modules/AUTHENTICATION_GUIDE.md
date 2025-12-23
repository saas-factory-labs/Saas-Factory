# Authentication Provider Configuration Guide

AppBlueprint supports multiple authentication providers through a unified abstraction. Choose the provider that best fits your SaaS application needs.

## Supported Providers

- **Logto** - Open-source identity solution (recommended for new projects)
- **Auth0** - Popular authentication platform  
- **Firebase** - Google's authentication and app platform (Coming soon)
- **Azure AD B2C** - Microsoft's B2C identity solution (Coming soon)
- **AWS Cognito** - Amazon's user authentication service (Coming soon)
- **JWT** - Simple JWT-based authentication (development/testing)
- **Mock** - No authentication for testing (development only)

## Configuration

### Step 1: Choose Your Provider

In your `appsettings.json` or environment variables, set the authentication provider:

```json
{
  "Authentication": {
    "Provider": "Logto"
  }
}
```

### Step 2: Configure Provider Settings

#### Logto Configuration

```json
{
  "Authentication": {
    "Provider": "Logto",
    "Logto": {
      "Endpoint": "https://your-tenant.logto.app",
      "ClientId": "your-client-id",
      "ClientSecret": "your-client-secret",
      "Scope": "openid profile email",
      "Resource": "https://api.yourdomain.com"
    }
  }
}
```

**Environment Variables:**
```bash
Authentication__Provider=Logto
Authentication__Logto__Endpoint=https://your-tenant.logto.app
Authentication__Logto__ClientId=your-client-id
Authentication__Logto__ClientSecret=your-client-secret
Authentication__Logto__Resource=https://api.yourdomain.com
```

#### Auth0 Configuration

```json
{
  "Authentication": {
    "Provider": "Auth0",
    "Auth0": {
      "Domain": "https://your-domain.auth0.com",
      "ClientId": "your-client-id",
      "ClientSecret": "your-client-secret",
      "Audience": "https://your-api.example.com",
      "Scope": "openid profile email"
    }
  }
}
```

**Environment Variables:**
```bash
Authentication__Provider=Auth0
Authentication__Auth0__Domain=https://your-domain.auth0.com
Authentication__Auth0__ClientId=your-client-id
Authentication__Auth0__ClientSecret=your-client-secret
Authentication__Auth0__Audience=https://your-api.example.com
```

#### Azure AD B2C Configuration (Coming Soon)

```json
{
  "Authentication": {
    "Provider": "AzureAD",
    "AzureAD": {
      "Instance": "https://login.microsoftonline.com/",
      "TenantId": "your-tenant-id",
      "ClientId": "your-client-id",
      "ClientSecret": "your-client-secret",
      "Domain": "yourtenant.onmicrosoft.com",
      "SignUpSignInPolicyId": "B2C_1_signupsignin1"
    }
  }
}
```

#### AWS Cognito Configuration (Coming Soon)

```json
{
  "Authentication": {
    "Provider": "Cognito",
    "Cognito": {
      "UserPoolId": "us-east-1_xxxxxxxxx",
      "ClientId": "your-client-id",
      "ClientSecret": "your-client-secret",
      "Region": "us-east-1",
      "Domain": "your-domain.auth.us-east-1.amazoncognito.com"
    }
  }
}
```

#### Firebase Configuration (Coming Soon)

```json
{
  "Authentication": {
    "Provider": "Firebase",
    "Firebase": {
      "ProjectId": "your-firebase-project-id",
      "ApiKey": "your-web-api-key",
      "AuthDomain": "your-project-id.firebaseapp.com",
      "DatabaseUrl": "https://your-project-id.firebaseio.com",
      "StorageBucket": "your-project-id.appspot.com",
      "MessagingSenderId": "123456789012",
      "AppId": "1:123456789012:web:abc123def456"
    }
  }
}
```

**Environment Variables:**
```bash
Authentication__Provider=Firebase
Authentication__Firebase__ApiKey=your-web-api-key
```

⚠️ **Security Note:** While Firebase's Web API Key is meant for client-side use, it's still good practice to use environment variables to separate configs per environment (dev/staging/prod).

#### JWT Configuration (Development/Testing)

```json
{
  "Authentication": {
    "Provider": "JWT",
    "JWT": {
      "SecretKey": "YourSuperSecretKey_MustBeAtLeast32Characters!",
      "Issuer": "AppBlueprintAPI",
      "Audience": "AppBlueprintClient",
      "ExpirationMinutes": 60
    }
  }
}
```

⚠️ **WARNING:** JWT provider is for development/testing only. Use proper OAuth2/OIDC providers in production!

#### Mock Configuration (Testing Only)

```json
{
  "Authentication": {
    "Provider": "Mock"
  }
}
```

⚠️ **WARNING:** NEVER use Mock provider in production!

## Service Registration

### Option 1: Configuration-Based (Recommended)

```csharp
// Program.cs
using AppBlueprint.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add authentication with automatic provider selection
builder.Services.AddAppBlueprintAuthentication(builder.Configuration);

var app = builder.Build();
app.Run();
```

### Option 2: Manual Provider Selection

```csharp
// Program.cs
using AppBlueprint.Application.Options;
using AppBlueprint.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configure authentication options programmatically
builder.Services.Configure<AuthenticationOptions>(options =>
{
    options.Provider = "Logto";
    options.Logto = new LogtoOptions
    {
        Endpoint = "https://your-tenant.logto.app",
        ClientId = "your-client-id",
        ClientSecret = "your-client-secret"
    };
});

builder.Services.AddAppBlueprintAuthentication(builder.Configuration);

var app = builder.Build();
app.Run();
```

## Production Configuration with Azure Key Vault

For production deployments, use Azure Key Vault:

### 1. Add Azure Key Vault NuGet Package
```bash
dotnet add package Azure.Extensions.AspNetCore.Configuration.Secrets
```

### 2. Configure in Program.cs
```csharp
var builder = WebApplication.CreateBuilder(args);

// Add Azure Key Vault if running in Azure
if (builder.Environment.IsProduction())
{
    var keyVaultEndpoint = new Uri(builder.Configuration["KeyVaultEndpoint"]!);
    builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());
}

// Register authentication with configuration
builder.Services.AddAppBlueprintAuthentication(builder.Configuration);
```

### 3. Store Secrets in Key Vault
```bash
# Using Azure CLI
az keyvault secret set --vault-name "your-vault" \
  --name "Authentication--Logto--ClientSecret" \
  --value "your-secret-value"

az keyvault secret set --vault-name "your-vault" \
  --name "Authentication--Auth0--ClientSecret" \
  --value "your-secret-value"
```

### 4. Grant Access to Managed Identity
```bash
# For App Service
az webapp identity assign --resource-group "rg-name" --name "app-name"

# Grant access to Key Vault
az keyvault set-policy --name "your-vault" \
  --object-id "<managed-identity-object-id>" \
  --secret-permissions get list
```

### AWS Secrets Manager Alternative

For AWS deployments:

```csharp
using Amazon.SecretsManager;
using Amazon.SecretsManager.Extensions.Caching;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    var cache = new SecretsManagerCache();
    var secret = await cache.GetSecretString("your-secret-name");
    // Parse and add to configuration
}
```

## Usage in Blazor Components

```csharp
@inject IAuthenticationProvider AuthProvider

<button @onclick="Login">Sign In</button>

@code {
    private async Task Login()
    {
        var result = await AuthProvider.LoginAsync(new LoginRequest
        {
            Email = "user@example.com",
            Password = "password123"
        });

        if (result.IsSuccess)
        {
            // Navigate to dashboard
            NavigationManager.NavigateTo("/dashboard");
        }
        else
        {
            // Show error message
            await JSRuntime.InvokeVoidAsync("alert", result.Error);
        }
    }
}
```

## Usage in API Controllers

```csharp
public class MyController : ControllerBase
{
    private readonly IAuthenticationProvider _authProvider;

    public MyController(IAuthenticationProvider authProvider)
    {
        _authProvider = authProvider;
    }

    [HttpPost("api/auth/login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authProvider.LoginAsync(new LoginRequest
        {
            Email = dto.Email,
            Password = dto.Password
        });

        if (result.IsSuccess)
        {
            return Ok(new { token = result.AccessToken });
        }

        return Unauthorized(new { error = result.Error });
    }
}
```

## Switching Providers

To switch authentication providers, simply update your configuration:

```json
{
  "Authentication": {
    "Provider": "Auth0"  // Changed from "Logto" to "Auth0"
  }
}
```

Or via environment variable:
```bash
export Authentication__Provider=Auth0
```

## Provider Comparison

| Feature | Logto | Auth0 | Firebase | Azure AD B2C | AWS Cognito |
|---------|-------|-------|----------|--------------|-------------|
| Open Source | ✅ | ❌ | ❌ | ❌ | ❌ |
| Self-Hosted | ✅ | ❌ | ❌ | ❌ | ❌ |
| Free Tier | ✅ | ✅ | ✅ | ✅ | ✅ |
| B2C Support | ✅ | ✅ | ✅ | ✅ | ✅ |
| B2B Support | ✅ | ✅ | ⚠️ | ✅ | ⚠️ |
| Social Login | ✅ | ✅ | ✅ | ✅ | ✅ |
| MFA | ✅ | ✅ | ✅ | ✅ | ✅ |
| Mobile SDKs | ✅ | ✅ | ✅ | ✅ | ✅ |
| Realtime DB | ❌ | ❌ | ✅ | ❌ | ❌ |
| Implementation Status | ✅ | ✅ | 🚧 | 🚧 | 🚧 |

## Troubleshooting

### Provider Not Configured

**Error:** `Unknown authentication provider: XYZ`

**Solution:** Make sure you've set the correct provider name in configuration:
- Valid values: `Logto`, `Auth0`, `Firebase`, `AzureAD`, `Cognito`, `JWT`, `Mock`
- Provider names are case-insensitive

### Missing Configuration

**Error:** `Logto Endpoint is required in configuration`

**Solution:** Ensure all required configuration values are set for your chosen provider.

### Provider Not Implemented

**Error:** `Firebase authentication provider is not yet implemented`

**Solution:** Firebase, Azure AD, and Cognito providers are coming soon. Use Logto or Auth0 in the meantime.

## Best Practices

1. **Use environment variables** for sensitive values (ClientSecret, SecretKey)
2. **Never commit secrets** to version control
3. **Use Mock provider** only in local development
4. **Enable MFA** for production environments
5. **Rotate credentials** regularly
6. **Monitor authentication logs** for suspicious activity

## Next Steps

- [Multi-Tenancy Configuration](./MULTI_TENANCY_GUIDE.md)
- [Security Best Practices](./SECURITY_GUIDE.md)
- [Production Deployment](./DEPLOYMENT_GUIDE.md)
