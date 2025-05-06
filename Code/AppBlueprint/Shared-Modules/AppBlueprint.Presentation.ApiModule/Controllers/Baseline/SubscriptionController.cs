//using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;
//using AppBlueprint.Infrastructure.Repositories.Interfaces;
//using AppBlueprint.Infrastructure.UnitOfWork;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace AppBlueprint.Presentation.ApiModule.Controllers.Baseline;

//[Authorize(Roles = Roles.CustomerAdmin)]
//[ApiController]
//[Route("api/subscriptions")]
//[Produces("application/json")]
//public class SubscriptionController : BaseController
//{
//    private readonly IConfiguration _configuration;
//    private readonly ISubscriptionRepository _subscriptionRepository;
//    private readonly IUnitOfWork _unitOfWork;

//    public SubscriptionController(IConfiguration configuration, ISubscriptionRepository subscriptionRepository, IUnitOfWork unitOfWork) : base(configuration)
//    {
//        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
//        _subscriptionRepository = subscriptionRepository ?? throw new ArgumentNullException(nameof(subscriptionRepository));
//        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
//    }

//    /// <summary>
//    /// Gets all subscriptions.
//    /// </summary>
//    /// <returns>List of subscriptions</returns>
//    [HttpGet(ApiEndpoints.Subscriptions.GetAll)]
//    [ProducesResponseType(typeof(IEnumerable<SubscriptionResponseDto>), StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status404NotFound)]
//    public async Task<ActionResult<IEnumerable<SubscriptionResponseDto>>> Get(CancellationToken cancellationToken)
//    {
//        IEnumerable<SubscriptionEntity>? subscriptions = await _subscriptionRepository.GetAllAsync(cancellationToken);
//        if (!subscriptions.Any()) return NotFound(new { Message = "No subscriptions found." });

//        IEnumerable<SubscriptionResponseDto>? response = subscriptions.Select(subscription => new SubscriptionResponseDto
//        {
//            // Id = subscription.Id,
//            // Name = subscription.Name
//        });

//        return Ok(response);
//    }

//    /// <summary>
//    /// Gets a subscription by ID.
//    /// </summary>
//    /// <param name="id">Subscription ID.</param>
//    /// <param name="cancellationToken">Cancellation Token</param>
//    /// <returns>Subscription</returns>
//    [HttpGet(ApiEndpoints.Subscriptions.GetById)]
//    [ProducesResponseType(typeof(SubscriptionResponseDto), StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status404NotFound)]
//    public async Task<ActionResult<SubscriptionResponseDto>> Get(int id, CancellationToken cancellationToken)
//    {
//        SubscriptionEntity? subscription = await _subscriptionRepository.GetByIdAsync(id, cancellationToken);
//        if (subscription is null) return NotFound(new { Message = $"Subscription with ID {id} not found." });

//        var response = new SubscriptionResponseDto
//        {
//            // Id = subscription.Id,
//            // Name = subscription.Name
//        };

//        return Ok(response);
//    }

//    /// <summary>
//    /// Creates a new subscription.
//    /// </summary>
//    /// <param name = "subscriptionDto" > Subscription data transfer object.</param>
//    /// <returns>Created subscription.</returns>
//    [HttpPost]
//    [ProducesResponseType(typeof(SubscriptionResponseDto), StatusCodes.Status201Created)]
//    [ProducesResponseType(StatusCodes.Status400BadRequest)]
//    public async Task<ActionResult> Post([FromBody] SubscriptionRequestDto subscriptionDto, CancellationToken cancellationToken)
//    {
//        if (!ModelState.IsValid) return BadRequest(ModelState);

//        // var newSubscription = new SubscriptionModel
//        // {
//        //     Name = subscriptionDto.Name
//        // };
//        //
//        // await _subscriptionRepository.AddAsync(newSubscription, cancellationToken);
//        // await _unitOfWork.SaveChangesAsync(cancellationToken);

//        return Created();

//        //return CreatedAtAction(nameof(Get), new { id =  }, newSubscription);
//    }

//    /// <summary>
//    /// Updates an existing subscription.
//    /// </summary>
//    /// <param name="id">Subscription ID.</param>
//    /// <param name="subscriptionDto">Subscription data transfer object.</param>
//    /// <param name="cancellationToken">Cancellation Token</param>
//    /// <returns>No content.</returns>
//    [HttpPut(ApiEndpoints.Subscriptions.UpdateById)]
//    [ProducesResponseType(StatusCodes.Status204NoContent)]
//    [ProducesResponseType(StatusCodes.Status404NotFound)]
//    public async Task<ActionResult> Put(int id, [FromBody] SubscriptionRequestDto subscriptionDto, CancellationToken cancellationToken)
//    {
//        SubscriptionEntity? existingSubscription = await _subscriptionRepository.GetByIdAsync(id, cancellationToken);
//        if (existingSubscription is null) return NotFound(new { Message = $"Subscription with ID {id} not found." });

//        // existingSubscription.n = subscriptionDto.Name;

//        // _subscriptionRepository.Update(existingSubscription);
//        // await _unitOfWork.SaveChangesAsync(cancellationToken);

//        return NoContent();
//    }

//    /// <summary>
//    /// Deletes a subscription by ID.
//    /// </summary>
//    /// <param name="id">Subscription ID.</param>
//    /// <returns>No content.</returns>
//    // [HttpDelete("{id}")]
//    // [ProducesResponseType(StatusCodes.Status204NoContent)]
//    // [ProducesResponseType(StatusCodes.Status404NotFound)]
//    // public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
//    // {
//    //     var existingSubscription = await _subscriptionRepository.GetByIdAsync(id, cancellationToken);
//    //     if (existingSubscription is null) return NotFound(new { Message = $"Subscription with ID {id} not found." });
//    //
//    //     _subscriptionRepository.Delete(existingSubscription);
//    //     await _unitOfWork.SaveChangesAsync(cancellationToken);
//    //
//    //     return NoContent();
//    // }
//}

namespace AppBlueprint.Presentation.ApiModule.Controllers.Baseline;
