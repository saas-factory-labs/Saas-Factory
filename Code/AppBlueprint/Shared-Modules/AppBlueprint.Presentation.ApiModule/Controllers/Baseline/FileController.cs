using AppBlueprint.Application.Interfaces.Storage;
using AppBlueprint.Contracts.Baseline.File.Requests;
using AppBlueprint.Contracts.Baseline.File.Responses;
using AppBlueprint.SharedKernel;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.FileManagement;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppBlueprint.Presentation.ApiModule.Controllers.Baseline;

/// <summary>
/// File management controller integrating database metadata with Cloudflare R2 storage.
/// </summary>
[Authorize]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/file")]
[Produces("application/json")]
public class FileController : BaseController
{
    private readonly IFileRepository _fileRepository;
    private readonly IFileStorageService _storageService;

    public FileController(
        IFileRepository fileRepository,
        IFileStorageService storageService,
        IConfiguration configuration)
        : base(configuration)
    {
        _fileRepository = fileRepository ?? throw new ArgumentNullException(nameof(fileRepository));
        _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
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

        // Delete from R2 storage
        bool storageDeleted = await _storageService.DeleteAsync(existingFile.FilePath, cancellationToken);
        
        // Delete metadata from database
        await _fileRepository.DeleteAsync(existingFile, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Uploads a file to Cloudflare R2 storage with metadata tracking.
    /// </summary>
    /// <param name="file">The file to upload.</param>
    /// <param name="folder">Optional folder path (e.g., "avatars", "documents").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>File metadata with URL.</returns>
    [HttpPost("upload")]
    [ProducesResponseType(typeof(FileUploadResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [RequestSizeLimit(104_857_600)] // 100MB
    public async Task<ActionResult<FileUploadResponse>> Upload(
        IFormFile file,
        [FromQuery] string? folder,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(file);

        if (file.Length == 0)
        {
            return BadRequest(new { Message = "File is empty." });
        }

        try
        {
            // Generate unique key for R2
            string fileId = PrefixedUlid.Generate("file");
            string extension = Path.GetExtension(file.FileName);
            string key = string.IsNullOrEmpty(folder)
                ? $"{fileId}{extension}"
                : $"{folder.Trim('/')}/{fileId}{extension}";

            // Upload to R2
            FileUploadResult result;
            await using (Stream stream = file.OpenReadStream())
            {
                result = await _storageService.UploadAsync(
                    stream,
                    key,
                    file.ContentType,
                    cancellationToken);
            }

            // Save metadata to database
            var fileEntity = new FileEntity
            {
                Id = fileId,
                OwnerId = GetCurrentUserId(), // Implement this method in BaseController
                FileName = file.FileName,
                FileExtension = extension,
                FilePath = result.Key,
                FileSize = result.Size
            };

            await _fileRepository.AddAsync(fileEntity, cancellationToken);

            var response = new FileUploadResponse
            {
                Id = fileEntity.Id,
                FileName = fileEntity.FileName,
                Url = result.Url,
                Size = result.Size,
                ContentType = result.ContentType,
                UploadedAt = DateTime.UtcNow
            };

            return CreatedAtAction(nameof(Get), new { id = fileEntity.Id }, response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "File upload failed.", Error = ex.Message });
        }
    }

    /// <summary>
    /// Downloads a file from Cloudflare R2 storage.
    /// </summary>
    /// <param name="id">File ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>File stream.</returns>
    [HttpGet("download/{id}")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Download(string id, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(id);

        FileEntity? fileEntity = await _fileRepository.GetByIdAsync(id, cancellationToken);
        if (fileEntity is null)
        {
            return NotFound(new { Message = $"File with ID {id} not found." });
        }

        try
        {
            FileDownloadResult result = await _storageService.DownloadAsync(fileEntity.FilePath, cancellationToken);
            
            return File(result.Stream, result.ContentType, fileEntity.FileName);
        }
        catch (FileNotFoundException)
        {
            return NotFound(new { Message = "File not found in storage." });
        }
    }

    /// <summary>
    /// Generates a presigned URL for direct download from Cloudflare R2.
    /// </summary>
    /// <param name="id">File ID.</param>
    /// <param name="expiresInHours">URL expiration time in hours (default 1).</param>
    /// <returns>Presigned URL.</returns>
    [HttpGet("{id}/presigned-url")]
    [ProducesResponseType(typeof(PresignedUrlResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PresignedUrlResponse>> GetPresignedUrl(
        string id,
        [FromQuery] int expiresInHours = 1)
    {
        ArgumentNullException.ThrowIfNull(id);

        FileEntity? fileEntity = await _fileRepository.GetByIdAsync(id, default);
        if (fileEntity is null)
        {
            return NotFound(new { Message = $"File with ID {id} not found." });
        }

        string url = _storageService.GeneratePresignedUrl(
            fileEntity.FilePath,
            TimeSpan.FromHours(expiresInHours));

        var response = new PresignedUrlResponse
        {
            Url = url,
            ExpiresAt = DateTime.UtcNow.AddHours(expiresInHours)
        };

        return Ok(response);
    }

    /// <summary>
    /// Generates a presigned URL for direct upload from client to Cloudflare R2.
    /// </summary>
    /// <param name="fileName">File name.</param>
    /// <param name="contentType">Content type.</param>
    /// <param name="folder">Optional folder path.</param>
    /// <param name="expiresInMinutes">URL expiration time in minutes (default 15).</param>
    /// <returns>Presigned upload URL and key.</returns>
    [HttpPost("presigned-upload-url")]
    [ProducesResponseType(typeof(PresignedUploadUrlResponse), StatusCodes.Status200OK)]
    public ActionResult<PresignedUploadUrlResponse> GetPresignedUploadUrl(
        [FromQuery] string fileName,
        [FromQuery] string contentType,
        [FromQuery] string? folder,
        [FromQuery] int expiresInMinutes = 15)
    {
        ArgumentNullException.ThrowIfNull(fileName);
        ArgumentNullException.ThrowIfNull(contentType);

        string fileId = PrefixedUlid.Generate("file");
        string extension = Path.GetExtension(fileName);
        string key = string.IsNullOrEmpty(folder)
            ? $"{fileId}{extension}"
            : $"{folder.Trim('/')}/{fileId}{extension}";

        string url = _storageService.GeneratePresignedUploadUrl(
            key,
            contentType,
            TimeSpan.FromMinutes(expiresInMinutes));

        var response = new PresignedUploadUrlResponse
        {
            UploadUrl = url,
            Key = key,
            FileId = fileId,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expiresInMinutes)
        };

        return Ok(response);
    }

    private string GetCurrentUserId()
    {
        // TODO: Extract from JWT claims
        return User.FindFirst("sub")?.Value ?? "unknown-user";
    }
}
