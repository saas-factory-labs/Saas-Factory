# Email Template System

✅ **Status: Fully Implemented & Verified** (February 2026)
- Template rendering with RazorLight ✅
- Email sending via Resend ✅
- Demo portal at `/email-templates` ✅
- Domain verification working ✅

A flexible email templating system using RazorLight that allows deployed applications to customize email templates while providing generic defaults in the AppBlueprint framework.

---

## Quick Start - Demo Portal

The fastest way to test the email system:

1. **Access the demo portal:**
   - Navigate to `http://localhost:9200/email-templates` (or your app URL)

2. **Configure Resend API (first time only):**
   ```bash
   # Option A: Use Resend sandbox domain for testing
   doppler secrets set APPBLUEPRINT_RESEND_APIKEY "re_your_api_key_here"
   doppler secrets set APPBLUEPRINT_RESEND_FROMEMAIL "onboarding@resend.dev"
   doppler secrets set APPBLUEPRINT_RESEND_FROMNAME "AppBlueprint Demo"
   
   # Option B: Use your verified domain
   doppler secrets set APPBLUEPRINT_RESEND_APIKEY "re_your_api_key_here"
   doppler secrets set APPBLUEPRINT_RESEND_FROMEMAIL "noreply@yourdomain.com"
   doppler secrets set APPBLUEPRINT_RESEND_FROMNAME "Your App Name"
   ```

3. **Test templates:**
   - Select a template (Welcome Email, Password Reset, Order Confirmation)
   - Fill in the form with sample data
   - Click "Preview Template" to see live rendering
   - Enter your email and click "Send Test Email"
   - Check your inbox! 📧

**Demo Portal Features:**
- 🎨 Live template preview with iframe rendering
- 📝 Dynamic forms for each template type
- 🔍 HTML source view toggle
- ✉️ Send test emails to any address
- 📱 Responsive flexbox layout

---

## Overview

The email template system provides:
- **Generic framework templates** - Ready-to-use email templates for common scenarios
- **Template override support** - Deployed apps can customize templates for their specific needs
- **Strongly-typed models** - Compile-time safety with IntelliSense support
- **Razor syntax** - Familiar `.cshtml` template syntax
- **Layout support** - Reusable email layouts for consistent branding

---

## Architecture

### Framework Templates (AppBlueprint.Infrastructure)

Generic templates provided by the framework:
```
AppBlueprint.Infrastructure/Services/Email/Templates/
├── _Layout.cshtml              # Base HTML email structure
├── WelcomeEmail.cshtml         # User welcome/onboarding
├── PasswordReset.cshtml        # Password reset requests
└── OrderConfirmation.cshtml    # Order confirmations
```

### Deployed App Templates (Override Pattern)

Each deployed application can override framework templates:
```
YourApp/Templates/
├── _Layout.cshtml              # Custom branding (optional override)
├── WelcomeEmail.cshtml         # App-specific welcome email (override)
├── BookingConfirmation.cshtml  # New template (app-specific)
└── MatchNotification.cshtml    # New template (app-specific)
```

**Template Resolution Order:**
1. Check deployed app's `/Templates/` folder first
2. Fall back to framework's embedded templates if not found

---

## Setup

### 1. Configure Resend (Optional for Email Sending)

The email template system has two capabilities:
- **Template Rendering** - Works without any configuration
- **Email Sending** - Requires Resend API configuration

**Without Resend configured:**
- ✅ Template rendering and preview works
- ❌ Email sending will throw `InvalidOperationException`

**To enable email sending, configure Resend:**

The system supports multiple environment variable naming conventions (checked in priority order):

**⚠️ IMPORTANT: Domain Verification Required**
- You must verify your sending domain at https://resend.com/domains before sending emails
- For testing, you can use Resend's sandbox domain (e.g., `onboarding@resend.dev`)
- Production apps must use a verified custom domain (e.g., `noreply@yourdomain.com`)

#### Using Environment Variables (Recommended)

**New Standard Naming** (AppBlueprint framework):
```bash
# PowerShell
$env:APPBLUEPRINT_RESEND_APIKEY = "re_your_api_key_here"
$env:APPBLUEPRINT_RESEND_FROMEMAIL = "noreply@yourdomain.com"  # Must be verified domain!
$env:APPBLUEPRINT_RESEND_FROMNAME = "Your App Name"
```

**Generic Naming** (for deployed SaaS apps):
```bash
# PowerShell - Works for any deployed app (dating app, property rental, etc.)
$env:RESEND_API_KEY = "re_your_api_key_here"
$env:RESEND_FROM_EMAIL = "noreply@yourdomain.com"  # Must be verified domain!
$env:RESEND_FROM_NAME = "Your App Name"
```

