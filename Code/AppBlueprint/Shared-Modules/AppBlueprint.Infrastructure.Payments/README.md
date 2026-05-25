# AppBlueprint.Infrastructure.Payments

Stripe payment and webhook infrastructure for the AppBlueprint SaaS platform.

## What this package provides

- Stripe subscription management (`StripeSubscriptionService`)
- Stripe webhook processing with signature verification and idempotency (`StripeWebhookService`)
- Stripe webhook event repository registration
- Pre-built handlers for common Stripe events (payment intents, subscriptions, invoices, checkout sessions)

## Usage

```csharp
builder.Services.AddAppBlueprintPayments(builder.Configuration);
```

Set the following environment variables or configuration keys:
- `Stripe:ApiKey` — Stripe secret API key
- `STRIPE_WEBHOOK_SECRET` or `Stripe:WebhookSecret` — Stripe webhook endpoint secret

## NuGet packages included

- `Stripe.net`
