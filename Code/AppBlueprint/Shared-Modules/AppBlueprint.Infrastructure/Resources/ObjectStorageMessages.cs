namespace AppBlueprint.Infrastructure.Resources;

/// <summary>
/// Contains constant messages for object storage operations.
/// </summary>
public static class ObjectStorageMessages
{
    public const string FileDownloadedSuccessfully = "File downloaded successfully: {FilePath}";
    public const string ErrorReadingObject = "Error encountered on server. Message: '{ErrorMessage}' when reading an object";
    public const string ETagFormat = "ETag: {ETag}";
    public const string ErrorWritingObject = "Error encountered on server. Message: '{ErrorMessage}' when writing an object";
}
