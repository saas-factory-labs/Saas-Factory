namespace AppBlueprint.Application.Options;

public sealed class ObjectStorageOptions
{
    public const string SectionName = "Storage";

    public string Endpoint { get; set; } = string.Empty;
    public string BucketName { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
}
