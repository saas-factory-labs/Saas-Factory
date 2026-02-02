using AppBlueprint.Application.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Presentation.ApiModule.Controllers.Webhooks;

/// <summary>
/// Controller for handling Stripe webhook events.
/// This endpoint must be publicly accessible (no authentication) per Stripe's webhook requirements.
/// Security is enforced via signature verification in the service layer.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/webhooks/stripe")]
[Produces("application/json")]
[AllowAnonymous]
public sealed class StripeWebhookController : ControllerBase
{
    private readonly IStripeWebhookService _webhookService;
    private readonly ILogger<StripeWebhookController> _logger;

    public StripeWebhookController(
        IStripeWebhookService webhookService,
        ILogger<StripeWebhookController> logger)
    {
        ArgumentNullException.ThrowIfNull(webhookService);
        ArgumentNullException.ThrowIfNull(logger);

        _webhookService = webhookService;
        _logger = logger;
    }

    /// <summary>
    /// Receives and processes Stripe webhook events.
    /// This endpoint must be publicly accessible (no [Authorize] attribute).
    /// Stripe will POST events to this endpoint with a signature header for verification.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>200 OK if event was processed or is a duplicate, 400 Bad Request if signature verification fails.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> HandleWebhook(CancellationToken cancellationToken)
    {
        try
        {
            // Enable buffering so the request body can be read multiple times if needed
            Request.EnableBuffering();
            
            // Read the raw body (required for signature verification)
            using var reader = new StreamReader(Request.Body, leaveOpen: true);
            string json = await reader.ReadToEndAsync(cancellationToken);
            
            // Reset the stream position for potential re-reading
            Request.Body.Position = 0;

            // Get the Stripe-Signature header
            string? signatureHeader = Request.Headers["Stripe-Signature"].FirstOrDefault();

            if (string.IsNullOrEmpty(signatureHeader))
            {
                _logger.LogWarning("Webhook received without Stripe-Signature header");
                return BadRequest(new { Error = "Missing Stripe-Signature header" });
            }

            if (string.IsNullOrEmpty(json))
            {
                _logger.LogWarning("Webhook received with empty body");
                return BadRequest(new { Error = "Empty request body" });
            }

            // Process the webhook
            WebhookProcessingResult result = await _webhookService.ProcessWebhookAsync(
                json,
                signatureHeader,
                cancellationToken
            );

            if (!result.Success)
            {
                _logger.LogWarning(
                    "Webhook processing failed: {ErrorMessage}",
                    result.ErrorMessage
                );
                return BadRequest(new { Error = result.ErrorMessage });
            }

            // Return 200 OK even for duplicates (as required by Stripe)
            if (result.WasDuplicate)
            {
                _logger.LogInformation(
                    "Duplicate webhook event: {EventId}",
                    result.EventId
                );
            }

            return Ok(new
            {
                Message = "Webhook processed successfully",
                EventId = result.EventId,
                EventType = result.EventType,
                WasDuplicate = result.WasDuplicate
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error processing webhook"
            );

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { Error = "Internal server error" }
            );
        }
    }

    /// <summary>
    /// Gets recent webhook events (for testing/debugging).
    /// Requires authentication.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecentEvents(CancellationToken cancellationToken)
    {
        IEnumerable<WebhookEventDetails> events = await _webhookService.GetRecentWebhookEventsAsync(50, cancellationToken);
        return Ok(events);
    }

    /// <summary>
    /// Gets a specific webhook event by ID (for testing/debugging).
    /// Requires authentication.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEventById(string id, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(id);

        WebhookEventDetails? webhookEvent = await _webhookService.GetWebhookEventByIdAsync(id, cancellationToken);

        if (webhookEvent is null)
        {
            return NotFound(new { Error = "Webhook event not found" });
        }

        return Ok(webhookEvent);
    }
}
