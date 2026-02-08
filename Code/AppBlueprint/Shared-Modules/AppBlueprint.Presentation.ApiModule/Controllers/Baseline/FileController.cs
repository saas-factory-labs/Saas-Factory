using AppBlueprint.Contracts.Baseline.File.Requests;
using AppBlueprint.Contracts.Baseline.File.Responses;
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
[Route("api/v{version:apiVersion}/file")]
[Produces("application/json")]
public class FileController : BaseController
{
    private readonly IFileRepository _fileRepository;
    // Removed IUnitOfWork dependency for repository DI pattern

    public FileController(IFileRepository fileRepository, IConfiguration configuration)
        : base(configuration)
    {
        _fileRepository = fileRepository ?? throw new ArgumentNullException(nameof(fileRepository));
        // Removed IUnitOfWork assignment
    }

    /// <summary>
    ///     Retrieves all files in the system.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>List of file metadata (not actual file contents).</returns>
    /// <remarks>
    ///     Returns metadata for all files uploaded to the system.
    ///     Does not return actual file contents - use GetById for individual file download.
    ///     Requires authentication.
    /// </remarks>
    /// <response code="200">Returns the list of file metadata successfully.</response>
    /// <response code="404">No files found in the system.</response>
    /// <response code="401">User is not authenticated.</response>
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
    ///     Retrieves file metadata by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the file (e.g., "file_01HX...").</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>File metadata including name, path, extension, and owner information.</returns>
    /// <remarks>
    ///     Sample request:
    ///     <code>GET /api/v1/file/GetFile/file_01HX...</code>
    ///     Returns file metadata only. For actual file download, additional implementation needed.
    /// </remarks>
    /// <response code="200">Returns the file metadata successfully.</response>
    /// <response code="404">File with the specified ID was not found.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet("GetFile/{id}")]
    [ProducesResponseType(typeof(FileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Get(string id, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(id);

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
    ///     Creates a new file metadata entry in the system.
    /// </summary>
    /// <param name="fileDto">File creation request containing filename.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>The newly created file metadata with generated ID and path.</returns>
    /// <remarks>
    ///     Registers file metadata in the system. Actual file upload handling requires additional implementation.
    ///     Sample request:
    ///     <code>
    ///     POST /api/v1/file/CreateFile
    ///     {
    ///         "fileName": "document.pdf"
    ///     }
    ///     </code>
    ///     Automatically generates:
    ///     - Unique file ID with "file" prefix
    ///     - File extension from filename
    ///     - File path in /files/ directory
    ///     - Owner ID (currently hardcoded - needs user context)
    /// </remarks>
    /// <response code="201">File metadata created successfully. Returns the created file entry with its ID.</response>
    /// <response code="400">Invalid request data, validation failed, or filename missing.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpPost("CreateFile")]
    [ProducesResponseType(typeof(FileResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult> Post([FromBody] CreateFileRequest fileDto, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(fileDto);

        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (string.IsNullOrEmpty(fileDto.FileName))
            return BadRequest(new { Message = "FileName is required" });

        string fileExtension = Path.GetExtension(fileDto.FileName) ?? ".bin";
        var filePath = $"/files/{PrefixedUlid.Generate("file")}{fileExtension}";

        var newFile = new FileEntity
        {
            OwnerId = PrefixedUlid.Generate("usr"),
            FileExtension = fileExtension,
            FilePath = filePath,
            FileName = fileDto.FileName
        };

        await _fileRepository.AddAsync(newFile, cancellationToken);
        // If SaveChangesAsync is required, inject a service for it or handle in repository.

        return CreatedAtAction(nameof(Get), new { id = newFile.Id }, newFile);
    }

    /// <summary>
    ///     Updates an existing file's metadata (filename).
    /// </summary>
    /// <param name="id">The unique identifier of the file to update.</param>
    /// <param name="fileDto">Updated file data containing the new filename.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>No content on success.</returns>
    /// <remarks>
    ///     Updates file metadata only. Does not modify actual file contents.
    ///     Sample request:
    ///     <code>
    ///     PUT /api/v1/file/UpdateFile/file_01HX...
    ///     {
    ///         "fileName": "renamed-document.pdf"
    ///     }
    ///     </code>
    ///     FileName is required and cannot be empty.
    /// </remarks>
    /// <response code="204">File metadata updated successfully.</response>
    /// <response code="400">Filename is missing or empty.</response>
    /// <response code="404">File with the specified ID was not found.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpPut("UpdateFile/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Put(string id, [FromBody] UpdateFileRequest fileDto,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(fileDto);

        FileEntity? existingFile = await _fileRepository.GetByIdAsync(id, cancellationToken);
        if (existingFile is null) return NotFound(new { Message = $"File with ID {id} not found." });

        if (string.IsNullOrEmpty(fileDto.FileName))
            return BadRequest(new { Message = "FileName is required" });

        existingFile.FileName = fileDto.FileName;


        await _fileRepository.UpdateAsync(existingFile, cancellationToken);
        // If SaveChangesAsync is required, inject a service for it or handle in repository.

        return NoContent();
    }

    /// <summary>
    ///     Deletes a file and its metadata by unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the file to delete.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>No content on success.</returns>
    /// <remarks>
    ///     Sample request:
    ///     <code>DELETE /api/v1/file/DeleteFile/file_01HX...</code>
    ///     Warning: This operation permanently removes the file metadata.
    ///     Additional implementation needed to delete actual file from storage (S3/Azure/Cloudflare R2).
    /// </remarks>
    /// <response code="204">File deleted successfully.</response>
    /// <response code="404">File with the specified ID was not found.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpDelete("DeleteFile/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(id);

        FileEntity? existingFile = await _fileRepository.GetByIdAsync(id, cancellationToken);
        if (existingFile is null) return NotFound(new { Message = $"File with ID {id} not found." });

        await _fileRepository.DeleteAsync(existingFile, cancellationToken);
        // If SaveChangesAsync is required, inject a service for it or handle in repository.

        return NoContent();
    }
}
