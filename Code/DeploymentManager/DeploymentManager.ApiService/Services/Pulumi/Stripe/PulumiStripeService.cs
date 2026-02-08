namespace DeploymentManager.ApiService.Services.Pulumi.Stripe;

public enum StripeResourceType // Stripe resource types
{
    Customer,
    Subscription,
    Invoice,
    PaymentIntent,
    PaymentMethod,
    Price,
    Product,
    TaxRate,
    Webhook
}

public class PulumiStripeService
{
}
