using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;

namespace AppBlueprint.Infrastructure.Services;

/// <summary>
/// Service for sending transactional emails (welcome, order confirmation, etc.).
/// </summary>
public interface ITransactionEmailService
{
    /// <summary>
    /// Sends a welcome email to a newly signed-up user.
    /// </summary>
    Task SendSignUpWelcomeEmail(string from, string to, string siteName);

    /// <summary>
    /// Sends an order confirmation email to a customer.
    /// </summary>
    Task SendOrderConfirmationEmail(string from, string to, string orderId, CustomerEntity customerEntity);
}