**Legacy Dotnet Format** (still supported):
```bash
# PowerShell
$env:APPBLUEPRINT_Resend__ApiKey = "re_your_api_key_here"
$env:APPBLUEPRINT_Resend__FromEmail = "noreply@yourdomain.com"  # Must be verified domain!
$env:APPBLUEPRINT_Resend__FromName = "Your App Name"
```

#### Using Doppler (Development)

If you're using Doppler for environment variable management:

```bash
# Configure Doppler secrets (new standard naming)
doppler secrets set APPBLUEPRINT_RESEND_APIKEY "re_your_api_key_here"
doppler secrets set APPBLUEPRINT_RESEND_FROMEMAIL "noreply@yourdomain.com"
doppler secrets set APPBLUEPRINT_RESEND_FROMNAME "Your App Name"

# Or use generic naming for deployed apps
doppler secrets set RESEND_API_KEY "re_your_api_key_here"
doppler secrets set RESEND_FROM_EMAIL "noreply@yourdomain.com"
doppler secrets set RESEND_FROM_NAME "Your App Name"

# Run with Doppler
doppler run -- dotnet run
```

**✅ Verified Working Configuration (February 2026):**
```bash
# Tested and confirmed working:
doppler secrets set APPBLUEPRINT_RESEND_APIKEY "re_abc123..."
doppler secrets set APPBLUEPRINT_RESEND_FROMEMAIL "onboarding@resend.dev"  # Sandbox domain
doppler secrets set APPBLUEPRINT_RESEND_FROMNAME "AppBlueprint"

# Restart AppHost to apply changes
# Test at: http://localhost:9200/email-templates
# Result: Email successfully delivered ✅
```

# Run with Doppler
doppler run -- dotnet run
```

#### Using User Secrets (Local Development)

```bash
dotnet user-secrets set "Resend:ApiKey" "re_your_api_key_here"
dotnet user-secrets set "Resend:FromEmail" "noreply@yourdomain.com"
dotnet user-secrets set "Resend:FromName" "Your App Name"
```

#### Using appsettings.json (Not Recommended)

```json
{
  "Resend": {
    "ApiKey": "re_your_api_key_here",
    "FromEmail": "noreply@yourdomain.com",
    "FromName": "Your App Name"
  }
}
```

**Get a Resend API Key:**
1. Sign up at https://resend.com
2. Get your API key from the dashboard
3. Add a verified domain for production

### 2. Register Service in Program.cs

**Option A: Use Framework Templates Only**
```csharp
using AppBlueprint.Infrastructure.Extensions;

// Use generic framework templates
builder.Services.AddEmailTemplateService();
```

**Option B: Enable Template Overrides**
```csharp
using AppBlueprint.Infrastructure.Extensions;

// Allow deployed app to override templates
string customTemplatesPath = Path.Combine(
    builder.Environment.ContentRootPath, 
    "Templates"
);

builder.Services.AddEmailTemplateService(customTemplatesPath);
```

### 3. Inject Service

```csharp
using AppBlueprint.Application.Interfaces;

public class UserService
{
    private readonly IEmailTemplateService _emailTemplateService;

    public UserService(IEmailTemplateService emailTemplateService)
    {
        _emailTemplateService = emailTemplateService;
    }
}
```

---

## Usage Examples

### Render Template (Preview)

Template rendering works without Resend configuration:

```csharp
using AppBlueprint.Contracts.Baseline.Email;

// Render template to HTML string (no email sending)
var model = new WelcomeEmailModel(
    UserName: "John Doe",
    EmailAddress: "john@example.com",
    TenantName: "Acme Corp",
    ActivationLink: "https://app.example.com/activate?token=abc123"
);

string htmlContent = await _emailTemplateService.RenderTemplateAsync(
    "WelcomeEmail",
    model
);

// Use htmlContent for preview, testing, or manual sending
```

### Send Welcome Email

Requires Resend configuration:

```csharp
using AppBlueprint.Contracts.Baseline.Email;

var model = new WelcomeEmailModel(
    UserName: "John Doe",
    EmailAddress: "john@example.com",
    TenantName: "Acme Corp",
    ActivationLink: "https://app.example.com/activate?token=abc123"
);

