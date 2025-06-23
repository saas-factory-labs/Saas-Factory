using AppBlueprint.Application.Constants;
using AppBlueprint.TodoApp.Domain;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using AppBlueprint.Infrastructure.UnitOfWork;
using AppBlueprint.Presentation.ApiModule.Controllers;
using AppBlueprint.Presentation.ApiModule.Controllers.Baseline;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppBlueprint.TodoApp.Api;

[Authorize(Policy.Over18)]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/todos")]
[Produces("application/json")]
public class TodoController(
    ILogger<AccountController> logger,
    IAccountRepository accountRepository,
    IUnitOfWork unitOfWork,
    IConfiguration configuration)
    : BaseController(configuration)
{
    private readonly IConfiguration __configuration =
        configuration ?? throw new ArgumentNullException(nameof(configuration));

    private readonly IAccountRepository _accountRepository =
        accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));

    private readonly ILogger<AccountController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    // [HttpGet(ApiEndpoints.Accounts.GetAll)]
    [EndpointSummary("Get Todos V1")]
    [EndpointName("GetTodosV1")]
    [EndpointDescription("Gets all todos")]
    [ProducesResponseType(typeof(IEnumerable<TodoEntity>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult> GetTodosV1([FromHeader(Name = "Authorization")] string authorizationHeader,
        CancellationToken cancellationToken)
    {
        // call TodoService instead of repository here
        IEnumerable<AccountEntity?> accounts = await _accountRepository.GetAllAsync(cancellationToken);

        _logger.LogInformation("running GetTodosV1");

        // if (!accounts.Any()) return NotFound(new { Message = "No accounts found." });
        //
        // // ContractMapping.MapToAccountResponse();
        //
        // var accountDtOs = accounts.Select(account => new
        // {
        //     Id = account.AccountId,
        //     account.Email,
        // }).ToList();
        //
        // return Ok(accountDtOs);
        return Ok();
    }

    /// <summary>
    ///     Creates a new account.
    /// </summary>
    /// <param name="todo">Todo entity.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Created account.</returns>
    /// POST: /account
    // [HttpPost(ApiEndpoints.Todo.Create)]
    [ProducesResponseType(typeof(IEnumerable<TodoEntity>), StatusCodes.Status201Created)]
    public async Task<ActionResult> CreateTodo([FromBody] TodoEntity todo,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // await _unitOfWork.AccountRepository.AddAsync(account, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        // return CreatedAtAction(nameof(GetAccountsV1), new { id = account.AccountId }, account);
        return Ok();
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
    // [HttpPut(ApiEndpoints.Todos.UpdateById)]
    [ProducesResponseType(typeof(IEnumerable<TodoEntity>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoEntity>> UpdateAccount(int id, [FromBody] TodoEntity account,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // await _unitOfWork.AccountRepository.UpdateAsync(account, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    ///     Deletes an account by ID.
    /// </summary>
    /// <param name="id">Account ID.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>No content.</returns>    /// DELETE: /account/1
    // [HttpDelete(ApiEndpoints.Todos.DeleteById)]
    [ProducesResponseType(typeof(IEnumerable<TodoEntity>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteAccount(string id, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        await _unitOfWork.AccountRepository.DeleteAsync(id, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
}
