# Authentication Configuration Examples

This directory contains example configuration files for different authentication providers.

## ⚠️ CRITICAL SECURITY WARNING

**NEVER commit sensitive credentials to version control:**
- ❌ **Client Secrets** (OAuth2/OIDC authentication)
- ❌ **API Keys** (third-party service authentication)
- ❌ **JWT Secret Keys** (token signing)
- ❌ **Private Keys** (encryption/signing)

**Client IDs are PUBLIC identifiers** and safe to commit:
- ✅ Visible in OAuth redirect URLs (e.g., `?client_id=abc123`)
- ✅ Cannot authenticate without Client Secret
- ✅ Designed to be non-confidential per OAuth2 spec

**Note:** Some organizations still keep Client IDs private in source control as a blanket policy (to avoid advertising infrastructure details), but this provides minimal security benefit since they're exposed during normal OAuth flows.

## Secure Configuration Methods

### 1. User Secrets (Local Development)

**Recommended for local development:**

```bash
cd YourProject
dotnet user-secrets init
dotnet user-secrets set "Authentication:Logto:ClientSecret" "your-secret-here"
dotnet user-secrets set "Authentication:Auth0:ClientSecret" "your-secret-here"
```

### 2. Environment Variables

**For Docker containers and CI/CD:**

```bash
# Windows
$env:Authentication__Logto__ClientSecret="your-secret"

# Linux/Mac
export Authentication__Logto__ClientSecret="your-secret"
```

**In Docker Compose:**
```yaml
services:
  web:
    environment:
      - Authentication__Provider=Logto
      - Authentication__Logto__ClientSecret=${LOGTO_CLIENT_SECRET}
    env_file:
      - .env.secrets  # Never commit this file!
```

### 3. Azure Key Vault (Production)

**For Azure deployments:**

```csharp
// Program.cs
if (builder.Environment.IsProduction())
{
    var keyVaultEndpoint = new Uri(builder.Configuration["KeyVaultEndpoint"]!);
    builder.Configuration.AddAzureKeyVault(
        keyVaultEndpoint, 
        new DefaultAzureCredential()
    );
}
```

**Store secrets in Key Vault:**
```bash
az keyvault secret set \
  --vault-name "your-vault" \
  --name "Authentication--Logto--ClientSecret" \
  --value "your-secret"
```

### 4. AWS Secrets Manager (AWS Deployments)

**For AWS deployments:**

```csharp
// Program.cs
using Amazon.SecretsManager;
using Amazon.SecretsManager.Extensions.Caching;

var cache = new SecretsManagerCache();
var secretJson = await cache.GetSecretString("appblueprint/auth");
var secrets = JsonSerializer.Deserialize<Dictionary<string, string>>(secretJson);

builder.Configuration.AddInMemoryCollection(secrets);
```

## Configuration File Structure

Each example file shows:
1. **Non-sensitive settings** - Safe to commit (Provider type, endpoints, client IDs)
2. **Environment variable instructions** - How to set sensitive values

### Example Files

- `appsettings.mock.example.json` - Mock provider (testing only)
- `appsettings.azuread.example.json` - Azure AD B2C
- `appsettings.cognito.example.json` - AWS Cognito
- `appsettings.firebase.example.json` - Firebase Authentication
- `appsettings.jwt.example.json` - Simple JWT (dev/test only)

## Usage

1. Copy the example file to your project's `appsettings.json`
2. Set sensitive values using one of the secure methods above
3. Never commit files containing actual secrets

## .gitignore Entries

Ensure these patterns are in your `.gitignore`:

```gitignore
# User secrets
**/appsettings.*.json
!**/appsettings.example.json
*.secrets.json

# Environment files
.env
.env.local
.env.*.local
```

## Further Reading

- [AUTHENTICATION_GUIDE.md](../../AUTHENTICATION_GUIDE.md) - Complete authentication setup guide
- [ASP.NET Core Secret Management](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)
- [Azure Key Vault Configuration Provider](https://learn.microsoft.com/en-us/aspnet/core/security/key-vault-configuration)