Guid emailId = await _emailTemplateService.SendTemplatedEmailAsync(
    from: "noreply@acmecorp.com",
    to: "john@example.com",
    subject: "Welcome to Acme Corp!",
    templateName: "WelcomeEmail",
    model: model
);
```

### Send Password Reset Email

```csharp
using AppBlueprint.Contracts.Baseline.Email;

var model = new PasswordResetEmailModel(
    UserName: "Jane Smith",
    EmailAddress: "jane@example.com",
    ResetLink: "https://app.example.com/reset-password?token=xyz789",
    ExpiresAt: DateTime.UtcNow.AddHours(24)
);

await _emailTemplateService.SendTemplatedEmailAsync(
    from: "security@acmecorp.com",
    to: "jane@example.com",
    subject: "Reset Your Password",
    templateName: "PasswordReset",
    model: model
);
```

### Render Template Without Sending

```csharp
// Just render HTML (useful for preview/testing)
string html = await _emailTemplateService.RenderTemplateAsync(
    templateName: "WelcomeEmail",
    model: welcomeModel
);
```

---

## Creating Custom Templates

### Step 1: Create Template Folder

In your deployed application:
```bash
mkdir Templates
```

### Step 2: Create Custom Template

Create `Templates/WelcomeEmail.cshtml`:

```cshtml
@using AppBlueprint.Contracts.Baseline.Email
@model WelcomeEmailModel
@{
    Layout = "_Layout";
    ViewBag.Title = "Welcome!";
    ViewBag.CompanyName = Model.TenantName;
    ViewBag.RecipientEmail = Model.EmailAddress;
}

<h2>🎉 Welcome to @Model.TenantName!</h2>

<p>
    Hey @Model.UserName,
</p>

<p>
    We're thrilled to have you join our property rental platform! 
    Get ready to find your perfect home.
</p>

@if (!string.IsNullOrEmpty(Model.ActivationLink))
{
    <div style="text-align: center; margin: 30px 0;">
        <a href="@Model.ActivationLink" class="button">
            Activate Your Account →
        </a>
    </div>
}

<p>
    Happy house hunting!<br>
    <strong>The @Model.TenantName Team</strong>
</p>
```

### Step 3: Custom Layout (Optional)

Create `Templates/_Layout.cshtml` to customize branding:

```cshtml
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>@ViewBag.Title</title>
    <style>
        /* Your custom branding styles */
        .email-header {
            background-color: #10B981; /* Your brand color */
        }
        .button {
            background-color: #10B981;
        }
    </style>
</head>
<body>
    <div class="email-container">
        <div class="email-header">
            <img src="https://yourdomain.com/logo.png" alt="Logo">
        </div>
        <div class="email-body">
            @RenderBody()
        </div>
        <div class="email-footer">
            <p>© @DateTime.UtcNow.Year @ViewBag.CompanyName</p>
        </div>
    </div>
</body>
</html>
```

---

## Available Template Models

All models are in `AppBlueprint.Contracts.Baseline.Email`:

| Model                              | Properties                                                                                   | Use Case             |
|------------------------------------|----------------------------------------------------------------------------------------------|----------------------|
| `WelcomeEmailModel`                | UserName, EmailAddress, TenantName, ActivationLink                                           | New user onboarding  |
| `PasswordResetEmailModel`          | UserName, EmailAddress, ResetLink, ExpiresAt                                                 | Password reset flow  |
| `OrderConfirmationEmailModel`      | CustomerName, OrderId, OrderDate, TotalAmount, OrderDetailsLink                              | E-commerce orders    |
| `InvoiceEmailModel`                | CustomerName, InvoiceNumber, InvoiceDate, DueDate, TotalAmount, InvoiceLink                  | Billing/invoices     |
| `BookingConfirmationEmailModel`    | UserName, PropertyName, CheckInDate, CheckOutDate, TotalPrice, BookingReference              | Property rentals     |
| `WeeklyDigestEmailModel`           | UserName, WeekStartDate, WeekEndDate, NewNotifications, NewMessages, HighlightedActivities   | Weekly summaries     |
| `AdminNotificationEmailModel`      | AdminName, NotificationType, Title, Message, ActionLink, OccurredAt                          | Admin alerts         |

---

## Best Practices

### 1. Email HTML Compatibility

Email clients have limited CSS support. Follow these rules:

✅ **Do:**
- Use table-based layouts
- Inline styles (not external CSS)
- Absolute URLs for images
- Web-safe fonts
- Test in multiple email clients

❌ **Don't:**
- Use Flexbox or Grid
- Use JavaScript
- Use embedded `<style>` tags in body
- Rely on external stylesheets

### 2. Template Organization

```
Templates/
├── Shared/
│   └── _Layout.cshtml           # Shared layout
├── Transactional/               # One-time emails
│   ├── WelcomeEmail.cshtml
│   └── PasswordReset.cshtml
└── Marketing/                   # Bulk/marketing emails
    ├── Newsletter.cshtml
    └── ProductAnnouncement.cshtml
