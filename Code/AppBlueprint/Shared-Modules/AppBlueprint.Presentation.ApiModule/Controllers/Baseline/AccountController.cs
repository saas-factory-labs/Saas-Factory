using AppBlueprint.Application.Interfaces.UnitOfWork;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;

namespace AppBlueprint.Presentation.ApiModule.Controllers.Baseline;

[Authorize(AuthorizationPolicies.Over18)]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[ApiVersion(ApiVersions.V2)]
[Route("api/v{version:apiVersion}/account")]
[Produces("application/json")]
public class AccountController : BaseController
{
    private readonly IAccountRepository _accountRepository;

    public AccountController(
        IAccountRepository accountRepository,
        IConfiguration configuration)
        : base(configuration)
    {
        _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
    }

    [HttpGet(ApiEndpoints.Accounts.GetById)]
    [EndpointSummary("Get Account by ID V1")]
    [EndpointName("GetAccountByIdV1")]
    [EndpointDescription("Gets a single account by ID or slug")]
    [ProducesResponseType(typeof(AccountEntity), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult> GetAccountByIdV1([FromHeader(Name = "Authorization")] string authorizationHeader, string idOrSlug, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(idOrSlug);

        AccountEntity? account = await _accountRepository.GetByIdAsync(idOrSlug, cancellationToken);
        if (account is null) return NotFound(new { Message = "No account found." });

        // map to dto
        var accountDto = new
        {
            Id = account.Id,
            Email = account.Email,
        };

        return Ok(accountDto);
    }

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
        IEnumerable<AccountEntity>? accounts = await _accountRepository.GetAllAsync(cancellationToken);
        if (!accounts.Any()) return NotFound(new { Message = "No accounts found." });

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
        IEnumerable<AccountEntity>? accounts = await _accountRepository.GetAllAsync(cancellationToken);
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
    [ProducesResponseType(typeof(AccountEntity), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateAccount([FromBody] AccountEntity account, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(account);
        if (!ModelState.IsValid) return BadRequest(ModelState);

        await _accountRepository.AddAsync(account, cancellationToken);
        // If SaveChangesAsync is required, inject a service for it or handle in repository.

        return CreatedAtAction(nameof(GetAccountByIdV1), new { id = account.Id }, account);
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateAccount(string id, [FromBody] AccountEntity account,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        await _accountRepository.UpdateAsync(account, cancellationToken);
        // If SaveChangesAsync is required, inject a service for it or handle in repository.

        return NoContent();
    }

    /// <summary>
    ///     Deletes an account by ID.
    /// </summary>
    /// <param name="id">Account ID.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>No content.</returns>
    /// DELETE: /account/1
    [HttpDelete(ApiEndpoints.Accounts.DeleteById)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteAccount(string id, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        await _accountRepository.DeleteAsync(id, cancellationToken);
        // If SaveChangesAsync is required, inject a service for it or handle in repository.

        return NoContent();
    }
}
