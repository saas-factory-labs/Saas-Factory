// using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Logging;
// using Stripe;
//
// namespace AppBlueprint.Presentation.ApiModule.Controllers.Baseline;
//
// [Authorize]
// [ApiController]
// [Route("api/webhooks")]
// [Produces("application/json")]
// public class WebhooksController : BaseController
// {
//     // private readonly IWebhookRepository _webhookRepository;
//     // private readonly IUnitOfWork _unitOfWork;
//
//     /// <summary>
//     /// Webhooks for the App it self to handle things like payment, subscription and other types of events - but also for customers to register their own webhooks to facilitate additional use cases.
//     /// </summary>
//     /// <returns>List of webhooks</returns>
//     /// IWebhookRepository webhookRepository, IUnitOfWork unitOfWork
//     public WebhooksController()
//     {
//         // _webhookRepository = webhookRepository ?? throw new ArgumentNullException(nameof(webhookRepository));
//         // _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
//     }
//     
//     /// <summary>
//     /// Handles Stripe webhook events.
//     /// </summary>
//     [AllowAnonymous]
//     [HttpPost("Webhook")]
//     [ProducesResponseType(StatusCodes.Status200OK)]
//     [ProducesResponseType(StatusCodes.Status400BadRequest)]
//     public async Task<ActionResult> StripeWebhook(CancellationToken cancellationToken, ILogger<WebhooksController> _logger)
//     {
//         var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
//         
//         try
//         {
//             var webhookSecret = Environment.GetEnvironmentVariable("APPBLUEPRINT_STRIPE_WEBHOOK_SIGNING_SECRET", EnvironmentVariableTarget.User);
//
//            
//             
//             // verify the authenticity of the incoming webhook event and parse it
//             var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], webhookSecret);
//             _logger.LogInformation($"Webhook notification with type: {stripeEvent.Type} found for {stripeEvent.Id}");
//             
//             switch (stripeEvent.Type)
//             {
//                 case "customer.created":
//                     _logger.LogInformation($"Customer created for event {stripeEvent.Id}");
//                     break;
//                 
//                 case "customer.updated":
//                     _logger.LogInformation($"Customer updated for event {stripeEvent.Id}");
//                     break;
//                 
//                 case "customer.deleted":
//                     _logger.LogInformation($"Customer deleted for event {stripeEvent.Id}");
//                     break;                    
//                 
//                 case "billing_portal.configuration.created":
//                     _logger.LogInformation($"Billing portal configuration created for event {stripeEvent.Id}");
//                     break;
//                 case "checkout.session.completed":
//                     _logger.LogInformation($"Checkout session completed for event {stripeEvent.Id}");
//                     break;
//                 case "invoice.paid":
//                     _logger.LogInformation($"Invoice paid for event {stripeEvent.Id}");
//                     break;
//                 case "invoice.payment_failed":
//                     _logger.LogWarning($"Invoice payment failed for event {stripeEvent.Id}");
//                     break;
//                 case "customer.subscription.created":
//                 default:
//                     _logger.LogWarning($"Unhandled event type: {stripeEvent.Type}");
//                     break;
//             }
//         }
//         catch (Exception e)
//         {
//             _logger.LogError($"Webhook processing failed: {e}");
//             return BadRequest();
//         }
//
//         return Ok();
//     }
//
//     // /// <summary>
//     // /// Gets all webhooks.
//     // /// </summary>
//     // /// <returns>List of webhooks</returns>
//     // [HttpGet]
//     // [ProducesResponseType(typeof(IEnumerable<WebhookResponseDto>), StatusCodes.Status200OK)]
//     // [ProducesResponseType(StatusCodes.Status404NotFound)]
//     // public async Task<ActionResult<IEnumerable<WebhookResponseDto>>> Get(CancellationToken cancellationToken)
//     // {
//     //     var webhooks = await _webhookRepository.GetAllAsync(cancellationToken);
//     //     if (!webhooks.Any()) return NotFound(new { Message = "No webhooks found." });
//     //
//     //     var response = webhooks.Select(w => new WebhookResponseDto
//     //     {
//     //         Id = w.Id,
//     //         Url = w.Url,
//     //         Secret = w.Secret
//     //     });
//     //
//     //     return Ok(response);
//     // }
//
//     // /// <summary>
//     // /// Creates a new webhook.
//     // /// </summary>
//     // /// <param name="webhookDto">Webhook data transfer object.</param>
//     // /// <returns>Created webhook.</returns>
//     // [HttpPost]
//     // [ProducesResponseType(typeof(WebhookResponseDto), StatusCodes.Status201Created)]
//     // [ProducesResponseType(StatusCodes.Status400BadRequest)]
//     // public async Task<ActionResult> Post([FromBody] WebhookRequestDto webhookDto, CancellationToken cancellationToken)
//     // {
//     //     if (!ModelState.IsValid) return BadRequest(ModelState);
//     //
//     //     var newWebhook = new WebhookEntity
//     //     {
//     //         Url = webhookDto.Url,
//     //         // Event = webhookDto.Event,
//     //         CreatedAt = DateTime.UtcNow
//     //     };
//     //
//     //     await _webhookRepository.AddAsync(newWebhook, cancellationToken);
//     //     await _unitOfWork.SaveChangesAsync();
//     //
//     //     return CreatedAtAction(nameof(Get), new { id = newWebhook.Id }, newWebhook);
//     // }
//
//     // /// <summary>
//     // /// Updates an existing webhook.
//     // /// </summary>
//     // /// <param name="id">Webhook ID.</param>
//     // /// <param name="webhookDto">Webhook data transfer object.</param>
//     // /// <returns>No content.</returns>
//     // [HttpPut("{id}")]
//     // [ProducesResponseType(StatusCodes.Status204NoContent)]
//     // [ProducesResponseType(StatusCodes.Status404NotFound)]
//     // public async Task<ActionResult> Put(int id, [FromBody] WebhookRequestDto webhookDto, CancellationToken cancellationToken)
//     // {
//     //     var existingWebhook = await _webhookRepository.GetByIdAsync(id, cancellationToken);
//     //     if (existingWebhook is null) return NotFound(new { Message = $"Webhook with ID {id} not found." });
//     //
//     //     // existingWebhook.Url = webhookDto.Url;
//     //     // existingWebhook.Event = webhookDto.Event;
//     //     //
//     //     // _webhookRepository.Update(existingWebhook);
//     //     // await _unitOfWork.SaveChangesAsync(cancellationToken);
//     //
//     //     return NoContent();
//     // }
//     //
//     // /// <summary>
//     // /// Deletes a webhook by ID.
//     // /// </summary>
//     // /// <param name="id">Webhook ID.</param>
//     // /// <returns>No content.</returns>
//     // [HttpDelete("{id}")]
//     // [ProducesResponseType(StatusCodes.Status204NoContent)]
//     // [ProducesResponseType(StatusCodes.Status404NotFound)]
//     // public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
//     // {
//     //     var webhook = await _webhookRepository.GetByIdAsync(id, cancellationToken);
//     //     if (webhook is null) return NotFound(new { Message = $"Webhook with ID {id} not found." });
//     //
//     //     _webhookRepository.Delete(webhook);
//     //     await _unitOfWork.SaveChangesAsync();
//     //
//     //     return NoContent();
//     // }
// }
//
// public interface IWebhookRepository
// {
//     Task<IEnumerable<WebhookEntity>> GetAllAsync(CancellationToken cancellationToken);
//     Task<WebhookEntity> GetByIdAsync(int id, CancellationToken cancellationToken);
//     Task AddAsync(WebhookEntity webhook, CancellationToken cancellationToken);
//     void Update(WebhookEntity webhook);
//     void Delete(WebhookEntity webhook);
// }
//
// public class WebhookRequestDto
// {
//     public string Url { get; set; }
//     public string Event { get; set; }
// }
//
// public class WebhookResponseDto
// {
//     public int Id { get; set; }
//     public string Url { get; set; }
//     public string Secret { get; set; }
// }

namespace AppBlueprint.Presentation.ApiModule.Controllers.Baseline;
