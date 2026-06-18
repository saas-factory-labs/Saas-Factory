using System.Security.Claims;
using AppBlueprint.Application.Constants;
using AppBlueprint.Contracts.Baseline.Account.Requests;
using AppBlueprint.Contracts.Baseline.Account.Responses;
using AppBlueprint.Infrastructure.Persistence.DatabaseContexts.Baseline.Entities.Customer.Account;
using AppBlueprint.Infrastructure.Persistence.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.Infrastructure.Persistence.DatabaseContexts.Baseline.Entities.User.Profile;
using AppBlueprint.Infrastructure.Persistence.Repositories.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppBlueprint.Presentation.ApiModule.Controllers.Baseline;

[Authorize(AuthorizationPolicyNames.Over18)]
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

        // Guard clause: Account not found
        if (account is null)
            return NotFound(new { Message = "No account found." });

        // map to dto
        var accountDto = new
        {
            account.Id,
            account.Email,
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

        // Guard clause: No accounts found
        if (!accounts.Any())
            return NotFound(new { Message = "No accounts found." });

        var accountDtOs = accounts.Select(account => new
        {
            account.Id,
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
            account.Id,
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
    [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateAccount([FromBody] CreateAccountRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // SECURITY (OWASP A03/mass assignment): bind to a DTO, never the EF entity. Tenant and
        // owner identity are taken from the authenticated principal, not from the request body.
        string? callerTenantId = GetCallerTenantId();
        string callerUserId = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        if (string.IsNullOrEmpty(callerTenantId) || string.IsNullOrEmpty(callerUserId)) return Forbid();

        var account = new AccountEntity
        {
            Name = request.Name,
            Email = request.Email,
            CustomerType = request.CustomerType,
            IsActive = true,
            TenantId = callerTenantId,
            UserId = callerUserId,
            Owner = BuildOwnerFromClaims(callerUserId)
        };

        await _accountRepository.AddAsync(account, cancellationToken);

        var response = new AccountResponse
        {
            Id = account.Id,
            Email = account.Email
        };

        return CreatedAtAction(nameof(GetAccountByIdV1), new { idOrSlug = account.Id }, response);
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
    public async Task<ActionResult> UpdateAccount(string id, [FromBody] UpdateAccountRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (!ModelState.IsValid) return BadRequest(ModelState);

        AccountEntity existingAccount = await _accountRepository.GetByIdAsync(id, cancellationToken);

        // SECURITY (OWASP A01/anti-IDOR): only allow updates within the caller's own tenant.
        // The repository returns a sentinel "not-found" account rather than null.
        if (existingAccount.TenantId == "not-found" || !IsCallerTenant(existingAccount.TenantId))
            return NotFound(new { Message = $"Account with ID {id} not found." });

        // SECURITY (OWASP A03/mass assignment): only mutate the fields the DTO exposes.
        existingAccount.Name = request.Name;
        existingAccount.Email = request.Email;
        existingAccount.CustomerType = request.CustomerType;

        await _accountRepository.UpdateAsync(existingAccount, cancellationToken);

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

        AccountEntity existingAccount = await _accountRepository.GetByIdAsync(id, cancellationToken);
        if (existingAccount.TenantId == "not-found" || !IsCallerTenant(existingAccount.TenantId))
            return NotFound(new { Message = $"Account with ID {id} not found." });

        await _accountRepository.DeleteAsync(id, cancellationToken);

        return NoContent();
    }

    /// <summary>The tenant id resolved for the current request by TenantMiddleware.</summary>
    private string? GetCallerTenantId() => HttpContext.Items["TenantId"]?.ToString();

    /// <summary>True when <paramref name="tenantId"/> matches the caller's tenant.</summary>
    private bool IsCallerTenant(string tenantId)
        => string.Equals(GetCallerTenantId(), tenantId, StringComparison.Ordinal);

    /// <summary>Builds a minimal owner entity from the authenticated principal's claims.</summary>
    private UserEntity BuildOwnerFromClaims(string userId)
    {
        return new UserEntity
        {
            Email = User.FindFirst("email")?.Value ?? "unknown@example.com",
            FirstName = User.FindFirst("given_name")?.Value ?? "Unknown",
            LastName = User.FindFirst("family_name")?.Value ?? "User",
            UserName = User.Identity?.Name ?? userId,
            Profile = new ProfileEntity()
        };
    }
}
