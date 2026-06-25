using AppBlueprint.Application.Exceptions;
using AppBlueprint.Application.Interfaces;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace AppBlueprint.Presentation.ApiModule.Endpoints;

public static class DocumentUploadEndpoints
{
    // Anchor type used solely as the generic parameter for ILogger<T>
    private sealed class Log { }

    private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB
    private const string AllowedContentType = "application/pdf";

    public static WebApplication MapDocumentUploadEndpoints(this WebApplication app)
    {
        app.MapPost("/api/v1/upload/document", UploadDocumentAsync)
            .RequireAuthorization()
            .DisableAntiforgery()
            .WithName("UploadDocument")
            .WithTags("Upload")
            .WithSummary("Securely upload a PDF document to object storage")
            .WithDescription(
                "Validates magic bytes, sanitizes the filename to prevent path traversal, " +
                "and stores the file with security headers (Content-Disposition: attachment, " +
                "X-Content-Type-Options, Content-Security-Policy).")
            .Accepts<IFormFile>("multipart/form-data")
            .Produces<DocumentUploadResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status413RequestEntityTooLarge);

        return app;
    }

    private static async Task<IResult> UploadDocumentAsync(
        IFormFile file,
        IStorageService storageService,
        ILogger<Log> logger,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return Results.Problem(
                title: "No file provided",
                detail: "The request must contain a non-empty file.",
                statusCode: StatusCodes.Status400BadRequest);

        if (file.Length > MaxFileSizeBytes)
            return Results.Problem(
                title: "File too large",
                detail: $"Maximum allowed size is {MaxFileSizeBytes / (1024 * 1024)} MB. " +
                        $"Received: {file.Length / (1024.0 * 1024.0):F1} MB.",
                statusCode: StatusCodes.Status413RequestEntityTooLarge);

        // Reject if the client Content-Type is not application/pdf.
        // This is a soft guard — the hard guard is magic bytes validation inside the service.
        if (!string.Equals(file.ContentType, AllowedContentType, StringComparison.OrdinalIgnoreCase))
            return Results.Problem(
                title: "Unsupported file type",
                detail: $"Only {AllowedContentType} is accepted.",
                statusCode: StatusCodes.Status400BadRequest);

        try
        {
            // IFormFile.OpenReadStream() returns a seekable FileBufferingReadStream in ASP.NET Core.
            await using Stream fileStream = file.OpenReadStream();

            string objectKey = await storageService.UploadDocumentAsync(
                fileStream, file.FileName, cancellationToken);

            logger.LogInformation(
                "Document uploaded via endpoint. OriginalName: {OriginalName}, ObjectKey: {ObjectKey}",
                file.FileName, objectKey);

            return Results.Ok(new DocumentUploadResponse(
                ObjectKey: objectKey,
                OriginalFileName: file.FileName,
                SizeInBytes: file.Length));
        }
        catch (InvalidFileTypeException ex)
        {
            logger.LogWarning(
                "File type validation failed. FileName: {FileName}, Reason: {Reason}",
                file.FileName, ex.Message);

            return Results.Problem(
                title: "Invalid file type",
                detail: ex.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Storage operation failed. FileName: {FileName}", file.FileName);

            return Results.Problem(
                title: "Storage error",
                detail: "The file could not be stored. Please try again.",
                statusCode: StatusCodes.Status502BadGateway);
        }
    }

    private sealed record DocumentUploadResponse(
        string ObjectKey,
        string OriginalFileName,
        long SizeInBytes);
}
