# Email Templates Implementation

Complete Razor-based email template system for AppBlueprint with local preview capability.

## 📁 What Was Created

### 1. **Email Template Models** (Contracts Layer)
- `AppBlueprint.Contracts/Email/EmailTemplateModels.cs`
  - `WelcomeEmailModel` - Welcome email after signup
  - `PasswordResetEmailModel` - Password reset requests
  - `EmailVerificationModel` - Email verification links
  - `OrderConfirmationEmailModel` - Order confirmations with line items
  - `TeamInvitationEmailModel` - Team member invitations

### 2. **Interface** (Application Layer)
- `AppBlueprint.Application/Interfaces/Email/IEmailTemplateService.cs`
  - `RenderTemplateAsync<TModel>()` - Render by template name
  - `RenderTemplateAsync<TModel>(model)` - Render using model's template name
  - `GetAvailableTemplatesAsync()` - List all templates

### 3. **Razor Templates** (Infrastructure Layer)
Located in `AppBlueprint.Infrastructure/EmailTemplates/`:
- `_Layout.cshtml` - Shared layout with professional styling
- `WelcomeEmail.cshtml` - Welcome email template
- `PasswordResetEmail.cshtml` - Password reset template
- `EmailVerification.cshtml` - Email verification template
- `OrderConfirmation.cshtml` - Order confirmation with items table
- `TeamInvitation.cshtml` - Team invitation template

### 4. **Service Implementation**
- `AppBlueprint.Infrastructure/Services/Email/RazorEmailTemplateService.cs`
  - Uses Razor view engine for server-side rendering
  - Logs rendering performance
  - Embeds templates as resources
  
- `AppBlueprint.Infrastructure/Services/TransactionEmailService.cs`
  - Updated to use template system
  - 5 email methods with full HTML rendering

### 5. **Preview Controller** (Dev Tools)
- `AppBlueprint.Presentation.ApiModule/Controllers/Dev/EmailPreviewController.cs`
  - Only available in DEBUG mode
  - Preview emails in browser at `http://localhost:5000/api/dev/email-preview/{template-name}`

## 🚀 Usage

### Sending Emails

```csharp
public class MyService
{
    private readonly TransactionEmailService _emailService;
    
    public async Task SendWelcomeEmail(string userEmail, string userName)
    {
        await _emailService.SendSignUpWelcomeEmail(
            from: "noreply@example.com",
            to: userEmail,
            userName: userName,
            siteName: "AppBlueprint",
            loginUrl: "https://app.example.com/login",
            supportEmail: "support@example.com"
        );
    }
}
```

### Rendering Templates Directly

```csharp
public class MyController
{
    private readonly IEmailTemplateService _templateService;
    
    public async Task<string> PreviewEmail()
    {
        var model = new WelcomeEmailModel(
            UserName: "John Doe",
            SiteName: "AppBlueprint",
            LoginUrl: "https://app.example.com/login",
            SupportEmail: "support@example.com"
        );
        
        string html = await _templateService.RenderTemplateAsync(model);
        return html;
    }
}
```

## 🔍 Local Preview

Preview templates in your browser during development:

```bash
# Start AppHost
dotnet run --project AppBlueprint.AppHost

# Open in browser:
http://localhost:5000/api/dev/email-preview/welcome
http://localhost:5000/api/dev/email-preview/password-reset
http://localhost:5000/api/dev/email-preview/email-verification
http://localhost:5000/api/dev/email-preview/order-confirmation
http://localhost:5000/api/dev/email-preview/team-invitation
```

### Customize Preview Data

Use query parameters to test different scenarios:

```
http://localhost:5000/api/dev/email-preview/welcome?userName=Jane&siteName=MyApp&loginUrl=https://myapp.com/login
```

## 📧 Available Templates

| Template | Purpose | Key Fields |
|----------|---------|------------|
| **WelcomeEmail** | Post-signup welcome | UserName, SiteName, LoginUrl |
| **PasswordResetEmail** | Password reset request | UserName, ResetUrl, ExpirationHours |
| **EmailVerification** | Verify email address | UserName, VerificationUrl, SiteName |
| **OrderConfirmation** | Order receipt | CustomerName, OrderId, Items, TotalAmount |
| **TeamInvitation** | Invite to team | InviteeName, InviterName, TeamName, AcceptUrl |

