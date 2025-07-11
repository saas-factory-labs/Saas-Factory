using Microsoft.Extensions.Logging;

internal class LoggingHandler : DelegatingHandler
{
    private readonly ILogger<LoggingHandler> _logger;

    public LoggingHandler(HttpMessageHandler innerHandler, ILogger<LoggingHandler> logger) : base(innerHandler)
    {
        _logger = logger;
    }

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
