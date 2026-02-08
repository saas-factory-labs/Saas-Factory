using System.Net.Http.Headers;
using System.Text.Json;
using AppBlueprint.Contracts.Baseline.File.Responses;

namespace AppBlueprint.Web.Services;

/// <summary>
/// Service for interacting with the File Storage API endpoints.
/// </summary>
internal sealed class FileStorageService(HttpClient httpClient, ILogger<FileStorageService> logger)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<FileStorageService> _logger = logger;

    /// <summary>
    /// Uploads a file to storage.
    /// </summary>
    public async Task<FileStorageResponse?> UploadFileAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        string? folder = null,
        bool isPublic = false,
        Dictionary<string, object>? customMetadata = null)
    {
        ArgumentNullException.ThrowIfNull(fileStream);
        ArgumentNullException.ThrowIfNull(fileName);
        ArgumentNullException.ThrowIfNull(contentType);

        // Copy stream to MemoryStream so it can be retried by Polly
        using var memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        using var content = new MultipartFormDataContent();

        // Use ByteArrayContent instead of StreamContent so Polly can retry
        byte[] fileBytes = memoryStream.ToArray();
        using var byteArrayContent = new ByteArrayContent(fileBytes);
        byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        content.Add(byteArrayContent, "file", fileName);

        if (!string.IsNullOrWhiteSpace(folder))
        {
            content.Add(new StringContent(folder), "folder");
        }

        content.Add(new StringContent(isPublic.ToString()), "isPublic");

        if (customMetadata is not null)
        {
            string metadataJson = JsonSerializer.Serialize(customMetadata);
            content.Add(new StringContent(metadataJson), "customMetadata");
        }

        var requestUri = new Uri("api/v1/filestorage/upload", UriKind.Relative);
        Uri? fullUrl = _httpClient.BaseAddress is not null ? new Uri(_httpClient.BaseAddress, requestUri) : null;

        HttpResponseMessage response = await _httpClient.PostAsync(requestUri, content);

        if (!response.IsSuccessStatusCode)
        {
            string errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError(
                "File upload failed. BaseAddress: {BaseAddress}, RelativeUrl: {RelativeUrl}, FullUrl: {FullUrl}, StatusCode: {StatusCode}, ReasonPhrase: {ReasonPhrase}, Error: {Error}",
                _httpClient.BaseAddress,
                requestUri,
                fullUrl,
                response.StatusCode,
                response.ReasonPhrase,
                errorContent);

            // Log additional diagnostic info
            _logger.LogError(
                "Upload diagnostics - FileName: {FileName}, ContentType: {ContentType}, FileSize: {FileSize} bytes, Folder: {Folder}, IsPublic: {IsPublic}",
                fileName,
                contentType,
                fileBytes.Length,
                folder ?? "(none)",
                isPublic);

            // Throw exception with details so UI can show the error
            throw new HttpRequestException(
                $"Upload failed: {response.StatusCode} {response.ReasonPhrase}. {errorContent}",
                null,
                response.StatusCode);
        }

        string jsonResponse = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<FileStorageResponse>(jsonResponse,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    /// <summary>
    /// Gets a list of files with optional filtering.
    /// </summary>
    public async Task<List<FileStorageResponse>> GetFilesAsync(string? folder = null)
    {
        string url = "api/v1/filestorage/list";
        if (!string.IsNullOrWhiteSpace(folder))
        {
            url += $"?folder={Uri.EscapeDataString(folder)}";
        }

        var requestUri = new Uri(url, UriKind.Relative);
        Uri? fullUrl = _httpClient.BaseAddress is not null ? new Uri(_httpClient.BaseAddress, requestUri) : null;

        HttpResponseMessage response = await _httpClient.GetAsync(requestUri);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError(
                "Failed to retrieve files. BaseAddress: {BaseAddress}, RelativeUrl: {RelativeUrl}, FullUrl: {FullUrl}, StatusCode: {StatusCode}",
                _httpClient.BaseAddress,
                requestUri,
                fullUrl,
                response.StatusCode);
            return [];
        }

        string jsonResponse = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<FileStorageResponse>>(jsonResponse,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];
    }

    /// <summary>
    /// Generates a pre-signed URL for private file access.
    /// </summary>
    public async Task<string?> GetPresignedUrlAsync(string fileKey, int expiresInMinutes = 60)
    {
        ArgumentNullException.ThrowIfNull(fileKey);

        string url = $"api/v1/filestorage/presigned-url?fileKey={Uri.EscapeDataString(fileKey)}&expiresInMinutes={expiresInMinutes}";
        var requestUri = new Uri(url, UriKind.Relative);
        Uri? fullUrl = _httpClient.BaseAddress is not null ? new Uri(_httpClient.BaseAddress, requestUri) : null;

        HttpResponseMessage response = await _httpClient.GetAsync(requestUri);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError(
                "Failed to generate pre-signed URL. BaseAddress: {BaseAddress}, RelativeUrl: {RelativeUrl}, FullUrl: {FullUrl}, StatusCode: {StatusCode}",
                _httpClient.BaseAddress,
                requestUri,
                fullUrl,
                response.StatusCode);
            return null;
        }

        return await response.Content.ReadAsStringAsync();
    }

    /// <summary>
    /// Downloads a file from storage.
    /// </summary>
    public async Task<Stream?> DownloadFileAsync(string fileKey)
    {
        ArgumentNullException.ThrowIfNull(fileKey);

        var requestUri = new Uri($"api/v1/filestorage/download/{Uri.EscapeDataString(fileKey)}", UriKind.Relative);
        Uri? fullUrl = _httpClient.BaseAddress is not null ? new Uri(_httpClient.BaseAddress, requestUri) : null;

        HttpResponseMessage response = await _httpClient.GetAsync(requestUri);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError(
                "Failed to download file. BaseAddress: {BaseAddress}, RelativeUrl: {RelativeUrl}, FullUrl: {FullUrl}, StatusCode: {StatusCode}",
                _httpClient.BaseAddress,
                requestUri,
                fullUrl,
                response.StatusCode);
            return null;
        }

        return await response.Content.ReadAsStreamAsync();
    }

    /// <summary>
    /// Deletes a file from storage.
    /// </summary>
    public async Task<bool> DeleteFileAsync(string fileKey)
    {
        ArgumentNullException.ThrowIfNull(fileKey);

        var requestUri = new Uri($"api/v1/filestorage/{Uri.EscapeDataString(fileKey)}", UriKind.Relative);
        Uri? fullUrl = _httpClient.BaseAddress is not null ? new Uri(_httpClient.BaseAddress, requestUri) : null;

        HttpResponseMessage response = await _httpClient.DeleteAsync(requestUri);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError(
                "Failed to delete file. BaseAddress: {BaseAddress}, RelativeUrl: {RelativeUrl}, FullUrl: {FullUrl}, StatusCode: {StatusCode}",
                _httpClient.BaseAddress,
                requestUri,
                fullUrl,
                response.StatusCode);
            return false;
        }

        return true;
    }
}
