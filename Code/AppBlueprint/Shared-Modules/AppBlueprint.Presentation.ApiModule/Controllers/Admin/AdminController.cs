using AppBlueprint.Presentation.ApiModule.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using AppBlueprint.Application.Constants;
using AppBlueprint.Application.Interfaces.UnitOfWork;
using AppBlueprint.Contracts.Baseline.Account.Responses;
using AppBlueprint.Contracts.Baseline.Account.Requests;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Admin;

namespace AppBlueprint.Presentation.ApiModule.Controllers.Admin;

[Authorize(Roles = Roles.SaaSProviderAdmin)]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/admin")]
[Produces("application/json")]
public class AdminController : BaseController
{
    private readonly ILogger<AdminController> _logger;
    private readonly IAdminRepository _adminRepository;
    private readonly IUnitOfWork _unitOfWork;

    private static readonly Action<ILogger, string, Exception?> LogNoAdminsFound = LoggerMessage.Define<string>(
        LogLevel.Information,
        new EventId(1, nameof(GetAdminAccounts)),
        "No admin accounts found: {Message}");

    private static readonly Action<ILogger, string, Exception?> LogAdminNotFound = LoggerMessage.Define<string>(
        LogLevel.Information,
        new EventId(2, nameof(GetAdminAccount)),
        "Admin account with ID {Id} not found");

    private static readonly Action<ILogger, string, Exception?> LogAdminCreated = LoggerMessage.Define<string>(
        LogLevel.Information,
        new EventId(3, nameof(CreateAdminAccount)),
        "Created new admin account with ID {AccountId}");

    public AdminController(
        IConfiguration configuration,
        ILogger<AdminController> logger,
        IAdminRepository adminRepository,
        IUnitOfWork unitOfWork) : base(configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _adminRepository = adminRepository ?? throw new ArgumentNullException(nameof(adminRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <summary>
    /// Gets all admin accounts.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of admin accounts</returns>
    [HttpGet(ApiEndpoints.Admins.GetAll)]
    [ProducesResponseType(typeof(IEnumerable<AccountResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult<IEnumerable<CreateAccountRequest>>> GetAdminAccounts(
        CancellationToken cancellationToken)
    {
        IEnumerable<AdminEntity> admins = await _adminRepository.GetAllAsync();
        if (!admins.Any())
        {
            LogNoAdminsFound(_logger, "No accounts found.", null);
            return NotFound(new { Message = "No accounts found." });
        }

        var accountDtOs = admins.Select(account => new AccountResponse()
        {
            Id = account.Id,
            Email = account.Email,
        }).ToList();

        return Ok(accountDtOs);
    }

    /// <summary>
    /// Gets an admin account by ID.
    /// </summary>
    /// <param name="id">Admin account ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Admin account</returns>
    [HttpGet(ApiEndpoints.Admins.GetById)]
    [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult<AccountResponse>> GetAdminAccount(string id, CancellationToken cancellationToken)
    {
        AdminEntity? account = await _adminRepository.GetByIdAsync(id);
        if (account == null)
        {
            LogAdminNotFound(_logger, id, null);
            return NotFound(new { Message = $"Admin account with ID {id} not found." });
        }

        var accountDto = new AccountResponse()
        {
            Id = account.Id,
            Email = account.Email,
        };

        return Ok(accountDto);
    }

    /// <summary>
    /// Creates a new admin account.
    /// </summary>
    /// <param name="request">Account creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created admin account.</returns>
    [HttpPost(ApiEndpoints.Admins.Create)]
    [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status201Created)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult<AccountResponse>> CreateAdminAccount(
        [FromBody] CreateAccountRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var account = new AdminEntity
        {
            Email = request.Email,
            Role = Roles.SaaSProviderAdmin
        };

        await _adminRepository.AddAsync(account);
        await _unitOfWork.SaveChangesAsync();

        LogAdminCreated(_logger, account.Id, null);

        var response = new AccountResponse()
        {
            Id = account.Id,
            Email = account.Email,
        };

        return CreatedAtAction(nameof(GetAdminAccount), new { id = account.Id }, response);
    }
}



