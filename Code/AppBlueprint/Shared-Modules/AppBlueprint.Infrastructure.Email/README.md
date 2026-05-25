# AppBlueprint.Infrastructure.Email

Transactional email infrastructure for the AppBlueprint SaaS platform using Resend and RazorLight templates.

## What this package provides

- `TransactionEmailService` — send welcome, order confirmation, and other transactional emails via Resend
- `RazorEmailTemplateService` — render Razor (.cshtml) email templates using RazorLight
- Built-in email templates: WelcomeEmail, OrderConfirmation, PasswordReset
- Template override pattern: deployed apps can supply custom templates to override the framework defaults

## Usage

```csharp
builder.Services.AddAppBlueprintEmail(
    builder.Configuration,
    customTemplatesPath: null); // or provide a path to custom templates
```

Set the following environment variables:
- `RESEND_APIKEY` or `RESEND_API_KEY` — Resend API key
- `RESEND_FROMEMAIL` or `RESEND_FROM_EMAIL` — sender email address
- `RESEND_FROMNAME` or `RESEND_FROM_NAME` — sender display name (optional)

## NuGet packages included

- `Resend`
- `RazorLight`
- `Newtonsoft.Json`
