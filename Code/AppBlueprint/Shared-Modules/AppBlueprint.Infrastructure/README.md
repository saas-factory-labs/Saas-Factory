# AppBlueprint.Infrastructure

## Components

- Database Contexts (EF Core code-first)
    - ApplicationDBContext
    - BaselineDBContext
    - B2BDBContext
    - B2CDBContext
- Repositories for each database table
- Data Seeding for each database table
- Data Migrations applied to ApplicationDBContext
- Caching using Redis
- Databases (PostgreSQL, SQL Server, NoSQL, Redis)
- File/Object storage, eg. Azure Blob storage/Cloudflare R2
- Payment Services (Stripe integration)

## Payment Services

### Stripe Integration

The infrastructure layer includes comprehensive Stripe payment integration through the `StripeSubscriptionService` class.

#### Features:
- Customer creation and management
- Subscription lifecycle management (create, retrieve, cancel)
- Customer subscription listing
- Comprehensive error handling and logging
- Async/await pattern implementation

#### Configuration:
The Stripe API key should be configured in the connection strings section:
```json
{
  "ConnectionStrings": {
    "StripeApiKey": "sk_test_your_stripe_secret_key_here"
  }
}
```

#### Usage:
The service is automatically registered in the DI container and can be injected into controllers or other services:

```csharp
public class PaymentController : ControllerBase
{
    private readonly StripeSubscriptionService _stripeService;
    
    public PaymentController(StripeSubscriptionService stripeService)
    {
        _stripeService = stripeService;
    }
    
    // Use the service methods...
}
```

#### Available Methods:
- `CreateCustomerAsync(string email, string paymentMethodId)` - Creates a new Stripe customer
- `CreateSubscriptionAsync(string customerId, string priceId)` - Creates a subscription for a customer
- `GetSubscriptionAsync(string subscriptionId)` - Retrieves subscription details
- `CancelSubscriptionAsync(string subscriptionId)` - Cancels a subscription
- `GetCustomerSubscriptionsAsync(string customerId)` - Lists all subscriptions for a customer

#### Error Handling:
All methods include comprehensive error handling:
- Parameter validation with `ArgumentException`
- Stripe API errors wrapped in `InvalidOperationException`
- Proper logging of errors without exposing sensitive details

External systems
Databases
Messaging
Email providers
Storage services
Identity
System clock

EF Core
DbContext
Entity configurations
Repositories
Optimistic concurrency
Publishing Domain events
