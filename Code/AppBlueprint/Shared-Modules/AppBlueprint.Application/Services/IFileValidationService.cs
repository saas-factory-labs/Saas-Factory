namespace AppBlueprint.Application.Services;

/// <summary>
/// Validates file uploads for security and business rules.
/// </summary>
public interface IFileValidationService
{
    /// <summary>
    /// Validates a file upload request.
    /// </summary>
    /// <param name="fileName">Original file name.</param>
    /// <param name="contentType">MIME type.</param>
    /// <param name="fileSizeBytes">File size in bytes.</param>
    /// <param name="fileStream">File stream for content validation.</param>
    /// <returns>Validation result with any errors.</returns>
    Task<FileValidationResult> ValidateAsync(string fileName, string contentType, long fileSizeBytes, Stream fileStream);
}

/// <summary>
/// Result of file validation.
/// </summary>
public sealed record FileValidationResult(bool IsValid, List<string> Errors)
{
    public static FileValidationResult Success()
    {
        return new(true, []);
    }

    public static FileValidationResult Failure(params string[] errors)
    {
        return new(false, [.. errors]);
    }
}
