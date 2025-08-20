using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AppBlueprint.Infrastructure.Services;
using AppBlueprint.Contracts.Baseline.Payment.Requests;
using AppBlueprint.Contracts.Baseline.Payment.Responses;
using Stripe;

namespace AppBlueprint.Presentation.ApiModule.Controllers.Baseline;

[ApiController]
[Route("api/payment")]
[Produces("application/json")]
public class PaymentController : ControllerBase
{
    private readonly StripeSubscriptionService _stripeSubscriptionService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(
        StripeSubscriptionService stripeSubscriptionService,
        ILogger<PaymentController> logger)
    {
        _stripeSubscriptionService = stripeSubscriptionService ?? throw new ArgumentNullException(nameof(stripeSubscriptionService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Creates a new customer in Stripe.
    /// </summary>
    /// <param name="request">Customer creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created customer information.</returns>
    [HttpPost("create-customer")]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var customer = await _stripeSubscriptionService.CreateCustomerAsync(request.Email, request.PaymentMethodId);
            
            if (customer is null)
            {
                return BadRequest(new ProblemDetails 
                { 
                    Title = "Customer Creation Failed",
                    Detail = "Failed to create customer in payment system"
                });
            }

            var response = new CustomerResponse
            {
                Id = customer.Id,
                Email = customer.Email,
                Name = customer.Name,
                Phone = customer.PhoneNumber,
                CreatedAt = customer.CreatedAt
            };

            return CreatedAtAction(nameof(CreateCustomer), new { customerId = customer.Id }, response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid request parameters for customer creation");
            return BadRequest(new ProblemDetails 
            { 
                Title = "Invalid Request",
                Detail = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Failed to create customer");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails 
            { 
                Title = "Customer Creation Failed",
                Detail = "An error occurred while creating the customer"
            });
        }
    }

    /// <summary>
    /// Creates a subscription for a customer.
    /// </summary>
    /// <param name="request">Subscription creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created subscription information.</returns>
    [HttpPost("create-subscription")]
    [ProducesResponseType(typeof(SubscriptionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateSubscription([FromBody] CreateSubscriptionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var subscription = await _stripeSubscriptionService.CreateSubscriptionAsync(request.CustomerId, request.PriceId);

            var response = new SubscriptionResponse
            {
                Id = subscription.Id,
                CustomerId = subscription.CustomerId,
                Status = subscription.Status,
                PriceId = subscription.Items.Data.First().Price.Id,
                Amount = subscription.Items.Data.First().Price.UnitAmount ?? 0,
                Currency = subscription.Items.Data.First().Price.Currency,
                CreatedAt = subscription.Created,
                CurrentPeriodStart = subscription.CurrentPeriodStart,
                CurrentPeriodEnd = subscription.CurrentPeriodEnd
            };

            return CreatedAtAction(nameof(GetSubscription), new { subscriptionId = subscription.Id }, response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid request parameters for subscription creation");
            return BadRequest(new ProblemDetails 
            { 
                Title = "Invalid Request",
                Detail = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Failed to create subscription");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails 
            { 
                Title = "Subscription Creation Failed",
                Detail = "An error occurred while creating the subscription"
            });
        }
    }

    /// <summary>
    /// Retrieves a specific subscription.
    /// </summary>
    /// <param name="subscriptionId">The subscription ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Subscription information.</returns>
    [HttpGet("subscription/{subscriptionId}")]
    [ProducesResponseType(typeof(SubscriptionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSubscription(string subscriptionId, CancellationToken cancellationToken)
    {
        try
        {
            var subscription = await _stripeSubscriptionService.GetSubscriptionAsync(subscriptionId);

            var response = new SubscriptionResponse
            {
                Id = subscription.Id,
                CustomerId = subscription.CustomerId,
                Status = subscription.Status,
                PriceId = subscription.Items.Data.First().Price.Id,
                Amount = subscription.Items.Data.First().Price.UnitAmount ?? 0,
                Currency = subscription.Items.Data.First().Price.Currency,
                CreatedAt = subscription.Created,
                CanceledAt = subscription.CanceledAt,
                CurrentPeriodStart = subscription.CurrentPeriodStart,
                CurrentPeriodEnd = subscription.CurrentPeriodEnd
            };

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid subscription ID: {SubscriptionId}", subscriptionId);
            return BadRequest(new ProblemDetails 
            { 
                Title = "Invalid Request",
                Detail = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Failed to retrieve subscription: {SubscriptionId}", subscriptionId);
            return NotFound(new ProblemDetails 
            { 
                Title = "Subscription Not Found",
                Detail = "The requested subscription could not be found"
            });
        }
    }

    /// <summary>
    /// Retrieves subscriptions for a customer.
    /// </summary>
    /// <param name="customerId">The customer ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of customer subscriptions.</returns>
    [HttpGet("customer/{customerId}/subscriptions")]
    [ProducesResponseType(typeof(SubscriptionListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCustomerSubscriptions(string customerId, CancellationToken cancellationToken)
    {
        try
        {
            var subscriptions = await _stripeSubscriptionService.GetCustomerSubscriptionsAsync(customerId);

            var response = new SubscriptionListResponse
            {
                Subscriptions = subscriptions.Data.Select(s => new SubscriptionResponse
                {
                    Id = s.Id,
                    CustomerId = s.CustomerId,
                    Status = s.Status,
                    PriceId = s.Items.Data.First().Price.Id,
                    Amount = s.Items.Data.First().Price.UnitAmount ?? 0,
                    Currency = s.Items.Data.First().Price.Currency,
                    CreatedAt = s.Created,
                    CanceledAt = s.CanceledAt,
                    CurrentPeriodStart = s.CurrentPeriodStart,
                    CurrentPeriodEnd = s.CurrentPeriodEnd
                }).ToList(),
                HasMore = subscriptions.HasMore,
                TotalCount = subscriptions.Data.Count()
            };

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid customer ID: {CustomerId}", customerId);
            return BadRequest(new ProblemDetails 
            { 
                Title = "Invalid Request",
                Detail = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Failed to retrieve customer subscriptions: {CustomerId}", customerId);
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails 
            { 
                Title = "Failed to Retrieve Subscriptions",
                Detail = "An error occurred while retrieving customer subscriptions"
            });
        }
    }

    /// <summary>
    /// Cancels a subscription.
    /// </summary>
    /// <param name="request">Cancellation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Canceled subscription information.</returns>
    [HttpPost("cancel-subscription")]
    [ProducesResponseType(typeof(SubscriptionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CancelSubscription([FromBody] CancelSubscriptionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var subscription = await _stripeSubscriptionService.CancelSubscriptionAsync(request.SubscriptionId);

            var response = new SubscriptionResponse
            {
                Id = subscription.Id,
                CustomerId = subscription.CustomerId,
                Status = subscription.Status,
                PriceId = subscription.Items.Data.First().Price.Id,
                Amount = subscription.Items.Data.First().Price.UnitAmount ?? 0,
                Currency = subscription.Items.Data.First().Price.Currency,
                CreatedAt = subscription.Created,
                CanceledAt = subscription.CanceledAt,
                CurrentPeriodStart = subscription.CurrentPeriodStart,
                CurrentPeriodEnd = subscription.CurrentPeriodEnd
            };

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid request parameters for subscription cancellation");
            return BadRequest(new ProblemDetails 
            { 
                Title = "Invalid Request",
                Detail = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Failed to cancel subscription");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails 
            { 
                Title = "Subscription Cancellation Failed",
                Detail = "An error occurred while canceling the subscription"
            });
        }
    }
}
