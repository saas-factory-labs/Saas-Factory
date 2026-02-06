using AppBlueprint.Application.Interfaces;
using AppBlueprint.Application.Services;
using AppBlueprint.Contracts.Baseline.File.Requests;
using AppBlueprint.Contracts.Baseline.File.Responses;
using AppBlueprint.Presentation.ApiModule.Attributes;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Presentation.ApiModule.Controllers.Baseline;

/// <summary>
/// File storage controller for managing file uploads, downloads, and metadata.
/// Supports both public files (dating app images) and private files (CRM documents, rental agreements).
/// </summary>
[Authorize]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/filestorage")]
[Produces("application/json")]
public sealed class FileStorageController : BaseController
{
    private readonly IFileStorageService _fileStorageService;
    private readonly IFileValidationService _fileValidationService;
    private readonly ILogger<FileStorageController> _logger;

    public FileStorageController(
        IFileStorageService fileStorageService,
        IFileValidationService fileValidationService,
        ILogger<FileStorageController> logger,
        IConfiguration configuration)
        : base(configuration)
    {
        ArgumentNullException.ThrowIfNull(fileStorageService);
        ArgumentNullException.ThrowIfNull(fileValidationService);
        ArgumentNullException.ThrowIfNull(logger);

        _fileStorageService = fileStorageService;
        _fileValidationService = fileValidationService;
        _logger = logger;
    }

