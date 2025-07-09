using AppBlueprint.Contracts.Baseline.File.Requests;
using AppBlueprint.Contracts.Baseline.File.Responses;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.FileManagement;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using AppBlueprint.Application.Interfaces.UnitOfWork;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppBlueprint.Presentation.ApiModule.Controllers.Baseline;

[Authorize]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/file")]
[Produces("application/json")]
public class FileController : BaseController
{
    private readonly IConfiguration _configuration;
    private readonly IFileRepository _fileRepository;
    // Removed IUnitOfWork dependency for repository DI pattern

    public FileController(IFileRepository fileRepository, IConfiguration configuration)
        : base(configuration)
    {
        _fileRepository = fileRepository ?? throw new ArgumentNullException(nameof(fileRepository));
        // Removed IUnitOfWork assignment
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    ///     Gets all files.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of files</returns>
    [HttpGet("GetFiles")]
    [ProducesResponseType(typeof(IEnumerable<FileResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Get(CancellationToken cancellationToken)
    {
        IEnumerable<FileEntity> files = await _fileRepository.GetAllAsync(cancellationToken);
        if (!files.Any()) return NotFound(new { Message = "No files found." });

        IEnumerable<FileResponse> response = files.Select(file => new FileResponse());

        return Ok(response);
    }

    /// <summary>
    ///     Gets a file by ID.
    /// </summary>
    /// <param name="id">File ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>File</returns>
    [HttpGet("GetFile/{id}")]
    [ProducesResponseType(typeof(FileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Get(string id, CancellationToken cancellationToken)
    {
        FileEntity? file = await _fileRepository.GetByIdAsync(id, cancellationToken);
        if (file is null) return NotFound(new { Message = $"File with ID {id} not found." });

        var response = new FileResponse
        {
            // Name = file.Name,
            // Email = file.Email
        };

        return Ok(response);
    }

    /// <summary>
    ///     Creates a new file.
    /// </summary>
    /// <param name="fileDto">File data transfer object.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created file.</returns>
    [HttpPost("CreateFile")]
    [ProducesResponseType(typeof(FileResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult> Post([FromBody] CreateFileRequest fileDto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var newFile = new FileEntity
        {
            FileName = fileDto.FileName
        };

        await _fileRepository.AddAsync(newFile, cancellationToken);
        // If SaveChangesAsync is required, inject a service for it or handle in repository.

        return CreatedAtAction(nameof(Get), new { id = newFile.Id }, newFile);
    }

    /// <summary>
    ///     Updates an existing file.
    /// </summary>
    /// <param name="id">File ID.</param>
    /// <param name="fileDto">File data transfer object.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content.</returns>
    [HttpPut("UpdateFile/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]    public async Task<ActionResult> Put(string id, [FromBody] UpdateFileRequest fileDto,
        CancellationToken cancellationToken)
    {
        FileEntity? existingFile = await _fileRepository.GetByIdAsync(id, cancellationToken);
        if (existingFile is null) return NotFound(new { Message = $"File with ID {id} not found." });

        existingFile.FileName = fileDto.FileName;


        await _fileRepository.UpdateAsync(existingFile, cancellationToken);
        // If SaveChangesAsync is required, inject a service for it or handle in repository.

        return NoContent();
    }

    /// <summary>
    ///     Deletes a file by ID.
    /// </summary>
    /// <param name="id">File ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content.</returns>
    [HttpDelete("DeleteFile/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        FileEntity? existingFile = await _fileRepository.GetByIdAsync(id, cancellationToken);
        if (existingFile is null) return NotFound(new { Message = $"File with ID {id} not found." });

        await _fileRepository.DeleteAsync(existingFile, cancellationToken);
        // If SaveChangesAsync is required, inject a service for it or handle in repository.

        return NoContent();
    }
}
