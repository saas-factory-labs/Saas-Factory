using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AppBlueprint.Presentation.ApiModule.Middleware;

/// <summary>
/// Global exception handler that provides consistent error responses and detailed logging
/// for all unhandled exceptions across the API.
/// </summary>
public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(exception);

        string traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        // Log different exception types with appropriate context
        switch (exception)
        {
            case ArgumentNullException argNullEx:
                logger.LogWarning(
                    exception,
                    "Null argument validation failed. Parameter: {ParamName}. TraceId: {TraceId}. Path: {Path}",
                    argNullEx.ParamName,
                    traceId,
                    httpContext.Request.Path);

                await WriteValidationErrorResponse(
                    httpContext,
                    $"Missing required parameter: {argNullEx.ParamName}",
                    traceId,
                    cancellationToken);
                break;

            case ArgumentException argEx:
                logger.LogWarning(
                    exception,
                    "Argument validation failed. Parameter: {ParamName}. TraceId: {TraceId}. Path: {Path}",
                    argEx.ParamName,
                    traceId,
                    httpContext.Request.Path);

                await WriteValidationErrorResponse(
                    httpContext,
                    $"Invalid parameter '{argEx.ParamName}': {argEx.Message}",
                    traceId,
                    cancellationToken);
                break;

            case InvalidOperationException:
                logger.LogWarning(
                    exception,
                    "Invalid operation. TraceId: {TraceId}. Path: {Path}. User: {UserId}",
                    traceId,
                    httpContext.Request.Path,
                    httpContext.User?.Identity?.Name ?? "Anonymous");

                await WriteBadRequestResponse(
                    httpContext,
                    exception.Message,
                    traceId,
                    cancellationToken);
                break;

            case TimeoutException:
                logger.LogError(
                    exception,
                    "Request timeout. TraceId: {TraceId}. Path: {Path}",
                    traceId,
                    httpContext.Request.Path);

                await WriteTimeoutResponse(
                    httpContext,
                    traceId,
                    cancellationToken);
                break;

            case UnauthorizedAccessException:
                logger.LogWarning(
                    exception,
                    "Unauthorized access attempt. TraceId: {TraceId}. Path: {Path}. User: {UserId}",
                    traceId,
                    httpContext.Request.Path,
                    httpContext.User?.Identity?.Name ?? "Anonymous");

                await WriteUnauthorizedResponse(
                    httpContext,
                    traceId,
                    cancellationToken);
                break;

            default:
                logger.LogError(
                    exception,
                    "Unhandled exception occurred. TraceId: {TraceId}. Path: {Path}. Method: {Method}. User: {UserId}. TenantId: {TenantId}",
                    traceId,
                    httpContext.Request.Path,
                    httpContext.Request.Method,
                    httpContext.User?.Identity?.Name ?? "Anonymous",
                    httpContext.Items["TenantId"]?.ToString() ?? "None");

                await WriteInternalServerErrorResponse(
                    httpContext,
                    traceId,
                    cancellationToken);
                break;
        }

        return true; // Exception handled
    }

    private static async Task WriteValidationErrorResponse(
        HttpContext httpContext,
        string message,
        string traceId,
        CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation Error",
            Detail = message,
            Instance = httpContext.Request.Path,
            Extensions = { ["traceId"] = traceId }
        };

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
    }

    private static async Task WriteBadRequestResponse(
        HttpContext httpContext,
        string message,
        string traceId,
        CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Bad Request",
            Detail = message,
            Instance = httpContext.Request.Path,
            Extensions = { ["traceId"] = traceId }
        };

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
    }

    private static async Task WriteTimeoutResponse(
        HttpContext httpContext,
        string traceId,
        CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status504GatewayTimeout,
            Title = "Request Timeout",
            Detail = "The request took too long to complete.",
            Instance = httpContext.Request.Path,
            Extensions = { ["traceId"] = traceId }
        };

        httpContext.Response.StatusCode = StatusCodes.Status504GatewayTimeout;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
    }

    private static async Task WriteUnauthorizedResponse(
        HttpContext httpContext,
        string traceId,
        CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized,
            Title = "Unauthorized",
            Detail = "Authentication is required to access this resource.",
            Instance = httpContext.Request.Path,
            Extensions = { ["traceId"] = traceId }
        };

        httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
    }

    private static async Task WriteInternalServerErrorResponse(
        HttpContext httpContext,
        string traceId,
        CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Internal Server Error",
            Detail = "An unexpected error occurred while processing your request.",
            Instance = httpContext.Request.Path,
            Extensions = { ["traceId"] = traceId }
        };

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
    }
}
