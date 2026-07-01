# JWT Token Generator - Browser Automation

## Overview

The JWT Token Generator now includes **browser automation** to automatically extract JWT tokens from Logto authentication. This eliminates the need to manually copy tokens from browser developer tools.

## Features

- **Automated Browser Extraction**: Opens a browser window, waits for login, and automatically extracts the JWT token from local storage
- **Manual Fallback**: Option to manually paste tokens if automation fails or is not desired
- **Security Best Practice**: Uses OAuth 2.0 recommended flows (no ROPC/password grant)
- **Works with All Auth Methods**: Supports social logins, MFA, passwordless, and any authentication method Logto offers

## Prerequisites

### First-Time Setup: Install Playwright Browsers

Before using the automated token extraction, you must install Playwright browsers **once**:

```powershell
# Navigate to the DeveloperCli output directory
cd C:\Development\Development-Projects\saas-factory-labs\Code\AppBlueprint\AppBlueprint.DeveloperCli\bin\Debug\net9.0

# Install Playwright browsers
pwsh playwright.ps1 install
```

**Alternative method:**
```powershell
# Install Playwright CLI globally
dotnet tool install --global Microsoft.Playwright.CLI

# Install browsers
playwright install chromium
```

> **Note**: You only need to do this once. The browsers will be cached for future use.

## Usage

### Interactive Mode

1. Run the Developer CLI
2. Select "Generate JWT Token" from the menu
3. Choose to use automated browser extraction (default: Yes)
4. A browser window will open automatically
5. Log in with your credentials
6. The token will be extracted automatically and displayed in the CLI

### Command Line Mode

```powershell
# With config file
devcli jwt-token --config ../AppBlueprint.Web/appsettings.Development.json

# Interactive (will prompt for config)
devcli jwt-token
```

## How It Works

1. **Browser Launch**: Playwright launches a Chromium browser instance
2. **Navigate**: Automatically navigates to your web application (default: https://localhost:7002)
3. **User Login**: You log in normally through the Logto UI
4. **Token Detection**: The CLI monitors local storage for authentication tokens
5. **Extraction**: Once detected, extracts the access token from the stored data
6. **Validation**: Validates and displays the JWT token with claims
7. **Cleanup**: Closes the browser and cleans up resources

## Configuration

The tool reads configuration from `appsettings.json`:

```json
{
  "Authentication": {
    "Provider": "Logto",
    "Logto": {
      "Endpoint": "https://your-tenant.logto.app",
      "ClientId": "your-client-id"
    }
  },
  "WebApp": {
    "Url": "https://localhost:7002"
  }
}
```

### Configuration Options

- `Authentication:Provider`: Must be "Logto"
- `Authentication:Logto:Endpoint`: Your Logto tenant URL
- `Authentication:Logto:ClientId`: Your application's client ID
- `WebApp:Url`: (Optional) URL of your web application (default: https://localhost:7002)

## Timeout & Retry

- **Maximum Wait Time**: 5 minutes for authentication
- **Check Interval**: 1 second between local storage checks
- **Automatic Fallback**: If timeout occurs or automation fails, falls back to manual token paste

## Security Considerations

### Why Browser Automation Instead of ROPC?

This implementation uses **browser automation** instead of ROPC (Resource Owner Password Credentials) flow for several important security reasons:

✅ **Security Best Practices**
- Follows OAuth 2.0 Security Best Current Practice (RFC 8252)
- No password handling in the CLI
- Supports MFA and all authentication methods
- Better audit trail

✅ **Compliance Friendly**
- Passes security audits (SOC2, ISO 27001)
- No credential exposure
- Authentic user experience

✅ **Future Proof**
- Works with modern auth providers
- Supports social logins
- Compatible with passwordless authentication

### Development Certificates

The browser automation handles self-signed development certificates automatically:
- `--ignore-certificate-errors` flag for Chromium
- `IgnoreHTTPSErrors: true` for page context

## Troubleshooting

### "Playwright not found" Error

**Solution**: Install Playwright browsers (see Prerequisites above)

### Browser Doesn't Open

**Possible causes:**
1. Playwright browsers not installed
2. Running in headless environment (CI/CD)

**Solution**: Use manual token paste option when prompted

### Token Not Detected

**Possible causes:**
1. Web application not running
2. Login not completed
3. Token stored under different key

**Solution**: 
- Ensure web app is running on the configured URL
- Complete the login flow
- Try manual token paste as fallback

### Timeout After 5 Minutes

**Solution**: Try again or use manual token paste option

## Manual Token Extraction (Fallback)

If automation fails or you prefer manual extraction:

1. Open your web application in a browser
2. Log in with your credentials
3. Press F12 to open Developer Tools
4. Go to Application/Storage → Local Storage
5. Look for keys containing "@logto", "access_token", or "id_token"
6. Copy the JWT token value
7. Paste it into the CLI when prompted

## Example Output

```
╔══════════════════════════════════════════════════════════╗
║                       Success                            ║
╚══════════════════════════════════════════════════════════╝
✓ User token validated successfully!

╭─────────────────────────────────────────────────────────╮
│ Property          │ Value                                │
├───────────────────┼──────────────────────────────────────┤
│ Subject (User ID) │ abc123xyz                            │
│ Email             │ user@example.com                     │
│ Name              │ John Doe                             │
│ Expires           │ 2025-10-31 15:30:00 UTC             │
│ Provider          │ Logto                                │
│ Issuer            │ https://tenant.logto.app/oidc       │
│ Audience          │ your-client-id                       │
╰─────────────────────────────────────────────────────────╯

════════════════════════════════════════════════════════════
USER JWT TOKEN (copy this)
════════════════════════════════════════════════════════════

  eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...

```

## Benefits

1. **Developer Productivity**: No manual token copying
2. **Security**: Uses recommended OAuth flows
3. **Flexibility**: Works with any authentication method
4. **Reliability**: Automatic fallback to manual mode
5. **Convenience**: One-click token generation

## Dependencies

- **Microsoft.Playwright**: For browser automation
- **Spectre.Console**: For CLI UI
- **System.IdentityModel.Tokens.Jwt**: For token validation

