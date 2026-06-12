using AppBlueprint.Application.Options;
using AppBlueprint.Application.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AppBlueprint.Infrastructure.Storage;

/// <summary>
/// Implementation of file validation service with security checks.
/// </summary>
public sealed class FileValidationService : IFileValidationService
{
    private readonly ILogger<FileValidationService> _logger;
    private readonly CloudflareR2Options _options;

    // Allowed image types for dating app profiles
    private static readonly HashSet<string> AllowedImageTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/jpg",
        "image/png",
        "image/webp",
        "image/gif"
    };

    // Allowed document types for CRM and property rentals
    private static readonly HashSet<string> AllowedDocumentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "application/pdf",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "text/plain",
        "text/csv"
    };

    // Allowed video types for property tours
    private static readonly HashSet<string> AllowedVideoTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "video/mp4",
        "video/webm",
        "video/quicktime"
    };

    private static readonly HashSet<string> AllowedContentTypes = new(
        AllowedImageTypes.Concat(AllowedDocumentTypes).Concat(AllowedVideoTypes)
    );

    // Dangerous file extensions
    private static readonly HashSet<string> DangerousExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".exe", ".bat", ".cmd", ".sh", ".ps1", ".vbs", ".js", ".jar", ".dll", ".so", ".dylib"
    };

    // Maps a file extension to the content types it is allowed to be declared as. Prevents
    // type-confusion uploads (e.g. an executable renamed to .png, or a .pdf served as image/jpeg).
    private static readonly Dictionary<string, HashSet<string>> ExtensionContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        [".jpg"] = new(StringComparer.OrdinalIgnoreCase) { "image/jpeg", "image/jpg" },
        [".jpeg"] = new(StringComparer.OrdinalIgnoreCase) { "image/jpeg", "image/jpg" },
        [".png"] = new(StringComparer.OrdinalIgnoreCase) { "image/png" },
        [".webp"] = new(StringComparer.OrdinalIgnoreCase) { "image/webp" },
        [".gif"] = new(StringComparer.OrdinalIgnoreCase) { "image/gif" },
        [".pdf"] = new(StringComparer.OrdinalIgnoreCase) { "application/pdf" },
        [".doc"] = new(StringComparer.OrdinalIgnoreCase) { "application/msword" },
        [".docx"] = new(StringComparer.OrdinalIgnoreCase) { "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
        [".xls"] = new(StringComparer.OrdinalIgnoreCase) { "application/vnd.ms-excel" },
        [".xlsx"] = new(StringComparer.OrdinalIgnoreCase) { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
        [".txt"] = new(StringComparer.OrdinalIgnoreCase) { "text/plain" },
        [".csv"] = new(StringComparer.OrdinalIgnoreCase) { "text/csv", "text/plain" },
        [".mp4"] = new(StringComparer.OrdinalIgnoreCase) { "video/mp4" },
        [".webm"] = new(StringComparer.OrdinalIgnoreCase) { "video/webm" },
        [".mov"] = new(StringComparer.OrdinalIgnoreCase) { "video/quicktime" }
    };

    /// <summary>
    /// Returns true when the declared <paramref name="contentType"/> is a valid match for the
    /// file <paramref name="extension"/>. Unknown extensions return false (rejected by default).
    /// </summary>
    public static bool IsExtensionConsistentWithContentType(string extension, string contentType)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(extension);
        ArgumentException.ThrowIfNullOrWhiteSpace(contentType);

        return ExtensionContentTypes.TryGetValue(extension, out HashSet<string>? allowed)
               && allowed.Contains(contentType);
    }

    public FileValidationService(
        ILogger<FileValidationService> logger,
        IOptions<CloudflareR2Options> options)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(options);
        _logger = logger;
        _options = options.Value;
    }

    public async Task<FileValidationResult> ValidateAsync(
        string fileName,
        string contentType,
        long fileSizeBytes,
        Stream fileStream)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(contentType);
        ArgumentNullException.ThrowIfNull(fileStream);

        List<string> errors = [];

        // 1. Validate content type
        if (!AllowedContentTypes.Contains(contentType))
        {
            errors.Add($"Content type '{contentType}' is not allowed. Allowed types: images, documents, and videos.");
            _logger.LogWarning("Rejected file with invalid content type: {ContentType}, FileName: {FileName}", contentType, fileName);
        }

        // 2. Validate file extension
        string extension = Path.GetExtension(fileName);
        if (string.IsNullOrWhiteSpace(extension))
        {
            errors.Add("File must have an extension.");
        }
        else if (DangerousExtensions.Contains(extension))
        {
            errors.Add($"File extension '{extension}' is not allowed for security reasons.");
            _logger.LogWarning("Rejected file with dangerous extension: {Extension}, FileName: {FileName}", extension, fileName);
        }
        else if (!string.IsNullOrWhiteSpace(contentType) && !IsExtensionConsistentWithContentType(extension, contentType))
        {
            // SECURITY: declared content type must match the extension to prevent type-confusion uploads.
            errors.Add($"File extension '{extension}' does not match the declared content type '{contentType}'.");
            _logger.LogWarning("Rejected file with extension/content-type mismatch: {Extension} vs {ContentType}, FileName: {FileName}",
                extension, contentType, fileName);
        }

        // 3. Validate file name
        if (fileName.Length > 255)
        {
            errors.Add("File name is too long (max 255 characters).");
        }

        if (fileName.Contains("..", StringComparison.Ordinal) || fileName.Contains('/'))
        {
            errors.Add("File name contains invalid path characters.");
            _logger.LogWarning("Rejected file with invalid path characters: {FileName}", fileName);
        }

        // 4. Validate file size
        long maxImageSizeBytes = _options.MaxImageSizeMB * 1024L * 1024L;
        long maxDocumentSizeBytes = _options.MaxDocumentSizeMB * 1024L * 1024L;
        long maxVideoSizeBytes = _options.MaxVideoSizeMB * 1024L * 1024L;

        if (fileSizeBytes <= 0)
        {
            errors.Add("File is empty.");
        }
        else if (IsImageType(contentType) && fileSizeBytes > maxImageSizeBytes)
        {
            errors.Add($"Image file size exceeds maximum of {_options.MaxImageSizeMB} MB.");
        }
        else if (IsDocumentType(contentType) && fileSizeBytes > maxDocumentSizeBytes)
        {
            errors.Add($"Document file size exceeds maximum of {_options.MaxDocumentSizeMB} MB.");
        }
        else if (IsVideoType(contentType) && fileSizeBytes > maxVideoSizeBytes)
        {
            errors.Add($"Video file size exceeds maximum of {_options.MaxVideoSizeMB} MB.");
        }

        // 5. Validate file signature (magic bytes) for images to prevent spoofing
        if (IsImageType(contentType) && fileStream.CanSeek)
        {
            bool validSignature = await ValidateImageSignatureAsync(fileStream, contentType);
            if (!validSignature)
            {
                errors.Add("File signature does not match declared image type. File may be corrupted or spoofed.");
                _logger.LogWarning("Rejected file with invalid image signature: {ContentType}, FileName: {FileName}", contentType, fileName);
            }
        }

        if (errors.Count > 0)
        {
            return FileValidationResult.Failure(errors.ToArray());
        }

        return FileValidationResult.Success();
    }

    private static bool IsImageType(string contentType)
    {
        return AllowedImageTypes.Contains(contentType);
    }

    private static bool IsDocumentType(string contentType)
    {
        return AllowedDocumentTypes.Contains(contentType);
    }

    private static bool IsVideoType(string contentType)
    {
        return AllowedVideoTypes.Contains(contentType);
    }

    private static async Task<bool> ValidateImageSignatureAsync(Stream stream, string contentType)
    {
        byte[] buffer = new byte[12];
        long originalPosition = stream.Position;

        try
        {
            stream.Position = 0;
            int bytesRead = await stream.ReadAsync(buffer.AsMemory(0, 12));

            if (bytesRead < 4)
            {
                return false;
            }

            // JPEG: FF D8 FF
            if (contentType.Contains("jpeg", StringComparison.OrdinalIgnoreCase) ||
                contentType.Contains("jpg", StringComparison.OrdinalIgnoreCase))
            {
                return buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF;
            }

            // PNG: 89 50 4E 47
            if (contentType.Contains("png", StringComparison.OrdinalIgnoreCase))
            {
                return buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47;
            }

            // GIF: 47 49 46 38
            if (contentType.Contains("gif", StringComparison.OrdinalIgnoreCase))
            {
                return buffer[0] == 0x47 && buffer[1] == 0x49 && buffer[2] == 0x46 && buffer[3] == 0x38;
            }

            // WebP: 52 49 46 46 ... 57 45 42 50
            if (contentType.Contains("webp", StringComparison.OrdinalIgnoreCase))
            {
                if (bytesRead < 12) return false;
                return buffer[0] == 0x52 && buffer[1] == 0x49 && buffer[2] == 0x46 && buffer[3] == 0x46 &&
                       buffer[8] == 0x57 && buffer[9] == 0x45 && buffer[10] == 0x42 && buffer[11] == 0x50;
            }

            return true; // Unknown image type, allow for now
        }
        finally
        {
            stream.Position = originalPosition;
        }
    }
}
