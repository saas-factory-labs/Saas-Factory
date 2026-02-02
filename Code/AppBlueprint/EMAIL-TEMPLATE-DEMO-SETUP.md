# Email Template Demo Portal Setup

## Quick Start (Template Preview Only)

The email template demo portal at `/email-templates` works immediately for **template preview** without any configuration.

**What works out of the box:**
- ✅ Select any of 7 email templates
- ✅ Fill in form data
- ✅ Preview rendered HTML
- ✅ View raw HTML source

**What requires configuration:**
- ❌ Sending test emails (requires Resend API)

---

## Enable Email Sending with Resend

To enable the "Send Test Email" functionality, configure Resend:

### Using Doppler (Recommended for Development)

If you're using Doppler for environment variable management:

```bash
# Configure Doppler secrets (new standard naming)
doppler secrets set APPBLUEPRINT_RESEND_APIKEY "re_your_api_key_here"
doppler secrets set APPBLUEPRINT_RESEND_FROMEMAIL "noreply@yourdomain.com"
doppler secrets set APPBLUEPRINT_RESEND_FROMNAME "AppBlueprint"

# Run AppHost with Doppler
cd Code\AppBlueprint\AppBlueprint.AppHost
doppler run -- dotnet run
```

### Using Environment Variables

```powershell
# Set environment variables (PowerShell) - new standard naming
$env:APPBLUEPRINT_RESEND_APIKEY = "re_your_api_key_here"
$env:APPBLUEPRINT_RESEND_FROMEMAIL = "noreply@yourdomain.com"
$env:APPBLUEPRINT_RESEND_FROMNAME = "AppBlueprint"

# Restart AppHost
cd Code\AppBlueprint\AppBlueprint.AppHost
dotnet run
```

### Alternative: Generic Environment Variables (for deployed apps)

```bash
# Generic naming (works for any SaaS app deployment)
RESEND_API_KEY="re_your_api_key_here"
RESEND_FROM_EMAIL="noreply@yourdomain.com"
RESEND_FROM_NAME="Your App Name"
```

### Using User Secrets (Alternative)

```bash
cd Code\AppBlueprint\AppBlueprint.Web
dotnet user-secrets set "Resend:ApiKey" "re_your_api_key_here"
dotnet user-secrets set "Resend:FromEmail" "noreply@yourdomain.com"
dotnet user-secrets set "Resend:FromName" "AppBlueprint"
```

---

## Get a Resend API Key

1. **Sign up** at https://resend.com
2. **Get API key** from dashboard (free tier available)
3. **Verify domain** for production (development allows limited testing)

---

## Testing the Demo Portal

1. **Start AppHost** (or restart if already running)
   ```bash
   cd Code\AppBlueprint\AppBlueprint.AppHost
   dotnet run
   ```

2. **Navigate to demo portal**
   - URL: http://localhost:9200/email-templates
   - Or click "Email Templates" in navigation

3. **Preview templates** (works without Resend)
   - Select template from dropdown
   - Fill in form fields
   - Click "Preview Template"
   - Toggle "Show HTML Source" to view raw HTML

4. **Send test email** (requires Resend configuration)
   - After previewing, enter recipient email
   - Click "Send Test Email"
   - Check email inbox

---

## Troubleshooting

### "Resend not configured" error

**Symptom:** Error when clicking "Send Test Email"

**Solution:** Configure Resend API key as described above

**Note:** Template preview works without Resend - only email sending requires it

### AppHost crashes on startup

**Symptom:** `Unable to resolve service for type 'Resend.IResend'`

**Solution:** This was fixed - `IResend` is now optional. Rebuild the Infrastructure project:

```bash
cd Code\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure
dotnet build
```

### Templates not loading

**Symptom:** "Template not found" error

**Solution:** Templates are embedded resources. Run:

```bash
dotnet clean
dotnet build
```

---

## Available Templates

1. **Welcome Email** - User onboarding
2. **Password Reset** - Password recovery
3. **Order Confirmation** - E-commerce orders
4. **Invoice** - Billing invoices
5. **Booking Confirmation** - Appointment/reservation confirmations
6. **Weekly Digest** - Newsletter/updates
7. **Admin Notification** - System alerts

Each template has strongly-typed models with IntelliSense support.

---

## Architecture Notes

- **Template Rendering** - Uses RazorLight engine (no external dependencies)
- **Email Sending** - Uses Resend API (optional, only for actual email delivery)
- **Template Override** - Deployed apps can customize framework templates
- **Embedded Resources** - Framework templates compiled into assembly

See [README.md](Shared-Modules/AppBlueprint.Infrastructure/Services/Email/README.md) for full documentation.
