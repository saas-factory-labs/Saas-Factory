// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.Logging;
// using Newtonsoft.Json;
// using Stripe;
//
// namespace AppBlueprint.Presentation.ApiModule.Controllers.Baseline;
//
// [Authorize (Roles = Roles.CustomerAdmin)]
// [ApiController]
// [Route("api/payment")]
// [Produces("application/json")]
// public class PaymentController : BaseController
// {
//     private readonly IConfiguration _configuration;
//     private readonly ILogger<PaymentController> _logger;
//
//     public PaymentController(ILogger<PaymentController> logger): base(configuration)
//     {
//         _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
//         _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//         
//     }
//
//     /// <summary>
//     /// Retrieves subscriptions.
//     /// </summary>
//     [HttpGet("get-subscriptions")]
//     [ProducesResponseType(StatusCodes.Status200OK)]
//     public async Task<IActionResult> GetSubscriptions(CancellationToken cancellationToken)
//     {
//         // Simulate retrieving subscriptions (replace with real implementation)
//         return Ok("Subscriptions....");
//     }
//
//     /// <summary>
//     /// Creates a subscription for the customer.
//     /// </summary>
//     [HttpPost("create-subscription")]
//     [ProducesResponseType(StatusCodes.Status200OK)]
//     public async Task<IActionResult> CreateSubscription(CancellationToken cancellationToken)
//     {
//         var options = new SubscriptionCreateOptions
//         {
//             Customer = "cus_9s6XGagagagagDTHzA66Po",
//             Items = new List<SubscriptionItemOptions>
//             {
//                 new SubscriptionItemOptions
//                 {
//                     Price = "subscription_plan_price_id",
//                 },
//             }
//         };
//         var service = new SubscriptionService();
//         service.Create(options);
//
//         return Ok("Subscription created successfully.");
//     }
//
//     /// <summary>
//     /// Cancels the subscription with the specified ID.
//     /// </summary>
//     /// <param name="req">Request containing subscription ID.</param>
//     [HttpPost("cancel-subscription")]
//     [ProducesResponseType(typeof(Subscription), StatusCodes.Status200OK)]
//     [ProducesResponseType(StatusCodes.Status400BadRequest)]
//     public ActionResult<Subscription> CancelSubscription([FromBody] CancelSubscriptionRequest req)
//     {
//         if (string.IsNullOrEmpty(req.Subscription))
//         {
//             return BadRequest("Subscription ID is required.");
//         }
//
//         var service = new SubscriptionService();
//         var subscription = service.Cancel(req.Subscription, null);
//         return Ok(subscription);
//     }
//
//     
// }
//
// public class CancelSubscriptionRequest
// {
//     [JsonProperty("subscriptionId")]
//     public string Subscription { get; set; }
// }

namespace AppBlueprint.Presentation.ApiModule.Controllers.Baseline;
