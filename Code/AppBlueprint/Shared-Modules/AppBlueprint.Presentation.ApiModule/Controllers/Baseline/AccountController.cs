using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using AppBlueprint.Infrastructure.UnitOfWork;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppBlueprint.Presentation.ApiModule.Controllers.Baseline;

[Authorize(Policy.Over18)]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[ApiVersion(ApiVersions.V2)]
[Route("api/v{version:apiVersion}/account")]
[Produces("application/json")]
public class AccountController(
    ILogger<AccountController> logger,
    IAccountRepository accountRepository,
    IUnitOfWork unitOfWork,
    IConfiguration configuration)
    : BaseController(configuration)
{
    private readonly IAccountRepository _accountRepository =
        accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));

    private readonly IConfiguration _configuration =
        configuration ?? throw new ArgumentNullException(nameof(configuration));

    private readonly ILogger<AccountController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    // private readonly IFeatureManager _featureManager = featureManager ?? throw new ArgumentNullException(nameof(featureManager));

    // [HttpGet(ApiEndpoints.Accounts.GetById)]    
    // [EndpointSummary("Get Accounts V1")]
    // [EndpointName("GetAccountsV1")]
    // [EndpointDescription("Gets all accounts for customers")]
    // [ProducesResponseType(typeof(IEnumerable<AccountEntity>), StatusCodes.Status200OK)]
    // [ProducesResponseType(StatusCodes.Status404NotFound)]
    // [MapToApiVersion(ApiVersions.V1)]
    // public async Task<ActionResult> GetAccountsV1([FromHeader(Name = "Authorization")] string authorizationHeader, string idOrSlug, CancellationToken cancellationToken)
    // {
    //     // IEnumerable<AccountEntity>? accounts = await _accountRepository.GetByIdAsync(, cancellationToken);        
    //     // if (!accounts.Any()) return NotFound(new { Message = "No accounts found." });
    //     //
    //     // // map to dto
    //     // var accountDtOs = accounts.Select(account => new
    //     // {
    //     //     Id = account.AccountId,
    //     //     Email = account.Email,
    //     // }).ToList();
    //     //
    //     // return Ok(accountDtOs); 
    // }

    [HttpGet(ApiEndpoints.Accounts.GetAll)]
    [EndpointSummary("Get Accounts V1")]
    [EndpointName("GetAccountsV1")]
    [EndpointDescription("Gets all accounts for customers")]
    [ProducesResponseType(typeof(IEnumerable<AccountEntity>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult> GetAccountsV1([FromHeader(Name = "Authorization")] string authorizationHeader,
        CancellationToken cancellationToken)
    {
        // call accountService instead of repository here
        IEnumerable<AccountEntity>? accounts = await _accountRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        if (!accounts.Any()) return NotFound(new { Message = "No accounts found." });        // ContractMapping.MapToAccountResponse();

        var accountDtOs = accounts.Select(account => new
        {
            Id = account.Id,
            account.Email
        }).ToList();

        return Ok(accountDtOs);
    }

    [HttpGet(ApiEndpoints.Accounts.GetAll)]
    [EndpointSummary("Get Accounts V2")]
    [EndpointName("GetAccountsV2")]
    [EndpointDescription("Gets all accounts for customers")]
    [ProducesResponseType(typeof(IEnumerable<AccountEntity>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V2)]
    public async Task<ActionResult> GetAccountsV2([FromHeader(Name = "Authorization")] string authorizationHeader,
        CancellationToken cancellationToken)
    {
        IEnumerable<AccountEntity>? accounts = await _accountRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        if (!accounts.Any()) return NotFound(new { Message = "No accounts found." });        // map to dto
        var accountDtOs = accounts.Select(account => new
        {
            Id = account.Id,
            account.Email
        }).ToList();

        return Ok(accountDtOs);
    }


    /// <summary>
    ///     Creates a new account.
    /// </summary>
    /// <param name="account">Account entity.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Created account.</returns>
    /// POST: /account
    [HttpPost(ApiEndpoints.Accounts.Create)]
    [ProducesResponseType(typeof(IEnumerable<AccountEntity>), StatusCodes.Status201Created)]
    public async Task<ActionResult> CreateAccount([FromBody] AccountEntity account, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        await _unitOfWork.AccountRepository.AddAsync(account, cancellationToken).ConfigureAwait(false);
        await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

        return CreatedAtAction(nameof(GetAccountsV1), new { id = account.Id }, account);
    }

    /// <summary>
    ///     Updates an existing account.
    /// </summary>
    /// <param name="id">Account ID.</param>
    /// <param name="account">Account entity.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>No content.</returns>
    /// PUT: /account/1
    [Authorize]
    [HttpPut(ApiEndpoints.Accounts.UpdateById)]
    [ProducesResponseType(typeof(IEnumerable<AccountEntity>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AccountEntity>> UpdateAccount(int id, [FromBody] AccountEntity account,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        await _unitOfWork.AccountRepository.UpdateAsync(account, cancellationToken).ConfigureAwait(false);
        await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

        return NoContent();
    }

    /// <summary>
    ///     Deletes an account by ID.
    /// </summary>    /// <param name="id">Account ID.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>No content.</returns>
    /// DELETE: /account/1
    [HttpDelete(ApiEndpoints.Accounts.DeleteById)]
    [ProducesResponseType(typeof(IEnumerable<AccountEntity>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteAccount(string id, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        await _unitOfWork.AccountRepository.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

        return NoContent();
    }
}