```

### 3. Testing Templates

Create preview endpoints in development:

```csharp
#if DEBUG
app.MapGet("/email-preview/{templateName}", async (
    string templateName,
    IEmailTemplateService emailService) =>
{
    var model = new WelcomeEmailModel(
        "John Doe",
        "john@example.com",
        "Acme Corp",
        "https://example.com/activate"
    );

    string html = await emailService.RenderTemplateAsync(templateName, model);
    return Results.Content(html, "text/html");
});
#endif
```

### 4. Security Considerations

- ✅ Templates are embedded resources (read-only)
- ✅ Strongly-typed models prevent injection
- ✅ Razor auto-escapes HTML by default
- ⚠️ Never allow untrusted users to upload templates
- ⚠️ Validate all data before passing to templates

---

## Customization Examples

### Example 1: Property Rental Platform

```cshtml
@* Templates/BookingConfirmation.cshtml *@
@using AppBlueprint.Contracts.Baseline.Email
@model BookingConfirmationEmailModel

<h2>🏡 Booking Confirmed!</h2>

<p>Hi @Model.UserName,</p>

<p>Great news! Your reservation for <strong>@Model.PropertyName</strong> is confirmed.</p>

<div class="booking-details">
    <table>
        <tr>
            <td>Check-in:</td>
            <td>@Model.CheckInDate.ToString("MMM dd, yyyy")</td>
        </tr>
        <tr>
            <td>Check-out:</td>
            <td>@Model.CheckOutDate.ToString("MMM dd, yyyy")</td>
        </tr>
        <tr>
            <td>Total:</td>
            <td><strong>@Model.TotalPrice.ToString("C")</strong></td>
        </tr>
        <tr>
            <td>Reference:</td>
            <td>@Model.BookingReference</td>
        </tr>
    </table>
</div>
```

### Example 2: Dating App

```cshtml
@* Templates/WelcomeEmail.cshtml *@
@model WelcomeEmailModel

<h2>❤️ Welcome to @Model.TenantName!</h2>

<p>Hey @Model.UserName,</p>

<p>Your dating journey starts now! We're excited to help you find meaningful connections.</p>

<a href="@Model.ActivationLink" class="button">
    Complete Your Profile →
</a>

<p><small>Pro tip: Add photos to get 3x more matches!</small></p>
```

---

## Troubleshooting

### Templates Not Found

**Error:** `Template 'WelcomeEmail' not found`

**Solution:**
1. Verify template file exists in `/Templates/` folder
2. Ensure filename matches exactly (case-sensitive on Linux)
3. Check custom templates path is correct in registration

### Layout Not Applied

**Error:** Template renders but layout is missing

**Solution:**
1. Ensure `_Layout.cshtml` exists in same folder
2. Check `Layout = "_Layout";` is set in template
3. Verify layout file is also embedded/copied

### IntelliSense Not Working

**Solution:**
1. Add using statement: `@using AppBlueprint.Contracts.Baseline.Email`
2. Specify model: `@model WelcomeEmailModel`
3. Rebuild project

---

## Migration from TransactionEmailService

If migrating from the old `TransactionEmailService`:

**Before:**
```csharp
await _emailService.SendSignUpWelcomeEmail(
    from: "noreply@app.com",
    to: "user@example.com",
    siteName: "Acme Corp"
);
```

**After:**
```csharp
var model = new WelcomeEmailModel(
    UserName: user.Name,
    EmailAddress: user.Email,
    TenantName: "Acme Corp",
    ActivationLink: activationUrl
);

await _emailTemplateService.SendTemplatedEmailAsync(
    from: "noreply@app.com",
    to: user.Email,
    subject: "Welcome to Acme Corp!",
    templateName: "WelcomeEmail",
    model: model
);
```

---

## Future Enhancements

Planned features:
- [ ] Liquid templating engine option (for tenant-editable templates)
- [ ] Template versioning
- [ ] A/B testing support
- [ ] Template analytics
- [ ] Visual template editor

---

## Support

For questions or issues:
- Check [FRAMEWORK-MISSING-SERVICES.md](../../../FRAMEWORK-MISSING-SERVICES.md)
- Review example templates in framework
- Test templates using preview endpoint
