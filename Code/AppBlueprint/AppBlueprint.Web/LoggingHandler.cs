namespace AppBlueprint.Web;

internal sealed class LoggingHandler(HttpMessageHandler innerHandler, ILogger<LoggingHandler> logger) : DelegatingHandler(innerHandler)
{
    private readonly ILogger<LoggingHandler> _logger = logger;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Log Request
        _logger.LogInformation("Request:");
        _logger.LogInformation("{Request}", request);

        if (request.Content is not null)
        {
            string? requestBody = await request.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogInformation("Request Body: {RequestBody}", requestBody);
        }

        HttpResponseMessage? response = await base.SendAsync(request, cancellationToken);

        // Log Response
        _logger.LogInformation("Response:");
        _logger.LogInformation("{Response}", response);

        string? responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        _logger.LogInformation("Response Body: {ResponseBody}", responseBody);

        return response;
    }
}