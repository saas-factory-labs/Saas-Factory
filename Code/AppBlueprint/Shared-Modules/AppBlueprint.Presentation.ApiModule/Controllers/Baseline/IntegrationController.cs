using AppBlueprint.Application.Interfaces.UnitOfWork;
using AppBlueprint.Contracts.Baseline.Integrations.Requests;
using AppBlueprint.Contracts.Baseline.Integrations.Responses;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using AppBlueprint.SharedKernel;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppBlueprint.Presentation.ApiModule.Controllers.Baseline;

[Authorize]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/integration")]
[Produces("application/json")]
public class IntegrationController : BaseController
{
    private readonly IConfiguration _configuration;
    private readonly IIntegrationRepository _integrationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public IntegrationController(
        IConfiguration configuration,
        IIntegrationRepository integrationRepository,
        IUnitOfWork unitOfWork) : base(configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _integrationRepository =
            integrationRepository ?? throw new ArgumentNullException(nameof(integrationRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <summary>
    ///     Gets all integrations.
    /// </summary>
    /// <returns>List of integrations</returns>
    [HttpGet(ApiEndpoints.Integrations.GetAll)]
    [ProducesResponseType(typeof(IEnumerable<IntegrationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult<IEnumerable<IntegrationResponse>>> GetIntegrations(
        CancellationToken cancellationToken)
    {
        IEnumerable<IntegrationEntity> integrations = await _integrationRepository.GetAllAsync(cancellationToken);
        if (!integrations.Any()) return NotFound(new { Message = "No integrations found." });

        IEnumerable<IntegrationResponse> response = integrations.Select(integration => new IntegrationResponse
        {
            // Id = integration.Id,
            // Name = integration.Name,
            // Description = integration.Description,
            // Type = integration.Type,
            // Status = integration.Status
        });

        return Ok(response);
    }

    /// <summary>
    ///     Gets an integration by ID.
    /// </summary>
    /// <param name="id">Integration ID.</param>
    /// <param name="cancellationToken">Cancellation Token</param>    /// <returns>Integration</returns>
    [HttpGet(ApiEndpoints.Integrations.GetById)]
    [ProducesResponseType(typeof(IntegrationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Get(string id, CancellationToken cancellationToken)
    {
        IntegrationEntity? integration = await _integrationRepository.GetByIdAsync(id, cancellationToken);
        if (integration is null) return NotFound(new { Message = $"Integration with ID {id} not found." });

        // var response = new IntegrationResponse
        // {
        //     Id = integration.Id,
        //     Name = integration.Name,
        //     Description = integration.Description,
        //     Type = integration.Type,
        //     Status = integration.Status
        // };

        // return Ok(response);
        return Ok();
    }

    /// <summary>
    ///     Creates a new integration.
    /// </summary>
    /// <param name="integrationDto">Integration data transfer object.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Created integration.</returns>
    [HttpPost(ApiEndpoints.Integrations.Create)]
    [ProducesResponseType(typeof(IntegrationResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult> Post([FromBody] CreateIntegrationRequest integrationDto,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(integrationDto);

        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (string.IsNullOrEmpty(integrationDto.Title))
            return BadRequest(new { Message = "Title is required" });

        if (string.IsNullOrEmpty(integrationDto.Message))
            return BadRequest(new { Message = "Message is required" });

        var newIntegration = new IntegrationEntity
        {
            OwnerId = PrefixedUlid.Generate("usr"),
            Name = integrationDto.Title,
            ServiceName = "Generic", // Default service name, should be specified by client in real implementation
            ApiKeySecretReference = $"secret-{PrefixedUlid.Generate("ref")}",
            Description = integrationDto.Message
        };

        await _integrationRepository.AddAsync(newIntegration, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new IntegrationResponse
        {
            // Id = newIntegration.Id,
            // Name = newIntegration.Name,
            // Description = newIntegration.Description,
            // Type = newIntegration.Type,
            // Status = newIntegration.Status
        };

        return CreatedAtAction(nameof(Get), new { id = newIntegration.Id }, response);
    }

    /// <summary>
    ///     Updates an existing integration.
    /// </summary>
    /// <param name="id">Integration ID.</param>
    /// <param name="integrationDto">Integration data transfer object.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>No content.</returns>
    [HttpPut(ApiEndpoints.Integrations.UpdateById)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult Put(int id, [FromBody] UpdateIntegrationRequest integrationDto,
        CancellationToken cancellationToken)
    {
        // IntegrationEntity? existingIntegration = await _integrationRepository.GetByIdAsync(id);
        // if (existingIntegration is null) return NotFound(new { Message = $"Integration with ID {id} not found." });

        // existingIntegration.Name = integrationDto.Name;
        // existingIntegration.Description = integrationDto.Description;
        // existingIntegration.Type = integrationDto.Type;
        // existingIntegration.Status = integrationDto.Status;

        // _integrationRepository.Update(existingIntegration);
        // await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    ///     Deletes an integration by ID.
    /// </summary>
    /// <param name="id">Integration ID.</param>
    /// <param name="cancellationToken">Cancellation Token</param>    /// <returns>No content.</returns>
    [HttpDelete(ApiEndpoints.Integrations.DeleteById)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        IntegrationEntity? existingIntegration = await _integrationRepository.GetByIdAsync(id, cancellationToken);
        if (existingIntegration is null) return NotFound(new { Message = $"Integration with ID {id} not found." });

        await _integrationRepository.DeleteAsync(existingIntegration.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