## 🎨 Template Styling

All templates use a shared `_Layout.cshtml` with:
- **Responsive design** (mobile-friendly)
- **Professional gradient headers**
- **Accessible color contrast**
- **Email client compatibility** (Gmail, Outlook, Apple Mail)
- **Inline styles** (no external CSS)

## 🔧 Adding New Templates

1. **Create Model** in `AppBlueprint.Contracts/Email/`:
```csharp
public sealed record MyNewEmailModel(
    string RecipientName,
    string CustomField
) : IEmailTemplateModel
{
    public string TemplateName => "MyNewEmail";
}
```

2. **Create Razor Template** in `AppBlueprint.Infrastructure/EmailTemplates/`:
```cshtml
@model AppBlueprint.Contracts.Email.MyNewEmailModel
@{
    Layout = "_Layout";
    ViewData["Title"] = "My Custom Email";
    ViewData["HeaderTitle"] = "Hello!";
}

<p>Hi <strong>@Model.RecipientName</strong>,</p>
<p>Your custom content here with @Model.CustomField</p>
```

3. **Add Method** to `TransactionEmailService`:
```csharp
public async Task SendMyNewEmail(
    string from,
    string to,
    string recipientName,
    string customField,
    CancellationToken cancellationToken = default)
{
    var model = new MyNewEmailModel(recipientName, customField);
    string htmlBody = await _templateService.RenderTemplateAsync(model, cancellationToken);
    
    var message = new EmailMessage
    {
        From = from,
        To = to,
        Subject = "My Subject",
        HtmlBody = htmlBody
    };
    
    await _resend.EmailSendAsync(message);
}
```

4. **Add Preview Endpoint** (optional):
```csharp
[HttpGet("my-new-email")]
[Produces("text/html")]
public async Task<IActionResult> PreviewMyNewEmail(
    [FromQuery] string recipientName = "John",
    [FromQuery] string customField = "Example")
{
    var model = new MyNewEmailModel(recipientName, customField);
    string html = await _templateService.RenderTemplateAsync(model);
    return Content(html, "text/html");
}
```

## ⚙️ Configuration

Resend email service configuration in `appsettings.json`:

```json
{
  "Resend": {
    "ApiKey": "re_123456789",
    "FromEmail": "noreply@yourdomain.com",
    "FromName": "AppBlueprint",
    "BaseUrl": "https://api.resend.com",
    "TimeoutSeconds": 30
  }
}
```

Or via environment variables:
```bash
APPBLUEPRINT_Resend__ApiKey=re_123456789
APPBLUEPRINT_Resend__FromEmail=noreply@yourdomain.com
```

## 🧪 Testing

Templates are automatically embedded as resources and loaded by the Razor view engine. No additional setup needed!

```csharp
// Unit test example
[Fact]
public async Task Should_Render_Welcome_Email()
{
    var model = new WelcomeEmailModel(
        "John Doe",
        "TestApp",
        "https://test.com/login",
        "support@test.com"
    );
    
    string html = await _templateService.RenderTemplateAsync(model);
    
    Assert.Contains("Welcome to TestApp", html);
    Assert.Contains("John Doe", html);
}
```

## 📊 Benefits

✅ **Full Control** - All templates in your codebase  
✅ **Version Control** - Templates tracked in Git  
✅ **Type Safe** - Compile-time checking for model properties  
✅ **Local Preview** - See changes instantly without sending emails  
✅ **Provider Agnostic** - Switch from Resend to SendGrid easily  
✅ **Testable** - Unit test template rendering logic  
✅ **Professional** - Responsive, accessible HTML emails  

## 🔒 Security

- Preview controller only available in `DEBUG` mode
- Production builds exclude preview endpoints
- No email sending in preview mode (render only)

## 📝 Notes

- Templates use **Razor syntax** (same as Blazor/MVC)
- Layout provides consistent branding across all emails
- All styles are **inline** for email client compatibility
- Templates support **ViewData** for shared values (site name, support email, etc.)

---

**Ready to use!** Start sending beautiful HTML emails with full local preview capability. 🎉
