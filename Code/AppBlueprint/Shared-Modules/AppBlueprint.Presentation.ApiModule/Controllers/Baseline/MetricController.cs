// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
//
// namespace AppBlueprint.Presentation.ApiModule.Controllers.Baseline;
//
// [Authorize]
// [ApiController]
// [Route("api/metrics")]
// [Produces("application/json")]
// public class MetricsController : ControllerBase
// {
//     private readonly IMetricsService _metricsService;
//
//     public MetricsController(IMetricsService metricsService)
//     {
//         _metricsService = metricsService ?? throw new ArgumentNullException(nameof(metricsService));
//     }
//
//     /// <summary>
//     /// Gets an overview of metrics.
//     /// </summary>
//     /// <returns>Metrics overview data.</returns>
//     [HttpGet("overview")]
//     [ProducesResponseType(typeof(MetricsOverviewDto), StatusCodes.Status200OK)]
//     [ProducesResponseType(StatusCodes.Status404NotFound)]
//     public async Task<ActionResult<MetricsOverviewDto>> GetOverview(CancellationToken cancellationToken)
//     {
//         var metrics = await _metricsService.GetOverviewAsync(cancellationToken);
//         if (metrics is null) return NotFound(new { Message = "Metrics data not available." });
//
//         return Ok(metrics);
//     }
//
//     /// <summary>
//     /// Gets user metrics.
//     /// </summary>
//     /// <returns>User metrics data.</returns>
//     [HttpGet("users")]
//     [ProducesResponseType(typeof(UserMetricsDto), StatusCodes.Status200OK)]
//     [ProducesResponseType(StatusCodes.Status404NotFound)]
//     public async Task<ActionResult<UserMetricsDto>> GetUserMetrics(CancellationToken cancellationToken)
//     {
//         var metrics = await _metricsService.GetUserMetricsAsync(cancellationToken);
//         if (metrics is null) return NotFound(new { Message = "User metrics data not available." });
//
//         return Ok(metrics);
//     }
//
//     /// <summary>
//     /// Gets subscription metrics.
//     /// </summary>
//     /// <returns>Subscription metrics data.</returns>
//     [HttpGet("subscriptions")]
//     [ProducesResponseType(typeof(SubscriptionMetricsDto), StatusCodes.Status200OK)]
//     [ProducesResponseType(StatusCodes.Status404NotFound)]
//     public async Task<ActionResult<SubscriptionMetricsDto>> GetSubscriptionMetrics(CancellationToken cancellationToken)
//     {
//         var metrics = await _metricsService.GetSubscriptionMetricsAsync(cancellationToken);
//         if (metrics is null) return NotFound(new { Message = "Subscription metrics data not available." });
//
//         return Ok(metrics);
//     }
// }
//
// public interface IMetricsService
// {
//     Task<MetricsOverviewDto> GetOverviewAsync(CancellationToken cancellationToken);
//     Task<UserMetricsDto> GetUserMetricsAsync(CancellationToken cancellationToken);
//     Task<SubscriptionMetricsDto> GetSubscriptionMetricsAsync(CancellationToken cancellationToken);
// } 
//
// public class MetricsOverviewDto
// {
//     public int TotalUsers { get; set; }
//     public int ActiveSubscriptions { get; set; }
//     public decimal TotalRevenue { get; set; }
// }
//
// public class UserMetricsDto
// {
//     public int ActiveUsers { get; set; }
//     public int NewUsersThisMonth { get; set; }
// }
//
// public class SubscriptionMetricsDto
// {
//     public int TotalSubscriptions { get; set; }
//     public int CanceledSubscriptions { get; set; }
// }

namespace AppBlueprint.Presentation.ApiModule.Controllers.Baseline;
