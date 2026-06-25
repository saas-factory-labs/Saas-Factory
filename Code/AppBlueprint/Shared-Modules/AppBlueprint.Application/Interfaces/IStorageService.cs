namespace AppBlueprint.Application.Interfaces;

public interface IStorageService
{
    /// <summary>
    /// Validates, sanitizes, and uploads a document to S3-compatible object storage.
    /// </summary>
    /// <param name="fileStream">Seekable stream of the file content. Must support <see cref="Stream.Seek"/>.</param>
    /// <param name="fileName">Original filename. Will be sanitized server-side — never trust client input.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The object key of the stored file.</returns>
    /// <exception cref="InvalidFileTypeException">Thrown when magic bytes do not match the expected file type.</exception>
    Task<string> UploadDocumentAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default);
}
