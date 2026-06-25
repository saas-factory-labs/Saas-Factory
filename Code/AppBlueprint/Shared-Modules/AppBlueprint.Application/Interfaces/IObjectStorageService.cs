namespace AppBlueprint.Application.Interfaces;

public interface IObjectStorageService
{
    Task UploadAsync(string objectKey, Stream fileStream, string contentType, CancellationToken cancellationToken = default);
    Task<Stream> DownloadAsync(string objectKey, CancellationToken cancellationToken = default);
    Task DeleteAsync(string objectKey, CancellationToken cancellationToken = default);
}