    /// <summary>
    /// Uploads a file to R2 storage with automatic tenant scoping.
    /// </summary>
    /// <param name="file">File to upload (multipart/form-data).</param>
    /// <param name="folder">Optional folder path for organizing files.</param>
    /// <param name="isPublic">Whether the file should be publicly accessible without authentication.</param>
    /// <param name="customMetadata">Optional JSON string containing custom metadata key-value pairs.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>File metadata including storage key and URL.</returns>
    /// <remarks>
    /// Sample request:
    /// <code>
    /// POST /api/v1/filestorage/upload
    /// Content-Type: multipart/form-data
    /// 
    /// file: [binary data]
    /// folder: "profile-images"
    /// isPublic: true
    /// customMetadata: { "user_id": "usr_123", "image_type": "avatar" }
    /// </code>
    /// </remarks>
    /// <response code="200">File uploaded successfully.</response>
    /// <response code="400">Invalid file or validation failed.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpPost("upload")]
    [RequireScope("write:files")]
    [ProducesResponseType(typeof(FileStorageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FileStorageResponse>> UploadFile(
        IFormFile file,
        [FromForm] string? folder = null,
        [FromForm] bool isPublic = false,
        [FromForm] string? customMetadata = null,
        CancellationToken cancellationToken = default)
    {
        // Guard clause: File validation
        if (file is null || file.Length == 0)
        {
            return BadRequest(new { Message = "No file provided or file is empty" });
        }

        // Parse custom metadata from JSON string
        Dictionary<string, string>? metadata = null;
        if (!string.IsNullOrWhiteSpace(customMetadata))
        {
            try
            {
                metadata = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(customMetadata);
            }
            catch (System.Text.Json.JsonException ex)
            {
                _logger.LogWarning(ex, "Failed to parse custom metadata JSON");
                return BadRequest(new { Message = "Invalid custom metadata JSON format" });
            }
        }

        // Validate file
        await using Stream fileStream = file.OpenReadStream();
        FileValidationResult? validationResult = await _fileValidationService.ValidateAsync(
            file.FileName,
            file.ContentType,
            file.Length,
            fileStream
        );

        // Guard clause: Validation failure
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("File validation failed: {FileName}, Errors: {Errors}",
                file.FileName, string.Join(", ", validationResult.Errors));
            return BadRequest(new { Message = "File validation failed", Errors = validationResult.Errors });
        }

        // Reset stream position for upload
        fileStream.Position = 0;

        // Upload file
        var uploadRequest = new UploadFileRequest(
            fileStream,
            file.FileName,
            file.ContentType,
            folder,
            isPublic,
            metadata
        );

        StoredFile? storedFile = await _fileStorageService.UploadAsync(uploadRequest, cancellationToken);

        var response = new FileStorageResponse
        {
            FileKey = storedFile.FileKey,
            OriginalFileName = storedFile.OriginalFileName,
            ContentType = storedFile.ContentType,
            SizeInBytes = storedFile.SizeInBytes,
            UploadedBy = storedFile.UploadedBy,
            UploadedAt = storedFile.UploadedAt,
            Folder = storedFile.Folder,
            IsPublic = storedFile.IsPublic,
            PublicUrl = storedFile.PublicUrl,
            CustomMetadata = storedFile.CustomMetadata
        };

        _logger.LogInformation("File uploaded successfully: {FileKey}, Size: {Size} bytes",
            storedFile.FileKey, storedFile.SizeInBytes);

        return Ok(response);
    }

    /// <summary>
    /// Downloads a file from R2 storage with tenant validation.
    /// </summary>
    /// <param name="fileKey">Unique file key in storage.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>File stream.</returns>
    /// <response code="200">File downloaded successfully.</response>
    /// <response code="404">File not found.</response>
    /// <response code="401">User is not authenticated or not authorized.</response>
    [RequireScope("read:files")]
    [HttpGet("download/{*fileKey}")]
    [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadFile(string fileKey, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileKey);

        try
        {
            FileDownloadResult? result = await _fileStorageService.DownloadAsync(fileKey, cancellationToken);
            return File(result.FileStream, result.ContentType, result.FileName);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized file access attempt: {FileKey}", fileKey);
            return NotFound(new { Message = "File not found or access denied" });
        }
    }

    /// <summary>
    /// Downloads a public file from R2 storage without authentication.
    /// Only works for files that were uploaded with isPublic=true.
    /// </summary>
    /// <param name="fileKey">Unique file key in storage.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>File stream.</returns>
    /// <response code="200">File downloaded successfully.</response>
    /// <response code="404">Public file not found.</response>
    [AllowAnonymous]
    [HttpGet("public/{*fileKey}")]
    [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadPublicFile(string fileKey, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileKey);

        try
        {
            FileDownloadResult? result = await _fileStorageService.DownloadPublicAsync(fileKey, cancellationToken);
            return File(result.FileStream, result.ContentType, result.FileName);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Public file not found: {FileKey}", fileKey);
            return NotFound(new { Message = "Public file not found" });
        }
    }

    /// <summary>
    /// Generates a pre-signed URL for secure time-limited file access.
    /// Used for private files (CRM documents, rental agreements).
    /// </summary>
    /// <param name="request">Pre-signed URL request with file key and expiry.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Pre-signed URL with expiration time.</returns>
    /// <response code="200">Pre-signed URL generated successfully.</response>
    /// <response code="404">File not found.</response>
    /// <response code="401">User is not authenticated or not authorized.</response>
    [RequireScope("read:files")]
    [HttpPost("presigned-url")]
    [ProducesResponseType(typeof(PreSignedUrlResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PreSignedUrlResponse>> GetPreSignedUrl(
        [FromBody] GetPreSignedUrlRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.FileKey);

        try
        {
            TimeSpan expiry = TimeSpan.FromSeconds(request.ExpirySeconds);
            string url = await _fileStorageService.GetPreSignedUrlAsync(request.FileKey, expiry, cancellationToken);

            var response = new PreSignedUrlResponse
            {
                Url = new Uri(url, UriKind.Absolute),
                ExpiresAt = DateTime.UtcNow.Add(expiry)
            };

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized pre-signed URL request: {FileKey}", request.FileKey);
            return NotFound(new { Message = "File not found or access denied" });
        }
    }

    /// <summary>
    /// Lists files for the current tenant with optional filtering.
    /// </summary>
    /// <param name="folder">Filter by folder.</param>
    /// <param name="fileNamePrefix">Filter by file name prefix.</param>
    /// <param name="maxResults">Maximum number of results.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of file metadata.</returns>
    /// <response code="200">Files retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    [RequireScope("read:files")]
    [HttpGet("list")]
    [ProducesResponseType(typeof(IEnumerable<FileStorageResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FileStorageResponse>>> ListFiles(
        [FromQuery] string? folder = null,
        [FromQuery] string? fileNamePrefix = null,
        [FromQuery] int? maxResults = null,
        CancellationToken cancellationToken = default)
    {
        var query = new FileListQuery(folder, fileNamePrefix, maxResults);
        IEnumerable<StoredFile>? files = await _fileStorageService.ListFilesAsync(query, cancellationToken);

        var response = files.Select(f => new FileStorageResponse
        {
            FileKey = f.FileKey,
            OriginalFileName = f.OriginalFileName,
            ContentType = f.ContentType,
            SizeInBytes = f.SizeInBytes,
            UploadedBy = f.UploadedBy,
            UploadedAt = f.UploadedAt,
            Folder = f.Folder,
            IsPublic = f.IsPublic,
            PublicUrl = f.PublicUrl,
            CustomMetadata = f.CustomMetadata
        });

        return Ok(response);
    }

    /// <summary>
    /// Deletes a file from R2 storage with tenant validation.
    /// </summary>
    /// <param name="fileKey">Unique file key in storage.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">File deleted successfully.</response>
    /// <response code="404">File not found.</response>
    /// <response code="401">User is not authenticated or not authorized.</response>
    [HttpDelete("{*fileKey}")]
    [RequireScope("write:files")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteFile(string fileKey, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileKey);

        // URL decode the fileKey (e.g., "tenant_xxx%2Ffolder%2Ffile.png" -> "tenant_xxx/folder/file.png")
        string decodedFileKey = Uri.UnescapeDataString(fileKey);

        try
        {
            await _fileStorageService.DeleteAsync(decodedFileKey, cancellationToken);
            _logger.LogInformation("File deleted successfully: {FileKey}", decodedFileKey);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized file deletion attempt: {FileKey}", decodedFileKey);
            return NotFound(new { Message = "File not found or access denied" });
        }
    }

    /// <summary>
    /// Deletes multiple files in a single operation.
    /// </summary>
    /// <param name="fileKeys">List of file keys to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Files deleted successfully.</response>
    /// <response code="400">Invalid request.</response>
    /// <response code="401">User is not authenticated or not authorized.</response>
    [HttpPost("delete-many")]
    [RequireScope("write:files")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteManyFiles(
        [FromBody] List<string> fileKeys,
        CancellationToken cancellationToken = default)
    {
        if (fileKeys is null || fileKeys.Count == 0)
        {
            return BadRequest(new { Message = "No file keys provided" });
        }

        try
        {
            await _fileStorageService.DeleteManyAsync(fileKeys, cancellationToken);
            _logger.LogInformation("Bulk deleted {Count} files", fileKeys.Count);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized bulk file deletion attempt");
            return BadRequest(new { Message = "Some files not found or access denied" });
        }
    }
}
