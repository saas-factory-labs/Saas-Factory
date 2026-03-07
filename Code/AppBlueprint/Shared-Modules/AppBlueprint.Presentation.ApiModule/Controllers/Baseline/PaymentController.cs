using AppBlueprint.Application.Constants;
using AppBlueprint.Contracts.Baseline.Payment.Requests;
using AppBlueprint.Contracts.Baseline.Payment.Responses;
using AppBlueprint.Infrastructure.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppBlueprint.Presentation.ApiModule.Controllers.Baseline;

[Authorize]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/payments")]
[Produces("application/json")]
public class PaymentController : BaseController
{
    private readonly StripeSubscriptionService _stripeSubscriptionService;

    public PaymentController(
        IConfiguration configuration,
        StripeSubscriptionService stripeSubscriptionService)
        : base(configuration)
    {
        _stripeSubscriptionService = stripeSubscriptionService ?? throw new ArgumentNullException(nameof(stripeSubscriptionService));
    }

    /// <summary>
    /// Creates a Stripe subscription for a customer.
    /// </summary>
    /// <param name="dto">Customer ID and Stripe Price ID.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Created subscription details.</returns>
    [HttpPost(ApiEndpoints.Payments.CreateSubscription)]
    [ProducesResponseType(typeof(StripeSubscriptionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [MapToApiVersion(ApiVersions.V1)]
    public ActionResult<StripeSubscriptionResponse> CreateSubscription(
        [FromBody] CreateStripeSubscriptionRequest dto,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var subscription = _stripeSubscriptionService.CreateSubscription(dto.CustomerId, dto.PriceId);

        var response = new StripeSubscriptionResponse
        {
            SubscriptionId = subscription.Id,
            Status = subscription.Status,
            CustomerId = subscription.CustomerId,
            CurrentPeriodEnd = subscription.Items?.Data?.FirstOrDefault()?.CurrentPeriodEnd
        };

        return CreatedAtAction(nameof(CreateSubscription), response);
    }

    /// <summary>
    /// Cancels a Stripe subscription.
    /// </summary>
    /// <param name="dto">Subscription ID to cancel.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Cancelled subscription details.</returns>
    [HttpPost(ApiEndpoints.Payments.CancelSubscription)]
    [ProducesResponseType(typeof(StripeSubscriptionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [MapToApiVersion(ApiVersions.V1)]
    public ActionResult<StripeSubscriptionResponse> CancelSubscription(
        [FromBody] CancelStripeSubscriptionRequest dto,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var subscription = _stripeSubscriptionService.CancelSubscription(dto.SubscriptionId);

        var response = new StripeSubscriptionResponse
        {
            SubscriptionId = subscription.Id,
            Status = subscription.Status,
            CustomerId = subscription.CustomerId,
            CurrentPeriodEnd = subscription.Items?.Data?.FirstOrDefault()?.CurrentPeriodEnd
        };

        return Ok(response);
    }
}

